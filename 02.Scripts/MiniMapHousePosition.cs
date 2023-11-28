using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MiniMapHousePosition : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject myHouse;
    double miniRatio = 0.7874f;
    double defaultPosX = -5.2f;
    double defaultPosY = -81.5f;
    public RectTransform myPosition;
    public GameObject PigCharacter;
    double dx;
    double dy;
    private PigHouseManager PHM;

    public GameObject myHouseText;
    public GameObject HeirloomImg;

    private void Awake()
    {
        myPosition = GetComponent<RectTransform>();
    }
    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        // 어차피 미니맵 캔버스가 켜져있을 때만 실행 되므로 제한하지 않아도 될 듯.
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        if (PigCharacter == null)
        {
            return;
        }
        if (PHM.GetMyHouse() == null)
        {
            myPosition.anchoredPosition = new Vector2(9999, 9999); // 집이 파괴되거나 없는 상태라면 안보이게 가리기
            return;
        }
        Vector3 myPos = PHM.GetMyHouse().transform.position;
        dx = defaultPosX + myPos.x * 0.7874;
        dy = defaultPosY + myPos.z * 0.7874;
        if (PHM.GetMyHouse().GetComponent<HouseManager>().CH.GetHaveHeirloom()) // 하우스 매니저에 접근해서 CH를 찾아야 편함
        {
            HeirloomImg.SetActive(true);
        }
        else
        {
            HeirloomImg.SetActive(false);
        }
        myPosition.anchoredPosition = new Vector2((float)dx, (float)dy);
    }

    public void SetPigCharacter(int myPhotonViewId)
    {
        // 포톤 뷰 아이디로 my Object 할당하기
        PigCharacter = PhotonView.Find(myPhotonViewId).gameObject;
        if (PigCharacter.GetComponent<PhotonView>().IsMine)
        {
            myHouseText.SetActive(true);
        }
        PHM = PigCharacter.GetComponent<PigHouseManager>();
    }

}
