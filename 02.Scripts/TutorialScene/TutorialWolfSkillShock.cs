using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWolfSkillShock : MonoBehaviour
{
    public GameObject shockWolf;
    public GameObject shockEffect;

    private Animator shockWolfAnim;

    private float curTime = 0;

    private void Awake()
    {
        shockWolfAnim = shockWolf.GetComponent<Animator>();
    }

    void OnEnable()
    {
        curTime = 0;
        shockEffect.SetActive(false);
    }

    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime <= 1)
        {
            // 1초 대기
            shockWolfAnim.SetBool("isShock", false);
            shockWolfAnim.SetBool("isIdle", true);
        }
        else if (curTime <= 3)
        {
            shockWolfAnim.SetBool("isIdle", false);
            shockWolfAnim.SetBool("isShock", true);
            if (curTime > 2)  // 효과는 1초 동안만
            {
                shockEffect.SetActive(true);
            }
        }
        else
        {
            curTime = 0;
            shockEffect.SetActive(false);
        }
    }
}
