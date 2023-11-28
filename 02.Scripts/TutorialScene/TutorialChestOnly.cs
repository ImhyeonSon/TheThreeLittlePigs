using System.Collections;
using System.Collections.Generic;
using UltimateClean;
using UnityEngine;

public class TutorialChestONly : MonoBehaviour
{
    public GameObject chestLid;

    public GameObject chestLidOpen;
    public GameObject chestLidClose;

    private Quaternion chestLidOpenRotation;
    private Quaternion chestLidCloseRotation;

    private float curTime = 0;

    void OnEnable()
    {
        curTime = 0;
        chestLidOpenRotation = chestLidOpen.transform.rotation;
        chestLidCloseRotation = chestLidClose.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime <= 1)
        {
            // 열린 상태로 4초 대기
        }
        else if (curTime <= 4)  // 3초 대기
        {
            // 상자 닫기
            chestLid.transform.rotation = Quaternion.Slerp(chestLid.transform.rotation, chestLidCloseRotation, 1.5f * Time.deltaTime);
        }
        else if (curTime <= 8)
        {
            // 상자 열기
            chestLid.transform.rotation = Quaternion.Slerp(chestLid.transform.rotation, chestLidOpenRotation, 1.5f * Time.deltaTime);
        }
        else
        {
            curTime = 0;
        }
    }
}
