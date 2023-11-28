using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heirloom : MonoBehaviour
{
    

    GameObject HeirloomPanel;
    private bool haveHeirloom;
    Image heirloomImg;
    PhotonView PV;
    public GameObject HeirloomTrasnform; // AttackRange와 같은 범위를 사용
    int HeirloomLayer = 1 << 9;
    private bool InputV=false;
    // Start is called before the first frame update
    void Awake()
    {
        // 이미지 할당
        PV = GetComponent<PhotonView>();
        HeirloomPanel = GameObject.Find("Canvas").transform.Find("HeirloomBack").gameObject;
        HeirloomPanel.SetActive(true); // Canvas On은 빨리 해주어야 함.
        heirloomImg = GameObject.Find("Canvas").transform.Find("HeirloomBack/HeirloomPanel/Image").GetComponent<Image>();
        SetHaveHeirloom(false); // 초기화
    }
    private void Start()
    {
    }

    public void HeirloomInput()
    {
        InputV = Input.GetKeyDown(KeyCode.V);
    }

    public void UseHeirloomRPC(bool TF)
    {
        PV.RPC("SetHeirloomRPC", RpcTarget.All, TF);
    }

    public bool GetHeirloom()
    {
        return haveHeirloom;
    }

    [PunRPC]
    public void SetHeirloomRPC(bool TF)
    {
        SetHaveHeirloom(TF);
    }


    // Update is called once per frame
    public void SetHaveHeirloom(bool TF)
    {
        haveHeirloom = TF; // haveHeirloom 값 설정
        SetHeirloomImg();
    }

    public void SetHeirloomImg()
    {
        if (PV.IsMine)
        {
            if (haveHeirloom)
            {
                heirloomImg.sprite = Resources.Load("InGameUI/Heirloom", typeof(Sprite)) as Sprite;
            }
            else 
            {
                heirloomImg.sprite = Resources.Load("InGameUI/NoHeirloom", typeof(Sprite)) as Sprite;        
            }
        }
    }

    public void PutOrGetHeirloomToChest()
    {
        if (InputV)
        {
            Collider[] hitColliders = Physics.OverlapSphere(HeirloomTrasnform.transform.position, 3f, HeirloomLayer);
            foreach (var hitCollider in hitColliders)
            {
                ChestHeirloom CH = hitCollider.GetComponent<ChestHeirloom>();
                PhotonView chestPV = hitCollider.GetComponent<PhotonView>(); // 상자의 PhotonView를 가져옴
                // 나에게 있다면 상자에 넣기
                if (haveHeirloom)
                {
                    CH.SetHaveHeirloom(true);
                    UseHeirloomRPC(false);
                    chestPV.RPC("ShowHeirloomRPC", RpcTarget.All); //집 위에 보이게 하기
                }
                else
                {
                    if (CH.GetHaveHeirloom()) // 상자에 있다면
                    {
                        CH.SetHaveHeirloom(false);
                        UseHeirloomRPC(true);
                        chestPV.RPC("ShowHeirloomRPC", RpcTarget.All); // 집위에 보이게 하기
                    }          
                }            
            }
        }

    }
}
