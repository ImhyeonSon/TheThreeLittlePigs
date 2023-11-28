using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiger : MonoBehaviour
{

    private int wolfLayer = 1 << 17; // 늑대 레이어를 지정하기 위한 LayerMask
    private PhotonView PV;



    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    private void Update()
    {
        CheckWolf();
    }


    
    public void CheckWolf()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f, wolfLayer);
        foreach (var hitCollider in hitColliders)
        {
            Debug.Log("늑대의 속도를 줄이는 메소드를 호출");
            hitCollider.GetComponent<PhotonView>().RPC("ReduceWolfSpeed", RpcTarget.All);
        }
    }
}
