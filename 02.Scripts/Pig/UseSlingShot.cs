using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using static PigEquipList;


public class UseSlingShot : MonoBehaviour
{
    //다른스크립트 참조
    private PhotonView PV; // 포톤 뷰
    private PigEquipList PEL;//
    private PigGetItem PGI;
    private PlayerController PC;

    // 새총관련 변수 
    private float shootForce = 120f; // 총알 발사 속도
    public GameObject slingshotObject; // 새총에 대한 참조
    public GameObject MetalSphere; // 총알 프리팹에 대한 참조
    public GameObject mainCamera; // 카메라 오브젝트에 대한 참조
    // 발사 위치 조정 (캐릭터의 머리 위나 옆으로 설정)
    //Vector3 offset = new Vector3(0, -1, 0); // 적절한 오프셋 값 설정

    // 총알 생성 관련
    Vector3 shootDirection;
    Vector3 spawnPosition;
    float distance;
    // 상향 각도 추가 (예: 5도 위로)
    float upAngle = -3.7f; // 원하는 각도

    //애니메이션 관련
    private Animator anim;

    //소리관련
    public AudioSource SlingShotShootSFX;

    //쿨타임 관련
    private bool isActiveShooting;
    private float TimeFlow = 1.0f;

    //에임 관련
    public GameObject PigAim;

    void Awake()
    {
        PC = GetComponent<PlayerController>();
        PV = GetComponent<PhotonView>(); // photon view
        PEL = GetComponent<PigEquipList>();
        PGI = GetComponent<PigGetItem>();
        shootDirection = mainCamera.transform.forward;



        spawnPosition = slingshotObject.transform.position + shootDirection * distance;
        CheckSpawnPosition();
        
    }


    // 위치를 업데이트 해주어야 한다
    public void CheckSpawnPosition()
    {
        distance = PC.isFirstPoint ? 2.5f : 12.5f;
        spawnPosition = slingshotObject.transform.position + shootDirection * distance;
        
    }


    //발사 방향벡터를 업데이트
    public void UpdateShootDirection()
    {
        
        shootDirection =  mainCamera.transform.forward;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
    }



    // 새총 공격 쿨타임 체크 
    public void CheckCooltime()
    {
        if (isActiveShooting)
        {
            TimeFlow -= Time.deltaTime;

            if (TimeFlow <= 0f)
            {
                isActiveShooting = false;
                TimeFlow = 1.0f; // 쿨타임이 끝나면 TimeFlow를 초기값으로 재설정
            }
        }
    }

    

    public void ReadySlingShot(PigItemType SelectedPigItem)
    {
 
        if (SelectedPigItem == PigItemType.SLINGSHOT && !isActiveShooting && PGI.CountItem("SlingShot") > 0)
        { 
            // 애니메이션 PhotonView를 사용하기 때문에 Animation을 RPC로 동기화 할 필요 없음.
            isActiveShooting = true;
            PV.RPC("SlingShotRPC", RpcTarget.All); // 트리거 애니메이션 동기화가 안되서 rpc로 쏴봄 일단
            //UpdateSpawnPosition();

            // 카메라의 전방 방향을 발사 방향으로 사용
            GameObject bullet = PhotonNetwork.Instantiate(MetalSphere.name, spawnPosition, Quaternion.identity);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = shootDirection * shootForce; // 총알을 앞쪽으로 발사

            //인벤토리에서 새총개수 줄이기
            PGI.DeleteItem("SlingShot");

            //소리가 나온다 
            PV.RPC("SlingShotShootAudioPlay", RpcTarget.All,true);
        }
    }               


    //트리거 애니메이션이 동기화 안되는 문제
    [PunRPC]
    public void SlingShotRPC()
    {
        anim.SetTrigger("SLINGSHOT");
    }


    public void PigAimManager()
    {
        //Debug.Log($"새총장착 여부 : {PEL.isSlingshotEquiped}");
        if (PEL.isSlingshotEquiped)
        {
            PigAim.SetActive(true);
        }
        else
        {
            PigAim.SetActive(false);
        }
    }



    //소리관련RPC
    [PunRPC]
    public void SlingShotShootAudioPlay(bool TF)
    {
        if (TF)
        {
            SlingShotShootSFX.Play();
        }
        else
        {
            SlingShotShootSFX.Stop();
        }
    }
}
