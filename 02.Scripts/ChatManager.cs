using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatManager : MonoBehaviourPunCallbacks
{
    // 채팅 입력창
    public TMP_InputField inputField;
    // 텍스트 내용 프리펩
    public GameObject textChatPrefab;

    public GameObject guidePanel;
    public GameObject mContent;
    GameObject mContentText;

    // 채팅창
    public Transform parentContent;
    private string nickName;
    public PhotonView pv;
    // 채팅 입력, 보내기 버튼 부모 오브젝트
    public Scrollbar scrollbar;
    public ScrollRect scrollRect;

    public GameObject chatInput; // real inputField

    private void Start()
    {
        // 닉네임 호출
        nickName = GameManager.Instance.nickName;
        Debug.Log(nickName);
        //mContentText = mContent.transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        KeyDownEnter(); // 로그창 때문에 잠깐 막아둡니다. - 정원
    }

    // Input 창 옆에 버튼 누를 때 호출되는 함수
    public void OnEndEditEventMethod()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            UpdateChat();
        }
    }

    public void UpdateChat()
    {
        // 채팅창에 아무것도 입력 안할 시 종료
        Debug.Log("채팅 입력!");
        if (inputField.text=="")
        {
            return;
        }
        Debug.Log("길이" + inputField.text.Length);
        // 닉네임 : 대화내용
        string msg = $"{nickName} : {inputField.text}";
        // 채팅 RPC 호출
        pv.RPC("RPC_Chat", RpcTarget.All, msg);
        // 채팅 입력창 내용 초기화
        inputField.text = "";
    }

    void AddChatMessage(string message)
    {
        // 채팅 내용 출력을 위해 TEXT UI 생성
        //GameObject clone = Instantiate(textChatPrefab, parentContent);

        GameObject goText = Instantiate(textChatPrefab, mContent.transform);

        goText.GetComponent<TextMeshProUGUI>().text = message;

        Canvas.ForceUpdateCanvases(); // Canvas 업데이트
        scrollRect.verticalNormalizedPosition = 0; // 스크롤바 이동
    }

    public void KeyDownEnter()
    {
        // 채팅창 비활성화 시
        if (Input.GetKeyDown(KeyCode.Return) && !chatInput.activeSelf)
        {
            // 채팅 입력 창 초기화
            inputField.text = "";
            // 부모 오브젝트 활성화
            chatInput.SetActive(true);
            guidePanel.SetActive(false);
            // 채팅 입력창에 커서 활성화
            inputField.ActivateInputField();
            inputField.Select();
        }
        else if (Input.GetKeyDown(KeyCode.Return) && chatInput.activeSelf) // 채팅 부모 오브젝트 활성화 후 엔터 시 
        {
            // 부모 오브젝트 비활성화
            UpdateChat();
            chatInput.SetActive(false);
            guidePanel.SetActive(true);
        }
        else if (!inputField.isFocused) // 채팅창에서 커서가 옮겨질 시 ex 화면 클릭
        {
            // 부모 오브젝트 비활성화
            chatInput.SetActive(false);
            guidePanel.SetActive(true);
        }
    }

    [PunRPC]
    void RPC_Chat(string message)
    {
        Debug.Log("RPC메세지");
        AddChatMessage(message);
    }

}
