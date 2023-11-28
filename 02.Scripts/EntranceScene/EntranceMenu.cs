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
        // ��ǲ�� �Է��� �г���
        nickName = nickNameInput.text;
        // ���ӸŴ����� �г��� ����
        Debug.Log(nickName);
        GameManager.Instance.nickName = nickName;
        SceneManager.LoadScene("LobbyScene");
    }

    public void OnclickQuitGame()
    {
        Application.Quit();
    }

}
