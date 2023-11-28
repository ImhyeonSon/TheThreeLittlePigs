using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChestInventoryUI : MonoBehaviour
{
    private CheckItemIdx CII;
    //public 
    public GameObject inventoryUI; // 유저 인벤토리 UI
    public GameObject chestInventoryUI; // 상자 인벤토리 UI
    public bool activeChestInventory = false; // 인벤토리 상태

    [System.Serializable]
    public struct SlotUI
    {
        public GameObject slotObject;
        public Image itemIconImage;
        public TextMeshProUGUI itemCount;
    }
    public SlotUI[] slots = new SlotUI[12]; // 슬롯 구조체 배열로 정의
    public List<InventoryData> ChestInventoryDatas { get; private set; } // 상자 인벤토리 데이터

    public ChestGetItem currentChestGetItem;

    private void Awake()
    {
        CII = GetComponent<CheckItemIdx>();
        ChestInventoryDatas = new List<InventoryData>(); // 리스트 초기화
    }
    public void SetCurrentChest(ChestGetItem chestGetItem) // 현재 상자 인벤토리 설정
    {
        currentChestGetItem = chestGetItem;
    }
    public void ToggleInventoryUI(bool isOpening)
    {
        if (isOpening)
        {
            currentChestGetItem.ExtractChestItemData(); // 상자에 저장된 아이템의 이름, 수량, 이미지 경로를 추출하여 UI에 표시할 준비
        }

        // CGI.Extract -> ConvertToInventoryData -> UpdateUI

        // 과정을 거치는 이유는 RPC로 인벤토리 주고 받는 걸 봐야 하는데, 기존 인벤토리 형식처럼 보내지지 않고, json 형식으로 보내야만 한다. 이를 위해서 데이터 추출 후 바꾸는 것.

        activeChestInventory = !activeChestInventory; // 인벤토리 상태 토글
        inventoryUI.SetActive(activeChestInventory); // UI 활성화 / 비활성화
        chestInventoryUI.SetActive(activeChestInventory); // 상자 활성화 / 비활성화
        UpdateChestInventoryUI();
    }

    // 상자 인벤토리의 데이터를 UI에 표시하기 위한 형식으로 변환
    public void ConvertToInventoryData(string[] itemNames, int[] itemCounts, string[] itemImages)
    {
        ChestInventoryDatas.Clear();
        for (int i = 0; i < itemNames.Length; i++)
        {
            // 새로운 InventoryData 객체를 생성하여 리스트에 추가합니다.
            InventoryData data = new InventoryData
            {
                ItemName = itemNames[i],
                ItemCount = itemCounts[i],
                ItemImage = itemImages[i] // 이미지 경로나 이름을 직접 저장
            };
            ChestInventoryDatas.Add(data);
        }
    }

    public void UpdateChestInventoryUI()
    {
        if (currentChestGetItem == null)
            return;
        var chestInventoryDatas = currentChestGetItem.chestInventoryDatas;
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < chestInventoryDatas.Count)
            {
                InventoryData data = chestInventoryDatas[i];
                slots[i].slotObject.SetActive(true);
                slots[i].itemIconImage.sprite = Resources.Load<Sprite>(data.ItemImage); // data.ItemImage는 경로
                slots[i].itemCount.text = data.ItemCount.ToString();
            }
        }
        CII.CheckIndex("Chest", chestInventoryDatas);
        CII.CheckEmptySlot("Chest");
        
        CII.PrintIndicesInfo("Chest"); // UI 업데이트 후 인덱스 뽑아보자. 토글 될 때마다 열린다.
    }

    public bool GetIsActiveChest()
    {
        return chestInventoryUI.activeSelf;
    }
}
