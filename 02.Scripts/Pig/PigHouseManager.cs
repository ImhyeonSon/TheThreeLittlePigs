using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PigHouseManager : MonoBehaviour
{
    //다른 스크립트
    private PigGetItem PGI;
    private PhotonView PV;
    GameObject nearObject; // 가장 가까이 있는 대상 객체
    public bool isPigHouseBuilt = false; // 돼지가 집을 지은 상태를 나타내는 변수
    private bool InputB;
    private int houseLayer = 1 << 18;
    
    //안내 메시지 관련 변수
    public GameObject NotificationUI;

    //소리
    public AudioSource BadNoticeSFX;
    public TextMeshProUGUI NoText;
    private Coroutine hideCoroutine;

    private GameObject MyHouse=null;


    private void Awake()
    {
        PGI = GetComponent<PigGetItem>();
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        NotificationUI = GameObject.Find("Canvas/NotificationUI");
        NoText = GameObject.Find("Canvas/NotificationUI/Background/Text").GetComponent<TextMeshProUGUI>();
        hideCoroutine = null; // 초기화
    }

    // 입력을 체크하는 함수
    public void PigInput()
    {
        InputB = Input.GetKeyDown(KeyCode.B);
    }

    public void HouseDupleCheck()
    {
        if (InputB)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3f, houseLayer);
            if (hitColliders.Length > 0)
            {
                foreach (var hitCollider in hitColliders)
                {
                    BuildableSite buildableSite = hitCollider.GetComponent<BuildableSite>();

                    if (buildableSite != null)
                    {

                        if (PGI.CountItem("Rice(Clone)") >= 2)
                        {
                            if (MyHouse==null && !buildableSite.IsHouseBuilt())
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    PGI.DeleteItem("Rice(Clone)");
                                }
                                hideCoroutine = StartCoroutine(ShowAndHideNotification("초가집 건설을 시작합니다.", 3f));
                                MyHouse = buildableSite.Build();
                                UseSetMyHousRPC();//집 지어졌다고 동기화
                            }
                            else if (MyHouse==null && buildableSite.IsHouseBuilt())
                            {
                                hideCoroutine = StartCoroutine(ShowAndHideNotification("이곳엔 이미 집이 있습니다. 다른곳에 건설해주세요", 3f));
                                BadNoticeSFX.Play();
                            }
                            else if (MyHouse!=null && !buildableSite.IsHouseBuilt())
                            {
                                hideCoroutine = StartCoroutine(ShowAndHideNotification("현재 집이 부서져야 다른곳에 지을 수 있습니다.", 3f));
                                BadNoticeSFX.Play();
                            }
                            else if (MyHouse==null && buildableSite.IsHouseBuilt())
                            {
                                hideCoroutine = StartCoroutine(ShowAndHideNotification("이곳엔 이미 집이 있습니다. 다른곳에 건설해주세요.", 3f));
                                BadNoticeSFX.Play();
                            }
                            //Debug.Log($"돼지근처의 집터에 집이 있는지의여부:{buildableSite.IsHouseBuilt()}.");
                            //Debug.Log($"돼지가 집을 지었는지 여부:{isPigHouseBuilt}.");
                        }
                        else
                        {
                            hideCoroutine = StartCoroutine(ShowAndHideNotification($"벼가 부족합니다. 보유 벼 개수 : {PGI.CountItem("Rice(Clone)")}/2", 3f));
                            BadNoticeSFX.Play();
                        }
                    }
                    else
                    {
                        hideCoroutine = StartCoroutine(ShowAndHideNotification("이곳엔 이미 집이 있습니다. 다른곳에 건설해주세요", 3f));
                        //소리이펙트
                        BadNoticeSFX.Play();
                    }
                }
            }
            else
            {
                hideCoroutine = StartCoroutine(ShowAndHideNotification("근처에 집터가 없습니다. 집터 근처에서 B를 눌러주세요", 3f));

            }
        }
    }



    // 알림 표시 후 일정 시간이 지난 후에 숨기는 코루틴
    private IEnumerator ShowAndHideNotification(string message, float duration)
    {
        NoText.text = message;
        NotificationUI.GetComponent<CanvasGroup>().alpha = 1f;

        // 이전에 실행 중인 숨기기 코루틴이 있다면 중지
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // 일정 시간 기다린 후에 알파 값을 변경하여 숨김
        yield return new WaitForSeconds(duration);

        NotificationUI.GetComponent<CanvasGroup>().alpha = 0f;

        // 숨김 코루틴을 null로 초기화하여 다시 사용 가능하도록 함
        hideCoroutine = null;
    }

    public void UseSetMyHousRPC()
    {
        int viewId = MyHouse.GetComponent<PhotonView>().ViewID;
        PV.RPC("SetMyHouseRPC", RpcTarget.Others, viewId);
    }

    [PunRPC]
    public void SetMyHouseRPC(int viewId)
    {
        MyHouse = PhotonView.Find(viewId).gameObject;
        isPigHouseBuilt = true;
    }
    public GameObject GetMyHouse()
    {
        return MyHouse;
    }


    //소리이펙트
    [PunRPC]
    public void BadNoticeAudioPlay(bool TF)
    {
        if (TF)
        {
            BadNoticeSFX.Play();
        }
        else
        {
            BadNoticeSFX.Stop();
        }
    }


}



