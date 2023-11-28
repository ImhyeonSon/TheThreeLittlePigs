using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseIsMineCheck : MonoBehaviour
{

    public GameObject TrueCircle;
    public GameObject FalseCircle;
    private PhotonView PV;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        CheckIsMine();
    }


    // Update is called once per frame
    void Update()
    {
       
    }



    public void CheckIsMine()
    {

            // 모든 돼지 객체를 찾아옴
            GameObject[] pigs = GameObject.FindGameObjectsWithTag("Pig"); // "Pig" 태그를 가진 돼지 객체를 찾음

            foreach (var pig in pigs)
            {
                // 돼지 객체의 Photon View를 가져옴
                PhotonView pigPV = pig.GetComponent<PhotonView>();
                if (pigPV.IsMine)
                {
                    TrueCircle.SetActive(true);
                    FalseCircle.SetActive(false);
                }
                else
                {
                    TrueCircle.SetActive(false);
                    FalseCircle.SetActive(true);
                }
            }
        }
   
}
