using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialWASD : MonoBehaviour
{
    public GameObject movePig;
    public GameObject jumpPig;

    public GameObject leftMovePig;
    public GameObject rightMovePig;

    public GameObject downJumpPig;
    public GameObject upJumpPig;

    public Animator jumpPigAnim;

    private Vector3 leftMovePigPosition;
    private Vector3 rightMovePigPosition;
    private Quaternion leftMovePigRotation;
    private Quaternion rightMovePigRotation;

    private float curTime = 0;
    private float jumpCurTime = 0;
    private float keyTime = 0;

    public GameObject WKey;
    public GameObject SKey;
    public GameObject DKey;
    public GameObject AKey;
    public GameObject SpaceBar;

    private float originalScale = 0.7f;

    void OnEnable()
    {
        leftMovePigPosition = leftMovePig.transform.position;
        rightMovePigPosition = rightMovePig.transform.position;
        leftMovePigRotation = leftMovePig.transform.rotation;
        rightMovePigRotation = rightMovePig.transform.rotation;

        movePig.transform.position = rightMovePigPosition;
        movePig.transform.rotation = leftMovePigRotation;
        jumpPig.transform.position = downJumpPig.transform.position;

        jumpPigAnim = jumpPig.GetComponent<Animator>();

        curTime = 0;
        jumpCurTime = 0;
        keyTime = 0;
    }

    void Update()
    {
        curTime += Time.deltaTime;
        if (curTime <= 3)  // 왼쪽에서 오른쪽으로 이동
        {
            float moveX = Mathf.Lerp(movePig.transform.position.x, leftMovePigPosition.x, 1f * Time.deltaTime);
            //float moveX = Mathf.Lerp(movePig.transform.position.x, leftMovePigPosition.x, curTime / 6);
            //movePig.transform.position = Vector3.Lerp(movePig.transform.position, leftMovePigPosition, curTime / 4);
            movePig.transform.position = new Vector3(moveX, leftMovePigPosition.y, leftMovePigPosition.z);
            //Debug.Log("왼쪽에서 오른쪽으로" + curTime);
        }
        else if (curTime <= 4.5)  // 회전
        {
            movePig.transform.rotation = Quaternion.Slerp(movePig.transform.rotation, rightMovePigRotation * Quaternion.Euler(new Vector3(0, -200, 0)), 3f * Time.deltaTime);
            //Debug.Log("회전" + curTime + "현재" + movePig.transform.rotation + "목표" + rightMovePigRotation * Quaternion.Euler(new Vector3(0, -200, 0)));
        }
        else if (curTime <= 7.5)  // 오른쪽에서 왼쪽으로 이동
        {
            float moveX = Mathf.Lerp(movePig.transform.position.x, rightMovePigPosition.x, 1f * Time.deltaTime);
            //float moveX = Mathf.Lerp(movePig.transform.position.x, rightMovePigPosition.x, (curTime - 6) / 6);
            //movePig.transform.position = Vector3.Lerp(movePig.transform.position, rightMovePigPosition, (curTime - 7.5f) / 4);
            movePig.transform.position = new Vector3(moveX, leftMovePigPosition.y, leftMovePigPosition.z);
            //Debug.Log("오른쪽에서 왼쪽으로" + curTime);
        }
        else if (curTime <= 9)  // 이동
        {
            movePig.transform.rotation = Quaternion.Slerp(movePig.transform.rotation, leftMovePigRotation * Quaternion.Euler(new Vector3(0, -200, 0)), 3f * Time.deltaTime);
            //Debug.Log("회전" + curTime + "현재" + movePig.transform.rotation + "목표" + leftMovePigRotation * Quaternion.Euler(new Vector3(0, -200, 0)));
        }
        else
        {
            curTime = 0;
        }

        jumpCurTime += Time.deltaTime;
        if (jumpCurTime <= 2.25f)
        {
            // 대기
            jumpPigAnim.SetBool("isIdle", true);
            //Debug.Log("대기모션");
            if (jumpCurTime > 2.1f)
            {
                PressKey(jumpCurTime - 2.1f, SpaceBar);
            }
        }
        else if (jumpCurTime <= 2.65)
        {
            PressKey(jumpCurTime - 2.1f, SpaceBar);
            // 정점
            jumpPigAnim.SetBool("isIdle", false);
            jumpPigAnim.SetBool("isLand", false);
            jumpPigAnim.SetBool("isJump", true);
            float jumpY = Mathf.Lerp(jumpPig.transform.position.y, upJumpPig.transform.position.y, 5.5f * Time.deltaTime);
            jumpPig.transform.position = new Vector3(jumpPig.transform.position.x, jumpY, jumpPig.transform.position.z);
            //Debug.Log("위로");
        }
        else if (jumpCurTime <= 3.05f)
        {
            if (jumpCurTime <= 2.9f)
            {
                PressKey(jumpCurTime - 2.1f, SpaceBar);
            }
            // 착지
            //jumpPigAnim.SetBool("isJump", false);
            jumpPigAnim.SetBool("isIdle", false);
            jumpPigAnim.SetBool("isLand", false);
            jumpPigAnim.SetBool("isJump", true);
            float jumpY = Mathf.Lerp(jumpPig.transform.position.y, downJumpPig.transform.position.y, 5.5f * Time.deltaTime);
            jumpPig.transform.position = new Vector3(jumpPig.transform.position.x, jumpY, jumpPig.transform.position.z);
            //Debug.Log("아래로");
        }
        else if (jumpCurTime <= 4.25f)
        {
            jumpPigAnim.SetBool("isJump", false);
            jumpPigAnim.SetBool("isIdle", false);
            jumpPigAnim.SetBool("isLand", true);
            // 착지모션 실행동안 대기
            //Debug.Log("착지모션");
        }
        else
        {
            jumpPigAnim.SetBool("isLand", false);
            jumpPigAnim.SetBool("isJump", false);
            jumpPigAnim.SetBool("isIdle", true);
            jumpCurTime = 0;
        }

        keyTime += Time.deltaTime;
        if (keyTime <= 1)
        {
            PressKey(keyTime, AKey);
        }
        else if (keyTime <= 2)
        {
            PressKey(keyTime - 1, DKey);
        }
        else if (keyTime <= 3)
        {
            PressKey(keyTime - 2, WKey);
        }
        else if (keyTime <= 4)
        {
            PressKey(keyTime - 3, SKey);
        }
        else
        {
            keyTime = 0;
        }
    }

    private void PressKey(float time, GameObject key)
    {
        if (time <= 0.5f)
        {
            // 작아짐
            float scaleV = Mathf.Lerp(key.transform.localScale.x, originalScale * 0.8f, time);
            key.transform.localScale = new Vector3(scaleV, scaleV, scaleV);
        }
        else if (time <= 0.8f)
        {
            // 원래대로
            float scaleV = Mathf.Lerp(key.transform.localScale.x, originalScale, time);
            key.transform.localScale = new Vector3(scaleV, scaleV, scaleV);
        }
    }
}
