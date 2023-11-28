using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMinimap : MonoBehaviour
{
    public GameObject key;
    public GameObject minimap;

    public float originalScale = 1;

    private float curTime;

    // Start is called before the first frame update
    void OnEnable()
    {
        minimap.SetActive(false);
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
            minimap.SetActive(true);
        }
        else if (curTime <= 1.8f)
        {
            // 원래대로
            float scaleV = Mathf.Lerp(key.transform.localScale.x, originalScale, curTime - 1.5f);
            key.transform.localScale = new Vector3(scaleV, scaleV, scaleV);
        }
        else if (curTime <= 2.8)
        {
            // 대기
        }
        else if (curTime <= 3.3f)
        {
            // 작아짐
            float scaleV = Mathf.Lerp(key.transform.localScale.x, originalScale * 0.8f, curTime - 2.8f);
            key.transform.localScale = new Vector3(scaleV, scaleV, scaleV);
            minimap.SetActive(false);
        }
        else if (curTime <= 3.6f)
        {
            // 원래대로
            float scaleV = Mathf.Lerp(key.transform.localScale.x, originalScale, curTime - 3.3f);
            key.transform.localScale = new Vector3(scaleV, scaleV, scaleV);
        }
        else
        {
            curTime = 0;
        }
    }
}
