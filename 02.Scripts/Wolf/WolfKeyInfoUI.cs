using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfKeyInfoUI : MonoBehaviour
{
    public GameObject wolfKeyInfoUI;
    public bool isWolfOpenKeyInfoUI;
    PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            wolfKeyInfoUI.SetActive(true);
        }
    }

    public void WolfOpenKeyInfoUI()
    {
        if (!wolfKeyInfoUI.activeSelf & Input.GetKeyDown(KeyCode.F1))
        {
            wolfKeyInfoUI.SetActive(true);
        }
        else if (wolfKeyInfoUI.activeSelf && Input.GetKeyDown(KeyCode.F1))
        {
            wolfKeyInfoUI.SetActive(false);
        }
    }
    public bool GetIsWolfOpenKeyInfoUI()
    {
        return wolfKeyInfoUI.activeSelf;
    }
}
