using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EntrancePhotonManager : MonoBehaviour // 이름만 포톤 매니저
{
    public TMP_InputField nickNameInput;
    public string nickName;

    // Popup창 관리
    public GameObject NicknameNullPopup;
    public GameObject NicknameLongPopup;

    public void OnclickStartGame()
    {
        // 인풋에 입력한 닉네임
        nickName = nickNameInput.text;
        nickName = nickName.Trim();
        if (string.IsNullOrEmpty(nickName))
        {
            // 문자열이 모두 공백 문자로만 이루어져 있음
            NicknameNullPopup.SetActive(true);
        }
        else
        {
            if (nickName.Length > 9)
            {
                NicknameLongPopup.SetActive(true);
                Debug.Log("닉네임을 9글자 이내로 해주세요 ! 닉네임[" + nickName + "]");
            }
            else if (nickName.Length > 0)
            {
                // 게임매니저에 닉네임 저장
                GameManager.Instance.nickName = nickName;
                SceneManager.LoadScene("LobbyScene");

            }
        }        
    }

    // 게임 나가는 함수
    public void OnclickQuitGame()
    {
        Application.Quit();
    }

    public void CloseNicknameNullPopup()
    {
        NicknameNullPopup.SetActive(false);
    }
    public void CloseNicknameLongPopup()
    {
        NicknameLongPopup.SetActive(false);
    }
}
