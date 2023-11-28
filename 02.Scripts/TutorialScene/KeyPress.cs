using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPress : MonoBehaviour
{
    public GameObject key;
    public float originalScale = 1;

    private float curTime;

    void OnEnable()
    {
        curTime = 0;
        key.transform.localScale = new Vector3(originalScale, originalScale, originalScale);
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime <= 1)
        {
            // 대기
        }
        else if (curTime <= 1.5f)
        {
            // 작아짐
            float scaleV = Mathf.Lerp(key.transform.localScale.x, originalScale * 0.8f, curTime - 1);
            key.transform.localScale = new Vector3(scaleV, scaleV, scaleV);  
        }
        else if (curTime <= 1.8f)
        {
            // 원래대로
            float scaleV = Mathf.Lerp(key.transform.localScale.x, originalScale, curTime - 1.5f);
            key.transform.localScale = new Vector3(scaleV, scaleV, scaleV);
        }
        else
        {
            curTime = 0;
        }
    }
}
