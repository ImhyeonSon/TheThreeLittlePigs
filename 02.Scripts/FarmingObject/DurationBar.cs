using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DurationBar : MonoBehaviour
{
    //public float duration;
    public GameObject durMeter;
    public Image durMeterImage;

    public void SetDurBar(float duration)
    {
        durMeter.SetActive(true);
        durMeterImage.fillAmount = 0;
        StartCoroutine(StartDurationBar(duration));
    }

    private IEnumerator StartDurationBar(float duration)
    {
        if (duration > 0)
        {
            float curDuration = 0;

            while (curDuration < duration)
            {
                curDuration += Time.deltaTime;
                durMeterImage.fillAmount = curDuration / duration;
                yield return null;
            }

            durMeterImage.fillAmount = 1;
            durMeter.SetActive(false);
        }
    }



    //public void SetDuration(float duration)
    //{
    //    this.duration = duration;
    //}
}
