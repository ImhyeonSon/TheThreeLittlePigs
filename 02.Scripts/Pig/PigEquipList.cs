using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;
using static WolfSkillList;

public class PigEquipList : MonoBehaviour
{
    //다른 스크립트 참조 
    private PhotonView PV; // 포톤 뷰
    private UseSlingShot USS;// 새총 사용 스크립트
    private UseUmbrella UU;// 우산 사용 스크립트
    private UseScarecrow USC; // 허수아비 사용 스크립트
    private PlayerStatus PS; // 플레이어 스테이터스 
    private PigAttack PA; // 돼지 일반공격
    private PigGetItem PGI; // 돼지 인벤토리 및 아이템 획득
    private PigSaveLife PSL; // 돼지가 서로 살리고 주사기 관련
    private PigInteractionChest PIC;
    private PigInteractionCreateTable PICT;


    //애니메이션 관련
    private Animator anim;

    //소리
    public AudioSource SlingShotEquipSFX;
    public AudioSource NotiSFX;
    public AudioSource BadNotiSFX;

    //알림 메시지 관련
    public FadeController FC;

    public enum PigItemType
    {
        NULL, // 빈손
        SLINGSHOT, // 새총
        SCARECROW,  // 허수아비 
        UMBRELLA,//우산
        AXE,// 도끼
        PICKAXE,//곡괭이
        SHOVEL,//삽
        SICKLE, //낫 
        KNIFE,//칼
        STRAWSHOES,//짚신
        LEATHERSHOES,// 가죽신발 
    }


    public enum PigSkillType
    {
        NULL,
        SLINGSHOT,
        UMBRELLA,
        GETITEM,
        SCARECROW,
    }

    public enum PigAttackType
    {
        NULL,
        ATTACK,
        BULLET,
    }


    // 스킬 ui 관련
    public GameObject[] skillImagesObjects = new GameObject[4];
    public Image[] skillImages = new Image[6];
    public PigSkillType[] Skills = {
        PigSkillType.SLINGSHOT,
        PigSkillType.UMBRELLA,
        PigSkillType.GETITEM,
        PigSkillType.SCARECROW,
    };
    public GameObject[] attackImagesObjects = new GameObject[1];
    public Image[] attackImages = new Image[1];


    // 길이가 9인 모든 장비의 게임 오브젝트가 들어있는 리스트
    public List<GameObject> allEquipments;
    // 현재 돼지가 선택중인 장비를 저장할 변수
    public PigItemType SelectedPigItem;
    // 스킬 쿨타임 관련 변수
    public float[] skillCooltimes = new float[4];
    public float[] maxSkillCooltimes = new float[4];
    public bool[] isCoolingDown = new bool[4];

    public float[] attackCooltimes = new float[2];
    public float[] maxAttackCooltimes = new float[2];
    public bool[] isAttackCoolingDown = new bool[2];

    void Awake()
    {
        PV = GetComponent<PhotonView>(); // photon view
        USS = GetComponent<UseSlingShot>(); // use sling shot
        UU = GetComponent<UseUmbrella>(); // use umbrella
        USC = GetComponent<UseScarecrow>(); // use scarecrow
        PS = GetComponent<PlayerStatus>(); //player status
        PA = GetComponent<PigAttack>(); // pig attack
        PGI = GetComponent<PigGetItem>();
        PSL = GetComponent<PigSaveLife>();
        PIC = GetComponent<PigInteractionChest>();
        PICT = GetComponent<PigInteractionCreateTable>();
        //알림 메시지 
        FC = GameObject.Find("Canvas").transform.Find("NotificationUI").GetComponent<FadeController>();
        
    }

