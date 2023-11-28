using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class TutorialHouseBuild : MonoBehaviour
{
    public GameObject fence;
    public GameObject house;

    // 위치값을 가져오기 위해 사용
    public GameObject fenceUp;
    public GameObject fenceDown;
    public GameObject houseUp;
    public GameObject houseDown;

    public GameObject BKey;
    public GameObject OKey;

    public GameObject frontDoor;
    public GameObject rightDoor;

    // 회전값을 가져오기 위해 사용
    public GameObject frontDoorClose;
    public GameObject frontDoorOpen;
    public GameObject rightDoorClose;
    public GameObject rightDoorOpen;

    private Quaternion frontDoorCloseRotation;
    private Quaternion rightDoorCloseRotation;

    // 월드 좌표 기준
    private Vector3 fenceUpPosition;
    private Vector3 fenceDownPosition;
    private Vector3 houseUpPosition;
    private Vector3 houseDownPosition;

    private float curTime = 0;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        curTime = 0;
        fenceUpPosition = fenceUp.transform.position;
        fenceDownPosition = fenceDown.transform.position;
        houseUpPosition = houseUp.transform.position;
        houseDownPosition = houseDown.transform.position;

        fence.transform.position = fenceUpPosition;
        house.transform.position = houseDownPosition;

        frontDoorCloseRotation = frontDoorClose.transform.rotation;
        rightDoorCloseRotation = rightDoorClose.transform.rotation;

        BKey.transform.localScale = Vector3.one;
        OKey.transform.localScale = Vector3.one;
    }

    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime <= 4)
        {
            // 4초 대기
            Debug.Log("대기" + curTime);

        }
        else if (curTime <= 6)  // 7초 동안 집 짓기
        {
            if (curTime <= 4.8)
            {
                PressKey(curTime - 4, BKey);
            }
            // 울타리 내리고 집 올리기
            float fenceY = Mathf.Lerp(fence.transform.position.y, fenceDownPosition.y, (curTime - 4) / 7);
            fence.transform.position = new Vector3(fence.transform.position.x, fenceY, fence.transform.position.z);

            float houseY = Mathf.Lerp(house.transform.position.y, houseUpPosition.y, (curTime - 4) / 7);
            house.transform.position = new Vector3(house.transform.position.x, houseY, house.transform.position.z);
            Debug.Log("집 짓기" + ((curTime - 4) / 7) + "울타리 변화값" + Vector3.Lerp(fence.transform.position, fenceUpPosition - new Vector3(0, -554, 0), (curTime - 4) / 7).ToString() + "집 변화값" + Vector3.Lerp(house.transform.position, houseUpPosition, (curTime - 4) / 7).ToString());
        }
        else if (curTime <= 13)  // 8초 대기
        {
            if (curTime <= 8)
            {
                if (curTime <= 6.8f)
                {
                    PressKey(curTime - 6, OKey);
                }
                // 문 열기
                frontDoor.transform.rotation = Quaternion.Slerp(frontDoor.transform.rotation, frontDoorCloseRotation * Quaternion.Euler(new Vector3(0, -115, 0)), 1.5f * Time.deltaTime);
            }
            else if (curTime <= 10)
            {
                if(curTime <= 8.8f)
                {
                    PressKey(curTime - 8, OKey);
                }
                // 문 닫기
                frontDoor.transform.rotation = Quaternion.Slerp(frontDoor.transform.rotation, frontDoorCloseRotation, 1.5f * Time.deltaTime);
            }
            else if (curTime <= 12)
            {
                if (curTime <= 10.8f)
                {
                    PressKey(curTime - 10, OKey);
                }
                // 문 열기
                rightDoor.transform.rotation = Quaternion.Slerp(rightDoor.transform.rotation, rightDoorCloseRotation * Quaternion.Euler(new Vector3(0, -115, 0)), 1.5f * Time.deltaTime);
            }
            else
            {
                if(curTime <= 12.8f)
                {
                    PressKey(curTime - 12, OKey);
                }
                // 문 닫기
                rightDoor.transform.rotation = Quaternion.Slerp(rightDoor.transform.rotation, rightDoorCloseRotation, 1.5f * Time.deltaTime);
            }
            Debug.Log("문 여닫기" + curTime);

        }
        else if (curTime <= 15)
        {
            // 울타리 올리고 집 내리기
            float fenceY = Mathf.Lerp(fence.transform.position.y, fenceUpPosition.y, (curTime - 13) / 7);
            fence.transform.position = new Vector3(fence.transform.position.x, fenceY, fence.transform.position.z);

            float houseY = Mathf.Lerp(house.transform.position.y, houseDownPosition.y, (curTime - 13) / 7);
            house.transform.position = new Vector3(house.transform.position.x, houseY, house.transform.position.z);

            Debug.Log("집 무너지기" + curTime);

        }
        else if (curTime > 15)  // 15초 주기로 과정 실행
        {
            curTime = 0;
            fence.transform.position = fenceUpPosition;
            house.transform.position = houseDownPosition;
            frontDoorCloseRotation = frontDoorClose.transform.rotation;
            rightDoorCloseRotation = rightDoorClose.transform.rotation;
            Debug.Log("다시 시작" + curTime);

        }
    }

    //private void OnDisable()
    //{
    //    fence.transform.position = fenceUpPosition;
    //    house.transform.position = houseDownPosition;
    //}

    private void PressKey(float time, GameObject key)
    {
        if (time <= 0.5f)
        {
            // 작아짐
            float scaleV = Mathf.Lerp(key.transform.localScale.x, 0.8f, time);
            key.transform.localScale = new Vector3(scaleV, scaleV, scaleV);
        }
        else if (time <= 0.8f)
        {
            // 원래대로
            float scaleV = Mathf.Lerp(key.transform.localScale.x, 1, time);
            key.transform.localScale = new Vector3(scaleV, scaleV, scaleV);
        }
    }
}
