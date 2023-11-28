using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfClockingAura : MonoBehaviour
{
    // Start is called before the first frame update
    public WolfSkillRPC WSRPC;
    private float defaultTime = 1.5f;
    private float nowTime;
    private bool isClocking;
    // Start is called before the first frame update
    private void OnEnable()
    {
        nowTime = 0f;
        isClocking = false;
    }

    // Update is called once per frame
    void Update()
    {
        ClockingTimerCheck();
    }
    public void ClockingTimerCheck()
    {
        if (nowTime < defaultTime)
        {

            nowTime += Time.deltaTime;
        }
        else
        {
            if (!isClocking)
            {
                isClocking = true;
                if (GameManager.Instance.GetMyCharacter() == "Pig")
                {
                    WSRPC.WolfClocking();
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