//public void HouseDupleCheck()
//{
//    if (InputB)
//    {
//        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3f, houseLayer);
//        if (hitColliders.Length > 0)
//        {
//            foreach (var hitCollider in hitColliders)
//            {
//                BuildableSite buildableSite = hitCollider.GetComponent<BuildableSite>();

//                if (buildableSite != null)
//                {

//                    if (PGI.CountItem("Rice(Clone)") >= 2)
//                    {
//                        if (!isPigHouseBuilt && !buildableSite.IsHouseBuilt())
//                        {
//                            PGI.DeleteItem("Rice(Clone)");
//                            hideCoroutine = StartCoroutine(ShowAndHideNotification("초가집 건설을 시작합니다.", 3f));
//                            MyHouse = buildableSite.Build();
//                            isPigHouseBuilt = true;
//                            UseSetMyHousRPC();//집 지어졌다고 동기화
//                        }
//                        else if (!isPigHouseBuilt && buildableSite.IsHouseBuilt())
//                        {
//                            hideCoroutine = StartCoroutine(ShowAndHideNotification("이곳엔 이미 집이 있습니다. 다른곳에 건설해주세요", 3f));
//                            BadNoticeSFX.Play();
//                        }
//                        else if (isPigHouseBuilt && !buildableSite.IsHouseBuilt())
//                        {
//                            hideCoroutine = StartCoroutine(ShowAndHideNotification("현재 집이 부서져야 다른곳에 지을 수 있습니다.", 3f));
//                            BadNoticeSFX.Play();
//                        }
//                        else if (isPigHouseBuilt && buildableSite.IsHouseBuilt())
//                        {
//                            hideCoroutine = StartCoroutine(ShowAndHideNotification("이곳엔 이미 집이 있습니다. 다른곳에 건설해주세요.", 3f));
//                            BadNoticeSFX.Play();
//                        }
//                        //Debug.Log($"돼지근처의 집터에 집이 있는지의여부:{buildableSite.IsHouseBuilt()}.");
//                        //Debug.Log($"돼지가 집을 지었는지 여부:{isPigHouseBuilt}.");
//                    }
//                    else
//                    {
//                        hideCoroutine = StartCoroutine(ShowAndHideNotification($"벼가 부족합니다. 보유 벼 개수 : {PGI.CountItem("Rice(Clone)")}/2", 3f));
//                        BadNoticeSFX.Play();
//                    }
//                }
//                else
//                {
//                    hideCoroutine = StartCoroutine(ShowAndHideNotification("이곳엔 이미 집이 있습니다. 다른곳에 건설해주세요", 3f));
//                    //소리이펙트
//                    BadNoticeSFX.Play();
//                }
//            }
//        }
//        else
//        {
//            hideCoroutine = StartCoroutine(ShowAndHideNotification("근처에 집터가 없습니다. 집터 근처에서 B를 눌러주세요", 3f));

//        }
//    }
//}
