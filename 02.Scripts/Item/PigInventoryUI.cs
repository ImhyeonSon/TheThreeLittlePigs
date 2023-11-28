using UnityEngine;
using UnityEngine.UI;
using TMPro; // 추가

public class PigInventoryUI : MonoBehaviour
{
    private PigGetItem PGI;
    private CheckItemIdx CII;
    public GameObject inventoryUI;

    private ChestInventoryUI CIUI;
    private ManagerInventory MI;

    [System.Serializable] // Unity 인스펙터에서 편집할 수 있도록 Serializable 속성을 추가합니다.
    public struct SlotUI
    {
        public GameObject slotObject;
        public Image itemIconImage;
        public TextMeshProUGUI itemCount; 
    }

    public SlotUI[] slots = new SlotUI[12]; // 슬롯 구조체 배열로 정의합니다.


    bool activeInventory = false;
    
    void Start()
    {
        PGI = GetComponent<PigGetItem>();
        CIUI = GetComponent<ChestInventoryUI>();
        MI = GetComponent<ManagerInventory>();
        CII = GetComponent<CheckItemIdx>();
        inventoryUI.SetActive(activeInventory);
    }

    public void OpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            activeInventory = !inventoryUI.activeSelf; // 코드 변경
            
            inventoryUI.SetActive(activeInventory);
            if (activeInventory)
            {
                UpdateInventoryUI();
            } else
            {
                CIUI.chestInventoryUI.SetActive(false);
                CIUI.activeChestInventory = false;
            }
        }
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < PGI.InventoryDatas.Count)
            {
                InventoryData data = PGI.InventoryDatas[i];

                // 아이템 수량 텍스트 설정
                slots[i].itemCount.text = data.ItemCount.ToString();

                if (data.ItemCount > 0) // 슬롯에 아이템이 있는 경우
                {
                    // Resources 폴더에서 경로를 사용하여 스프라이트를 로드
                    Sprite spriteToDisplay = Resources.Load<Sprite>(data.ItemImage);
                    slots[i].itemIconImage.sprite = spriteToDisplay;

                    //if (data.ItemImage == "Item/Images/Mud")
                    //{
                    //    slots[i].itemIconImage.color = new Color(113 / 255, 93 / 255, 58 / 255, 255 / 255);
                    //}
                    //else if (data.ItemImage == "Item/Images/Rice")
                    //{
                    //    slots[i].itemIconImage.color = new Color(255 / 255, 171 / 255, 0 / 255, 255 / 255);
                    //}
                    //else if (data.ItemImage == "Item/Images/Brick")
                    //{
                    //    slots[i].itemIconImage.color = new Color(123 / 255, 21 / 255, 21 / 255, 255 / 255);
                    //}
                    //CII.UpdateSpecificIndex("Pig", data.ItemName, i);
                }
                else // 슬롯이 비어있는 경우
                {
                    // 아이템이 없으므로 스프라이트를 지웁니다.
                    slots[i].itemIconImage.sprite = Resources.Load<Sprite>("Item/Images/NoItem");
                }

                // 슬롯은 항상 활성화되어 있다고 가정
                slots[i].slotObject.SetActive(true);
                // 필요한 경우 아이템의 특정 인덱스를 업데이트
            }
            else
            {
                // 데이터가 없는 슬롯을 비활성화
                slots[i].slotObject.SetActive(false);
            }
        }

        // UI 업데이트 후 나중에 빠른 접근을 위해 마지막 인덱스를 업데이트
        //CII.UpdateLastItemIndices("Pig", PGI.lastPigRiceIdx, PGI.lastPigWoodIdx, PGI.lastPigStoneIdx, PGI.lastPigMudIdx, PGI.lastPigSyringeIdx);
        CII.PrintIndicesInfo("Pig");
    }

    public bool GetIsActiveInventory()
    { // 직접 접근으로 변경
        return inventoryUI.activeSelf;
    }

}
