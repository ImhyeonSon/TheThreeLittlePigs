using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PigEquipList;

public class UseUmbrella : MonoBehaviour
{
    //다른스크립트 참조
    private PhotonView PV; // 포톤 뷰
    private PigGetItem PGI; // 
    
    public PigEquipList PEL; // 장비 리스트 
    public FadeController FC;

    //우산 관련 변수
    private float umbrellaRemainingTime = 0f; // 우산이 활성화될 남은 시간
    private const float umbrellaDuration = 2.0f; // 우산 활성화 지속 시간
    private bool isUmbrella; // 엄브렐라의 애니메이션을 관리하는 불린형 변수 

    //애니메이션 관련
    private Animator anim;

    //소리 
    //소리이펙트
    public AudioSource UmbrellaSFX;
    public AudioSource BadNotiSFX;

    void Awake()
    {
        PV = GetComponent<PhotonView>(); // photon view
        PEL = GetComponent<PigEquipList>(); // pig equip list
        PGI = GetComponent<PigGetItem>();
        FC = GameObject.Find("Canvas").transform.Find("NotificationUI").GetComponent<FadeController>();
    }


    void Start()
    {
        anim = GetComponent<Animator>();
    }


    // 돼지의 우산 펼치기 스킬 사용 (Q버튼)

    public void OpenUmbrella()
    {
        if (PGI.CountItem("Umbrella") > 0)
        {
            if (umbrellaRemainingTime <= 0)
            {
                Debug.Log("우산아이템 사용");
                umbrellaRemainingTime = umbrellaDuration;
                isUmbrella = true;
                PV.RPC("OpenUmbrellaRPC", RpcTarget.All); //모든 유저에게 우산 애니메이션을 동기화
                FC.StartFadeIn("우산을 펼쳐서 늑대의 바람불기 공격을 방어합니다.");
                //인벤토리에서 우산개수 줄이기
                PGI.DeleteItem("Umbrella");
            }
        }
        else
        {
            if(!BadNotiSFX.isPlaying)
            {
                FC.StartFadeIn("인벤토리에 우산이 없습니다.");
                BadNotiSFX.Play();

            }



        }

    }

    [PunRPC]
    public void OpenUmbrellaRPC()
    {
        PEL.allEquipments[2].SetActive(true); // 우산 게임오브젝트 활성화
        PV.RPC("UmbrellaAudioPlay", RpcTarget.All, true); //소리
        anim.SetBool("isUmbrella", isUmbrella);
    }

    // 우산의 사용여부를 불린형 변수로 체크
    public void CheckUmbrellaUsing()
    {
        if (umbrellaRemainingTime > 0)
        {
            umbrellaRemainingTime -= Time.deltaTime;

            if (umbrellaRemainingTime <= 0)
            {
                
                isUmbrella = false;
                PV.RPC("CheckUmbrellaUsingRPC", RpcTarget.All); //모든 유저에게 우산 애니메이션을 동기화
                PV.RPC("UmbrellaAudioPlay", RpcTarget.All, false); //소리
                // 늑대의 바람 파티클을 부수는 로직을 여기에 작성

            }
        }
    }

    [PunRPC]
    public void CheckUmbrellaUsingRPC()
    {
        PEL.allEquipments[2].SetActive(false); // 우산 게임 오브젝트 비활성화
        anim.SetBool("isUmbrella", isUmbrella);
    }


    //소리이펙트
    [PunRPC]
    public void UmbrellaAudioPlay(bool TF)
    {
        if (TF)
        {
            UmbrellaSFX.Play();
        }
        else
        {
            UmbrellaSFX.Stop();
        }
    }


}
