using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using static PlayerCharacter;
using static WolfSkillList;

public class WolfStatus : PlayerStatus
{
    private WolfSkillList WSL;
    private WolfSkillRPC WSRPC;
    private WolfSkillLearning WSLearning;
    private GameManager gameManager;
    public GameObject LevelupFX;

    private WolfKeyInfoUI WKIF;

    // Wolf 캐릭터 상속받아서 Exp와 기력 추가해야됨.
    int maxExp;
    int exp = 0;
    int maxMp;
    int mp = 0;

    // 물리동작 관련
    private float wolfCircleSize = 0.81f; // 늑대의 그라운드 체크 사이즈


    // 스킬 쿨타임 관련 변수
    public float[] skillCooltimes = new float[4];
    public float[] maxSkillCooltimes = new float[4];

    // 스킬 관련 변수
    public bool[] isLearnedSkill = new bool[4];
    public bool[] isActiveSkill = new bool[4];
    public bool[] isInputSkill = new bool[4];

    // 기본공격 관련 변수
    public bool isActiveAttacking = true;
    public float attackingTimeFlow = 0f;
   

    // 기본공격 이미지
    public Image attackImages;

    // 바람불기 관련 변수
    public bool isWindBlowing = false;
    public float windBlowingTimeFlow = 0f;
    public GameObject windBlowingVFX;
    public AudioSource windBlowingSFX;
    float windDamageCount; // 5초에 10번 데미지

    // 하울링 관련 변수
    public bool isHowling = false;
    public float howlingTimeFlow = 0f;
    public GameObject howlingEyesFX;
    
    public Material skyboxDay;
    public Material skyboxNight;
    private Light[] lights = new Light[3];


    private Material skyboxMaterial;
    private float currentFogDensity;

    // 경험치 바 관련 변수
    public Transform expBar;
    public TextMeshProUGUI expBarTxt;

    // 늑대 레벨 UI 관련 변수
    public Transform wolfLevelTxt;

    // 스킬 키코드
    private KeyCode[] keyCodes = { KeyCode.LeftShift, KeyCode.Q, KeyCode.E, KeyCode.R };

    // 늑대 속도 감소 관련 변수 
    private bool isSlowed = false; // 느려진 상태인지 확인하는 플래그

    // 애니메이션 관련

    // 클로킹
    private bool dontMoveClocking = false;
    public void SetClocking(bool TF)
    {
        dontMoveClocking = TF;
    }
    public override void Awake()
    {   
        base.Awake();
        PC = GetComponent<WolfController>();
        WSL = GetComponent<WolfSkillList>();
        WSRPC = GetComponent<WolfSkillRPC>();
        WKIF = GetComponent<WolfKeyInfoUI>();
        character = GetComponent<PlayerCharacter>();
        WSLearning = GetComponent<WolfSkillLearning>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (PV.IsMine)
        {
        expBar = GameObject.Find("Canvas").transform.Find("MyCharacterStatusPanel/ExpBar");
        expBarTxt = GameObject.Find("Canvas").transform.Find("MyCharacterStatusPanel/ExpBar/Fill/ExpText").GetComponent<TextMeshProUGUI>();
        wolfLevelTxt = GameObject.Find("Canvas").transform.Find("MyCharacterPanel/WolfLevel");
        expBar.gameObject.SetActive(true);
        wolfLevelTxt.gameObject.SetActive(true);
        }

        lights[0] = GameObject.Find("Lighting/Directional Light").GetComponent<Light>();
        lights[1] = GameObject.Find("Lighting/Point Light").GetComponent<Light>();
        lights[2] = GameObject.Find("Lighting/Point Light (1)").GetComponent<Light>();

    }
    public override void Start()
    {
        base.Start();
        SetCircleSize(wolfCircleSize); //그라운드 체크 사이즈 정하기
        attackImages = GameObject.Find("Canvas/CharacterCoolTimePanel/Attack/Image").GetComponent<Image>();
        SetExpBar();
        SetWolfLevelTxt();
        skyboxMaterial = RenderSettings.skybox;

    }

