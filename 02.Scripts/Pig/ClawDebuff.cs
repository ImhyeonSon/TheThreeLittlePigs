using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawDebuff : MonoBehaviour
{
    private float defaultTime=3f;
    private float nowTime;
    // Start is called before the first frame update
    private void OnEnable()
    {
        nowTime = defaultTime;
    }

    // Update is called once per frame
    void Update()
    {
        DebuffTimerCheck();
    }
    public void DebuffTimerCheck()
    {
        if (nowTime > 0)
        {
            nowTime -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
