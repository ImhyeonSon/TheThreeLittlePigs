using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    private PlayerCharacter character;
    private PhotonView PV;


    // 카메라 관련 변수
    public Transform cameraArm;
    private float scrollSpeed = 200.0f;
    private float firstPointDistance = 10.0f;
    public bool isFirstPoint = false;
    float mouseX;
    float mouseY;
    // X축 최대 시야 제한
    private float maxCamX;


    // 채팅 창이 켜져있는지 확인
    public bool isChatting;

    // 애니메이션
    public Animator anim;

    // move관련 변수
    public CharacterController CC;
    public PlayerStatus PS;


    private Vector3 moveDir;
    private float mouseSensitivity;

    // 소리이펙트
    public AudioSource WalkingSFX;
    public AudioSource JumpSFX;

    // 상태 변수
    private float speed = 8f;
    private float gravity = 40f;
    private float jumpSpeed = 20f;
    private float rotateTime = 15f;

    private float nowJumpSpeed = 0f;
    private bool isJumped = false;

    private bool isGround = false;


    // player 디버프 변수
    private bool isStun;

    // input Manage
    public int inputH;
    public int inputV;
    public bool inputJump;

    public virtual void CheckInput()
    {
        inputH = 0;
        inputV = 0;
        inputJump = false;
        isChatting = PS.GetIsChatting();
        // 초기화 후 input이 있는지 확인
        if (!isChatting && !PS.GetIsDie() && !PS.GetStun())
        {
            inputH = (int)Input.GetAxisRaw("Horizontal");
            inputV = (int)Input.GetAxisRaw("Vertical");
            inputJump = Input.GetButton("Jump");
        }
    }
    public virtual void Awake()
    {
        anim = GetComponent<Animator>();
        character = GetComponent<PlayerCharacter>();
        PV = GetComponent<PhotonView>();
        isChatting = false;
    }
    public virtual void Start()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 0.75f);
        PS = GetComponent<PlayerStatus>();

    }

    // Character를 바꾸는 테스트 코드 시험용으로 만든거라 지워야 함
    //public void CharacterTest()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        character.TestSet("pig");
    //    }
    //    else if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        character.TestSet("wolf");
    //    }
    //}

    // 실제로 움직이는 함수
    public void Move()
    {
        float moveValue = 0f;
        moveValue = PlayerRotate(); // 움직이면 1, 안움직이면 0을 반환함
        //UpdateAnimation(moveValue);  // 여기서 애니메이션 업데이트 호출
        moveDir = new Vector3(0, 0, moveValue);
        float jumpValue = PlayerJump();
        // 이전에 점프하지 않았다면, 공중이지 않을 시에만 점프 가능
        moveDir = new Vector3(0, jumpValue, moveDir.z * speed);
        moveDir = transform.TransformDirection(moveDir); // 로컬이동을 월드로 변경
        CC.Move(moveDir * Time.deltaTime);
    }

    // 플레이어 캐릭터의 회전을 담당함
    public float PlayerRotate()
    {
        float moveForward = 0f;
        if (inputH != 0 || inputV != 0)
        {
            anim.SetBool("isWalk", true);
            //if (PS.IsGrounded() && !effectSfx.isPlaying)
            //{
            //    RunSound();
            //}
            if (PS.IsGrounded() && !WalkingSFX.isPlaying)
            {
                PV.RPC("WalkingAudioPlay", RpcTarget.All, true);
            }
            else if (!PS.IsGrounded())
            {
                PV.RPC("WalkingAudioPlay", RpcTarget.All, false);
            }

            moveForward = 1f;
            Vector3 cameraRotate = new Vector3(0, cameraArm.eulerAngles.y, 0);
            if (inputH != 0)
            { //clockwise =plus
                if (inputH < 0)
                {
                    cameraRotate.y += -90f;
                    if (inputV < 0)
                    {
                        cameraRotate.y += -45f;
                    }
                    else if (inputV > 0)
                    {
                        cameraRotate.y += 45f;
                    }
                }
                else if (inputH > 0)
                {
                    cameraRotate.y += 90f;
                    if (inputV < 0)
                    {
                        cameraRotate.y += 45f;
                    }
                    else if (inputV > 0)
                    {
                        cameraRotate.y -= 45f;
                    }
                }

            }
            else
            {
                if (inputV < 0)
                {
                    cameraRotate.y += 180f;
                }
            }
            // 구형 선형 보간으로 회전 Lerp => ...
            Quaternion fromV = transform.rotation;
            Quaternion toV = Quaternion.Euler(cameraRotate);
            Quaternion nowV = cameraArm.rotation;
            transform.rotation = Quaternion.Slerp(fromV, toV, rotateTime * Time.deltaTime);
            // 카메라 Arm의 회전을 절대좌표로 막아주는 부분
            cameraArm.rotation = nowV;
        }
        else
        {
            anim.SetBool("isWalk", false);
            PV.RPC("WalkingAudioPlay", RpcTarget.All, false);
        }
        return moveForward;
    }

    [PunRPC]
    public void WalkingAudioPlay(bool TF)
    {
        if (TF)
        {
            WalkingSFX.Play();
        }
        else
        {
            WalkingSFX.Stop();
        }
    }

    [PunRPC]
    public void JumpAudioPlay(bool TF)
    {
        if (TF)
        {
            JumpSFX.Play();
        }
        else
        {
            JumpSFX.Stop();
        }
    }

    public float PlayerJump()
    {
        if (isGround)
        {
            anim.SetBool("isJump", false);
            // 땅이라면 초기화
            //if (Input.GetButtonDown("Jump")) // 채팅창 입력이 안 켜져 있다면
            if (inputJump && !JumpSFX.isPlaying)
            {
                //JumpSound();
                nowJumpSpeed = jumpSpeed;
                Debug.Log($"점프스피드 {jumpSpeed}");
                PV.RPC("JumpAudioPlay", RpcTarget.All, true);
                anim.SetBool("isJump", true);
            }
        }
        else
        {
            if (nowJumpSpeed > -900f)
            {
                nowJumpSpeed -= gravity * Time.deltaTime; // 계속해서 중력만큼 속도 빼주기
            }
            //anim.SetBool("isJump", true);
        }

        return nowJumpSpeed;
    }

    
    public virtual void GetMouseInput()
    {
        mouseX=Input.GetAxis("Mouse X");
        mouseY=Input.GetAxis("Mouse Y");
    }
    public void GetMouseZeroInput()
    {
        mouseX=0;
        mouseY=0;
    }

    // 카메라의 움직임
    public void LookAround()
    {
        // 마우스의 X축과 Y축의 값
        Vector2 mouseDelta = new Vector2(mouseX, mouseY) * mouseSensitivity * 3;
        // 카메라 앵글
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        maxCamX = camAngle.x - mouseDelta.y;

        if (maxCamX < 180f)
        {
            maxCamX = Mathf.Clamp(maxCamX, -1f, 80f);
        }
        else
        {
            maxCamX = Mathf.Clamp(maxCamX, 335f, 361f);
        }

        cameraArm.rotation = Quaternion.Euler(maxCamX, camAngle.y + mouseDelta.x, camAngle.z);
    }


    public void SetNowJumpSpeed(float JS)
    {
        nowJumpSpeed = JS;
    }

    public void SetSpeed(float speedValue)
    {
        speed = speedValue;
    }
    public void SetGravity(float gravityValue)
    {
        gravity = gravityValue;
    }
    public void SetJumpSpeed(float jumpSpeedValue)
    {
        jumpSpeed = jumpSpeedValue;
    }
    public void SetRotateTime(float rotateTimeValue)
    {
        rotateTime = rotateTimeValue;
    }
    public void SetStun(bool TF)
    {
        isStun = TF;
    }

    public void SetGround(bool TF)
    {
        isGround = TF;
    }


    // 마우스 휠을 조작했을 때, CameraArm의 위치를 조절하는 함수 추가해야함!
    public virtual void CameraZoom()
    {


        //메인카메라에 접근하기
        //Transform cameraArmTransform = cameraArm.transform;
        //Transform mainCameraTransform = cameraArmTransform.Find("Main Camera");

        float scroollWheel = Input.GetAxis("Mouse ScrollWheel");

        //방향 벡터 구하기
        //Vector3 playerPosition = character.transform.position;
        //playerPosition.y += 1;
        //Vector3 dir = (playerPosition - mainCameraTransform.position).normalized;
        //mainCameraTransform.transform.position += dir * Time.deltaTime * scroollWheel * scrollSpeed;
        // 카메라 로컬좌표 기준으로 수정
        if (scroollWheel > 0)
        {
            MouseZoomIn();
            //mainCameraTransform.transform.localPosition = new Vector3(0, 0.4f, 0.1f);
            ////mainCameraTransform.transform.position += dir * (firstPointDistance);
            //isFirstPoint = true;
        }
        else if (scroollWheel < 0)
        {
            MouseZoomOut();
            //mainCameraTransform.transform.localPosition = new Vector3(0, 0.2f, -9f);
            ////mainCameraTransform.transform.position -= dir * firstPointDistance;
            //isFirstPoint = false;
        }


    }

    public void MouseZoomIn()
    {
        Transform cameraArmTransform = cameraArm.transform;
        Transform mainCameraTransform = cameraArmTransform.Find("Main Camera");
        mainCameraTransform.transform.localPosition = new Vector3(0, 0.4f, 0.1f);
        //mainCameraTransform.transform.position += dir * (firstPointDistance);
        isFirstPoint = true;
    }

    public void MouseZoomOut()
    {
        Transform cameraArmTransform = cameraArm.transform;
        Transform mainCameraTransform = cameraArmTransform.Find("Main Camera");
        mainCameraTransform.transform.localPosition = new Vector3(0, 0.2f, -9f);
        //mainCameraTransform.transform.position -= dir * firstPointDistance;
        isFirstPoint = false;
    }


}