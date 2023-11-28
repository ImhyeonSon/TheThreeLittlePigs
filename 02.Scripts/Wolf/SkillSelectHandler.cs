using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillSelectHandler : MonoBehaviour
{
    public int index;
    private WolfSkillLearning WSLearning;


    void Awake()
    {
        WSLearning = GetComponent<WolfSkillLearning>();
    }

    // Update is called once per frame

    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭시 실행
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log($"{index}번째 스킬 선택");
            WSLearning.activeSkillSelect = false;
        }
    }


}
