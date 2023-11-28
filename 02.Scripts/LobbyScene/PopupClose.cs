using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupClose : MonoBehaviour
{
    public GameObject MakeRoomPopUp;
    public GameObject EnterRoomPopUp;
    public GameObject StartRoomFailPopUp;
    public GameObject NoTitlePopUp;
    public GameObject createPopup;
    public GameObject tutorialPopup;
    public GameObject PasswordFailPopup;
    public GameObject MakeRoomFailPopup;
    public GameObject FullRoomFailPopup;

    public void MakeRoomClosePopUp() // 방 만들기 팝업 닫기
    {
        MakeRoomPopUp.SetActive(false);
    }
    public void PasswordFailClosePopUp() // 비밀번호 실패 닫기
    {
        PasswordFailPopup.SetActive(false);
    }
    public void EnterRoomClosePopUp() // 비밀번호 입력 창 닫기
    {
        EnterRoomPopUp.SetActive(false);
    }
    public void StartRoomFailClosePopUp()  // 이미 시작한 방 창 닫기
    {
        StartRoomFailPopUp.SetActive(false);
    }
    public void CloseNoTitlePopUp() // 방제목 없을 때 팝업 닫기
    {
        NoTitlePopUp.SetActive(false);
    }

    public void OpenCloseTutorial() //튜토리얼
    {
        createPopup.SetActive(!createPopup.activeSelf);
        tutorialPopup.SetActive(!tutorialPopup.activeSelf);
    }
    public void MakeRoomFailClosePopup()
    {
        MakeRoomFailPopup.SetActive(false);
    }

    public void FullRoomFailClosePopup()
    {
        FullRoomFailPopup.SetActive(false);
    }
}
