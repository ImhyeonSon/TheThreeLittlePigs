using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour
{
    private float speed;
    private float jumpSpeed;
    private float gravity;
    private float rotateTime;
    private int maxHp;
    private PlayerStatus PS;
    private int maxExp;// 경험치
    private int maxMp; // 기력 
    private int damage; // 공격력
    private GameManager gameManager;


    private CharacterType myCharacter;
    public enum CharacterType
    { // 처음 생성시 DEFAULT로 되도록 설정
        PIG,
        WOLF,
        WOLF2,
        WOLF3,
        WOLF4
    };
    private void Awake()
    {
        PS = GetComponent<PlayerStatus>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Start()
    {
        // 동기화 할때 호출 주기가 꼬이므로 여기서 실행X => PlayerStatus에서 호출한다.
        //UpdateMyCharacter();
        //SetCharacter();
    }

    public float GetDefaultSpeed()
    {
        return speed;
    }

    public float GetDefaultGravity()
    {
        return gravity;
    }

    public float GetDefaultJumpSpeed()
    {
        return jumpSpeed;
    }

    public float GetDefaultRotateTime()
    {
        return rotateTime;
    }
    public int GetDefaultHp()
    {
        return maxHp;
    }
    public int GetDefaultMaxExp()
    {
        return maxExp;
    }
    public int GetDefaultMaxMp()
    {
        return maxMp;
    }
    public int GetDefaultDamage()
    {
        return damage;
    }

    public CharacterType GetCharacterType()
    {
        return myCharacter;
    }

    public void TestSet(string type)
    {
        Debug.Log("입력 on");
        if (type == "pig")
        {
            myCharacter = CharacterType.PIG;
            SetCharacter();
        }
        else if (type == "wolf")
        {
            myCharacter = CharacterType.WOLF;
            SetCharacter();
        }
    }

    
    public void UpdateMyCharacter() // 초기에만 사용할 함수
    {
        string character = gameManager.GetMyCharacter();
        if (character == "Wolf")
        {
            myCharacter = CharacterType.WOLF;
        }
        else if (character == "Wolf2")
        {
            myCharacter = CharacterType.WOLF2;
        }
        else if (character == "Wolf3")
        {
            myCharacter = CharacterType.WOLF3;
        }
        else if (character == "Wolf4")
        {
            myCharacter = CharacterType.WOLF4;
        }
        else if (character == "Pig")
        {
            myCharacter = CharacterType.PIG;
        }
    }

    [PunRPC]
    public void ChangeCharacter(string character)
    {
        SetCharacterType(character);
        SetCharacter();
    }

    public void SetCharacter()
    {
        switch (myCharacter)
        {
            case CharacterType.PIG:  // if (조건)
                Debug.Log("돼지");
                SetMyStatus(9.5f, 12f, 40f, 15f, 15, 1000000, 1000000, 1); //여기에 기본 status를 정의
                break;
            case CharacterType.WOLF:  // if (조건)
                Debug.Log("늑대1");
                SetMyStatus(8f, 15f, 40f, 15f, 10000, 60, 200, 2);
                break;
            case CharacterType.WOLF2:  // if (조건)
                Debug.Log("늑대2");
                SetMyStatus(10f, 20f, 40f, 15f, 10000, 180, 300, 4);
                break;
            case CharacterType.WOLF3:  // if (조건)
                Debug.Log("늑대3");
                SetMyStatus(11f, 20f, 40f, 20f, 10000, 360, 500, 6);
                break;
            case CharacterType.WOLF4:  // if (조건)
                Debug.Log("늑대4");
                SetMyStatus(12f, 25f, 40f, 20f, 10000, 1000000, 800, 8);
                break;
        }
    }

    public void SetCharacterType(string character)
    {
        Debug.Log("캐릭터 세팅 "+character);
        switch (character)
        {
            case "Pig":  // if (조건)
                myCharacter = CharacterType.PIG;
                break;
            case "Wolf":  // if (조건)
                myCharacter = CharacterType.WOLF;
                break;
            case "Wolf2":  // if (조건)
                myCharacter = CharacterType.WOLF2;
                break;
            case "Wolf3":  // if (조건)
                myCharacter = CharacterType.WOLF3;
                break;
            case "Wolf4":  // if (조건)
                myCharacter = CharacterType.WOLF4;
                break;
        }
    } 


    // Setting
    private void SetMyStatus(float speedValue, float jumpSpeedValue, float gravityValue, float rotateTimeValue, int hpValue, int maxExpValue, int maxMpValue, int damageValue)
    {
        speed = speedValue;
        jumpSpeed = jumpSpeedValue;
        gravity = gravityValue;
        rotateTime = rotateTimeValue;
        maxHp = hpValue;
        // 기본 값이 바뀌었으므로 Status를 반영해준다.
        maxExp = maxExpValue;
        maxMp = maxMpValue;
        damage = damageValue;
        PS.SetDefaultStatus();
    }


    /// 추가해야할 코드



    /// 



}
