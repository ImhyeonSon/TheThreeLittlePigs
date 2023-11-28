using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSeatTimer : MonoBehaviour
{

    private float canvasOnTime = 1f;
    // Start is called before the first frame update
    void OnEnable()
    {
        canvasOnTime = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        CheckTime();
    }

    public void CheckTime()
    {
        if (canvasOnTime > 0)
        {
            canvasOnTime -= Time.deltaTime;
        }
        else
        {
            canvasOnTime = 0f;
            gameObject.SetActive(false);
        }
    }

}
