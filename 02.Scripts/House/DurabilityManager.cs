using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class DurabilityManager : MonoBehaviour
{
    //이 집이 자신의 소유인지 보여주는 변수
    public GameObject TrueCircle;
    public GameObject FalseCircle;


    public int maxDurability = 200;
    public int curDurability = 200;

    public GameObject destroyParticle;
    public GameObject upgradeParticle;
    public GameObject explosionFX;
    public PhotonGameManager PGM;
    public ChestHeirloom CH;

    public BuildableSite buildableSite;

    private GameObject prfDurBar;
    private GameObject canvas;
    public Transform houseTransform; 

    private RectTransform durBar;
    private Image curDurBar;
    private PhotonView PV;

    //알림 메시지 관련
    public FadeController FC;

    //private float windBlowingTimeFlow = 0f;
    //private float windBlowingInterval = 0.5f;
    //private bool windBlowingDamage = false;
    private void Awake()
    {
        PGM = GameObject.Find("PhotonGameManager").GetComponent<PhotonGameManager>();
        canvas = transform.Find("DurationBar").gameObject;
        prfDurBar = canvas.transform.Find("bgDurBar").gameObject;
        durBar = prfDurBar.GetComponent<RectTransform>();
        curDurBar = durBar.transform.GetChild(0).GetComponent<Image>();
        PV = GetComponent<PhotonView>();
        //알림 메시지 관련
        FC = GameObject.Find("Canvas").transform.Find("NotificationUI").GetComponent<FadeController>();
    }

    private void Start()
    {
        prfDurBar.SetActive(false);  // 처음엔 내구도가 다 차있으므로 비활성화
        //durBar = Instantiate(prfDurBar, canvas.transform).GetComponent<RectTransform>();
        //buildableSite = transform.parent.gameObject.GetComponent<BuildableSite>();
        CheckExist();
    }
    private void Update()
    {
        //UpdateDurabilityBarPosition();
        CheckExist(); // 항상 검사할 필요 X 실제로 Durability가 바뀔때만 검사하면 됨.
        //ReadyToShowDurationBar();
        //CheckWindBlowingTime();
        CheckIsMine();
    }
    public void Initialize()
    {
        //StartCoroutine(durabilitydecay());
    }

    private void ReadyToShowDurationBar()
    {
        PV.RPC("UpdateDurabilityBar", RpcTarget.All);
    }

    //[PunRPC]
    //// 내구도 피통 구현
    //private void UpdateDurabilityBar()
    //{
    //    if (curDurability == maxDurability || curDurability == 0)
    //    {
    //        // 내구도 풀이거나 0이 아닌 경우는 bar 보여주지 않는다.
    //        durBar.gameObject.SetActive(false);
    //    }
    //    else
    //    {
    //        durBar.gameObject.SetActive(true);
    //        curDurBar.fillAmount = (float)curDurability / (float)maxDurability;
    //    }
    //}


    public void UpdateDurabilityBar()
    {
        if (curDurability == maxDurability || curDurability == 0)
        {
            Debug.Log("여긴 왜 안 오는데???");
            // 내구도 풀이거나 0이 아닌 경우는 bar 보여주지 않는다.
            prfDurBar.SetActive(false);
        }
        else
        {
            UpdateDurabilityBarPosition();
            prfDurBar.SetActive(true);
            curDurBar.fillAmount = (float)curDurability / (float)maxDurability;
        }
    }

    // 내구도 바 위치 잡아주는 함수
    private void UpdateDurabilityBarPosition()
    {
        if (houseTransform != null && durBar != null)
        {
            durBar.gameObject.SetActive(true);
            Vector3 housePosition = houseTransform.position;
            Vector3 barPosition = housePosition + new Vector3(-2, 6, 4); // 집의 정 중앙 위에 뜨도록 위치 보정
            durBar.position = barPosition;
        }
    }

    [PunRPC]
    // 늑대 기본공격시 집 내구도 깎이는 함수
    public void ReceiveWolfAttack(int Damage)
    {
        if (curDurability > 0) //
        {
            curDurability -= Damage;
            Debug.Log("최대 체력: " + maxDurability + " 현재 내구도: " + curDurability);
            CheckExist(); // durability 반영
            UpdateDurabilityBar();
        }
    }


    ///// <summary>
    ///// 삭제 예정
    ///// </summary>
    //// 늑대 바람 불기 이펙트와 얼마나 접촉하는지 시간 체크
    //public void CheckWindBlowingTime()
    //{
    //    windBlowingTimeFlow += Time.deltaTime;
    //    if (windBlowingTimeFlow >= windBlowingInterval)
    //    {
    //        windBlowingTimeFlow = 0f;
    //        windBlowingDamage = true;
    //    }
    //}
    //// 늑대 바람 불기 이펙트와 집 충돌시 실행
    //public void OnTriggerStay(Collider collision)
    //{
        
    //    if (collision.tag == "WindBlowing" && windBlowingDamage == true)
    //    {
    //        int Damage = collision.GetComponentInParent<WolfStatus>().GetDamage() * 2;
    //        windBlowingDamage = false;
    //        PV.RPC("ReceiveWolfWindBlowing", RpcTarget.All, Damage);
    //    }
    //}
    //[PunRPC]
    //// 늑대 바람 불기 사용시 집 내구도 깎이는 함수
    //public void ReceiveWolfWindBlowing(int Damage)
    //{
    //    curDurability -= Damage;
    //    Debug.Log("House에 바람불기");
    //    Debug.Log("최대 체력: " + maxDurability + " 현재 내구도: " + curDurability);
    //    UpdateDurabilityBar();
    //}


    public void UpdateStats(int curLevel) { 
        if (curLevel == 2)
        {
            curDurability = 400;
        } else if (curLevel == 3)
        {
            curDurability = 600;
        }
        maxDurability = curDurability;
    }
    private IEnumerator durabilitydecay()
    {
        while (buildableSite.isBuilt) // 오브젝트가 존재하는 동안 반복
        {
            //yield return null;
            yield return new WaitForSeconds(0.5f);
            curDurability -= 10;
            Debug.Log("최대 체력: " + maxDurability + " 현재 내구도: " + curDurability);

        }
    }

    void CheckExist() // 내구도 지속적으로 체크
    {
        if (curDurability <= 0)
        {
            PV.RPC("DestroyHouse", RpcTarget.All);
            //DestroyHouse();
            // 돼지에게 집 파괴 사실을 알린다.
            PV.RPC("InformPigHouseDestroyed", RpcTarget.All);
        }
        else if (curDurability <= 50)
        {
            PV.RPC("ShowDestroyEffect", RpcTarget.All);
            //ShowDestroyEffect();
        }
        else
        {
            destroyParticle.SetActive(false);
        }
    }

    [PunRPC]
    void ShowDestroyEffect()
    {
        destroyParticle.SetActive(true);
    }

    void HiddenAllObject()
    {
        destroyParticle.SetActive(false);
        durBar.gameObject.SetActive(false);
        upgradeParticle.SetActive(false);
        explosionFX.SetActive(false);
        buildableSite.isBuilt = false;
        Destroy(gameObject);
        //buildableSite.houseSiteFence.SetActive(true);
    }

    [PunRPC]
    public void DestroyHouse()
    {
        Debug.Log("아이고, 내 집 부서지네!");
        buildableSite.houseSiteFence.SetActive(true);
        destroyParticle.SetActive(false);
        explosionFX.SetActive(true);
        StartCoroutine(StartDestroy());
        // 패배 조건 호출 추가
        if (CH.GetHaveHeirloom()) // 가보가 있다면 패배
        {
            // RPC로 각자 GameManager에서 호출함
            PGM.PigLose();
        }
        //HiddenAllObject();
        //// 울타리 위로 올리기
    }

    private IEnumerator StartDestroy()
    {
        Debug.Log("집 무너지는 중..");
        float destroyDuration = 4f;
            float destroyTime = 0;

        Vector3 fenceFinalPosition = buildableSite.houseSiteFence.transform.position + new Vector3(0, 3.18f, 0);
        Vector3 houseFinalPosition = transform.position - new Vector3(0, 9.43f, 0);

        while (destroyTime < destroyDuration)
        {
            destroyTime += Time.deltaTime;
            if (destroyTime >  destroyDuration)
            {
                destroyTime = destroyDuration;
            }
            Debug.Log(destroyTime / destroyDuration + "시간 비교");
            // 울타리 위로 올리기
            buildableSite.houseSiteFence.transform.position = Vector3.Lerp(buildableSite.houseSiteFence.transform.position, fenceFinalPosition, destroyTime / destroyDuration);
            // 집을 밑으로 내리기
            Vector3 housePosition = transform.position;
            transform.position = Vector3.Lerp(housePosition, houseFinalPosition, destroyTime / destroyDuration);
            yield return null;
        }
        HiddenAllObject();
    }


    //PV.isMine으로 돼지의 집이 파괴 된 상태임을 알리는 함수
    [PunRPC]
    public void InformPigHouseDestroyed()
    {
        if (PV.IsMine)
        {
            // 모든 돼지 객체를 찾아옴
            GameObject[] pigs = GameObject.FindGameObjectsWithTag("Pig"); // "Pig" 태그를 가진 돼지 객체를 찾음

            foreach (var pig in pigs)
            {
                // 돼지 객체의 Photon View를 가져옴
                PhotonView pigPV = pig.GetComponent<PhotonView>();
                if (pigPV.IsMine)
                {
                    // 돼지 객체의 PigHouseManager 스크립트를 가져옴
                    PigHouseManager pigHouseManager = pig.GetComponent<PigHouseManager>();
                    // PigHouseManager 스크립트에 접근하여 필요한 작업 수행
                    pigHouseManager.isPigHouseBuilt = false; // 예시로 isPigHouseBuilt 값을 변경
                    Debug.Log("돼지야 집 부서졌다. 다시 지어라,라고 PV로 알려주기");
                    FC.StartFadeIn("늑대가 내 집을 부쉈어요!");
                }
            }
        }     
    }


    private void CheckIsMine()
    {
        if (PV.IsMine)
        {
            TrueCircle.SetActive(true);
            FalseCircle.SetActive(false);
        }
        else
        {
            TrueCircle.SetActive(false);
            FalseCircle.SetActive(true);
        }
    }

}
