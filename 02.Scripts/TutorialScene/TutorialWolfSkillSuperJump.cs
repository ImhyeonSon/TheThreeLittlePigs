using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWolfSkillSuperJump : MonoBehaviour
{
    public GameObject superJumpWolf;

    public GameObject upSuperJumpWolf;
    public GameObject downSuperJumpWolf;

    public Animator superJumpWolfAnim;

    private float curTime = 0;

    void OnEnable()
    {
        superJumpWolf.transform.position = downSuperJumpWolf.transform.position;
        curTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime <= 1.5f)
        {
            // 대기
            //superJumpWolfAnim.SetBool("isIdle", true);
            Debug.Log("대기모션");
        }
        else if (curTime <= 1.6f)
        {
            superJumpWolfAnim.SetBool("isIdle", false);
            superJumpWolfAnim.SetBool("isLand", false);
            superJumpWolfAnim.SetBool("isJump", true);
        }
        else if (curTime <= 3.75)
        {
            // 정점
            //superJumpWolfAnim.SetBool("isIdle", false);
            //superJumpWolfAnim.SetBool("isLand", false);
            //superJumpWolfAnim.SetBool("isJump", true);
            float jumpY = Mathf.Lerp(superJumpWolf.transform.position.y+20, upSuperJumpWolf.transform.position.y, 2f * Time.deltaTime);
            superJumpWolf.transform.position = new Vector3(superJumpWolf.transform.position.x, jumpY, superJumpWolf.transform.position.z);
            Debug.Log("위로");
        }
        else if (curTime <= 5.25f)
        {
            // 착지
            //jumpPigAnim.SetBool("isJump", false);
            //superJumpWolfAnim.SetBool("isIdle", false);
            //superJumpWolfAnim.SetBool("isLand", false);
            //superJumpWolfAnim.SetBool("isJump", true);
            float jumpY = Mathf.Lerp(superJumpWolf.transform.position.y, downSuperJumpWolf.transform.position.y, 2f * Time.deltaTime);
            superJumpWolf.transform.position = new Vector3(superJumpWolf.transform.position.x, jumpY, superJumpWolf.transform.position.z);
            Debug.Log("아래로");
        }
        else if (curTime <= 6.45f)
        {
            superJumpWolfAnim.SetBool("isJump", false);
            superJumpWolfAnim.SetBool("isIdle", false);
            superJumpWolfAnim.SetBool("isLand", true);
            // 착지모션 실행동안 대기
            Debug.Log("착지모션");
        }
        else
        {
            superJumpWolfAnim.SetBool("isJump", false);
            superJumpWolfAnim.SetBool("isIdle", false);
            superJumpWolfAnim.SetBool("isLand", false);
            curTime = 0;
        }
    }
}
