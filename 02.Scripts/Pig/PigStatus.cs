using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PigEquipList;
using TMPro;
using UnityEngine.UI;

public class PigStatus : PlayerStatus
{
    //다른 스크립트 참조
    private PigEquipList PEL;
    private PigItemRPC PIRPC;
    private PigGetItem PGI;
    private UseUmbrella UU;
    private PigAttack PA;
    private PigInventoryUI PIUI;
    private PigHouseManager PHM;
    private PigSaveLife PSL;
    private UseScarecrow USC;
    private UseSlingShot USS;
    private GameManager gameManager;
    private Heirloom heirloom;
    private ChestInventoryUI CIUI;
    private PigHouseCheck PCH;
    private PigInteractionCraftingInfoUI PICUI;
    private PigKeyInfoUI PKIUI;

    // 제작대 및 상자 코드
    private PigInteractionCreateTable PICT;
    private PigInteractionChest PIC;
    // 플레이어의 위치 받아오기
    public Vector3 playerPosition;

    // 점프동작 관련
    private float pigCircleSize = 0.52f; // 돼지의 그라운드 체크 사이즈

    // 스킬 쿨타임 관련 변수
    public float[] skillCooltimes = new float[4];
    public float[] maxSkillCooltimes = new float[4];
    // 스킬 관련 변수
    public bool[] isLearnedSkill = new bool[4];
    public bool[] isActiveSkill = new bool[4];
    public bool[] isInputSkill = new bool[4];
    // 기본공격 이미지
    public Image attackImages;
    // 경험치 바 관련 변수
    public Transform hpBar;
    public TextMeshProUGUI hpBarTxt;
    // 피격시 체력바
    public HpBar HpBarUI;

    // 스킬 키코드
    private KeyCode[] keyCodes = { KeyCode.LeftShift, KeyCode.Q, KeyCode.E, KeyCode.R };

    // 늑대 스킬 효과
    public GameObject clawDebuffEffect;

    public bool isInHouse = false;

    public void SetInHouse(bool TF)
    {
        isInHouse = TF;
    }
    public bool GetInHouse()
    {
        return isInHouse;
    }

    public override void Awake()
    {
        base.Awake();
        PEL = GetComponent<PigEquipList>();
        PIRPC = GetComponent<PigItemRPC>();
        PGI = GetComponent<PigGetItem>();
        UU = GetComponent<UseUmbrella>();
        PA = GetComponent<PigAttack>();
        PIUI = GetComponent<PigInventoryUI>();
        PHM = GetComponent<PigHouseManager>();
        PSL = GetComponent<PigSaveLife>();
        USC = GetComponent<UseScarecrow>();
        USS = GetComponent<UseSlingShot>();
        heirloom = GetComponent<Heirloom>();
        CIUI = GetComponent<ChestInventoryUI>();
        PCH = GetComponent<PigHouseCheck>();
        PICUI = GetComponent<PigInteractionCraftingInfoUI>();
        PKIUI = GetComponent<PigKeyInfoUI>();

        //ui 표시 관련///
        character = GetComponent<PlayerCharacter>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        hpBar = GameObject.Find("Canvas").transform.Find("MyCharacterStatusPanel/HpBar");
        hpBarTxt = GameObject.Find("Canvas").transform.Find("MyCharacterStatusPanel/HpBar/Fill/HpText").GetComponent<TextMeshProUGUI>();
        if (PV.IsMine)
        {
            hpBar.gameObject.SetActive(true);
        }
        
        ///

        // 제작대 상호작용
        PICT = GetComponent<PigInteractionCreateTable>();
        // 상자 상호작용
        PIC = GetComponent<PigInteractionChest>();
    }

    //스타트
    public override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        SetCircleSize(pigCircleSize); //그라운드 체크 사이즈 정하기 한번만 실행하면 됨