    // Update is called once per frame
    public override void Update()
    {
        if (PGM.GetIsGameStart()) // 게임 시작 시 조작, 컨트롤 가능
        {
            base.Update();
            if (PV.IsMine)
            {
                WolfMouse();
                if (!dontMoveClocking)
                {
                    // 나중에 추가할 예정
                }
                CheckSkillInput();
                CheckSkillCooltime();
                CheckAttackCooltime();                
                if (!isWindBlowing && !base.GetStun())
                {
                    UseSkill();
                    UseAttack();
                }

                CheckWindBlowing();
                CheckHowling();
                if (!GetIsChatting())
                {
                    TestMethod();
                }
                WKIF.WolfOpenKeyInfoUI();
            }
        }
    }

    public override void SetDefaultStatus()
    {
        base.SetDefaultStatus();
        Debug.Log("Exp, Mp 설정");
        maxExp = character.GetDefaultMaxExp();
        exp = 0;
        maxMp = character.GetDefaultMaxMp();
        mp = character.GetDefaultMaxMp();
        
    }


    public int GetMaxExp()
    {
        return maxExp;
    }

    public int GetExp()
    {
        return exp;
    }

    public int GetMaxMp()
    {
        return maxMp;
    }
    public int GetMp()
    {
        return mp;
    }

    public void SetExp(int value)
    {
        exp = value;
        SetExpBar();
    }

    public void SetExpBar()
    {
        if (PV.IsMine)
        {
        float expRatio = GetExp() / (float)GetMaxExp();
        Debug.Log(expRatio);
        expBar.GetComponent<Slider>().value = expRatio;
        expBarTxt.text = $"{(int)(expRatio * 100)} %";
        }
    }

    public void SetMp(int value)
    {
       mp = value;
    }
    public void PlusExp(int value)
    {
        int nextExp = GetExp() + value;
        SetExp(nextExp);
        Debug.Log($"{value} 만큼의 경험치를 얻음. 총 경험치 {nextExp}");
        if (nextExp >= maxExp)
        {
            int remainExp = nextExp - maxExp;
            LevelUp();
            Debug.Log($"레벨업 이후 경험치 {remainExp}");
            SetExp(remainExp);
        }
    }

    public void LevelUp()
    {
        CharacterType curCharacter = character.GetCharacterType();
        switch (curCharacter)
        {
            case CharacterType.WOLF:
                //gameManager.SetMyCharacter("Wolf2");
                WSL.LearnWolfSkill(WolfSkillType.DASH, 0);
                PV.RPC("ChangeCharacter", RpcTarget.All, "Wolf2");
                WSLearning.GiveSkillOptions(3, 1);
                Debug.Log("늑대 레벨 2로 레벨업");
                break;
            case CharacterType.WOLF2:
                //gameManager.SetMyCharacter("Wolf3");
                PV.RPC("ChangeCharacter", RpcTarget.All, "Wolf3");
                WSLearning.GiveSkillOptions(3, 2);
                Debug.Log("늑대 레벨 3로 레벨업");
                break;
            case CharacterType.WOLF3:
                //gameManager.SetMyCharacter("Wolf4");
                PV.RPC("ChangeCharacter", RpcTarget.All, "Wolf4");
                WSLearning.GiveSkillOptions(3, 3);
                Debug.Log("늑대 레벨 4로 레벨업");
                break;
            case CharacterType.WOLF4:
                //gameManager.SetMyCharacter("Wolf4");
                PV.RPC("ChangeCharacter", RpcTarget.All, "Wolf4");
                Debug.LogWarning("늑대 레벨 4에서 레벨업 하는 오류");
                break;
        }
        SetWolfLevelTxt();
        StartCoroutine(LevelupCoroutine());
    }

    private void SetWolfLevelTxt()
    {
        if (PV.IsMine)
        {
            CharacterType curCharacter = character.GetCharacterType();
            switch (curCharacter)
            {
                case CharacterType.WOLF:
                    wolfLevelTxt.GetComponent<TextMeshProUGUI>().text = "Level 1";
                    break;
                case CharacterType.WOLF2:
                    wolfLevelTxt.GetComponent<TextMeshProUGUI>().text = "Level 2";
                    break;
                case CharacterType.WOLF3:
                    wolfLevelTxt.GetComponent<TextMeshProUGUI>().text = "Level 3";
                    break;
                case CharacterType.WOLF4:
                    wolfLevelTxt.GetComponent<TextMeshProUGUI>().text = "Level 4";
                    break;
            }
        }
    }

