using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string nickName;
    private string myCharacter;

    public int testVar; // 테스트용 변수

    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }
    void Awake()
    { 
        if (instance == null)
        {
            // 이 GameManager 오브젝트가 다른 씬으로 전환될 때 파괴되지 않도록 함
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
        {
            // 이미 존재하는 GameManager 오브젝트가 있으므로 이 인스턴스를 파괴
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        GameObject photon = GameObject.Find("PhotonManager");
        if (photon != null)
        {
            //if (photon.GetComponent<PhotonManager>() != null)
            //{
            //    photon.GetComponent<PhotonManager>().DisconnectFromServer();
            //}
            //if (photon.GetComponent<LobbyPhotonManager>() != null)
            //{
            //    photon.GetComponent<LobbyPhotonManager>().DisconnectFromServer();
            //}
            if (photon.GetComponent<WaitingPhotonManager>() != null)
            {
                Debug.Log("나 나간다.");
                photon.GetComponent<WaitingPhotonManager>().DisconnectFromServer();
            }
        }
    }




    public void SetMyCharacter(string character)
    {
        myCharacter = character;
    }
    
    public string GetMyCharacter()
    {
        return myCharacter;
    }

}