        HpBarUI.SetMaxHp(GetMaxHp());
        if (PV.IsMine)
        {
            SetHpBar();
        }
    }


    // 업데이트
    public override void Update()
    {
        if (PGM.GetIsGameStart()) // 게임 시작 시 조작, 컨트롤 가능
        {
            CheckPigDie(); // Die 체크
            PCH.HouseCheck();
            base.Update();
            if (PV.IsMine)
            {
                if (!GetIsChatting() && !GetIsDie() && !GetStun())
                {
                    PGI.PigInput(); // 아이템 습득 입력키 관리
                    PGI.GetItem(); // 재료 습득
                    PEL.PigInputLogic(); // 돼지 아이템 실제발동 로직 입력키 
                    PEL.PigInputSelect(); // 돼지 손에 장착한 아이템 활성화 로직 입력키
                    PHM.PigInput(); // 집 중복체크 입력 키 관리
                    PHM.HouseDupleCheck(); // 돼지 근처에서 집의 중복을 체크
                    PSL.PigInput(); // 돼지 서로 살려주기 입력키
                    PSL.CheckIsSaving(); // 돼지 서로 살려주기 쿨타임
                    //USS.ShowAim(); // 줌인시 새총에임 활성화

                    heirloom.HeirloomInput(); // 상자에 가보 저장하는 입력키 V
                    heirloom.PutOrGetHeirloomToChest(); // 상자에 가보 저장, 꺼내오기

                    ///개발용 치트
                    //PGI.ItemCheet(); //1,2,3,4번 키로 아이템 획득
                    ///////////////

                }
                PA.CheckAttackCooltime(); // 돼지의 공격 쿨타임 체크
                PGI.CheckFarmingObject(); // 돼지가 피직스로 파밍 오브젝트를 체크
                PGI.CheckGetItemCooltime();  // 돼지의 아이템 채집 쿨타임 체크
                UU.CheckUmbrellaUsing(); // 우산사용여부 체크 
                USC.CheckCooltime(); // 호랑이 허수아비 쿨타임 체크
                USS.CheckCooltime(); // 새총 쿨타임 체크
                PSL.ShowDeadMessage(); // 돼지가 죽었을때 주사기를 사용하는 기능
                PIUI.OpenInventory();// 인벤토리 오픈
                PEL.CheckEquipZeroCount(); // 소비아이템 개수가 0이 되면 자동으로 장착한 장비가 해제되는 기능
                PEL.CheckEternalEquip(); // 영구 귀속 아이템의 존재를 체크한다
                PEL.ChangeAttackImageUI(); // 장착한 장비에 따라서 공격 UI를 바꾼다     
                USS.PigAimManager(); // 새총 장착 여부에 따라 에임을 보여주는 기능
                USS.CheckSpawnPosition(); //총알 생성위치를 계속 연산해서 체크
                USS.UpdateShootDirection(); //총알 발사방향을 계속 연산해서 체크
                PEL.CoolTimeRender(); // 돼지 쿨타임 표시
                

                // 제작대 및 상자 체크 함수들 => 일단은 채팅이 켜져있을때 안막았음
                GetPlayerPosition();
                PICT.CheckCreateTableInteractability(playerPosition); // 제작대와 상호작용 가능한 위치인지 판별
                PIC.CheckChestInteractability(playerPosition); // 상자를 열 수 있는 위치인지 판별

                PKIUI.PigOpenKeyInfoUI(); // 키 정보 띄우는 키 관리 F1
                PICUI.PigOpenCraftingInfoUI(); // 제작 정보 띄우는 키 관리 F2
                PigMouse();
            }
        }

    }


    public override void SetDefaultStatus()
    {
        base.SetDefaultStatus();
    }


    // 늑대의 공격 감지
    public void ReceiveAttack(int wolfDamage)
    {
        // 죽지 않았다면 데미지를 입음, 혹시 몰라서 Hp 중복 체크
        int curPigHp = GetHp();
        if (!GetIsDie() && curPigHp > 0 && !GetInHouse()) // 집안에 없다면 피해 입기
        {
            int changeHp = curPigHp - wolfDamage;
            PV.RPC("RecieveAttackRPC", RpcTarget.All, changeHp);
        }
    }

    [PunRPC]
    public void RecieveAttackRPC(int pigHp)
    {
        SetHp(pigHp);
        //Debug.Log("현재 체력 : "+Gethp());
        HpBarUI.SetHpBar(pigHp);
        Debug.Log("체력" + pigHp);
        if (PV.IsMine)
        {
            SetHpBar();
            anim.SetTrigger("RECEIVEATTACK");
        }
    }

    // 돼지의 기절 체크
    private void CheckPigDie()
    {
        int curPigHp = GetHp();
        //Debug.Log($"사망체크시 돼지의 체력: {curPigHp}");
        if (curPigHp <= 0 && !GetIsDie())
        {
            Debug.Log($"기절시 돼지의 체력: {curPigHp}");
            PigDieRPC();
            if (heirloom.GetHeirloom()) // 죽었을 때 가보가 있다면 패배.
            {
                if (PV.IsMine)
                {
                    PGM.PigLoseRPC();
                }
            }
        }
        if (curPigHp > 0)
        {
            // 체력차면 되살아나는 로직 => 즉, 주사기 사용 시 체력이 늘어나도록 설정
            SetIsDie(false);
            anim.SetBool("isDie", false);
        }
    }

    // 돼지 기절 RPC 
    public void PigDieRPC()
    {
        Debug.Log("돼지가 사망 했습니다.");
        SetIsDie(true);
        anim.SetBool("isDie", true);
    }

    //돼지의 위치를 받아오는 함수
    void GetPlayerPosition()
    {
        playerPosition = transform.position;
    }

    //돼지의 체력을 hp바에 반영

    public void SetHpBar()
    {
        float hpRatio = GetHp() / (float)GetMaxHp();
        if (hpRatio <= 0) {
            hpRatio = 0f;
        }
        Debug.Log(hpRatio);
        hpBar.GetComponent<Slider>().value = hpRatio;
        hpBarTxt.text = $"{(int)(hpRatio * 100)} %";
    }

    public void PigMouse()
    {
        if (GetIsChatting() || PICT.GetIsActiveCreateTable() || PIUI.GetIsActiveInventory() || CIUI.GetIsActiveChest() || GetIsActiveMiniMap() || PICUI.GetIsOpenCraftingInfoUI()
            || PKIUI.GetIsPigOpenKeyInfoUI())
        {
            MouseUnlock();
        }
        else
        {
            MouseLock();
        }
    }

    public override void ChangeSpeed(float CS, float time)
    {
        base.ChangeSpeed(CS, time);
    }


    public void OnClawDebuffEffect()
    {
        PV.RPC("OnClawDebuffEffectRPC", RpcTarget.All);
    }

    [PunRPC]
    public void OnClawDebuffEffectRPC()
    {
        clawDebuffEffect.SetActive(true);
        ChangeSpeed(GetSpeed()*0.85f, 3f); // 3초간 현재 속도에서 0.9배씩 감소
    }

    [PunRPC]
    public void RecieveShockRPC(float shockTime)
    {
        RecieveShock(shockTime);
    }

    public void RecieveShock(float shockTime)
    {
        ChangeStun(shockTime);
        StunEffectOn();
    }

    

}
