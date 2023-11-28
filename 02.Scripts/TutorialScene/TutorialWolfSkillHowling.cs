using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialWolfSkillHowling : MonoBehaviour
{
    public GameObject howlingWolf;

    public GameObject background;
    public Image bgImg;
    public GameObject eyeEffect;

    private float curTime = 0;

    void OnEnable()
    {
        curTime = 0;
        bgImg = background.GetComponent<Image>();
    }

    private void OnDisable()
    {
        bgImg.color = Color.white;
    }

    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime <= 1 )
        {

        }
        else if (curTime <= 3 )
        {
            eyeEffect.SetActive(true);
            Color lerpedColor = Color.Lerp(bgImg.color, Color.black, 2f * Time.deltaTime);
            bgImg.color = lerpedColor;
        }
        else if (curTime <= 5)
        {
            Color lerpedColor = Color.Lerp(bgImg.color, Color.white, 2f * Time.deltaTime);
            bgImg.color = lerpedColor;
        }
        else
        {
            eyeEffect.SetActive(false);
            curTime = 0;
        }
    }
}
