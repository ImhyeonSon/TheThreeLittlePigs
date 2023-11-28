using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMove : MonoBehaviour
{
    private Rigidbody myRigidbody;
    private AnimalStatus AS;
    float timedelay;
    float timeFrequency = 0;
    float moveDir = 0;

    Quaternion fromV;
    Quaternion toV;

    Vector3 yVector = new Vector3(0, 0, 0);
    Vector3 rotateDir = new Vector3(0, 0, 0);
    Vector3 speedVector = new Vector3(0, 0, 3f);  // 속도를 다르게 하고싶으면 이 값을 변화시키면 됨
    Vector3 moveVector = new Vector3(0, 0, 0);

    float reX = 0;
    float reY = 0;
    float reZ = 0;

    PhotonView PV;

    //Delay
    public void SetMoveVector(float speed)
    {
        speedVector = new Vector3(0, 0, speed);
    }

    float notMoveTimeDelay = -5f;
    float notMoveTimeFrequency = 0;
    public bool isMove = false;

    public virtual void Awake()
    {
        PV = GetComponent<PhotonView>();
        myRigidbody = GetComponent<Rigidbody>(); // 통과를 위해 코드를 변경
        AS = GetComponent<AnimalStatus>();
        if (PV.IsMine)
        {
            timedelay = UnityEngine.Random.Range(3, 7);
        }
    }

    public virtual void FixedUpdate()
    {
        if (PV.IsMine)
        {
            if (isMove)
            {
                TimeCheck();
            }
            else
            {
                NotMoveTimeCheck();
            }
        }
        if (isMove)  // 로컬 플레이어의 경우, isMove 값을 RPC 함수로 전달받음
        {
            RealMove();
        }
        else
        { 
            //Quaternion.Slerp(Quaternion.identity, Quaternion.identity, 5f * Time.deltaTime);
            transform.position = transform.position;
        }
    }

    private void NotMoveTimeCheck()
    {  // 움직이지 않는 동안 시간 확인
        notMoveTimeDelay += Time.deltaTime;
        if (notMoveTimeDelay > notMoveTimeFrequency)
        {  // 움직이지 않은 시간이 기준을 넘어가면 - 움직일 때가 됐다면
            isMove = true;  // 움직이고
            notMoveTimeDelay = 0;  // 움직이지 않은 시간 리셋
            //PV.RPC("SetRandomMoveRPC", RpcTarget.All, 0f, myRigidbody.velocity.y, speedVector.z, fromV, toV, isMove);
            AS.PeriodicSync();// 주기적 position 동기화
            RandomDirection();// 방향 바꾸기
        }
    }

    private void TimeCheck()  // 움직이는 동안 시간 확인
    {
        timeFrequency += Time.deltaTime;
        if (timeFrequency > timedelay)  // 경과 시간이 기준을 넘어가면 - 멈출 때가 됐다면
        {
            timeFrequency = 0;  // 경과 시간 리셋
            isMove = false;  // 멈추기
            timedelay = UnityEngine.Random.Range(3, 7);  // 움직일 시간 기준 설정
            notMoveTimeFrequency = UnityEngine.Random.Range(0, 9);  // 움직이지 않을 시간 기준 설정
            RandomMove();  
        }
    }

    private void RandomDirection()
    {  // 이동할 방향을 랜덤으로 설정하는 함수
        moveDir = UnityEngine.Random.Range(-18, 19);
        rotateDir = new Vector3(0, moveDir*20, 0);
        toV = Quaternion.Euler(rotateDir);
        RandomMove();
    }

    private void RandomMove()
    {
        //Debug.Log("랜덤무브 실행");
        fromV = transform.rotation;  // 현재 방향
        //// 여기서 초기화
        //moveVector = transform.TransformDirection(speedVector);
        ////Debug.Log(speedVector);
        //yVector.y = myRigidbody.velocity.y;
        //Vector3 randomMoveVector = moveVector + yVector;

        //// 움직임 값 및 여부 전송
        //PV.RPC("SetRandomMoveRPC", RpcTarget.All, randomMoveVector.x, randomMoveVector.y, randomMoveVector.z, fromV, toV, isMove);
        //float ySpeed = myRigidbody.velocity.y;
        //float zSpeed = speedVector.z;
        PV.RPC("SetRandomMoveRPC", RpcTarget.All, 0f, myRigidbody.velocity.y, speedVector.z, fromV, toV, isMove);
    }

    [PunRPC]
    public void SetRandomMoveRPC(float x, float y, float z, Quaternion from, Quaternion to, bool moveV)
    {
        fromV = from;
        toV = to;
        reX = x; reY = y; reZ = z;
        isMove = moveV;
    }

    public void RealMove()
    {
        //Debug.Log("from, to" + fromV+"  "+ toV);
        transform.rotation = Quaternion.Slerp(fromV, toV, 1.5f * Time.deltaTime);  // 방향 틀고
        Vector3 localVelocity = transform.forward * speedVector.z;
        myRigidbody.velocity = localVelocity;  // 속도 설정하고
        //myRigidbody.velocity = new Vector3(reX, reY, reZ);  // 속도 설정하고
    }

}