    void Start()
    {
        
        // 이 부분에서 skillImages 배열을 초기화
        skillImages = new Image[4];
        attackImages = new Image[1];

        anim = GetComponent<Animator>();

        skillImagesObjects[0] = GameObject.Find("Canvas/CharacterCoolTimePanel/Keyboard Shift/Image Shift");
        skillImagesObjects[1] = GameObject.Find("Canvas/CharacterCoolTimePanel/Keyboard Q/Image Q");
        skillImagesObjects[2] = GameObject.Find("Canvas/CharacterCoolTimePanel/Keyboard E/Image E");
        skillImagesObjects[3] = GameObject.Find("Canvas/CharacterCoolTimePanel/Keyboard R/Image R");

        attackImagesObjects[0] = GameObject.Find("Canvas/CharacterCoolTimePanel/Attack/Image");

        // 스킬 초기화
        Skills[0] = PigSkillType.SLINGSHOT;
        Skills[1] = PigSkillType.UMBRELLA;
        Skills[2] = PigSkillType.GETITEM;
        Skills[3] = PigSkillType.SCARECROW;

        // 스킬 쿨타임 초기화
        skillCooltimes[0] = 0.3f; // SLINGSHOT
        skillCooltimes[1] = 2f; // UMBRELLA
        skillCooltimes[2] = SelectedPigItem == PigItemType.NULL ? 4f : 2f;
        skillCooltimes[3] = 5f; // SCARECROW

        attackCooltimes[0] = 1f;
        attackCooltimes[1] = 1f;

        // 최대 스킬 쿨타임 초기화
        maxSkillCooltimes[0] = 0.3f;
        maxSkillCooltimes[1] = 2f;
        maxSkillCooltimes[2] = SelectedPigItem == PigItemType.NULL ? 4f : 2f;
        maxSkillCooltimes[3] = 5f;

        maxAttackCooltimes[0] = 1f;
        maxAttackCooltimes[1] = 1f;


        // 스킬 쿨타임 시각화 UI 설정
        for (int i = 0; i < 4; i++)
        {
            skillImages[i] = skillImagesObjects[i].GetComponent<Image>();
            skillImages[i].type = Image.Type.Filled;
            skillImages[i].fillMethod = Image.FillMethod.Radial360;
            skillImages[i].fillOrigin = 2;
            skillImages[i].fillClockwise = true;
        }

        attackImages[0] = attackImagesObjects[0].GetComponent<Image>();
        attackImages[0].type = Image.Type.Filled;
        attackImages[0].fillMethod = Image.FillMethod.Radial360;
        attackImages[0].fillOrigin = 2;
        attackImages[0].fillClockwise = true;



        if (PV.IsMine)
        {
            SetSkillImg();
            SetAttackImg(PigAttackType.ATTACK, 0);
        }

    }



    //장비 장착을 토글형식으로 구현하기위한 변수들
    public bool isSlingshotEquiped = false;
    private bool isCheetEquiped = false;
    private bool isKnifeEquiped = false;

