using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PigEquipList;

public class WolfSkillList : MonoBehaviour
{
    private WolfStatus WS;
    private WolfSkillRPC WSRPC;
    private WolfController WC;

    //애니메이션 관련
    private Animator anim;
    private int pigLayer = 1 << 16;
    public enum WolfSkillType
    {
        NULL,
        DASH,
        WIND_BLOWING,
        SUPER_JUMP,
        WOLFCLAW,
        HOWLING,
        CLOCKING,
        WOLFSHOCK,
        //이후에 추가 필요
    }


    // 스킬 이미지
    public GameObject[] skillImagesObjects = new GameObject[4];
    public Image[] skillImages = new Image[4];


    public WolfSkillType[] Skills = { WolfSkillType.NULL, WolfSkillType.NULL, WolfSkillType.NULL, WolfSkillType.NULL };

    // 스킬 쿨타임 Dictionary로 저장
    Dictionary<WolfSkillType, float> skillCoolTimesDIct = new Dictionary<WolfSkillType, float>();

    void Awake()
    {
        WS = GetComponent<WolfStatus>();
        WSRPC = GetComponent<WolfSkillRPC>();
        WC = GetComponent<WolfController>();
    }


    void Start()
    {
        anim = GetComponent<Animator>();
        skillImagesObjects[0] =  GameObject.Find("Canvas/CharacterCoolTimePanel/Keyboard Shift/Image Shift");
        skillImagesObjects[1] =  GameObject.Find("Canvas/CharacterCoolTimePanel/Keyboard Q/Image Q");
        skillImagesObjects[2] =  GameObject.Find("Canvas/CharacterCoolTimePanel/Keyboard E/Image E");
        skillImagesObjects[3] =  GameObject.Find("Canvas/CharacterCoolTimePanel/Keyboard R/Image R");


        // 스킬 쿨타임 시각화 UI 설정
        for (int i = 0; i < 4;  i++)
        {
            skillImages[i] = skillImagesObjects[i].GetComponent<Image>();
            skillImages[i].type = Image.Type.Filled;
            skillImages[i].fillMethod = Image.FillMethod.Radial360;
            skillImages[i].fillOrigin = 2;
            skillImages[i].fillClockwise = true;

        }

        // 스킬 쿨타임 Dictionary에 값 저장
        // DIct => Dict로 나중에 수정해야됨...ㅜ
        skillCoolTimesDIct.Add(WolfSkillType.NULL, 5.0f);
        skillCoolTimesDIct.Add(WolfSkillType.DASH, 6.0f);
        skillCoolTimesDIct.Add(WolfSkillType.WIND_BLOWING, 15.0f);
        skillCoolTimesDIct.Add(WolfSkillType.SUPER_JUMP, 8.0f);
        skillCoolTimesDIct.Add(WolfSkillType.HOWLING, 30.0f);        
        skillCoolTimesDIct.Add(WolfSkillType.WOLFCLAW, 10.0f);
        skillCoolTimesDIct.Add(WolfSkillType.CLOCKING, 30.0f);
        skillCoolTimesDIct.Add(WolfSkillType.WOLFSHOCK, 8.0f);
    }


    public void UseWolfSkill(WolfSkillType skillType)
    {
        switch (skillType)
        {
            case WolfSkillType.DASH:
                WSRPC.Dash();
                break;
            case WolfSkillType.WIND_BLOWING:
                WSRPC.WindBlowing();
                break;
            case WolfSkillType.SUPER_JUMP:
                WSRPC.ActiveSuperJump();
                break;
            case WolfSkillType.HOWLING:
                WSRPC.Howling();
                break;
            case WolfSkillType.WOLFCLAW:
                WSRPC.WolfClawAttack();
                break;
            case WolfSkillType.CLOCKING:
                WSRPC.UseWolfClocking();
                break;
            case WolfSkillType.WOLFSHOCK:
                WSRPC.UseWolfShock();
                break;
            default:
                Debug.Log("할당x");
                break;
        }
    }

    public void LearnWolfSkill(WolfSkillType skillType, int i)
    {
        Skills[i] = skillType;
        if (skillCoolTimesDIct.ContainsKey(skillType))
        {
            WS.maxSkillCooltimes[i] = skillCoolTimesDIct[skillType];
        }
        WS.isLearnedSkill[i] = true;
        Debug.Log($"{i} 번째 스킬 {skillType}을 배웠습니다.");
        SetSkillImg(skillType, i);
        /*
         구현 필요
         */
    }

    public void SetSkillImg(WolfSkillType skillType, int i)
    {
        Debug.Log("Skills/Wolf/" + skillType + "를" + i + "번째 사진에 넣기");
        skillImages[i].sprite = Resources.Load("Skills/Wolf/" + skillType, typeof(Sprite)) as Sprite;
    }



}