    IEnumerator LevelupCoroutine()
    {
        LevelupFX.SetActive(true);
        yield return new WaitForSeconds(1);
        LevelupFX.SetActive(false);
    }

    private void CheckAttackCooltime()
    {
        if (!isActiveAttacking)
        {
            attackingTimeFlow -= Time.deltaTime;
            // 스킬 이미지 쿨타임 있는 만틈 채워서 표시
            attackImages.fillAmount = (attackSpeed - attackingTimeFlow) / (float) attackSpeed;
            if (attackSpeed - attackingTimeFlow <= 0.3f)
            {
                PV.RPC("UseWolfAttackAnimationRPC", RpcTarget.All, false);
                //anim.SetBool("isAttack", false);
            }


            if (attackingTimeFlow <= 0f)
            {
                isActiveAttacking = true;
            }
        }
    }

    private void UseAttack()
    {
        if (isActiveAttacking && Input.GetMouseButton(0))
        {
            attackingTimeFlow = attackSpeed;
            isActiveAttacking = false;
            PV.RPC("UseWolfAttackAnimationRPC", RpcTarget.All, true);
            //anim.SetBool("isAttack", true);
            WSRPC.Attack();
        }
    }

    [PunRPC]
    public void UseWolfAttackAnimationRPC(bool TF)
    {
        anim.SetBool("isAttack", TF);
    }


    private void CheckSkillInput() {
        if (!GetIsChatting())
        { // 채팅이 안켜져있으면
            for (int i = 0; i < isLearnedSkill.Length; i++)
            {
                isInputSkill[i] = Input.GetKeyDown(keyCodes[i]);
            }
        }
        else
        { // input 초기화가 없어서 버그 생길 수 있음.
            for (int i = 0; i < isLearnedSkill.Length; i++)
            {
                isInputSkill[i] = false;
            }
        }

    }

    private void CheckSkillCooltime()
    {
        for (int i = 0; i < isLearnedSkill.Length; i++)
        {
            if (isLearnedSkill[i] && !isActiveSkill[i])
            {
                skillCooltimes[i] -= Time.deltaTime;
                // 스킬 이미지 쿨타임 있는 만틈 채워서 표시
                WSL.skillImages[i].fillAmount = 1 - skillCooltimes[i] / maxSkillCooltimes[i];

                if (skillCooltimes[i] < 0)
                {
                    isActiveSkill[i]= true;
                }
            }
        }
    }


    private void UseSkill() {
        for (int i = 0; i < isLearnedSkill.Length; i++)
        {
            if (isInputSkill[i] && isActiveSkill[i])
            {
                WSL.UseWolfSkill(WSL.Skills[i]);
                isActiveSkill[i]= false;
                skillCooltimes[i] = maxSkillCooltimes[i];
            }
        }
    }


    private void CheckWindBlowing()
    {
        if (isWindBlowing)
        {
            PV.RPC("WindBlowingRPC", RpcTarget.All, true);
            windBlowingTimeFlow -= Time.deltaTime;
            if ((windBlowingTimeFlow/0.5f)< windDamageCount)
            {
                windDamageCount -= 0.5f;
                WSRPC.WindBlowingAttack();
            }
            if (windBlowingTimeFlow <= 0f)
            {
                isWindBlowing = false;
                PC.anim.SetBool("isWindBlowing", false);
                PV.RPC("WindBlowingRPC", RpcTarget.All, false);
            }
        }
    }
    
    public void WindBlowing() {
        isWindBlowing = true;
        PC.anim.SetBool("isWindBlowing", true);
        windBlowingTimeFlow = 5f;
        windDamageCount = 10f;
    }