    //돼지의 장비(아이템) 선택 키맵
    public void PigInputSelect()
    {
        if (!PGI.isGettingItem)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                //PV.RPC("UsePigItem", RpcTarget.All, PigItemType.NULL);
            }
            else if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (PGI.CountItem("SlingShot") > 0)
                {
                    if (!isSlingshotEquiped)
                    {
                        FC.StartFadeIn("새총을 장착합니다.");
                        //소리관련RPC
                        PV.RPC("SlingShotEquipAudioPlay", RpcTarget.All, true);
                        PV.RPC("UsePigItem", RpcTarget.All, PigItemType.SLINGSHOT);
                        isSlingshotEquiped = true;

                    }
                    else
                    {
                        PV.RPC("UsePigItem", RpcTarget.All, PigItemType.NULL);
                        if (isSlingshotEquiped)
                        {
                            //소리관련RPC
                            PV.RPC("SlingShotEquipAudioPlay", RpcTarget.All, true);
                            FC.StartFadeIn("새총 장착을 해제합니다.");
                        }
                        isSlingshotEquiped = false;

                    }
                }
                else
                {
                    FC.StartFadeIn("인벤토리에 새총이 없습니다.");
                    BadNotiSFX.Play();
                }

            }
            else if (Input.GetKeyDown(KeyCode.Keypad7)) //치트키
            {
                if (!isKnifeEquiped)
                {
                    isKnifeEquiped = !isKnifeEquiped;
                    PV.RPC("UsePigItem", RpcTarget.All, PigItemType.KNIFE);  // 제작시 동물 공격시에만 활성화 합니다
                }

                else
                {
                    isKnifeEquiped = !isKnifeEquiped;
                    PV.RPC("UsePigItem", RpcTarget.All, PigItemType.NULL);
                }
            }

            else if (Input.GetKeyDown(KeyCode.Keypad6)) //치트키
            {
                if (!isCheetEquiped)
                {
                    isCheetEquiped = !isCheetEquiped;
                    PV.RPC("UsePigItem", RpcTarget.All, PigItemType.LEATHERSHOES);  // 제작시 영구 장착합니다.
                }
                else
                {
                    PV.RPC("UsePigItem", RpcTarget.All, PigItemType.NULL);
                    isCheetEquiped = !isCheetEquiped;
                }
            }
        }
    }




    // 장비 선택하기 (장비 오브젝트 활성화를 RPC로 다른 유저들에게 알려야 합니다.)
    [PunRPC]
    public void UsePigItem(PigItemType itemType)
    {

        // 먼저 모든 장비를 비활성화합니다.
        foreach (var equipment in allEquipments)
        {
            equipment.SetActive(false);
        }

        // 선택된 장비만 활성화
        switch (itemType)
        {
            case PigItemType.NULL:
                Debug.Log("무기 해제");

                SelectedPigItem = itemType;
                PS.ChangeSpeed(8f, 100000000f); // 기본 속도
                PS.ChangeJumpSpeed(15f, 100000000f);
                PS.SetDamage(1);

                if (CheckEternalEquipOnlyOne) //귀속 장비가 있을땐 NULL로 가도 귀속 장비를 활성화 시켜야 한다.
                {
                    allEquipments[8].SetActive(true);
                    allEquipments[9].SetActive(true);
                    PS.ChangeSpeed(11f, 100000000f);
                    PS.ChangeJumpSpeed(16f, 100000000f); // 신발 속도
                }

                break;
            case PigItemType.SLINGSHOT:
                Debug.Log("새총 사용?!");
                SelectedPigItem = itemType;
                allEquipments[0].SetActive(true);

                if (CheckEternalEquipOnlyOne) //귀속 장비가 있을땐 SLINGSHOT로 가도 귀속 장비를 활성화 시켜야 한다.
                {
                    allEquipments[8].SetActive(true);
                    allEquipments[9].SetActive(true);
                    PS.ChangeSpeed(12f, 100000000f);
                    PS.ChangeJumpSpeed(20f, 100000000f);
                }

                break;
            case PigItemType.SCARECROW:
                Debug.Log("허수아비 사용?!");
                SelectedPigItem = itemType;
                allEquipments[1].SetActive(true);
                break;
            case PigItemType.UMBRELLA:
                Debug.Log("우산 사용?!");
                SelectedPigItem = itemType;
                allEquipments[2].SetActive(true);
                break;
            case PigItemType.AXE:
                Debug.Log("도끼 사용?!");
                SelectedPigItem = itemType;
                allEquipments[3].SetActive(true);
                break;
            case PigItemType.PICKAXE:
                Debug.Log("곡괭이 사용?!");
                SelectedPigItem = itemType;
                allEquipments[4].SetActive(true);
                break;
            case PigItemType.SHOVEL:
                Debug.Log("삽 사용?!");
                SelectedPigItem = itemType;
                allEquipments[5].SetActive(true);
                break;
            case PigItemType.SICKLE:
                Debug.Log("낫 사용?!");
                SelectedPigItem = itemType;
                allEquipments[6].SetActive(true);
                break;
            case PigItemType.KNIFE:
                Debug.Log("칼 사용?!");
                SelectedPigItem = itemType;
                allEquipments[7].SetActive(true);
                PS.SetDamage(20);
                break;
            case PigItemType.STRAWSHOES:
                Debug.Log("짚신 사용?!");
                SelectedPigItem = itemType;
                allEquipments[8].SetActive(true);
                allEquipments[9].SetActive(true);
                PS.ChangeSpeed(12f, 100000000f);
                PS.ChangeJumpSpeed(20f, 100000000f);
                break;
            case PigItemType.LEATHERSHOES:
                SelectedPigItem = itemType;
                Debug.Log("가죽신발 사용?!");
                PS.ChangeSpeed(80f, 100000000f);
                PS.ChangeJumpSpeed(40f, 100000000f);
                break;
            default:
                break;
        }
    }


    // 아이템 사용 키맵
    public void PigInputLogic()
    {
        if (!PS.GetStun() && !PGI.isGettingItem) // 플레이어의 상태가 기절상태가 아니라면
        {

            if (Input.GetMouseButtonDown(0) && SelectedPigItem == PigItemType.NULL)
            {
                PA.UseAttack();  // 돼지 일반 공격
            }

            if (Input.GetMouseButtonDown(0) && SelectedPigItem == PigItemType.STRAWSHOES)
            {
                PA.UseAttack();  // 신발신은 돼지 일반 공격
            }

            if (Input.GetMouseButtonDown(0) && SelectedPigItem == PigItemType.KNIFE)
            {
                PA.UseAttack();  // 돼지 칼로 공격
            }

            if (Input.GetMouseButtonDown(0) && SelectedPigItem == PigItemType.SLINGSHOT)
            {
                USS.ReadySlingShot(SelectedPigItem);  // 돼지가 새총 투사체를 날릴때
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                UU.OpenUmbrella();  // 돼지가 우산을 펼침

            }

            if (Input.GetKey(KeyCode.E))
            {
                if (!PGI.isGettingItem && (PIC.chestsInRange.Length == 0) && (PICT.ObjCheck.Length == 0))
                {
                    FC.StartFadeIn("근처에 상호작용 할 수 있는 물체가 없습니다.");

                }
            }

            if (Input.GetKey(KeyCode.R))
            {
                USC.TrapScarecrow();  // 돼지가 허수아비를 설치함

            }

            if (Input.GetMouseButtonDown(1))
            {
                if (!PSL.isSavingLife)
                    FC.StartFadeIn("근처에 살릴수 있는 돼지가 없습니다.");
            }

        }

    }


    public void SetSkillImg(PigSkillType skillType, int i)
    {
        Debug.Log("Skills/Pig/" + skillType + "를" + i + "번째 사진에 넣기");

        skillImages[i].sprite = Resources.Load("Skills/Pig/" + skillType.ToString(), typeof(Sprite)) as Sprite;
    }

    public void SetSkillImg()
    {
        for (int i = 0; i < Skills.Length; i++)
        {
            SetSkillImg(Skills[i], i);
        }
    }

    public void SetAttackImg(PigAttackType attackType, int i)
    {

        Sprite loadedSprite = Resources.Load<Sprite>("Skills/Pig/" + attackType.ToString());
        attackImages[i].sprite = loadedSprite;
    }


    //쿨타임 관련
    public void CoolTimeRender()
    {
        // 쿨타임 시작
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isCoolingDown[0])
        {
            skillCooltimes[0] = maxSkillCooltimes[0];
            isCoolingDown[0] = true;
        }
        if (Input.GetKeyDown(KeyCode.Q) && !isCoolingDown[1])
        {
            skillCooltimes[1] = maxSkillCooltimes[1];
            isCoolingDown[1] = true;
        }
        if (Input.GetKeyDown(KeyCode.E) && !isCoolingDown[2])
        {
            skillCooltimes[2] = maxSkillCooltimes[2];
            isCoolingDown[2] = true;
        }
        if (Input.GetKeyDown(KeyCode.R) && !isCoolingDown[3])
        {
            skillCooltimes[3] = maxSkillCooltimes[3];
            isCoolingDown[3] = true;
        }
        if (Input.GetMouseButtonDown(0) && !isAttackCoolingDown[0])
        {
            attackCooltimes[0] = maxAttackCooltimes[0];
            isAttackCoolingDown[0] = true;
        }

        // 쿨타임 감소 이미지

        for (int i = 0; i < isCoolingDown.Length; i++)
        {
            bool isSkillAvailable = CheckSkillAvailability(i);


            if (isSkillAvailable && i != 2)
            {
                if (isCoolingDown[i])
                {
                    skillCooltimes[i] -= Time.deltaTime;
                    skillImages[i].fillAmount = 1 - skillCooltimes[i] / maxSkillCooltimes[i];

                    if (skillCooltimes[i] <= 0)
                    {
                        skillImages[i].fillAmount = 1;
                        isCoolingDown[i] = false; // 쿨타임 종료
                    }
                }
            }

            if (i != 2)
            {
                // 스킬 사용 가능 여부에 따른 이미지 색상 변경
                skillImages[i].color = isSkillAvailable ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1);
            }
        }


        // 공격 쿨타임 감소 이미지
        for (int i = 0; i < isAttackCoolingDown.Length; i++)
        {
            if (isAttackCoolingDown[i])
            {
                attackCooltimes[i] -= Time.deltaTime;
                attackImages[i].fillAmount = 1 - attackCooltimes[i] / maxAttackCooltimes[i];

                if (attackCooltimes[i] <= 0)
                {
                    attackImages[i].fillAmount = 1;
                    isAttackCoolingDown[i] = false; // 쿨타임 종료
                }
            }
        }
    }

    // 스킬 사용 가능 여부 확인
    private bool CheckSkillAvailability(int index)
    {
        switch (index)
        {
            case 0:
                return PGI.CountItem("SlingShot") > 0;
            case 1:
                return PGI.CountItem("Umbrella") > 0;
            case 2:
                return PGI.isGettingItem;
            case 3:
                return PGI.CountItem("Tiger") > 0;
            default:
                return false;
        }
    }


    //공격 이미지 토글 되게 

    public void ChangeAttackImageUI()
    {
        if (SelectedPigItem == PigItemType.SLINGSHOT)
        {
            SetAttackImg(PigAttackType.BULLET, 0);
        }
        else if (SelectedPigItem == PigItemType.NULL)
        {
            SetAttackImg(PigAttackType.ATTACK, 0);
        }
    }

    public bool GetUseSlingShot() // SlingShot 사용시 조준선 장착하도록하기 
    {
        if (SelectedPigItem == PigItemType.SLINGSHOT)
        {
            return true;
        }
        return false;
    }

    //소비되는 장비아이템 사용중에 
    //인벤토리에서 해당 아이템 개수가 0이 되면 
    //강제로 착용 해제하는 함수
    public void CheckEquipZeroCount()
    {
        if (SelectedPigItem == PigItemType.SLINGSHOT && PGI.CountItem("SlingShot") == 0)
        {
            PV.RPC("UsePigItem", RpcTarget.All, PigItemType.NULL);
            FC.StartFadeIn("새총을 전부 소비하였습니다.");
            isSlingshotEquiped = false;
        }
    }

    //영구장비 검사
    //게임이 로드되고 한번만 알려주면된다 
    private bool CheckEternalEquipOnlyOne = false;
    //채집중에도 SelectedPigItem == PigItemType.NULL인 순간이 있어서 조건문 안에 들어간다.
    public void CheckEternalEquip()
    {
        if (SelectedPigItem == PigItemType.NULL)
        {
            if (PGI.SearchItem("Shoes"))
            {
                if (!CheckEternalEquipOnlyOne)
                {
                    FC.StartFadeIn("신발을 획득해서 이동속도가 빨라집니다.");
                    NotiSFX.Play();
                }
                CheckEternalEquipOnlyOne = true;
                PV.RPC("UsePigItem", RpcTarget.All, PigItemType.NULL);
            }
        }
    }


    //소리관련RPC
    [PunRPC]
    public void SlingShotEquipAudioPlay(bool TF)
    {
        if (TF)
        {
            SlingShotEquipSFX.Play();
        }
        else
        {
            SlingShotEquipSFX.Stop();
        }
    }



}
