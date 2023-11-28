using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTimer
{
    float defaultRespawnTime;
    float respawnTime;

    public RespawnTimer(float defaultRespawnTime)
    {
        this.defaultRespawnTime = defaultRespawnTime;
        respawnTime = defaultRespawnTime;
    }

    public bool CheckRespawnTime(PhotonView PV, GameObject farmingObject)  // 타이머의 시간을 확인해
    {
        if (respawnTime > 0f)  // 시간이 남아있으면 흐르게 하고
        {
            respawnTime -= Time.deltaTime;
            return false;  // 시간이 남아있다면 false 반환
        }
        else  // 시간이 남아있지 않다면(리스폰 시간이 찼다면) 
        {
            //RespawnObject(PV, farmingObject);
            ResetRespawnTime();
            return true;  // 시간이 남아있지 않다면 true 반환
        }

    }

    public bool RespawnObject(PhotonView PV, GameObject farmingObject)
    {
        //if (!farmingObject.activeSelf)  // 파밍 오브젝트가 활성화되어있지 않다면
        //{
        //    //PV.RPC("RespawnObjectRPC", RpcTarget.All);
        //    return true;
        //}
        //return false;
        return true;
    }

    //[PunRPC]
    //public void RespawnObjectRPC()  // 파밍 오브젝트 리스폰 동기화
    //{
    //    gameObject.SetActive(true);  // 파밍 오브젝트 활성화
    //}

    public void ResetRespawnTime()  // 리스폰 시간이 다 되었을 때 리셋하는 함수
    {
        respawnTime = defaultRespawnTime;
    }
}
