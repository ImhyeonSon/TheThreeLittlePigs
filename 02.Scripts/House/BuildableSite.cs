using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class BuildableSite : MonoBehaviour
{
    //돼지 충돌감지 
    private bool isOnHitPig = false;
    // 안내 메시지 관련 변수
    public Canvas Canvas;
    public GameObject NotificationUI;
    public TextMeshProUGUI NoText;

    public Transform sitePosition; // 부지의 위치
    private float halfExtents = 10f; // 부지의 변의 길이의 절반
    public GameObject buildingHousePrefab;
    public GameObject houseSiteFence;

    public bool isBuilt = false; // 집이 지어져 있는가?
    bool isBuildable = false; // 집을 지을 수 있는가?(좌표)

    bool bDown = false;

    public Transform housePosition; // 집이 지어질 위치. 큐브의 정 가운데 지어진다(부지와 별개임)

    public GameObject upgradeParticle;

    private PhotonView PV; // 포톤 뷰

    private GameObject prfBuiltBar; // 유저들에게 보여질 건설 상태 바
    private GameObject canvas;
    private RectTransform builtBar;
    private Image curBuiltBar;

    // newHouse => MyHouse로 교체
    private GameObject MyHouse;

    // 집 건설 소리
    public AudioSource BuildSFX;

    //private GameObject house;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        canvas = transform.Find("BuiltBar").gameObject;
        prfBuiltBar = canvas.transform.Find("BgBuiltBar").gameObject;
        builtBar = prfBuiltBar.GetComponent<RectTransform>();
        curBuiltBar = builtBar.transform.GetChild(0).GetComponent<Image>();
        houseSiteFence = transform.Find("HouseSiteFence").gameObject;

    }
    private void Update()
    {
        GetInput();
        IsPositionBuildable();  
    }

    void GetInput()
    {
        bDown = Input.GetButtonDown("Build");
    }

    void IsPositionBuildable()
    {
        // 부지의 중심 위치와 크기를 설정
        Vector3 center = sitePosition.position;
        Vector3 size = new Vector3(halfExtents * 2, 6f, halfExtents * 2); // 부지의 너비, 높이, 길이

        // 돼지 플레이어가 있는 레이어 마스크 설정 (레이어 16)
        int pigLayer = 1 << 16;

        // Physics.OverlapBox를 사용하여 해당 영역 내에 돼지 플레이어가 있는지 확인
        Collider[] hitColliders = Physics.OverlapBox(center, size / 2, Quaternion.identity, pigLayer);

        if (hitColliders.Length > 0)
        {
            // 돼지 플레이어가 부지 내에 있음
            isBuildable = true;
            // 안내 메세지 관련   
        }
        else
        {
            // 돼지 플레이어가 부지 내에 없음
            isBuildable = false;
        }
    }

    public GameObject Build()  //돼지쪽에서 호출하도록 변경
    {
        bool canBuild = isBuildable && !isBuilt;
        if (canBuild && isOnHitPig)
        {
            //StartCoroutine(StartBuildHouse());


            
            PV.RPC("ReadyToBuildHouse", RpcTarget.All);
            Vector3 modifiedPosition = housePosition.position + new Vector3(2, 6f - 9.43f, -4f);  // x, y, z 좌표 수정  // 9.43f -> 집 올리는 효과 위해 일부러 일정 부분 내림
            //if (PV.IsMine)
            //{
            Debug.Log("집 짓기");
            MyHouse = PhotonNetwork.Instantiate("House", modifiedPosition, buildingHousePrefab.transform.rotation, 0);
            PV.RPC("SetHouse", RpcTarget.All, MyHouse.GetComponent<PhotonView>().ViewID, false);
            return MyHouse;
            //}
            //Vector3 modifiedPosition = housePosition.position + new Vector3(2, 6f, -4f);  // x, y, z 좌표 수정
            ////GameObject newHouse = Instantiate(buildingHousePrefab, modifiedPosition, buildingHousePrefab.transform.rotation);
            //GameObject newHouse = PhotonNetwork.Instantiate("House", modifiedPosition, buildingHousePrefab.transform.rotation, 0);
            //PV.RPC("SetHouse", RpcTarget.All, newHouse.GetComponent<PhotonView>().ViewID, false);
        }
        return null;
    }



    [PunRPC]
    void ReadyToBuildHouse()
    {
        Debug.Log("건설을 시작합니다.");
        //isBuilt = true;
        //float buildDuration = 7f; // 건설 시간을 7초로 설정
        //float buildTime = 0; // 현재 건설 시간을 0으로 초기화

        //builtBar.gameObject.SetActive(true);
        upgradeParticle.SetActive(true);
        BuildSFX.Play();

        //StartCoroutine(StartBuildHouse());
        ShowBuildBar(); // 건설 상태 바를 보여주는 함수 호출
    }

    private IEnumerator StartBuildHouse(GameObject house)
    {
        //Debug.Log("건설을 시작합니다.");
        isBuilt = true;
        float buildDuration = 7f; // 건설 시간을 7초로 설정
        float buildTime = 0; // 현재 건설 시간을 0으로 초기화

        // 울타리 목표 위치
        Vector3 fenceFinalPosition = houseSiteFence.transform.position - new Vector3(0, 3.18f, 0);
        // 집 목표 위치
        Vector3 houseFinalPosition = house.transform.position + new Vector3(0, 9.43f, 0);

        ////builtBar.gameObject.SetActive(true);
        //upgradeParticle.SetActive(true);
        //Vector3 modifiedPosition = housePosition.position + new Vector3(2, 6f-9.43f, -4f);  // x, y, z 좌표 수정  // 9.43f -> 집 올리는 효과 위해 일부러 일정 부분 내림

        //if (PV.IsMine)
        //{
        //    GameObject newHouse = PhotonNetwork.Instantiate("House", modifiedPosition, buildingHousePrefab.transform.rotation, 0);
        //    PV.RPC("SetHouse", RpcTarget.All, newHouse.GetComponent<PhotonView>().ViewID, false);
        //}

        // 건설 진행 상태를 업데이트하는 루프
        while (buildTime < buildDuration)
        {
            //Debug.Log(buildTime / buildDuration + "시간 비교");
            buildTime += Time.deltaTime;
            curBuiltBar.fillAmount = buildTime / buildDuration; // 진행도 업데이트
            yield return null;
        }

        curBuiltBar.fillAmount = 1; // 건설 상태 바를 100%로 설정

        // 건설 완료 후 건설 상태 바 숨기기
        builtBar.gameObject.SetActive(false);
        upgradeParticle.SetActive(false);


        float buildAnimTime = 0f;
        float buildAnimDuration = 7f;

        while (buildAnimTime < buildAnimDuration)
        {
            buildAnimTime += Time.deltaTime;
            // 울타리 밑으로 내리기
            houseSiteFence.transform.position = Vector3.Lerp(houseSiteFence.transform.position, fenceFinalPosition, buildAnimTime / buildAnimDuration);
            // 집을 위로 올리기
            house.transform.position = Vector3.Lerp(house.transform.position, houseFinalPosition, buildAnimTime / buildAnimDuration);
            yield return null;
        }
        houseSiteFence.SetActive(false);
    }

    private void ShowBuildBar()
    {
        builtBar.gameObject.SetActive(true); // 건설 상태 바 활성화
        curBuiltBar.fillAmount = 0; // 건설 상태 바의 진행도를 0으로 초기화
    }

    [PunRPC]
    public void SetHouse(int PVID, bool active)
    {
        GameObject house = PhotonView.Find(PVID).gameObject;
        //Debug.Log("집 건설 위치" + house.transform.position.ToString());
        house.GetComponent<DurabilityManager>().buildableSite = this;
        GameObject upgradeCanvas = transform.Find("UpgradeBar").gameObject;
        house.GetComponent<HouseManager>().canvas = upgradeCanvas;
        house.GetComponent<HouseManager>().prfBuiltBar = upgradeCanvas.transform.Find("BgUpgradeBar").gameObject;
        house.GetComponent<HouseManager>().sitePosition = this.sitePosition;
        house.GetComponent<HouseManager>().halfExtents = this.halfExtents;
        house.GetComponent<HouseManager>().builtBar = upgradeCanvas.transform.Find("BgUpgradeBar").gameObject.GetComponent<RectTransform>();
        StartCoroutine(StartBuildHouse(house));
    }

//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        Vector3 center = sitePosition.position;
//        Vector3 size = new Vector3(halfExtents * 2, 6f, halfExtents * 2);

//        Handles.color = isBuildable ? Color.green : Color.red;
//        Handles.DrawWireCube(center, size);
//    }
//#endif

    public bool IsHouseBuilt()
    {
        //Debug.Log("돼지와 buildabeSite가 충돌했습니다");
        isOnHitPig = true;
        //Debug.Log("buildable site가 돼지에게 집 완성여부를 불린형으로 알려줍니다.");
        return isBuilt;
    }
}
