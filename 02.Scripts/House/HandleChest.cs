using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 상자 여는 애니메이션 코드
public class HandleChest : MonoBehaviour
{
    Animator animator;
    private PhotonView PV;

    void Start()
    {
        animator = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
    }

    //상자가 여닫히는 RPC
    [PunRPC]
    public void handleOpen(bool isOpen)
    {
        animator.SetBool("isOpen", isOpen);
    }
}
