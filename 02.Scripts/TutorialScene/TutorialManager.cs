using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public GameObject LoadingCanvas;

    private void Awake()
    {
    }
    private void Start()
    {
        CanvasOff();
    }

    public void CanvasOff()
    {
        LoadingCanvas.SetActive(false);
    }

    public void MoveLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

}
