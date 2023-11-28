// ChestGetItem.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChestGetItem : MonoBehaviourPun
{
    //private ChestInventoryUI chestInventoryUI;
    private Dictionary<string, string> itemImagePath;
    private Dictionary<Sprite, string> spritePaths;
    public List<InventoryData> chestInventoryDatas;
    public CheckItemIdx CII;
    public ChestInventoryUI CIUI;

    private float interactionDistance = 2.0f;
    int pigLayer = 1 << 16; // 플레이어 레이어

    private Collider[] playerInRange; // 범위 내 플레이어

    public int lastChestRiceIdx = -1;
    public int lastChestWoodIdx = -1;
    public int lastChestStoneIdx = -1;
    public int lastChestMudIdx = -1;
    public int lastChestSyringeIdx = -1;
    public int lastChestLumberIdx = -1; // 판자
    public int lastChestBrickIdx = -1; // 벽돌
    public int lastChestSlingShotIdx = -1;
    public int lastChestUmbrellaIdx = -1;
    public int lastChestTigerIdx = -1;

    public int lastChestItemIdx = -1;
    public int firstChestEmptyIdx = 0;

    private void Awake()
    {
        //CIUI = GetComponent<ChestInventoryUI>();

        // 처음 시작했을 때 비어있는 인벤토리 12칸을 만든다
        chestInventoryDatas = new List<InventoryData>();
        for (int i = 0; i < 12; i++)
        {
            InventoryData inventoryData = new InventoryData
            {
                ItemName = null,
                ItemCount = 0,
                ItemImage = "Item/Images/NoItem",
            };
            chestInventoryDatas.Add(inventoryData);
        }
        InitializeItemSprites();
    }

    private void Start()
    {
        //CII = GetComponent<CheckItemIdx>();
    }

    private void InitializeItemSprites()
    {
        itemImagePath = new Dictionary<string, string> {
            { "Rice(Clone)", "Item/Images/Rice"},
            { "Tree(Clone)", "Item/Images/Wood"},
            { "Stone(Clone)", "Item/Images/Stone"},
            { "Mud(Clone)", "Item/Images/Mud" },
            { "Syringe", "Item/Images/Syringe" },
            { "Lumber", "Item/Images/Lumber" },
            { "Brick", "Item/Images/Brick" },
            { "SlingShot", "Item/Images/SlingShot" },
            { "Umbrella", "Item/Images/Umbrella" },
            { "Tiger", "Item/Images/Tiger" }
        };
        //spritePaths = new Dictionary<Sprite, string>();

        //LoadAndStoreSprite(PigItem.ItemType.Rice, "Item/Images/Rice");
        //LoadAndStoreSprite(PigItem.ItemType.Wood, "Item/Images/Wood");
        //LoadAndStoreSprite(PigItem.ItemType.Stone, "Item/Images/Stone");
        //LoadAndStoreSprite(PigItem.ItemType.Mud, "Item/Images/Mud");
    }

    //private void LoadAndStoreSprite(PigItem.ItemType itemType, string path)
    //{
    //    Sprite sprite = Resources.Load<Sprite>(path);
    //    if (sprite != null)
    //    {
    //        spritePaths[sprite] = path;
    //    }
    //    else
    //    {
    //        Debug.LogError("Failed to load sprite at path: " + path);
    //    }
    //}

    //private string GetImagePath(Sprite sprite)
    //{
    //    if (sprite != null && spritePaths.TryGetValue(sprite, out string path))
    //    {
    //        return path;
    //    }
    //    return null;
    //}

    // 현재 상자(ChestGetItem 인스턴스가 관리하는 상자)에 있는 아이템들의 데이터를 추출하는 함수
    public void ExtractChestItemData()
    {
        bool isExist = false;

        //Vector3 chestPosition = transform.position;
        //playerInRange = Physics.OverlapSphere(chestPosition, interactionDistance, pigLayer);
        //CII = playerInRange[0].GetComponent<CheckItemIdx>();

        string[] itemNames = new string[chestInventoryDatas.Count];
        int[] itemCounts = new int[chestInventoryDatas.Count];
        string[] itemImagePaths = new string[chestInventoryDatas.Count];
        for (int i = 0; i < chestInventoryDatas.Count; i++)
        {
            if (chestInventoryDatas[i].ItemName != null && chestInventoryDatas[i].ItemCount != 0 && chestInventoryDatas[i].ItemImage != null)
            {
                // 아이템 이름을 기반으로 이미지 경로 가져오기 ('itemImagePath' 딕셔너리 사용)
                string itemName = chestInventoryDatas[i].ItemName;
                string itemImgPath;
                if (itemImagePath.TryGetValue(itemName, out itemImgPath))
                {
                    itemImagePaths[i] = itemImgPath;
                }
                itemNames[i] = itemName;
                itemCounts[i] = chestInventoryDatas[i].ItemCount;
                isExist = true;
            }
        }
        if (isExist) // 데이터가 존재한다면,
        {
            CII.CheckIndex("Chest", chestInventoryDatas);
            Debug.Log(CIUI + "CIUI EXIST");
            Debug.Log(itemNames[0]);
            CIUI.ConvertToInventoryData(itemNames, itemCounts, itemImagePaths);
            //playerInRange[0].GetComponent<ChestInventoryUI>().ConvertToInventoryData(itemNames, itemCounts, itemImagePaths);
        }
    }

    public void SetCIUI(ChestInventoryUI CIUI, CheckItemIdx CII)
    {
        this.CIUI = CIUI;
        this.CII = CII;
    }
}
