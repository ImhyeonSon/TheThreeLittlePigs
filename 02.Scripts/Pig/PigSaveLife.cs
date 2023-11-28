using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PigEquipList;
using TMPro; 


public class PigSaveLife : MonoBehaviour
{
    // 다른 스크립트 참조
    private PhotonView PV; // 포톤 뷰
    private PlayerStatus PS;
    private PigStatus PST;
    private PigGetItem PGI;

    // 현재 진행중인 코루틴
    private Coroutine nowCoroutine;

    // 애니메이션 관련
    private Animator anim;

    // 돼지 서로 살리기 관련 변수
    private int PigLayer = 1 << 16; // 돼지 레이어
    public GameObject AttackRange;
    public DurationBar DurationBar;
    public bool isSavingLife = false;
    private float currenTime = 8.0f;

    // 안내 메시지 관련 변수 //오정원 
    public Canvas Canvas;
    public GameObject NotificationUI;
    public TextMeshProUGUI NoText;
    // 간단한 안내 메시지 관련 변수 (아직미사용)
    public GameObject SimpleMessageUI;
    public TextMeshProUGUI SMText;
    


    private void Awake()
    {
        // 포톤뷰 가져온다
        PV = GetComponent<PhotonView>();
        PS = GetComponent<PlayerStatus>();
        PST = GetComponent<PigStatus>();
        PGI = GetComponent<PigGetItem>();
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        NotificationUI = GameObject.Find("Canvas/NotificationUI");
        NoText = GameObject.Find("Canvas/NotificationUI/Background/Text").GetComponent<TextMeshProUGUI>();
        SimpleMessageUI = GameObject.Find("Canvas/SimpleMessageUI");
        SMText = GameObject.Find("Canvas/SimpleMessageUI/Background/Text").GetComponent<TextMeshProUGUI>();
        nowCoroutine = null;
    }

    public void PigInput()
    {
        if (Input.GetMouseButtonDown(1) && !isSavingLife) // 살리는 중이 아닐 때만 실행
        {
            SaveLife();
        }
    }

    public void CheckIsSaving()
    {

        if (isSavingLife) // true로 변했다면
        {
            currenTime -= Time.deltaTime;
            if (currenTime <= 0)
            {
                isSavingLife = false;
                currenTime = 8f;
            }
        }
    }

    public void SaveLife()
    {
        Collider[] hitColliders = Physics.OverlapSphere(AttackRange.transform.position, 1.5f, PigLayer);
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Pig"))
                {
                    Debug.Log("돼지 공격 범위에서 돼지를 감지.");
                    if (hitCollider.GetComponent<PigStatus>().isDie) // 죽은 돼지라면
                    {
                        isSavingLife = true;
                        PV.RPC("SavingAnimRPC", RpcTarget.All , true);
                        hitCollider.GetComponent<PhotonView>().RPC("SaveLifeDurationBarRPC", RpcTarget.All);
                        nowCoroutine = StartCoroutine(DelayRevival(hitCollider.GetComponent<PhotonView>())); // 코루틴 시작
                    }
                }
            }
        }
    }

    IEnumerator DelayRevival(PhotonView targetPV)
    {
        float elapsedTime = 0f;
        float waitTime = 8f;

        Debug.Log($"코루틴중에 돼지의 죽은 상태를 잘 불러오나요 :{PS.GetIsDie()}");
        while (elapsedTime < waitTime)
        {
            if (PS.GetIsDie()) // 여기서 살아있는지 여부를 계속 확인
            {
                StopCoroutine(nowCoroutine);
                PV.RPC("SavingAnimRPC", RpcTarget.All, false); // 애니메이션 중단
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        targetPV.RPC("Revival", RpcTarget.All);
        PV.RPC("SavingAnimRPC", RpcTarget.All, false);
    }



    //다른 돼지의 도움을 받아 부활
    [PunRPC]
    public void Revival()
    {
        PS.InitializeMyCharacter();
        if (PV.IsMine) // 로컬 플레이어인 경우에만 SetHpBar 호출
        {
            PST.SetHpBar();
            NotificationUI.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }


    [PunRPC]
    public void SaveLifeDurationBarRPC()
    {
        DurationBar.SetDurBar(currenTime); // DurationBar 설정

        if (PS.GetIsDie()) // 현재 조작 중인 돼지가 죽었는지 확인
        {
            PV.RPC("StopSaveLifeDurationBarRPC", RpcTarget.All); // 모든 클라이언트에게 DurationBar 중단 요청
        }
    }

    [PunRPC]
    public void StopSaveLifeDurationBarRPC()
    {
        if (PV.IsMine)
        {
            DurationBar.gameObject.SetActive(false); // DurationBar UI 숨기기
        }
    }


    // 돼지가 주사기로 스스로를 살린다
    // 돼지가 죽으면 부활제안 ui를 보여준다
    public void ShowDeadMessage()
    {
        //치트 ,테스트용 hp 0 설정
        /*if (Input.GetKeyDown(KeyCode.Keypad4)) //치트키
        {
            Debug.Log("돼지hp0강제로");
            PV.RPC("SelfDieRPC", RpcTarget.All);
        }*/

        if (PS.GetIsDie()) 
        {
            //주사기 개수 검사
            int syringeCnt = 0;
            syringeCnt = PGI.CountItem("Syringe");

            if (syringeCnt == 0)
            {
                NoText.text = "보유한 주사기가 없습니다. 다른 팀원의 구조를 기다리세요!";
                NotificationUI.GetComponent<CanvasGroup>().alpha = 1f;
                ///지울 예정///
                //테스트용 hp 10 설정
/*                if (Input.GetKeyDown(KeyCode.Keypad5)) // 치트키
                {
                    Debug.Log("돼지hp10강제로");
                    PV.RPC("SelfRevivalRPC", RpcTarget.All);
                    NotificationUI.GetComponent<CanvasGroup>().alpha = 0f;
                }*/
                ///지울 예정///
            }
            else
            {
                NoText.text = $"주사기를 하나 사용하여 부활하시겠습니까? 주사기 보유개수:{syringeCnt} (Z 키를 누르세요)";
                NotificationUI.GetComponent<CanvasGroup>().alpha = 1f;
                if (Input.GetKeyDown(KeyCode.Z))
                {
                    Debug.Log("주사기사용로직");
                    //캐릭터 HP를 초기화하고
                    PV.RPC("SyringeRevival",RpcTarget.All);
                    //Hp바 UI를 업데이트 하고
                    PST.SetHpBar();
                    //인벤토리에서 주사기 아이템 개수를 줄인다. 
                    PGI.DeleteItem("Syringe");
                    NotificationUI.GetComponent<CanvasGroup>().alpha = 0f;
                }
            }
        }
    }

    [PunRPC]
    public void SelfDieRPC()
    {
        PS.SetHp(0);
    }

    [PunRPC]
    public void SelfRevivalRPC()
    {
        PS.SetHp(10);
    }

    [PunRPC]
    public void SyringeRevival()
    {
        PS.SetHp(PS.GetMaxHp());
        if (PV.IsMine) // 로컬 플레이어인 경우에만 SetHpBar 호출
        {
            PST.SetHpBar();
            NotificationUI.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }


    [PunRPC]
    public void SavingAnimRPC(bool value)
    {
        anim.SetBool("isSaving", value);
    }
    


}
