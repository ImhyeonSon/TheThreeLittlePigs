using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public PlayerController PC;
    public PhotonView PV;
    public PlayerCharacter character;
    public Animator anim;
    public PhotonGameManager PGM;

    private float speed;
    private float jumpSpeed;
    private float gravity;
    private float rotateTime; // 숫자가 낮아질 수록 속도가 매우 느리게 회전함
    private int hp = 1; // 처음에 기절 상태라고 해서 초기값 1로 설정
    private int maxHp = 10; // 항상 maxHp를 기준으로 표현 할 것.
    private int damage;
    //float skillCoolDown = 10f;

    private bool isJumped; // 점프한 상태인지
    public Transform groundCheck;

    //private int groundLayer = 1 << LayerMask.NameToLayer("Plane"); // LayerMask가 안먹혀서 직접가져옴
    private int groundLayer = 1 << 6 | 1 << 7 | 1<< 13; // house layer는 제거 => house안에 Trigger가 있어서 HouseLayer체크를 하면 공중부양 함
    private float circleSize = 0.52f;

    // 캐릭터 상태 관리
    //speed
    bool isChangeSpeed;
    float changeSpeedTimeFlow;
    //jumpSpeed
    bool isChangeJumpSpeed;
    float changeJumpSpeedTimeFlow;
    //gravity
    bool isChangeGravity;
    float changeGravityTimeFlow;


    //isStun
    bool isStun = false;
    float stunTimeFlow;
    public GameObject stunEffect;



    //attackSpeed
    bool isChangeAttackSpeed = false;
    float changeAttackSpeedTimeFlow;
    public float attackSpeed = 1.0f;

    // isDie
    public bool isDie = false;

    // 채팅 입력 중일 시 키보드 입력, 마우스 등 인풋 막기
    public Transform chatInput;
    bool isChatting;
    // player 상태변수

    // MiniMap 활성화 변수
    bool InputM;
    GameObject MiniMapCanvas;


    public bool GetIsChatting()
    {
        return isChatting;
    }
    public void SetIsChatting()
    {
        isChatting = chatInput.gameObject.activeSelf;
    }

    // Player가 돼지인지 늑대인지에 따라서 Canvas

    public virtual void Awake()
    {
        chatInput = GameObject.Find("Canvas").transform.Find("ChatPanel/InputField");
        MiniMapCanvas = GameObject.Find("Canvas").transform.Find("MiniMapCanvas").gameObject;
        PGM = GameObject.Find("PhotonGameManager").GetComponent<PhotonGameManager>();
        PC = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();
        SetDefaultStatus(); // 기본 스텟을 가져온다
    }
    public virtual void Start()
    {
        // 플레이어마다 입장이 다르므로 InitializeCharacter 정보를 보내주는 함수
        // 플레이어 캐릭터가 생성 될 때마다 각 자 로컬의 PGM에 저장된 캐릭터(자기캐릭터 정보)를 동기화 한다.
        PGM.InitializedTrigger(); // 내 캐릭터라면 초기 내 캐릭터 동기화 시켜주기
        PGM.OnLoadingCharacterCount();
    }

    public virtual void Update()
    {
        // 채팅중인지 값을 가져온다.
        SetIsChatting();
        // 속도, 점프 속도 등의 변수들을 할당한다.
        StatusSet();
        // 현재 어떤 상태인지 할당한다
        StatusCheck();
        if (PV.IsMine)
        {
            // 플레이어가 입력한 값들을 가져온다.
            PC.CheckInput();
            // 카메라 무빙 인풋, 카메라 움직임
            PC.GetMouseInput();
            PC.LookAround();
            // 실제 움직임
            PC.Move();
            PC.CameraZoom();

            // 미니맵 관련 함수
            InputMiniMap();

            /*            if (Input.GetKeyDown(KeyCode.X))
                        {
                            InitializeMyCharacter();
                        }*/
        }
    }

    public int Gethp()
    {
        return hp;
    }

    // Ground Check

    public void NowGround() // 현재 Status를 땅으로 변경
    {
        isJumped = false;
        if (PC != null)
        {
            PC.SetNowJumpSpeed(0);
        }
    }

    public bool IsGrounded()
    {

        ////TF값 리턴
        //Debug.Log(Physics.CheckSphere(groundCheck.position, 0.52f, groundLayer));

        bool nowGround = Physics.CheckSphere(groundCheck.position, circleSize, groundLayer);
        if (nowGround)
        {
            NowGround();
            return true;
        }
        return false;
    }

    public void SetCircleSize(float value)
    {
        circleSize = value;
    }


    private void OnDrawGizmos() //goundCheck 범위확인용
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(groundCheck.position, boxSize);
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, circleSize);
    }

    // Getter
    public float GetGravity()
    {
        return gravity;
    }
    public float GetJumpSpeed()
    {
        return jumpSpeed;
    }
    public float GetSpeed()
    {
        return speed;
    }
    public float GetRotateTime()
    {
        return rotateTime;
    }
    public bool GetIsJumped()
    {
        return isJumped;
    }

    public bool GetStun()
    {
        return isStun;
    }
    public bool GetIsDie()
    {
        return isDie;
    }

    public int GetHp()
    {
        //Debug.Log("Current HP: " + hp);
        return hp;
    }

    public int GetMaxHp()
    {
        //Debug.Log("Max HP: " + maxHp);
        return maxHp;
    }

    public int GetDamage()
    {
        return damage;
    }
    // Setter
    public void SetHp(int value)
    {
        hp = value;
        //Debug.Log("Setting HP: " + hp);
    }

    public void SetIsStun(bool value)
    {
        isStun = value;
    }

    public void SetIsDie(bool value)
    {
        isDie = value;
    }
    public void SetGroundCheck()
    {
        groundCheck = transform.Find("GroundCheck").gameObject.transform;
    }
    public void SetDamage(int value)
    {
        damage = value;
    }

    // 캐릭터 별 기본 Status 가져오기
    public virtual void SetDefaultStatus()
    {
        speed = character.GetDefaultSpeed();
        gravity = character.GetDefaultGravity();
        jumpSpeed = character.GetDefaultJumpSpeed();
        Debug.Log("점프 스피드" + jumpSpeed);
        rotateTime = character.GetDefaultRotateTime();
        maxHp = character.GetDefaultHp();
        hp = character.GetDefaultHp();
        damage = character.GetDefaultDamage();
    }

    // 영구 속도 조절은 현재 필요X
    //public void SetSpeed()
    //{ 
    //    speed = character.GetDefaultSpeed();
    //}
    public void SpeedCheck()
    {
        if (isChangeSpeed)
        {
            changeSpeedTimeFlow -= Time.deltaTime;
            if (changeSpeedTimeFlow <= 0f)
            {
                ResetSpeed();
            }
        }
    }
    public void ResetSpeed()
    {
        changeSpeedTimeFlow = 0f;
        isChangeSpeed = false;
        // Start 호출 순서에 따른 선 할당 필요
        if (character == null)
        {
            character = gameObject.GetComponent<PlayerCharacter>();
        }
        speed = character.GetDefaultSpeed(); // => 스피드 가져오기
    }
    public virtual void ChangeSpeed(float CS, float timeFlow)
    {
        isChangeSpeed = true;
        speed = CS;
        changeSpeedTimeFlow = timeFlow;
    }

    /// JumpSpeed TimeFlow
    public void JumpSpeedCheck()
    {
        if (isChangeJumpSpeed)
        {
            changeJumpSpeedTimeFlow -= Time.deltaTime;
            if (changeJumpSpeedTimeFlow <= 0f)
            {
                ResetJumpSpeed();
            }
        }
    }
    public void ResetJumpSpeed()
    {
        changeJumpSpeedTimeFlow = 0f;
        isChangeJumpSpeed = false;
        jumpSpeed = character.GetDefaultJumpSpeed(); // => 점프스피드 가져오기
    }
    public void ChangeJumpSpeed(float CJS, float timeFlow)
    {
        isChangeJumpSpeed = true;
        jumpSpeed = CJS;
        changeJumpSpeedTimeFlow = timeFlow;
    }

    /// Gravity TimeFlow
    public void GravityCheck()
    {
        if (isChangeGravity)
        {
            changeGravityTimeFlow -= Time.deltaTime;
            if (changeGravityTimeFlow <= 0f)
            {
                ResetGravity();
            }
        }
    }
    public void ResetGravity()
    {
        changeGravityTimeFlow = 0f;
        isChangeGravity = false;
        gravity = character.GetDefaultGravity(); // => 점프스피드 가져오기
    }
    public void ChangeGravity(float CG, float timeFlow)
    {
        isChangeGravity = true;
        gravity = CG;
        changeGravityTimeFlow = timeFlow;
    }


    /// Stun TimeFlow
    public void StunCheck()
    {
        if (isStun)
        {
            PC.anim.SetBool("isStun", true);
            stunTimeFlow -= Time.deltaTime;
            if (stunTimeFlow <= 0f)
            {
                ResetStun();
                PC.anim.SetBool("isStun", false);
            }
        }
    }
    public void ResetStun()
    {
        isStun = false;
        stunEffect.SetActive(false);
    }

    public void ChangeStun(float timeFlow)
    {
        isStun = true;
        stunTimeFlow = timeFlow;
        stunEffect.SetActive(true);
    }

    public void StunEffectOn()
    {
        stunEffect.SetActive(true);
    }

    public void AttackSpeedCheck()
    {
        if (isChangeAttackSpeed)
        {
            changeAttackSpeedTimeFlow -= Time.deltaTime;
            if (changeAttackSpeedTimeFlow <= 0f)
            {
                ResetAttackSpeed();
            }
        }
    }
    public void ResetAttackSpeed()
    {
        changeSpeedTimeFlow = 0f;
        isChangeAttackSpeed = false;
        attackSpeed = 1.0f;
    }
    public virtual void ChangeAttackSpeed(float CAS, float timeFlow)
    {
        isChangeAttackSpeed = true;
        attackSpeed = CAS;
        changeAttackSpeedTimeFlow = timeFlow;
    }

    // 새로 추가되는 코드
    public void StatusSet()
    {
        if (PC == null)
        {
            PC.GetComponent<PlayerController>();
        }
        PC.SetSpeed(speed);
        PC.SetGravity(gravity);
        PC.SetJumpSpeed(jumpSpeed);
        PC.SetRotateTime(rotateTime);
        PC.SetStun(isStun);
        PC.SetGround(IsGrounded());
    }

    public void StatusCheck()
    {
        SpeedCheck();
        GravityCheck();
        JumpSpeedCheck();
        StunCheck();
        AttackSpeedCheck();
    }

    public void InitializeMyCharacter()
    {
        Debug.Log("초기화 호출 : " + GameManager.Instance.GetMyCharacter());
        if (PV.IsMine)
        {
            PV.RPC("ChangeCharacter", RpcTarget.All, GameManager.Instance.GetMyCharacter()); // PlayerCharacter에 있는 RPC 함수
        }
    }


    //처음 시작할때 자신의 체력을 RPC로 동기화 해주는 함수
    [PunRPC]
    public void HpData(int hpValue)
    {
        Debug.Log($"체력 동기화 {hpValue}");
        SetHp(hpValue);
    }

    public void MouseLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void MouseUnlock()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void InputMiniMap()
    {
        InputM = Input.GetKeyDown(KeyCode.M);
        OnMiniMap();
    }

    public void OnMiniMap()
    {
        if (InputM)
        {
            // 미니맵 toggle
            MiniMapCanvas.SetActive(!MiniMapCanvas.activeSelf);
        }
    }

    public bool GetIsActiveMiniMap()
    {
        return MiniMapCanvas.activeSelf;
    }


}
