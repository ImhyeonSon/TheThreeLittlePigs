using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
//using System.Drawing;
using TMPro;
using UltimateClean;
using Unity.VisualScripting;
//using UnityEditor.XR;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WaitingPhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0f";

    public GameObject loadingCanvas; // 로딩 캔버스를 참조하기 위한 변수
    GameManager gameManager;
    private string nickName;

    public GameObject FailLackPersonPopup;
    public GameObject ChangeSeatPopup;

    // 캔버스에 띄울 메시지
    public GameObject myCharacterMessage;

    // 게임 시작 버튼
    public GameObject gameStartButton;
    private Button gameButton;
    public TextMeshProUGUI gameButtonText;

    // 캔버스 동기화 변수
    public PhotonView PV;
    public Transform PigLayOut;
    public Transform WolfLayOut;
    public Transform[] PigList;
    public Transform[] WolfList;

    public TextMeshProUGUI PigCountCanvas;
    public TextMeshProUGUI WolfCountCanvas;
    public TextMeshProUGUI PlayerCountCanvas;
    public int pigCount;
    public int wolfCount;
    public int playerCount;

    // 동기화 변수
    TextMeshProUGUI PigPrevSeat;
    TextMeshProUGUI PigToSeat;
    TextMeshProUGUI WolfPrevSeat;
    TextMeshProUGUI WolfToSeat;


    public GameObject PigAsset;
    public GameObject WolfAsset;


    public string[] PigPlayerList = new string[6];
    public string[] WolfPlayerList = new string[6];

    string mySeat;
    TextMeshProUGUI myTextSeat;
    TextMeshProUGUI selectTextSeat;

    public GameObject[] PigMasterBadge;
    public GameObject[] WolfMasterBadge;

    // 자리 배치 시 로컬 시간차로 동기화 순서가 다르기 때문에 변수를 줘서 확인
    Room currentRoom;

    // 게임 시작 시 3초 로딩
    public GameObject GameLoadingCanvas;
    public TextMeshProUGUI myCharacterText;
    public GameObject myCharacterPig;
    public GameObject myCharacterWolf;
    public Slider loadingBar; // Slider
    public TextMeshProUGUI loadingBarText;
    public float gameLoadingTime = 3.0f; // 로딩 시간 (3초)

    void Awake()
    {
        currentRoom=PhotonNetwork.CurrentRoom;
        // 게임 매니저 찾아오기
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //
        nickName = gameManager.nickName;
        // 로딩 캔버스 활성화
        loadingCanvas.SetActive(true);
        // 같은 룸의 유저들에게 자동으로 씬을 로딩
        PhotonNetwork.AutomaticallySyncScene = true;
        GameLoadingCanvas.SetActive(false);
        gameButton = gameStartButton.GetComponent<Button>();
        gameButton.interactable = false;
        //.interactable = false;
        // 없어도 될 듯
        //Debug.Log("현재 게임 버전: " + PhotonNetwork.GameVersion);
        //Debug.Log("현재 닉네임: " + PhotonNetwork.NickName);
        //// 같은 버전의 유저끼리 접속 허용
        //PhotonNetwork.GameVersion = version;
        //// 유저 아이디 할당
        //PhotonNetwork.NickName = nickName;


        // 돼지와 늑대 버튼 가져오기 배치하기
        InitializeSeat();


        if (PhotonNetwork.InRoom)
        {
            Debug.Log("안되면 여기 넣어야지 뭐..");
        }
        // 유저 Character 할당
        
    }

    void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            OnJoinedRoom();
        }
        else
        {
            Debug.Log("방에 없음");
        }
        // 방장인 경우에만 button 실행 되게 하기
        if (PhotonNetwork.IsMasterClient)
        {
            DefaultText.SetActive(false);
            gameButton.interactable = true;
            // 자리 세팅 실행
            CanvasSynchronization();
        }
    }

    // 방장이 바뀐 경우 다른 플레이어에게도 버튼이 보이게 함
    public override void OnMasterClientSwitched(Player newMasterClient)
    { // Callback 함수
        SwitchGameStartButton();
    }
    // 방장권한 위임시 사용할 함수

    public GameObject DefaultText;
    public void SwitchGameStartButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("방장입니다.");
            DefaultText.SetActive(false);
            gameButtonText.color = new Color(255, 255, 255, 255);
            gameButton.interactable = true;
        }
        else
        {
            Debug.Log("방장이 아닙니다.");
            gameButtonText.color = new Color(137, 137, 137, 255);
            gameButton.interactable = false;
        }
    }


    // Photon Event 함수들
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 조인 실패");
    }

    // 방 조인 시 콜백 함수
    public override void OnJoinedRoom()
    {
        loadingCanvas.SetActive(false);
    }

    // 방 생성 시 콜백 함수
    public override void OnCreatedRoom()
    {
        loadingCanvas.SetActive(false);
    }

    // 새 플레이어 조인 시 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        //Debug.Log("New Player Entered");
        JoinCanvasSynchronization();
    }

    // 플레이어가 나갔을 때 콜백 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        syncroPlayerCount = 0;
        // 모든 플레이어를 비어있음으로 만들 함수추가
        ResetSeatName();
        // 파싱으로 현재 위치 구분
        string[] words = mySeat.Split(' ');
        //Debug.Log("index: "+words[1]);
        PV.RPC("LeftRoomPlayerSynchronization",RpcTarget.All,words, nickName);
        // 플레이어 정보 동기화
    }

    public void ResetSeatName()
    {
        for (int i = 0; i < 6; i++)
        {
            PigList[i].GetComponentInChildren<TextMeshProUGUI>().text="";
            WolfList[i].GetComponentInChildren<TextMeshProUGUI>().text="";
        }
    }
    

    [PunRPC]
    public void LeftRoomPlayerSynchronization(string[] words, string playerNickName)
    {
        // RPC로 받아서 돼지, 늑대로 구분
        if (words[0] == "Pig")
        {
            PigList[Int32.Parse(words[1])-1].GetComponentInChildren<TextMeshProUGUI>().text = playerNickName;
        }
        else // 늑대라면
        {
            WolfList[Int32.Parse(words[1])-1].GetComponentInChildren<TextMeshProUGUI>().text = playerNickName;
        }
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트의 정보만 동기화 하면 됨
        {
            string[] parseData = mySeat.Split(' ');
            PV.RPC("MasterBadgeRPC", RpcTarget.All, Int32.Parse(parseData[1]) - 1, parseData[0]);
        }
        // RPC호출시 +1로 플레이어 정보를 다 받았는지 검증 가능
        LeaveUserSyncro();
    }


    // 캔버스 동기화 함수들
    public void JoinCanvasSynchronization() // 새 플레이어가 처음 들어왔을 때
    {
        Debug.Log("새 플레이어가 들어와서 동기화 합니다.");
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트 기준으로 동기화
        {
            CanvasSynchronization();
        }
    }

    public void CanvasSynchronization()
    {
        for (int i = 0; i < 6; i++)
        {
            PigPlayerList[i] = PigList[i].GetComponentInChildren<TextMeshProUGUI>().text;
            WolfPlayerList[i] = WolfList[i].GetComponentInChildren<TextMeshProUGUI>().text;
        }
        PV.RPC("SetCanvasStatus", RpcTarget.All, PigPlayerList, WolfPlayerList);
        if (PhotonNetwork.IsMasterClient) // 마스터 클라이언트의 정보만 동기화 하면 됨
        {
            string[] parseData = mySeat.Split(' ');
            PV.RPC("MasterBadgeRPC", RpcTarget.All, Int32.Parse(parseData[1]) - 1, parseData[0]);
        }
    }

    [PunRPC]
    public void SetCanvasStatus(string[] PigListRPC, string[] WolfListRPC)
    {
        //Debug.Log("Canvas동기화 호출");
        for (int i = 0; i < 6; i++) // 플레이어 List를 받아서 캔버스 이름 동기화
        {
            PigList[i].GetComponentInChildren<TextMeshProUGUI>().text = PigListRPC[i];
            WolfList[i].GetComponentInChildren<TextMeshProUGUI>().text = WolfListRPC[i];
        }
        if (GameManager.Instance.GetMyCharacter() == null) //처음에만 호출하는 함수
        {

            // 처음에는 무조건 돼지로 배치
            SeatInPig();
            // 자리에 배치 후 캔버스 동기화
            CanvasSynchronization();
        }
        if (currentRoom == null)
        {
            //Debug.Log("null 입니다" + currentRoom);
            currentRoom = PhotonNetwork.CurrentRoom;
        }
        else
        {
            //Debug.Log("방 이름 : " + currentRoom.Name);
            //Debug.Log("방 최대 플레이어 수 : " + currentRoom.MaxPlayers);
            //Debug.Log("방에 있는 플레이어 수 " + currentRoom.PlayerCount);
        }
        SetCharacterNumber();
    }

    // List 초기화 함수
    public void InitializeSeat()
    {
        int childCount = PigLayOut.childCount;
        PigList = new Transform[childCount]; // 초기화
        WolfList = new Transform[childCount]; // 초기화
        for (int i = 0; i < childCount; i++)
        {
            PigList[i] = PigLayOut.GetChild(i);
            WolfList[i] = WolfLayOut.GetChild(i);
        }
    }

    // 돼지, 늑대 캐릭터를 선택하는 창
    public void SelectCharacter(bool isPig)
    {
        SelectEvent();
        // 자리가 비어있을때만 바꾸기
        if (selectTextSeat.text.Trim() == "") // 비어있다면
        {
            ChangeSeatPopup.SetActive(true);
            myTextSeat.text = "";
            myTextSeat.color = new Color(255, 255, 255, 255); // 이전 자리 색 바꾸고
            myTextSeat = selectTextSeat;
            myTextSeat.text = nickName;
            //Debug.Log("내자리는 : " + myTextSeat.name);
            //Debug.Log("인덱스 알고 있나?"+myTextSeat.)
            myTextSeat.color = new Color(0, 188, 255, 255); // 내 캐릭터만색 바꾸기
            SelectPigOrWolfTeam(isPig);
            // 자리를 바꾼 상태 동기화
            CanvasSynchronization();
            // 방장 상태도 동기화
        }
    }

    [PunRPC]
    public void MasterBadgeRPC(int onIdx, string character)
    {
        if (character == "Wolf")
        {
            for (int i = 0; i<6; i++)
            {
                PigMasterBadge[i].SetActive(false);
                if (i == onIdx)
                {
                    WolfMasterBadge[i].SetActive(true);
                }
                else
                {
                    WolfMasterBadge[i].SetActive(false);
                }
            
            }
        }
        else
        {
            for (int i = 0; i < 6; i++)
            {
                WolfMasterBadge[i].SetActive(false);
                if (i == onIdx)
                {
                    PigMasterBadge[i].SetActive(true);
                }
                else
                {
                    PigMasterBadge[i].SetActive(false);
                }

            }
        }
    }

    // 버튼 클릭 시 발생하는 이벤트
    public void SelectEvent()
    {
        // 클릭한 버튼을 가져오기
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        //Debug.Log(clickObject);
        // 이름 저장
        mySeat = clickObject.name;
        //Debug.Log(mySeat);
        // 클릭한 버튼을 selectTextSeat에 저장
        selectTextSeat = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>();
        //Debug.Log(selectTextSeat.text);
    }

    // 
    public void SelectPigOrWolfTeam(bool isPig)
    {
        if (isPig)
        {
            gameManager.SetMyCharacter("Pig");
            PigAsset.SetActive(true);
            WolfAsset.SetActive(false);
        }
        else
        {
            gameManager.SetMyCharacter("Wolf");
            PigAsset.SetActive(false);
            WolfAsset.SetActive(true);
        }
        //myCharacterMessage.GetComponent<TextMeshProUGUI>().text = "내가 선택한 캐릭터는 " + gameManager.GetMyCharacter();
    }

    // 처음 들어올 때 돼지로 자리 배치
    public void SeatInPig()
    {
        for (int idx = 0; idx < 6; idx++)
        {
            // 비어있으면 그 자리를 채워준다.
            if (PigList == null)
            {
                Debug.Log("Null 오류");
            }
            if (PigList[idx].GetComponentInChildren<TextMeshProUGUI>().text.Trim() == "")
            {
                // 현재 내가 선택한 Text를 가리킨다.
                //Debug.Log("새로 자리를 배치 받은 플레이어" + nickName + "| 위치는 : " + idx);
                myTextSeat = PigList[idx].GetComponentInChildren<TextMeshProUGUI>();
                myTextSeat.text = nickName;
                //Debug.Log("내자리는 : " + myTextSeat.name);
                myTextSeat.color = new Color(0, 255, 203, 255); // 내 캐릭터만색 바꾸기
                mySeat = "Pig "+(idx+1);
                if (PhotonNetwork.IsMasterClient) // 방장일 경우 동기화
                {
                    string[] parseData = mySeat.Split(' ');
                    PV.RPC("MasterBadgeRPC", RpcTarget.All, Int32.Parse(parseData[1]) - 1, parseData[0]);
                }
                //Debug.Log(mySeat);
                // 돼지 캐릭터를 선택 함
                SelectPigOrWolfTeam(true);
                return;
            }
        }
    }

    public void SetCharacterNumber()
    {
        pigCount = 6;
        wolfCount = 6;
        for (int i = 0; i < 6; i++)
        {
            if (PigList[i].GetComponentInChildren<TextMeshProUGUI>().text.Trim() == "")
            {
                pigCount -= 1;
            }
            else 
            {
                //Debug.Log(PigList[i].GetComponentInChildren<TextMeshProUGUI>().text);
            }
            if (WolfList[i].GetComponentInChildren<TextMeshProUGUI>().text.Trim() == "")
            {
                wolfCount -= 1;
            }
            else 
            {
                //Debug.Log(WolfList[i].GetComponentInChildren<TextMeshProUGUI>().text);
            }
        }
        playerCount = pigCount + wolfCount;
        PigCountCanvas.text = "돼지 : " + pigCount+"명";  
        WolfCountCanvas.text = "늑대 : " + wolfCount+"명";  
        PlayerCountCanvas.text = "플레이어 (" + playerCount+"/6)";  
    }
  
    public void GameStartEvent()
    {
        if (currentRoom==null)
        {
            currentRoom = PhotonNetwork.CurrentRoom;
        }
        //if (currentRoom.PlayerCount >= 1) // 현재 한명만 있어도 시작 가능
        if (currentRoom.PlayerCount == 6)// => 나중에 바꿀 코드
        {
            PlayerDistribution();
            PhotonNetwork.CurrentRoom.IsOpen=false; // 방 닫기
            Debug.Log("인원이 찼으므로 게임 시작!");
            // Test 변수
            gameManager.testVar = playerCount;
            // 로딩창 보여주기
            PV.RPC("GameStartEventRPC", RpcTarget.All);
        }
        else
        {
            FailLackPersonPopup.SetActive(true);
            Debug.Log("인원이 부족합니다!");
        }
    }

    public void TestGameStart()
    {
        PlayerDistribution();
        PhotonNetwork.CurrentRoom.IsOpen = false; // 방 닫기
        Debug.Log("인원이 찼으므로 게임 시작!");
        // Test 변수
        gameManager.testVar = playerCount;
        // 로딩창 보여주기
        PV.RPC("GameStartEventRPC", RpcTarget.All);
    }

    [PunRPC]
    public void GameStartEventRPC()
    {
        PigAsset.SetActive(false);
        GameLoadingCanvas.SetActive(true);
        MyCharacterOnOff();
        StartCoroutine(GameLoading());
    }

    public void MyCharacterOnOff()
    {
        if (gameManager.GetMyCharacter() == "Pig")
        {
            myCharacterPig.SetActive(true);
            myCharacterWolf.SetActive(false);
            myCharacterText.text = "당신은 [돼지] 팀 입니다.";
        }
        else
        {
            myCharacterPig.SetActive(false);
            myCharacterWolf.SetActive(true);
            myCharacterText.text = "당신은 [늑대] 팀 입니다.";
        }
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        gameManager.SetMyCharacter(null);
        // 지연으로 인해 방을 빨리 떠나기 힘들다.
    }

    public override void OnLeftRoom()
    {
        Debug.Log("방 떠나기입니다!");
        SceneManager.LoadScene("LobbyScene");
        base.OnLeftRoom();
    }

    // 연결 해제
    public void DisconnectFromServer()
    {
        // 캔버스 동기화
        //Debug.Log("나가기 전 " + myTextSeat.text);
        myTextSeat.text = "";
        //Debug.Log("나간 후 " + myTextSeat.text);
        CanvasSynchronization();
        // Photon 서버와의 연결 해제
        PhotonNetwork.Disconnect();
    }

    // 게임 시작 전 플레이어를 게임에 맞게 역할 분배를 하는 로직
    public void PlayerDistribution()
    { // index를 전해주어야 함
        List<int> PigPlayerIndexList = new List<int>();
        List<int> WolfPlayerIndexList = new List<int>();
        // bool로 갈 수 있는지 판단 true일 때, 배치 가능
        List<bool> PigPlayerBoolList = new List<bool>();
        List<bool> WolfPlayerBoolList = new List<bool>();
        for (int i = 0; i < 6; i++)
        {
            if (PigList[i].GetComponentInChildren<TextMeshProUGUI>().text.Trim() == "")
            {
                PigPlayerBoolList.Add(true);
            }
            else 
            {
                PigPlayerIndexList.Add(i);
                PigPlayerBoolList.Add(false);
            }
            if (WolfList[i].GetComponentInChildren<TextMeshProUGUI>().text.Trim() == "")
            {
                WolfPlayerBoolList.Add(true);
            }
            else
            {
                WolfPlayerIndexList.Add(i);
                WolfPlayerBoolList.Add(false);
            }
        }
        //if (playerCount == 6)
        //{
            if (PigPlayerIndexList.Count > 4)
            {
                Shuffle(PigPlayerIndexList);
                for (int i = PigPlayerIndexList.Count; i > 4; i--)
                {
                    //Debug.Log("리스트 인덱스 타입 "+i+"번째 : "+ PigPlayerIndexList[i - 1].GetType());
                    for (int j = 0; j < 6; j++)
                    {
                        if (WolfPlayerBoolList[j])
                        {
                            // Pig플레이어가 많으므로 Wolf 팀으로 보내기
                            WolfPlayerBoolList[j] = false;
                            PV.RPC("GameCharacterChange", RpcTarget.All, PigPlayerIndexList[i - 1], j, "Pig", PhotonNetwork.IsMasterClient);
                            break;
                        }
                    }
                    
                }
            }
            else if (WolfPlayerIndexList.Count > 2)
            {
                Shuffle(WolfPlayerIndexList);
                for (int i = WolfPlayerIndexList.Count; i > 2; i--)
                {
                    //Debug.Log("리스트 인덱스 타입 " + i + "번째 : " + WolfPlayerIndexList[i - 1].GetType());
                    for (int j = 0; j < 6; j++)
                    {
                        if (PigPlayerBoolList[j])
                        {
                        // Wolf플레이어가 많으므로 Pig 팀으로 보내기
                            PigPlayerBoolList[j] = false;
                            PV.RPC("GameCharacterChange", RpcTarget.All, WolfPlayerIndexList[i - 1], j, "Wolf", PhotonNetwork.IsMasterClient);
                            break;
                        }
                    }
                }
            }
        //Debug.Log("Pig Idx List : "+PigPlayerIndexList);
        //Debug.Log("Wolf Idx List : " + WolfPlayerIndexList);
        //}
    }

    // 캐릭터 랜덤 셔플
    public List<int> Shuffle(List<int> shuffleList)
    {
        int n = shuffleList.Count;
        int temp;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            temp = shuffleList[k];
            shuffleList[k] = shuffleList[n];
            shuffleList[n] = temp;
        }
        return shuffleList;
    }

    // 캐릭터 랜덤 셔플 동기화
    [PunRPC]
    public void GameCharacterChange(int idx,int toIdx, string character, bool isMaster)
    { // character가 Wolf라면 Wolf의 idx번째 사람을 Pig팀으로 바꾼다는 뜻
        int mySeatIdx = Int32.Parse(mySeat.Split(' ')[1])-1; // 내 자리의 번호 가져오기
        string temp;
        //Debug.Log(character + idx + "에서 " + toIdx + "로 이동!");
        if (character == "Pig")
        {
            PigPrevSeat = PigList[idx].GetComponentInChildren<TextMeshProUGUI>();
            WolfToSeat = WolfList[toIdx].GetComponentInChildren<TextMeshProUGUI>();
            temp = PigPrevSeat.text;
            // 이전 자리 이름 색 초기화
            PigPrevSeat.text = "";
            PigPrevSeat.color = new Color(255, 255, 255, 255);
            if (mySeatIdx == idx && character == gameManager.GetMyCharacter())
            {
                // mySeat도 
                mySeat = "Wolf " + (toIdx+1);
                myTextSeat = WolfToSeat;
                // 돼지 플레이어를 늑대로 변경
                gameManager.SetMyCharacter("Wolf");
                WolfToSeat.color = new Color(0, 255, 203, 255);
            }
            WolfToSeat.text = temp;
            if (isMaster)
            {
                WolfMasterBadge[idx].SetActive(false);
                PigMasterBadge[toIdx].SetActive(true);
            }

        }
        else 
        {
            WolfPrevSeat = WolfList[idx].GetComponentInChildren<TextMeshProUGUI>();
            PigToSeat = PigList[toIdx].GetComponentInChildren<TextMeshProUGUI>();
            temp = WolfPrevSeat.text;
            // 이전 자리 이름 색 초기화
            WolfPrevSeat.text = "";
            WolfPrevSeat.color = new Color(255, 255, 255, 255);
            if (mySeatIdx == idx&& character == gameManager.GetMyCharacter())
            {
                // mySeat도 
                mySeat = "Pig " + (toIdx + 1);
                myTextSeat = PigToSeat;
                // 늑대 플레이어를 돼지로 변경
                gameManager.SetMyCharacter("Pig");
                PigToSeat.color = new Color(0, 255, 203, 255);
            }
            PigToSeat.text = temp;
            if (isMaster)
            {
                PigMasterBadge[idx].SetActive(false);
                WolfMasterBadge[toIdx].SetActive(true);
            }
        }
    }

    private IEnumerator GameLoading()
    {
        float nowLoadingTime = 0.0f;
        while (nowLoadingTime < gameLoadingTime)
        {
            // 로딩바 갱신: 현재 진행 상황을 표시
            loadingBar.value = nowLoadingTime / gameLoadingTime;

            // 시간 경과
            nowLoadingTime += Time.deltaTime;
            loadingBarText.text = $"로딩 중... {loadingBar.value * 100:0.0}%";
            yield return null; // 다음 프레임까지 대기
        }
        loadingBar.value = 1.0f; // 로딩바를 100%로 설정
        //LeaveRoom();
        if (PhotonNetwork.IsMasterClient) { 
            SceneManager.LoadScene("MapScene");
        }
    }

    private int syncroPlayerCount=0;
    // 나간 유저의 수가 동기화 되도록 하는 함수
    public void LeaveUserSyncro()
    {
        syncroPlayerCount += 1;
        if (PhotonNetwork.CurrentRoom.PlayerCount == syncroPlayerCount&& PhotonNetwork.IsMasterClient)
        {
            JoinCanvasSynchronization();
        }
    }

    [PunRPC]
    public void JoinCanvasSynchronizationRPC()
    {
        JoinCanvasSynchronization();
    }
}
