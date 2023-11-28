using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPigChestOpen : MonoBehaviour
{
    public GameObject chestLid;
    public GameObject pot;
    public GameObject fallenPot;

    public GameObject chestLidOpen;
    public GameObject chestLidClose;
    public GameObject potUp;
    public GameObject potDown;
    public GameObject fallenPotBefore;
    public GameObject fallenPotAfter;

    private Quaternion chestLidOpenRotation;
    private Quaternion chestLidCloseRotation;
    private Vector3 potUpPosition;
    private Vector3 potDownPosition;
    private Quaternion fallenPotBeforeRotation;
    private Quaternion fallenPotAfterRotation;

    private float curTime = 0;
    private float fallenTime = 0;

    void OnEnable()
    {
        curTime = 0;
        chestLidOpenRotation = chestLidOpen.transform.rotation;
        chestLidCloseRotation = chestLidClose.transform.rotation;
        potUpPosition = potUp.transform.position;
        potDownPosition = potDown.transform.position;
        fallenPotBeforeRotation = fallenPotBefore.transform.rotation;
        fallenPotAfterRotation = fallenPotAfter.transform.rotation;
    }

    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime <= 1)
        {
            // 열린 상태로 4초 대기
        }
        else if (curTime <= 4)  // 3초 대기
        {
            // 상자 닫기, 가보 내리기
            float potY = Mathf.Lerp(pot.transform.position.y, potDownPosition.y, (curTime - 1) / 7);
            pot.transform.position = new Vector3(pot.transform.position.x, potY, pot.transform.position.z);

            chestLid.transform.rotation = Quaternion.Slerp(chestLid.transform.rotation, chestLidCloseRotation, 1.5f * Time.deltaTime);
        }
        else if (curTime <= 8)
        {
            // 상자 열기, 가보 올리기
            if (curTime > 5)
            {
                float potY = Mathf.Lerp(pot.transform.position.y, potUpPosition.y, (curTime - 5) / 7);
                pot.transform.position = new Vector3(pot.transform.position.x, potY, pot.transform.position.z);
            }

            chestLid.transform.rotation = Quaternion.Slerp(chestLid.transform.rotation, chestLidOpenRotation, 1.5f * Time.deltaTime);
        }
        else
        {
            curTime = 0;
        }

        fallenTime += Time.deltaTime;
        if (fallenTime <= 5)
        {
            // 오른쪽으로 데구르르
            fallenPot.transform.rotation = Quaternion.Slerp(fallenPot.transform.rotation, fallenPotAfterRotation, 0.5f * Time.deltaTime);
        }
        else if (fallenTime <= 10)
        {
            // 왼쪽으로 데구르르
            fallenPot.transform.rotation = Quaternion.Slerp(fallenPot.transform.rotation, fallenPotBeforeRotation, 0.5f * Time.deltaTime);
        }
        else
        {
            fallenTime = 0;
        }
    }
}
