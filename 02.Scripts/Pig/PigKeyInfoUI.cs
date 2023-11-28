using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigKeyInfoUI : MonoBehaviour
{
    public GameObject pigKeyInfoUI;
    PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            pigKeyInfoUI.SetActive(true);
        }
    }

    public void PigOpenKeyInfoUI()
    {
        if (!pigKeyInfoUI.activeSelf & Input.GetKeyDown(KeyCode.F1))
        {
            pigKeyInfoUI.SetActive(true);
        } else if (pigKeyInfoUI.activeSelf && Input.GetKeyDown(KeyCode.F1))
        {
            pigKeyInfoUI.SetActive(false);
        }
    }
    public bool GetIsPigOpenKeyInfoUI()
    {
        return pigKeyInfoUI.activeSelf;
    }
}
