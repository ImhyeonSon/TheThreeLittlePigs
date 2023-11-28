using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNickname : MonoBehaviour
{
    public TextMeshProUGUI nickName;
    public PhotonView PV;
    // Start is called before the first frame update
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (PV.IsMine)
        {
            PV.RPC("SetMyNickName",RpcTarget.All, GameManager.Instance.nickName);
        }
    }

    [PunRPC]
    public void SetMyNickName(string myNickName)
    {
        nickName.text= myNickName;
    }



}
