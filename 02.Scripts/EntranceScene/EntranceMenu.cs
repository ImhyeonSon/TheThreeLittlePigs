using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EntranceMenu : MonoBehaviour
{
    public TMP_InputField nickNameInput;
    public string nickName;
    
    public void OnclickStartGame() 
    {
        // 인풋에 입력한 닉네임
        nickName = nickNameInput.text;
        // 게임매니저에 닉네임 저장
        Debug.Log(nickName);
        GameManager.Instance.nickName = nickName;
        SceneManager.LoadScene("LobbyScene");
    }

    public void OnclickQuitGame()
    {
        Application.Quit();
    }

}
