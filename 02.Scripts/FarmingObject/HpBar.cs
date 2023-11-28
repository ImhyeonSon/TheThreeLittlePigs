using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public float maxHp;
    public GameObject HpMeter;
    public Image HpMeterImage;
    private float duration = 0;

    private void Update()
    {
        if (HpMeter.activeSelf)
        {
            if (duration > 0)
            {
                duration -= Time.deltaTime;
            } 
            else
            {
                duration = 0;
                HpMeter.SetActive(false);
            }
        }
    }

    public void SetHpBar(float HP)
    {
        if (maxHp > 0)
        { 
            if (HP / maxHp == 1 || HP / maxHp == 0)
            {
                duration = 0;
                HpMeter.SetActive(false);
            }
            else
            {
                duration = 3f;
                HpMeter.SetActive(true);
                HpMeterImage.fillAmount = HP / maxHp;
            }
        }
    }

    public void SetMaxHp(float maxHp)
    {
        this.maxHp = maxHp;
    }
}
