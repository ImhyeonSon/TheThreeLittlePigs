using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static WolfSkillList;

public class WolfSkillLearning : MonoBehaviour
{
    private WolfStatus WS;
    private WolfSkillRPC WSRPC;
    private WolfSkillList WSL;
    public GameObject[] SkillChoices = new GameObject[3];
    public GameObject SkillSelectUI;
    public GameObject SkillSelectUIPanel;

    public bool activeSkillSelect;

    public GameObject Skill1;
    public GameObject Skill2;
    public GameObject Skill3;

    public TextMeshProUGUI SkillText1;
    public TextMeshProUGUI SkillText2;
    public TextMeshProUGUI SkillText3;

    string skillName;
    int mySkillIndex=0;
    WolfSkillType[] resultList;
    // Start is called before the first frame update
    void Awake()
    {
        WS = GetComponent<WolfStatus>();
        WSRPC = GetComponent<WolfSkillRPC>();
        WSL = GetComponent<WolfSkillList>();
    }

    // Update is called once per frame
    public void GiveSkillOptions(int num, int skillIndex)
    {
        mySkillIndex = skillIndex;
        resultList = new WolfSkillType[num];

        // 아직 배우지 않은 스킬들을 LearnableSkills에 넣기
        SortedSet<WolfSkillType> AllSkills = new SortedSet<WolfSkillType>();
        SortedSet<WolfSkillType> LearnedSkills = new SortedSet<WolfSkillType>();
        AllSkills.Add(WolfSkillType.WIND_BLOWING);
        AllSkills.Add(WolfSkillType.SUPER_JUMP);
        AllSkills.Add(WolfSkillType.WOLFCLAW);
        AllSkills.Add(WolfSkillType.WOLFSHOCK);
        if (skillIndex > 2)
        {
            AllSkills.Add(WolfSkillType.HOWLING);
            AllSkills.Add(WolfSkillType.CLOCKING);
        }
        foreach (WolfSkillType skill in WSL.Skills)
        {
            LearnedSkills.Add(skill);
        }
        //SortedSet<WolfSkillType> LearnableSkills = (SortedSet<WolfSkillType>)AllSkills.Except(LearnedSkills);
        IEnumerable<WolfSkillType> ILearnableSkills = AllSkills.Except(LearnedSkills);
        // IEnumerable을 SortedSet으로 변환
        SortedSet<WolfSkillType> LearnableSkills = new SortedSet<WolfSkillType>(ILearnableSkills);

        //// LearnableSkills가 잘 나오는지 로그 찍어보기
        //foreach (WolfSkillType skill in LearnableSkills)
        //{
        //    Debug.Log(skill);
        //}

        for (int i = 0; i < num; i++)
        {
            int count = LearnableSkills.Count();
            int randNum = Random.Range(0, count);
            WolfSkillType resultSkill = LearnableSkills.ElementAt(randNum);
            resultList[i] = resultSkill;
            LearnableSkills.Remove(resultSkill);
        }
        SkillSelectUIPanel.SetActive(true);
        SkillSelectUI.SetActive(true);
        activeSkillSelect = true;
        Distribute();
        Debug.Log("아래부터 결과");
        foreach (WolfSkillType skill in resultList)
        {
            Debug.Log(skill);
        }
    }

    public void Distribute()
    {
        for (int idx = 0; idx < resultList.Length; idx++)
        {
            if (idx == 0)
            {
                Description(Skill1, idx);
            }
            else if (idx == 1)
            {
                Description(Skill2, idx);
            }
            else
            {
                Description(Skill3, idx);            
            }
            //GameObject SkillInterFace = SkillSelectUI.transform.GetChild(idx).gameObject;
            //Description(SkillInterFace, idx);
        }
    }

    public void Description(GameObject SkillInterFace, int idx)
    {
        //SkiameObject.Find("Canvas").transform.Find("MyCharacterStatusPanel/ExpBar");
        if (SkillInterFace == null)
        {
            Debug.Log("얘가 비었음");
        }
        else
        {
            Debug.Log("잘 나오는 중");
        }
        SkillInterFace.transform.Find("SkillImg/Image").GetComponent<Image>().sprite = Resources.Load("Skills/Wolf/" + resultList[idx], typeof(Sprite)) as Sprite; // 이미지
        SkillInterFace.transform.Find("SkillName/Text").GetComponent<TextMeshProUGUI>().text = WolfSkillKoreanName(resultList[idx]); // 스킬 이름
        SkillInterFace.transform.Find("SkillName/TextBa").GetComponent<TextMeshProUGUI>().text = resultList[idx].ToString(); // 스킬 타입 저장
        SkillInterFace.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = WolfSkillDescription(resultList[idx]); // 설명은 나중에 추가
    }

