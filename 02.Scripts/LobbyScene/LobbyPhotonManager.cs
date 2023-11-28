using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class LobbyPhotonManager : MonoBehaviourPunCallbacks
{
    // 버전 입력
    private readonly string version = "1.0f";

    public GameObject loadingCanvas; // 로딩 캔버스를 참조하기 위한 변수

    public List<RoomData> RoomDatas;
    public Transform Rooms;

    public GameObject makeRoomUi; // 방만들기 팝업
    public TextMeshProUGUI title; // 방제
    public TextMeshProUGUI password; // 방만들때 비밀번호

    public GameObject InRoomUI;
    public TextMeshProUGUI inRoomtitle; // 방제
    public TextMeshProUGUI inRoomPassword; // 방 입장시 제출하는 비밀번호

    public GameObject FailPasswordPopup; // 비밀번호 틀렸다는 팝업
    public GameObject FailStartRoomPopup; // 이미 시작한 방 팝업
    public GameObject NoTitlePopUp; // 방제 입력 x일 경우 팝업
    public GameObject MakeRoomFailPopup;
    public GameObject FullRoomFailPopUp; // 인원이 다 찼다는 경고 팝업

    public TextMeshProUGUI failInfo; // 실패 이유
    string[] parseRoomName;

    RoomData clickRoomData;
    TextMeshProUGUI clickedRoom;

    void Awake()
    {
        // 로딩 캔버스 활성화
        loadingCanvas.SetActive(true);
        
        // 같은 룸의 유저들에게 자동으로 씬을 로딩
        PhotonNetwork.AutomaticallySyncScene = true;
        // 같은 버전의 유저끼리 접속 허용
        PhotonNetwork.GameVersion = version;
        // 유저 아이디 할당
        PhotonNetwork.NickName = GameManager.Instance.nickName;
    }

    void Start()
    {
        RoomDatas = new List<RoomData>();
        //Cursor.lockState = CursorLockMode.None; // 마우스 커서 락
        //Cursor.visible = true;
        
        Debug.Log("포톤 매니저 시작");

        // 이미 로비라면 로딩 창 끄기, 닉네임 바꿀 때 오류가 있음.
        if (PhotonNetwork.InLobby)
        {
            loadingCanvas.SetActive(false);
        }

        // 서버 접속
        if (!PhotonNetwork.IsConnected)
        {
            // 연결이 되지 않을 경우 ConnectUsingSetting으로 연결하고 MasterClient에 연결이 됐을 때
            // 로비에 조인을 실행
            //    OnConnectedToMaster();
            //}
            //else
            //{
            Debug.Log("연결이 성공했니?");
            PhotonNetwork.ConnectUsingSettings();
        }
        else 
        {
            Debug.Log("연결이 성공했는지?");            
            OnConnectedToMaster();
        }
    }

    // 포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master!");
        if (PhotonNetwork.IsConnected)
        {
            TypedLobby lobbyType = new TypedLobby("default lobby", LobbyType.Default);
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby(lobbyType);
            }
        }
        else
        {
            Debug.Log("다시 로비에 연결");
            OnConnectedToMaster();
        }
    }

    // 로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
        // 로딩 캔버스 비활성화
        loadingCanvas.SetActive(false);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 엡데이트된 방 개수를 출력
        Debug.Log($"업데이트방 개수: {roomList.Count}\n\n");

        // 각 방의 정보 업데이트
        foreach (RoomInfo room in roomList)
        {
            // 업데이트 방인지
            bool isUpdate = false;
            for (int i = 0; i < RoomDatas.Count; i++)
            {
                // 기존에 존재하던 방 : 방 업데이트 또는 삭제
                parseRoomName = room.Name.Trim().Split("[_]");
                Debug.Log("0" + parseRoomName[0]);
                Debug.Log("1" + parseRoomName[1]);

                if (RoomDatas[i].RoomName.Equals(parseRoomName[0])&& RoomDatas[i].Password.Equals(parseRoomName[1]))
                {
                    Debug.Log("제대로 되는지 확인");
                    UpdateOrDeletRoomDatas(i, room);
                    isUpdate = true;

                    break;
                }
            }

            // 새로 추가된 방이며 존재하는 방일때(인게임 플레이어가 1명 이상)
            if (!isUpdate && room.PlayerCount > 0)
            {
                addRoomDatas(room);
            }
        }
        UpdateRoomList();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room!");
        Debug.Log($"Room Name = {PhotonNetwork.CurrentRoom.Name}");

        SceneManager.LoadScene("WaitingScene");
    }

    // 방 정보 추가
    public void addRoomDatas(RoomInfo room)
    {
        parseRoomName = room.Name.Split("[_]");
        RoomData roomData = new RoomData
        {
            RoomName = parseRoomName[0],
            Status = room.IsOpen,
            CurrentPlayers = room.PlayerCount,
            MaxPlayers = room.MaxPlayers,
            Password = parseRoomName[1]
        };
        RoomDatas.Add(roomData);
    }

    // 방 정보 업데이트 : 데이터 변경 또는 삭제
    public void UpdateOrDeletRoomDatas(int index, RoomInfo room)
    {
        // 플레이어가 0명이면 사라진 방
        if (room.PlayerCount == 0)
        {
            RoomDatas.RemoveAt(index);
            // Room Canvas 꺼주기 실행
            return;
        }

        
        parseRoomName= room.Name.Split("[_]");
        // 플레이거가 존재하는 방 정보 변경 : 플레이어 입장, 플레이어 퇴장, 게임 시작 등
        RoomData roomData = new RoomData
        {
            RoomName = parseRoomName[0],
            Status = room.IsOpen, //IsOpen에서는 합쳐진 이름 사용
            CurrentPlayers = room.PlayerCount,
            MaxPlayers = room.MaxPlayers,
            Password = parseRoomName[1]
        };
        RoomDatas[index] = roomData;
    }

    // 방정보 UI 출력 업데이트
    public void UpdateRoomList()
    {
        Debug.Log("업데이트룸리스트");
        Debug.Log(RoomDatas.Count);

        // 방의 개수만큼 ui 업데이트
        for (int i = 0; i < RoomDatas.Count; i++)
        {
            // i번째 방 정보 출력 열 오브젝트
            TextMeshProUGUI[] RoomInfoInputs =
            Rooms.GetChild(i).GetComponentsInChildren<TextMeshProUGUI>();
            Rooms.GetChild(i).gameObject.SetActive(true);
            // 방제
            Debug.Log(RoomDatas[i].RoomName);
            RoomInfoInputs[0].text = RoomDatas[i].RoomName;
            // 방 상태 표현
            RoomInfoInputs[1].text = RoomDatas[i].Status ? "대기 중.." : "게임 중..";
            // 키 아이콘 찾기
            GameObject KeyIcon = Rooms.GetChild(i).transform.Find("Key").gameObject;

            if (RoomDatas[i].Password.Length <= 1)
            {
                KeyIcon.SetActive(false);
            }
            else 
            {
                KeyIcon.SetActive(true);                
            }
            RoomInfoInputs[2].text = RoomDatas[i].CurrentPlayers.ToString(); // 현재 유저 수
            RoomInfoInputs[4].text = RoomDatas[i].MaxPlayers.ToString();     // 최대 유저 수
        }

        // 기존 방 ui 초기화 : 3개에서 2개로 줄어든 경우 3번째 방 정보는 사라지지 않음(최대 출력 5개)
        for (int i = RoomDatas.Count; i < 6; i++)
        {
            // i번째 방 정보 출력 열 오브젝트
            TextMeshProUGUI[] RoomInfoInputs =
            Rooms.GetChild(i).GetComponentsInChildren<TextMeshProUGUI>();
            RoomInfoInputs[0].text = "Room" + (i + 1); // 방제
            RoomInfoInputs[1].text = "Waiting"; // 방 상태
            RoomInfoInputs[2].text = "0";
            RoomInfoInputs[4].text = "0";
            // 0인 방 끄기
            Rooms.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void MakeRoomPopUp()
    {
        makeRoomUi.SetActive(true);
    }

    public void MakeRoom()
    {
        // 방제 입력 확인
        Debug.Log("생성버튼 누름");
        Debug.Log("PhotonNetwork.InLobby : "+ PhotonNetwork.InLobby);
        if (title.text.Length == 1)
        {
            NoTitlePopUp.SetActive(true);
            Debug.Log("방제를 입력하세요");
            return;
        }
        if (PhotonNetwork.InLobby)
        {
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = 6;
            ro.IsOpen = true;
            ro.IsVisible = true;

            // 커스텀 속성 추가
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties["captain"] = GameManager.Instance.nickName; // 방장정보 커스텀 속성
            ro.CustomRoomProperties = customProperties;

            // 방을 생성한 방
            ro.CustomRoomPropertiesForLobby = new string[] { "captain" };
            // 방은 제목 + 비밀번호로 되어있고, parsing을 한다.
            Debug.Log("만든 방의 비번 길이" + password.text.Length);
            string titlePassword = (title.text + "[_]" + password.text).Trim();
            PhotonNetwork.CreateRoom(titlePassword, ro, null);
        }
    }

    public void ClickRoom()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        string[] parsingRoomName = clickObject.name.Split("Room");
        // 선택한 Room의 정보를 리스트에서 가져오기 위해 parsing 함
        clickRoomData = RoomDatas[Int32.Parse(parsingRoomName[1]) - 1];
        Debug.Log("디버그 확인"+clickRoomData.RoomName);
        Debug.Log("디버그 확인"+clickRoomData.Password);
        clickedRoom =
                EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>();

        // 입장 전 게임 시작했는지 여부
        if (clickRoomData.Status)
        {
            if (clickRoomData.CurrentPlayers == clickRoomData.MaxPlayers)
            {
                FullRoomFailPopUp.SetActive(true);
            }
            else
            {
                // 왜인지 모르겠으나 Split으로 구한 방 비밀번호의 길이가 실제 길이보다 1 크게 나옴
                if (clickRoomData.Password.Length>1) 
                {
                    // 비밀번호 입력 창 띄우기
                    Debug.Log("비밀번호를 입력해 주세요");
                    //inRoomPassword.text = clickRoomData.RoomName;
                    inRoomtitle.text = clickRoomData.RoomName;
                    // 팝업 띄우기
                    InRoomUI.SetActive(true);
                }
                else
                {
                    // 비밀번호가 없다면 바로 입장.
                    EnterRoom(clickRoomData.RoomName + "[_]" + clickRoomData.Password);
                }
            }

        }
        else
        {
            // 이미 게임이 시작했다는 팝업 띄우기
            FailStartRoomPopup.SetActive(true);
            Debug.Log("이미 게임을 시작한 방입니다.");
        }
    }

    /// <summary>
    /// // 버튼 누르면 입력 받는거 추가하기
    /// </summary>
    /// <param name="roomData"></param>
    public void EnterPassword()
    {
        string roomName = clickRoomData.RoomName;
        if (clickRoomData.Password == inRoomPassword.text)
        {
            EnterRoom(clickRoomData.RoomName + "[_]" + clickRoomData.Password);
        }
        else
        {
            FailPasswordPopup.SetActive(true);
            Debug.Log("비밀번호가 틀렷습니다.");
        }
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        MakeRoomFailPopup.SetActive(true);
        if (PhotonNetwork.IsConnected && !PhotonNetwork.InLobby) // Fail하면 Lobby에서 나가지기 때문에 다시 연결
        {
            TypedLobby lobbyType = new TypedLobby("default lobby", LobbyType.Default);
            PhotonNetwork.JoinLobby(lobbyType);
        }
    }

    public void EnterRoom(string roomName)
    {
        Debug.Log(roomName);
        PhotonNetwork.JoinRoom(roomName);
        SceneManager.LoadScene("WaitingScene");
    }

    public void MoveEntranceScene()
    {
        PhotonNetwork.LeaveLobby();
        SceneManager.LoadScene("EntranceScene");
    }

    public void MoveTutorial()
    {
        PhotonNetwork.LeaveLobby();
        SceneManager.LoadScene("TutorialScene2");
    }
    //public override void OnLeftLobby()
    //{
    
    //}
}
