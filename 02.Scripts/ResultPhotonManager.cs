using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultPhotonManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    float sceneTime = 15f;
    float defaultSceneTime = 15f;
    void Start()
    {
        // 방에서 나가기
        PhotonNetwork.LeaveRoom();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        SceneTimerCheck();
    }

    public void SceneTimerCheck()
    {
        if (sceneTime > 0)
        {
            sceneTime -= Time.deltaTime;
        }
        else
        {
            MoveToLobby();
        }
    }
    public void MoveToLobby()
    {
        GameManager.Instance.SetMyCharacter(null);
        SceneManager.LoadScene("LobbyScene");
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
    }
}