    public void OnClickGetSkillButton1()
    {
        skillName = SkillText1.text;
        CloseLearnUI();
        Debug.Log(skillName);
        ChangeSkillToEnum(skillName);
    }
    public void OnClickGetSkillButton2()
    {
        skillName = SkillText2.text;
        CloseLearnUI();
        Debug.Log(skillName);
        ChangeSkillToEnum(skillName);
    }
    public void OnClickGetSkillButton3()
    {
        skillName = SkillText3.text;
        CloseLearnUI();
        Debug.Log(skillName);
        ChangeSkillToEnum(skillName);
    }


    public void ChangeSkillToEnum(string mySkillName)
    {
        foreach(WolfSkillType WST in resultList)
        {
            if (WST.ToString() == mySkillName)
            {
                SelectOneSkill(WST, mySkillIndex);
                return;
            }
        }

    } 


    public void CloseLearnUI()
    {
        SkillSelectUIPanel.SetActive(false);
        SkillSelectUI.SetActive(false);
        activeSkillSelect = false;
    }


    // 스킬 배우는 함수
    public void SelectOneSkill(WolfSkillType selectedSkill, int skillIndex)
    {
        WSL.LearnWolfSkill(selectedSkill, skillIndex);
    }



    public bool GetIsActiveSkillSelect()
    {
        return activeSkillSelect;
    }

    // 스킬에 대한 상세 설명을 할 부분
    public string WolfSkillKoreanName(WolfSkillType skillType)
    {
        string returnString = "";
        switch (skillType)
        {
            case WolfSkillType.DASH:
                returnString = "대쉬";
                break;
            case WolfSkillType.WIND_BLOWING:
                returnString = "바람 불기";
                break;
            case WolfSkillType.SUPER_JUMP:
                returnString = "슈퍼 점프";
                break;
            case WolfSkillType.HOWLING:
                returnString = "하울링";
                break;
            case WolfSkillType.WOLFCLAW:
                returnString = "늑대의 상처";
                break;
            case WolfSkillType.CLOCKING:
                returnString = "은신";
                break;
            case WolfSkillType.WOLFSHOCK:
                returnString = "대지의 울림";
                break;
            default:
                Debug.Log("할당x");
                break;
        }
        return returnString;
    }

    // 스킬에 대한 상세 설명을 할 부분
    public string WolfSkillDescription(WolfSkillType skillType)
    {
        string returnString="";
        switch (skillType)
        {
            case WolfSkillType.DASH:
                returnString= "몇 초간 빠르게 움직입니다.";
                break;
            case WolfSkillType.WIND_BLOWING:
                returnString = "몇 초간 움직일 수 없는 대신 강한 바람을 불어 피해를 줍니다. 돼지의 집에 특히 더 강한 피해를 줍니다. 돼지의 우산으로 막힐 수 있습니다.";
                break;
            case WolfSkillType.SUPER_JUMP:
                returnString = "몇 초간 더 높이 뛸 수 있습니다.";
                break;
            case WolfSkillType.HOWLING:
                returnString = "밤이 되고 늑대의 속도와 공격속도가 빨라집니다.";
                break;
            case WolfSkillType.WOLFCLAW:
                returnString = "3개의 발톱으로 전방의 넓은 범위를 공격합니다. 공격에 맞은 돼지는 데미지를 입고 슬로우 디버프에 걸립니다.";
                break;
            case WolfSkillType.CLOCKING:
                returnString = "일정시간 동안 돼지의 눈에 보이지 않고 빨라집니다. 스킬을 사용하거나 공격하면 해제됩니다. ";
                break;
            case WolfSkillType.WOLFSHOCK:
                returnString = "늑대가 주먹을 빠르게 내리쳐 충격파를 발생시켜 돼지를 스턴 상태로 만듭니다.";
                break;
            default:
                Debug.Log("할당x");
                break;
        }
        return returnString;
    }


}
