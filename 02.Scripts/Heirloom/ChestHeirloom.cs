using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChestHeirloom : MonoBehaviour
{
    private bool haveHeirloom=false;
    PhotonView PV;
    public HouseManager HM;
    public GameObject HeirloomObj;

    // Start is called before the first frame update
    void Awake()
    {
        // 추가로 메시지 담아두기
        PV = GetComponent<PhotonView>();
    }


    public bool GetHaveHeirloom()
    {
        return haveHeirloom; 
    }

    public void SetHaveHeirloom(bool TF)
    {
        PV.RPC("SetHaveHeirloomRPC", RpcTarget.All, TF);
    }

    [PunRPC]
    public void SetHaveHeirloomRPC(bool TF)
    { 
        // 가보를 넣었을 때, 집 레벨이 Max인지 검사해서 승리 조건을 판단하면 된다.
        haveHeirloom = TF;
        HM.IsHouseHaveHeirloom();
    }


    [PunRPC]
    public void ShowHeirloomRPC()
    {

        GameObject[] pigs = GameObject.FindGameObjectsWithTag("Pig"); // "Pig" 태그를 가진 돼지 객체를 찾음

        foreach (var pig in pigs)
        {
            // 돼지 객체의 Photon View를 가져옴
            PhotonView pigPV = pig.GetComponent<PhotonView>();
            if (pigPV.IsMine)
            {
                // 가보가 있는 집이면 효과가 있다.
                if (haveHeirloom)
                {
                    HeirloomObj.SetActive(true);
                }
                else
                {
                    HeirloomObj.SetActive(false);
                }
            }
        }
    }


    public void InformPigHouseDestroyed()
    {

    }

    // Update is called once per frame
}
