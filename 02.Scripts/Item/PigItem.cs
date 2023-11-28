using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PigItem : MonoBehaviour
{
    public PhotonView PV;
    public enum ItemType
    {
        Rice, 
        Wood,
        Stone,
        Mud,
        Leather,
        Syringe,
        Treasure,
        Lumber, 
        Brick, 
        SlingShot,
        Axe,
        Shovel,
        Pick,
        Sickle,
        Shoes,
        Umbrella,
        Tiger
    };

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    public void Update()
    {
        Timer();
    }

    public ItemType type;

    private float timeFlow;
    private bool timerIsActive = false;

    public void DestroyGetItemObject(string value)
    {
        Debug.Log(value);
        timeFlow = value == "NULL" ? 4f : 2f ;
        timerIsActive = true;
        
    }

    public void Timer()
    {
        if (timerIsActive)
        {
            timeFlow -= Time.deltaTime;
            if (timeFlow <= 0)
            {
                timerIsActive = false;
                PV.RPC("ObjectActiveFalse", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    public void ObjectActiveFalse()
    {
        gameObject.SetActive(false);
    }


}
