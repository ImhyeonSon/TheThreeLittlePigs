using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalStatus : MonoBehaviour
{
    public PhotonGameManager PGM; 

    public int maxHealth;
    public int currentHealth;
    public int exp;

    private bool isDie=false;

    public Animator anim;
    public PhotonView PV;
    public Transform myTransform;
    public HpBar HpBar;

    public GameObject[] RenderingList;

    int defaultperiodicCnt = 1;
    private int periodicCnt;// 늘리면 동기화 주기 up

    void Awake()
    {
        periodicCnt = defaultperiodicCnt;
        PGM = GameObject.Find("PhotonGameManager").GetComponent<PhotonGameManager>();
        currentHealth = maxHealth;  // 시작시 체력이 가득 차있음
        myTransform = GetComponent<Transform>();
        HpBar.SetMaxHp(maxHealth);
    }

    public void Update()
    {
        if (isDie)
        {
            anim.SetBool("isDie", false);
            foreach (GameObject renderMesh in RenderingList)
            {
                renderMesh.SetActive(false);
            }
            gameObject.SetActive(false);
        }
    }
    private void OnEnable()  // 리스폰됐을때 체력을 다시 채워줘야 함
    {
        UseSyncPosition();
        isDie = false;
        anim.SetBool("isDie", false);
        currentHealth = maxHealth;
        HpBar.SetHpBar(currentHealth);
        foreach (GameObject renderMesh in RenderingList)
        {
            renderMesh.SetActive(true);
        }
        //// 리스폰 위치 동기화
    }

    public void PeriodicSync()
    {
        periodicCnt -= 1;
        if (periodicCnt < 0)
        {
            periodicCnt = defaultperiodicCnt;
            UseSyncPosition();
        }
    }


    public void UseSyncPosition()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("SyncPosition", RpcTarget.All, myTransform.position);

        }
    }
    [PunRPC]
    public void SyncPosition(Vector3 transform)
    {
        myTransform.position = transform;
    }

    public int TakeDamage(int damage)  // 동물이 공격 받으면 죽었는지 체크하고 죽었으면 경험치, 살아있으면 0을 return
    {
        if (currentHealth > 0) // 살아있을때만 데미지를 받는다.
        {
            currentHealth -= damage;  // 현재 체력이 깎이고
            HpBar.SetHpBar(currentHealth);  // 체력바에 적용 => 내가 데미지를 줬을 때만
            StartCoroutine(Hit()); // 공격받는 애니메이션
           
            if (currentHealth>0) // 데미지를 받고 살아있다면
            {
                PV.RPC("SetStatusAnimal", RpcTarget.All, currentHealth, false);
                return 0;
            } else
            {
                // 죽었다면
                PV.RPC("CoroutineDieRPC", RpcTarget.All);
                return exp;
            }
        }
        return 0;
    }


    [PunRPC]
    public void SetStatusAnimal(int hp,bool TF) // RPC함수를 줄이려고 변수 두개를 받음
    {
        currentHealth = hp;
        isDie = TF;    
    }

    [PunRPC]
    public void CoroutineDieRPC()
    {
        StartCoroutine(Die());
    }
    
    IEnumerator Die()
    {
        yield return new WaitForSeconds(0.2f);
        Debug.Log("죽는 애니메이션 실행" + gameObject.GetComponent<PhotonView>().ViewID);
        anim.SetBool("isDie", true);  // 죽는 애니메이션 실행
        yield return new WaitForSeconds(0.5f);
        if (PV.IsMine)
        {
            PV.RPC("SetStatusAnimal", RpcTarget.All,0, true); // RPC로 Die 처리
        }
    }

    IEnumerator Hit()
    {
        anim.SetBool("isHit", true);  // 공격받는 애니메이션 실행
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("isHit", false);
    }   

}
