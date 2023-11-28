using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonGameManager : MonoBehaviourPunCallbacks
{

    GameObject playerCharacter;
    PlayerStatus PS; // 내 캐릭터만의 PS다.
    PhotonView PV;
    Transform[] points;
    Transform wolfStartPoints;
    Transform pigStartPoints;
    public FarmingObjectManager FOM;
    public Image portraitImg;

    // 게임 종료 Canvas
    public GameObject WinCanvas;
    public GameObject LoseCanvas;

    // 게임 시작 변수
    int playerCount = 0;
    bool isGameStart=false;

    // 게임 시작 타이머 캔버스
    public GameObject GameStartCanvas;
    public GameObject GameStartImg;
    public Image GameStartCountImg;
    // 타이머 시작 변수
    float defaultStartTime = 3.5f;
    float nowStartTime;
    bool IsTimerOn = false;
    // Object 동기화 한번만
    bool isObjectSync = true;

    bool isGameDone = false;

    int pigPlayer=0;
    int wolfPlayer=0;


    // 미니 맵에 내 캐릭터 img 할당하기
    public Transform MiniMapPigTransform;
    public Transform MiniMapWolfTransform;
    public Transform MiniMapPigHouseTransform;
    //public GameObject myMiniMapObject;

    // 캔버스 키 버튼 끄기
    public GameObject ButtonF2;


    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>(); // 자신의 포톤뷰 가져오기
        points = GameObject.Find("PlayerStartPoints").GetComponentsInChildren<Transform>();
        wolfStartPoints = GameObject.Find("WolfStartPoints").transform;
        pigStartPoints = GameObject.Find("PigStartPoints").transform;
        //GameStartCanvasText = GameStartCanvas.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        portraitImg.sprite = Resources.Load("InGameUI/" + GameManager.Instance.GetMyCharacter(), typeof(Sprite)) as Sprite;
        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
        }
        if (GameManager.Instance.GetMyCharacter() == "Wolf")
        {
            ButtonF2.SetActive(false);
        }
    }

    void Update()
    {
        if (IsTimerOn) 
        {
            StartTimer();
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("입장 성공");
        // 내 캐릭터가 늑대인지 돼지인지 보고 생성
        if (GameManager.Instance.GetMyCharacter() == "Wolf")
        {
            int idx = Random.Range(0, wolfStartPoints.childCount);
            playerCharacter = PhotonNetwork.Instantiate("WolfJay", wolfStartPoints.GetChild(idx).position, wolfStartPoints.GetChild(idx).rotation, 0);

            //playerCharacter = PhotonNetwork.Instantiate("WolfJay", points[idx].position, points[idx].rotation, 0);
        }
        else if (GameManager.Instance.GetMyCharacter() == "Pig")
        {
            int idx = Random.Range(0, pigStartPoints.childCount);
            playerCharacter = PhotonNetwork.Instantiate("Pig_test", pigStartPoints.GetChild(idx).position, pigStartPoints.GetChild(idx).rotation, 0);
        }
        // playerCharacter가 내가 생성한 캐릭터이므로 언제 동기화를 호출하던 내 캐릭터 정보만 보내준다.
        PS = playerCharacter.GetComponent<PlayerStatus>();
        // 미리 생성해 둘 것이라 필요 X
        //PhotonNetwork.Instantiate("FarmingObjectManager", new Vector3(0, 0, 0), Quaternion.identity, 0);
    }

    public void InitializedTrigger()
    {
        PS.InitializeMyCharacter();
    }

    public void OnLoadingCharacterCount()
    {
        playerCount += 1;
        // 게임 승패 판단을 위한 변수
        pigPlayer = GameObject.FindGameObjectsWithTag("Pig").Length;
        wolfPlayer = GameObject.FindGameObjectsWithTag("Wolf").Length;
        Debug.Log("플레이어 수는 : " + playerCount);
        if (PhotonNetwork.IsMasterClient)
        {
            if (playerCount == 6 || playerCount == GameManager.Instance.testVar) // 모든 플레이어의 캐릭터가 생성 되면 시작!
            {
                
                PV.RPC("GameStartTrigger", RpcTarget.All);
                DistributeHeirloom();
                // 미니맵 캐릭터도 마스터 클라이언트가 할당
                AllocateMiniMapPig();
            }
        }
    }

    [PunRPC]
    public void GameStartTrigger() 
    {   // 게임 시작 타이머 On
        
        nowStartTime = defaultStartTime;
        IsTimerOn = true;
    }

    // 게임 시작 타이머
    public void StartTimer()
    { // 캔버스는 로컬에서 관리
        if (nowStartTime > 0)
        {
            nowStartTime -= Time.deltaTime;
            if (nowStartTime < 0.5f)
            {
                GameStartCountImg.gameObject.SetActive(false);
                GameStartImg.SetActive(true);                
            }
            else if (nowStartTime < 1.5f)
            {
                if (isObjectSync&&PhotonNetwork.IsMasterClient)
                {
                    // 게임 시작 직전에 위치 동기화
                    FOM.ObjectSyncronize();
                    isObjectSync = false;
                }
                GameStartCountImg.sprite = Resources.Load("InGameUI/" + "one", typeof(Sprite)) as Sprite;
            }
            else if (nowStartTime < 2.5f)
            {
                GameStartCountImg.sprite = Resources.Load("InGameUI/" + "two", typeof(Sprite)) as Sprite;
            }
        }
        else 
        { // 게임 시작!
            if (PhotonNetwork.IsMasterClient)
            {
                PV.RPC("GameStartRPC", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void GameStartRPC()
    {
        GameStartCanvas.SetActive(false);
        isGameStart = true;
    }

    public bool GetIsGameStart()
    {
        return isGameStart;
    }

    //Timer

    public void DistributeHeirloom()
    {
        if (PhotonNetwork.IsMasterClient)
        { // 마스터 클라이언트가 연산
            GameObject[] Pigs = GameObject.FindGameObjectsWithTag("Pig");
            if (Pigs.Length < 1)
            {
                // 늑대 1인 테스트용
                return;
            }
            int idx = Random.Range(0, Pigs.Length);
            Pigs[idx].GetComponent<Heirloom>().UseHeirloomRPC(true); // 랜덤 유저가 Heirloom을 갖는다.
        }
    }


    public void PigLoseRPC()
    {
        PV.RPC("UsePigLoseRPC", RpcTarget.All);
    }
    [PunRPC]
    public void UsePigLoseRPC()
    {
        PigLose();
    }

    public void PigLose() // RPC를 타 스크립트에서 바로 호출하면 가독성이 떨어져서 분리함.
    {
        if (!isGameDone) // 게임이 끝난 상태가 아니라면 승리, 패배 처리
        {
            isGameDone = true;
            if (GameManager.Instance.GetMyCharacter() == "Wolf")
            {
                WinCanvas.SetActive(true);
            }
            else
            {
                LoseCanvas.SetActive(true);
            }
            StartCoroutine(CoFadeIn(4f, false));
        }
    }

    public void PigWinRPC() // house에서 RPC 사용하지 않아도 코루틴으로 체크하기 때문에 PigWin을 직접 사용함...
    {
        PV.RPC("UsePigWinRPC", RpcTarget.All);
    }
    [PunRPC]
    public void UsePigWinRPC()
    {
        PigWin();
    }

    public void PigWin() // RPC를 타 스크립트에서 바로 호출하면 가독성이 떨어져서 분리함.
    {
        if (!isGameDone) // 게임이 끝난 상태가 아니라면 승리, 패배 처리
        {
            isGameDone = true;
            if (GameManager.Instance.GetMyCharacter() == "Wolf")
            {
                LoseCanvas.SetActive(true);
            }
            else
            {
                WinCanvas.SetActive(true);
            }
            StartCoroutine(CoFadeIn(4f, true));
        }
    }

    IEnumerator CoFadeIn(float fadeOutTime, bool isPigWin)
    {
        Image image;
        if (isPigWin)
        {
            if (GameManager.Instance.GetMyCharacter() == "Wolf")
            {
                image = LoseCanvas.GetComponent<Image>();
            }
            else
            {
                image = WinCanvas.GetComponent<Image>();
            }
        }
        else
        {
            if (GameManager.Instance.GetMyCharacter() == "Wolf")
            {
                image = WinCanvas.GetComponent<Image>();
            }
            else
            {
                image = LoseCanvas.GetComponent<Image>();
            }
        }
        Color tempColor = image.color;
        tempColor.a = 0.3f;
        image.color = tempColor;
        while (tempColor.a < 1f)
        {
            tempColor.a +=((0.7f*Time.deltaTime)/fadeOutTime);
            image.color = tempColor;

            if (tempColor.a >= 1f)
            {
                tempColor.a = 1f;
            }
            yield return null;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            NextScene(isPigWin);
        }
    }



    public void NextScene(bool isPigWin)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (isPigWin)
            {
                SceneManager.LoadScene("GameResultPigWinScene");
            }
            else
            {
                SceneManager.LoadScene("GameResultPigLoseScene");                
            }
        }
        // 다음 씬으로 넘어갈 마스터 클라이언트 함수 & 처리 로직 추가
    }


    public void AllocateMiniMapPig()
    {
        GameObject[] PigObjectList = GameObject.FindGameObjectsWithTag("Pig");
        int viewId;
        for (int idx = 0; idx < PigObjectList.Length; idx++)
        {
            viewId = PigObjectList[idx].GetComponent<PhotonView>().ViewID;
            PV.RPC("AllocateMiniMapPigRPC", RpcTarget.All, idx, viewId);
            PV.RPC("AllocateMiniMapPigHouseRPC", RpcTarget.All, idx, viewId);
        }
        GameObject[] WolfObjectList = GameObject.FindGameObjectsWithTag("Wolf");
        for (int idx = 0; idx < WolfObjectList.Length; idx++)
        {
            viewId = WolfObjectList[idx].GetComponent<PhotonView>().ViewID;
            PV.RPC("AllocateMiniMapWolfRPC", RpcTarget.All, idx, viewId);
        }
    }

    [PunRPC]
    public void AllocateMiniMapPigRPC(int idx, int photonViewId)
    {
        // 돼지는 돼지만 보이게.
        if (GameManager.Instance.GetMyCharacter() == "Pig")
        {
            GameObject myMiniMapObject;
            myMiniMapObject = MiniMapPigTransform.GetChild(idx).gameObject;
            // 할당되면 Object를 킨다.
            myMiniMapObject.SetActive(true);
            myMiniMapObject.GetComponent<MiniMapPosition>().SetMyObject(photonViewId);
        }
    }

    [PunRPC] // 어떤 돼지가 집 아이콘을 쓸지 결정
    public void AllocateMiniMapPigHouseRPC(int idx, int photonViewId)
    {
        // 돼지는 돼지만 보이게.
        if (GameManager.Instance.GetMyCharacter() == "Pig")
        {
            GameObject myMiniMapObject;
            myMiniMapObject = MiniMapPigHouseTransform.GetChild(idx).gameObject;
            // 할당되면 Object를 킨다.
            myMiniMapObject.SetActive(true);
            myMiniMapObject.GetComponent<MiniMapHousePosition>().SetPigCharacter(photonViewId);
        }
    }


    [PunRPC]
    public void AllocateMiniMapWolfRPC(int idx, int photonViewId)
    {
        // 늑대만 보이게.
        if (GameManager.Instance.GetMyCharacter() == "Wolf")
        {
            GameObject myMiniMapObject;
            myMiniMapObject = MiniMapWolfTransform.GetChild(idx).gameObject;
            // 할당되면 Object를 킨다.
            myMiniMapObject.SetActive(true);
            myMiniMapObject.GetComponent<MiniMapPositionWolf>().SetMyObject(photonViewId);
        }
    }

    public GameObject GetMyCharacter()
    {
        return playerCharacter;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        int nowPigPlayers = GameObject.FindGameObjectsWithTag("Pig").Length;
        int nowWolfPlayers = GameObject.FindGameObjectsWithTag("Wolf").Length;
        if (nowPigPlayers - 4 > nowWolfPlayers - 2)
        {   // 돼지가 나갔으므로 돼지 승리
            PigWinRPC();
        }
        else
        { // 늑대가 나갔으므로 늑대 승리
            PigLoseRPC();
        }

    }


}