using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MiniMapPositionWolf : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject myObject;
    double miniRatio = 0.7874f;
    double defaultPosX = -5.2f;
    double defaultPosY = -81.5f;
    public RectTransform myPosition;
    double dx;
    double dy;
    // 할당 되었는지 변수
    bool isAllocate = true;

    public GameObject MyCharacterText;

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
        if (isAllocate)
        {
            UpdatePosition();
        }
    }

    public void UpdatePosition()
    {
        if (myObject == null)
        {
            return;
        }
        Vector3 myPos = myObject.transform.position;
        dx = defaultPosX + myPos.x * 0.7874;
        dy = defaultPosY + myPos.z * 0.7874;
        myPosition.anchoredPosition = new Vector2((float)dx, (float)dy);
    }

    public void SetMyObject(int myPhotonViewId)
    {
        // 포톤 뷰 아이디로 my Object 할당하기
        myObject = PhotonView.Find(myPhotonViewId).gameObject;
        if (myObject.GetComponent<PhotonView>().IsMine)
        {
            MyCharacterText.SetActive(true);
        }
        isAllocate = true;
    }

}

