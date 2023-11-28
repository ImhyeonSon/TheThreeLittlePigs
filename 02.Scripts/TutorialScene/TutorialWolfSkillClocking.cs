using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWolfSkillClocking : MonoBehaviour
{
    public GameObject clockingWolf;
    public GameObject CoyoteMesh;
    public GameObject clockingEffect;

    public Animator clockingWolfAnim;

    private float curTime = 0;

    void OnEnable()
    {
        curTime = 0;
        clockingWolfAnim.SetBool("isClocking", false);
        clockingWolfAnim.SetBool("isIdle", true);
        CoyoteMesh.SetActive(true);
        clockingEffect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime <= 2.2f)
        {
            // 은신 애니메이션
            clockingWolfAnim.SetBool("isIdle", false);
            clockingWolfAnim.SetBool("isClocking", true);
            clockingEffect.SetActive(true);
        }
        else if (curTime <= 6)
        {
            // 안보이기
            CoyoteMesh.SetActive(false); 
            clockingWolfAnim.SetBool("isClocking", false);
            clockingWolfAnim.SetBool("isIdle", true);
        }
        else if (curTime <= 8)
        {
            clockingEffect.SetActive(false);
            // 보이기
            CoyoteMesh.SetActive(true);
        }
        else
        {
            curTime = 0;
        }
    }
}
