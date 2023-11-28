using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDestory : MonoBehaviour
{
    public PhotonView PV;

    // Start is called before the first frame update
  

    private void Start()
    {
        Debug.Log("오디오 스크립트 실행");
        if (!PV.IsMine)
        {
            Debug.Log("오디오 파괴");
            Destroy(gameObject);
        }
    }
}



