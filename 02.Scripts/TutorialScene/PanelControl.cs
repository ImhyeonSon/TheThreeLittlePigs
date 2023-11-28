using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LayerLabTheStone
{
    public class PanelControl : MonoBehaviour
    {
        private int page = 0;
        private bool isReady = false;
        [SerializeField] private List<GameObject> panels = new List<GameObject>();
        public TextMeshProUGUI textTitle;
        public String category;
        [SerializeField] private Transform panelTransform;
        [SerializeField] private Button buttonPrev;
        [SerializeField] private Button buttonNext;
        

        private void Awake()
        {
            buttonPrev.onClick.AddListener(Click_Prev);
            buttonNext.onClick.AddListener(Click_Next);
            textTitle.text = GetDescription(page);

            foreach (Transform t in panelTransform)
            {
                panels.Add(t.gameObject);
                t.gameObject.SetActive(false);
            }
        }

        private void Start()
        {

            panels[page].SetActive(true);
            isReady = true;

            CheckControl();
        }

        void Update()
        {
            //if (gameObject.GetComponentInParent<Transform>.gameObjet.activeSelf)
            //{
            //    page = 0;
            //    textTitle.text = GetDescription(0);
            //}

            if (panels.Count <= 0 || !isReady) return;

            if (Input.GetKeyDown(KeyCode.LeftArrow))
                Click_Prev();
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                Click_Next();
        }

        private void OnEnable()
        {
            page = 0;
            panels[page].SetActive(true);
            textTitle.text = GetDescription(page);

        }

        private void OnDisable()
        {
            foreach (GameObject panel in panels)
            {
                panel.SetActive(false);
            } 
        }

        //Click_Prev
        public void Click_Prev()
        {
            Debug.Log("이전 페이지로 이동");
            if (page <= 0 || !isReady) return;

            panels[page].SetActive(false);
            panels[page -= 1].SetActive(true);
            textTitle.text = GetDescription(page);
            
            CheckControl();
        }

        private string GetDescription(int page)
        {
            if (category == "Common")
            {
                switch (page)
                {
                    case 0:
                        return "1. 로비 화면";
                    case 1:
                        return "2. 게임 방";
                }
            }
            else if (category == "Game")
            {
                switch (page)
                {
                    case 0:
                        return "1. 기본 조작법";
                    case 1:
                        return "2. 채팅하기";
                    case 2:
                        return "3. 미니맵";
                    case 3:
                        return "3. 돼지 승리 조건";
                    case 4:
                        return "4. 늑대 승리 조건";
                }
            }
            else if (category == "Pig")
            {
                switch (page)
                {
                    case 0:
                        return "1. 가보";
                    case 1:
                        return "2. 파밍 오브젝트 채집";
                    case 2:
                        return "3. 인벤토리 사용";
                    case 3:
                        return "4. 집 건설";
                    case 4:
                        return "5. 상자/제작대 사용";
                    case 5:
                        return "6. 도구/아이템 설명";
                    case 6:
                        return "7. 기절 상태";
                }
            }
            else if (category == "Wolf")
            {
                switch (page)
                {
                    case 0:
                        return "1. 야생동물 공격";
                    case 1:
                        return "2. 레벨 업 및 늑대 스킬";
                    case 2:
                        return "3. 돼지/집 공격";
                }
            }
            return "내용없음";
        }

        //Click_Next
        public void Click_Next()
        {
            Debug.Log("다음 페이지로 이동");
            if (page >= panels.Count - 1) return;

            panels[page].SetActive(false);
            panels[page += 1].SetActive(true);
            textTitle.text = GetDescription(page);
            CheckControl();
        }

        void SetArrowActive()
        {
            buttonPrev.gameObject.SetActive(page > 0);
            buttonNext.gameObject.SetActive(page < panels.Count - 1);
        }

        //SetTitle, SetArrow Active
        private void CheckControl()
        {
            //textTitle.text = panels[page].name.Replace("_", " ");
            SetArrowActive();
        }
    }
}