    [PunRPC]
    public void WindBlowingRPC(bool isWindBlowing)
    {
        windBlowingVFX.SetActive(isWindBlowing);
        if (isWindBlowing && !windBlowingSFX.isPlaying)
        {
            windBlowingSFX.Play();
        } else if (!isWindBlowing)
        {
            windBlowingSFX.Stop();
        }
    }

    // 하울링시 화면 밝기 조절 & 스카이박스 변경
    [PunRPC]
    public void HowlingRPC(float timeFlow)
    {
        float fogDensityDay = 0.0015f;
        float fogDensityNight = 0.005f;
        Color fogColorDay = Color.gray;
        Color fogColorNight = Color.black;
        float lightIntensityDay = 1.6f;
        float lightIntensityNight = 0f;
        float exposureDay = 0.5f;
        float exposureNight = 0.1f;


        // 스킬 시작 0~1초는 어두워지고, 끝나기 1초전~끝은 밝아지기.
        float a = Math.Min(timeFlow, WSRPC.HowlingDurationTime - timeFlow);
        float x = Math.Min(a, 1);

        RenderSettings.fogDensity = (fogDensityNight - fogDensityDay) * x + fogDensityDay;
        RenderSettings.fogColor = Color.Lerp(fogColorDay, fogColorNight, x);
        SetExposure((exposureNight - exposureDay) * x + exposureDay);
        foreach (Light light in lights)
        {
            light.intensity = (lightIntensityNight - lightIntensityDay) * x + lightIntensityDay;
        }

        if (timeFlow > 1f && timeFlow < WSRPC.HowlingDurationTime - 1f)
        {
            RenderSettings.skybox = skyboxNight;
        }
        else
        {
            RenderSettings.skybox = skyboxDay;
        }       
        if (timeFlow > WSRPC.HowlingDurationTime - 0.5f)
        {
            howlingEyesFX.SetActive(true);
        }
        else if (timeFlow < 0.5f)
        {
            howlingEyesFX.SetActive(false);
        }
    }

    private void CheckHowling()
    {
        if (isHowling)
        {
            howlingTimeFlow -= Time.deltaTime;
            PV.RPC("HowlingRPC", RpcTarget.All, howlingTimeFlow);
            if (howlingTimeFlow <= 0f)
            {
                isHowling = false;
                // particle system stop
            }
        }
    }


    private void SetExposure(float value)
    {
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetFloat("_Exposure", value);
        }
    }

    private void TestMethod() {
        if (Input.GetKeyDown(KeyCode.M))
        { 
            WSL.LearnWolfSkill(WolfSkillType.DASH, 0);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            WSL.LearnWolfSkill(WolfSkillType.WIND_BLOWING, 1);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            WSL.LearnWolfSkill(WolfSkillType.SUPER_JUMP, 2);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            WSL.LearnWolfSkill(WolfSkillType.HOWLING, 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            WSL.LearnWolfSkill(WolfSkillType.WOLFCLAW, 2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            WSL.LearnWolfSkill(WolfSkillType.CLOCKING, 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            WSLearning.GiveSkillOptions(3, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            WSL.LearnWolfSkill(WolfSkillType.WOLFSHOCK, 1);
        }
        if (Input.GetKeyDown(KeyCode.L))
            PlusExp(20);
    }


    // 오정원작성//
    // 늑대가 아이템에 당했을때 상태를 변화시키려고 작성

    //늑대 공격 감지 RPC 함수 
    [PunRPC]
    public void ReceiveAttack()
    {
        Debug.Log("늑대가 돼지의 일반공격을 받았습니다.");
    }

    //늑대 속도 감소 RPC 함수
    [PunRPC]
    public void ReduceWolfSpeed()
    {
        // 이미 느려진 상태가 아닌 경우에만 속도 감소
        if (!isSlowed)
        {
            float curSpeed = GetSpeed();
            ChangeSpeed(5f, 3f); // 3으로 속도 변경
        }
    }

    public void WolfMouse()
    {
        if (GetIsChatting() || GetIsActiveMiniMap() || WSLearning.GetIsActiveSkillSelect() || WKIF.GetIsWolfOpenKeyInfoUI())
        {
            MouseUnlock();
        }
        else
        {
            MouseLock();
        }
    }


}
