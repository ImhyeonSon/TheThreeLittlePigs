using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseScarecrow : MonoBehaviour
{
    //다른스크립트
    private PigGetItem PGI;

    //메시지관련
    public FadeController FC;
    private PhotonView PV; // 포톤 뷰
    public PigEquipList PEL; // 장비 리스트 
    //소리이펙트
    public AudioSource TigerSFX;
    public AudioSource BadNotiSFX;

    private Animator anim;
    public GameObject pigObject; // 새총에 대한 참조
    Vector3 shootDirection;
    Vector3 spawnPosition;
    public GameObject Tiger; // 호랑이 프리팹에 대한 참조
    public GameObject mainCamera; // 카메라 오브젝트에 대한 참조
    float distance;

    private bool isActive = false;
    private float cooldownTime = 5f; // 쿨다운 시간
    private float cooldownTimer = 0f; // 쿨다운 타이머

    void Awake()
    {
        PGI = GetComponent<PigGetItem>();
        PV = GetComponent<PhotonView>(); // photon view
        FC = GameObject.Find("Canvas").transform.Find("NotificationUI").GetComponent<FadeController>();
        shootDirection = mainCamera.transform.forward;
        distance = 2.0f;
        spawnPosition = pigObject.transform.position + shootDirection * distance;
        UpdateSpawnPosition();
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void UpdateSpawnPosition()
    {
        shootDirection = mainCamera.transform.forward;
        spawnPosition = pigObject.transform.position + shootDirection * distance;
    }


    public void TrapScarecrow()
    {

        
        if (PGI.CountItem("Tiger") > 0)
        {
            if (!isActive) // 쿨다운 중이 아닐 때만 사용 가능
            {
                FC.StartFadeIn("호랑이 허수아비를 설치해서 늑대의 움직임을 방해합니다.");
                isActive = true;
                cooldownTimer = cooldownTime;
                UpdateSpawnPosition();
                Debug.Log("호랑이 허수아비 설치");
                PV.RPC("TigerAudioPlay", RpcTarget.All, true);
                GameObject tiger = PhotonNetwork.Instantiate(Tiger.name, spawnPosition, Quaternion.identity);
                //인벤토리에서 호.허(호랑이허수아비) 개수 줄이기
                PGI.DeleteItem("Tiger");
            }
        }
        else
        {
            if (!BadNotiSFX.isPlaying)
            {
                FC.StartFadeIn("인벤토리에 호랑이허수아비가 없습니다.");
                BadNotiSFX.Play();
            }
        }
  
    }

    public void CheckCooltime()
    {
        // 쿨다운 타이머 업데이트
        if (isActive)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                isActive = false;
            }
        }
    }

    //소리이펙트

    //소리이펙트
    [PunRPC]
    public void TigerAudioPlay(bool TF)
    {
        if (TF)
        {
            TigerSFX.Play();
        }
        else
        {
            TigerSFX.Stop();
        }
    }


}
