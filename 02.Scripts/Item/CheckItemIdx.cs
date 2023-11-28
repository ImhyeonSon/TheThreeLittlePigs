using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Realtime;
using UnityEngine;

public class CheckItemIdx : MonoBehaviour
{
    private PigGetItem PGI;
    private ChestInventoryUI CIUI;

    private float interactionDistance = 2.0f;
    int chestLayer = 1 << 9; // 상자 레이어
    private Collider[] chestInRange; // 범위 내 상자

    void Awake()
    {
        PGI = GetComponentInParent<PigGetItem>(); // 루트 GameObject에서 PigGetItem 찾기
        CIUI = GetComponentInParent<ChestInventoryUI>();
    }
    public void CheckIndex(string inventoryType, List<InventoryData> inventoryDatas) // 모든 유형에 대해 마지막 인덱스 정해준다.
    {
        if (inventoryType == "Pig")
        {
            PGI.lastPigRiceIdx = -1;
            PGI.lastPigMudIdx = -1;
            PGI.lastPigStoneIdx = -1;
            PGI.lastPigWoodIdx = -1;
            PGI.lastPigSyringeIdx = -1;
            PGI.lastPigLumberIdx = -1;
            PGI.lastPigBrickIdx = -1;
            PGI.lastPigSlingshotIdx = -1;
            PGI.lastPigAxeIdx = -1;
            PGI.lastPigShovelIdx = -1;
            PGI.lastPigPickIdx = -1;
            PGI.lastPigSickleIdx = -1;
            PGI.lastPigShoesIdx = -1;
            PGI.lastPigUmbrellaIdx = -1;
            PGI.lastPigTigerIdx = -1;

            // 리스트를 역순으로 검사하여 마지막 인덱스를 찾기
            for (int i = inventoryDatas.Count - 1; i >= 0; i--)
            {
                if (inventoryDatas[i].ItemName != null && inventoryDatas[i].ItemCount > 0)
                {
                    switch (inventoryDatas[i].ItemName)
                    {
                        case "Rice(Clone)":
                            if (PGI.lastPigRiceIdx == -1) PGI.lastPigRiceIdx = i;
                            break;
                        case "Mud(Clone)":
                            if (PGI.lastPigMudIdx == -1) PGI.lastPigMudIdx = i;
                            break;
                        case "Stone(Clone)":
                            if (PGI.lastPigStoneIdx == -1) PGI.lastPigStoneIdx = i;
                            break;
                        case "Tree(Clone)":
                            if (PGI.lastPigWoodIdx == -1) PGI.lastPigWoodIdx = i;
                            break;
                        case "Syringe":
                            if (PGI.lastPigSyringeIdx == -1) PGI.lastPigSyringeIdx = i;
                            break;
                        case "Lumber":
                            if (PGI.lastPigLumberIdx == -1) PGI.lastPigLumberIdx = i;
                            break;
                        case "Brick":
                            if (PGI.lastPigBrickIdx == -1) PGI.lastPigBrickIdx = i;
                            break;
                        case "SlingShot":
                            if (PGI.lastPigSlingshotIdx == -1) PGI.lastPigSlingshotIdx = i;
                            break;
                        case "Axe":
                            if (PGI.lastPigAxeIdx == -1) PGI.lastPigAxeIdx = i;
                            break;
                        case "Shovel":
                            if (PGI.lastPigShovelIdx == -1) PGI.lastPigShovelIdx = i;
                            break;
                        case "Pick":
                            if (PGI.lastPigPickIdx == -1) PGI.lastPigPickIdx = i;
                            break;
                        case "Sickle":
                            if (PGI.lastPigSickleIdx == -1) PGI.lastPigSickleIdx = i;
                            break;
                        case "Shoes":
                            if (PGI.lastPigShoesIdx == -1) PGI.lastPigShoesIdx = i;
                            break;
                        case "Umbrella":
                            if (PGI.lastPigUmbrellaIdx == -1) PGI.lastPigUmbrellaIdx = i;
                            break;
                        case "Tiger":
                            if (PGI.lastPigTigerIdx == -1) PGI.lastPigTigerIdx = i;
                            break;
                    }
                }
                UpdateLastItemIndices("Pig");
            }
        }
        else if (inventoryType == "Chest")
        {
            CIUI.currentChestGetItem.lastChestRiceIdx = -1;
            CIUI.currentChestGetItem.lastChestMudIdx = -1;
            CIUI.currentChestGetItem.lastChestStoneIdx = -1;
            CIUI.currentChestGetItem.lastChestWoodIdx = -1;
            CIUI.currentChestGetItem.lastChestSyringeIdx = -1;
            CIUI.currentChestGetItem.lastChestLumberIdx = -1;
            CIUI.currentChestGetItem.lastChestBrickIdx = -1;
            CIUI.currentChestGetItem.lastChestSlingShotIdx = -1;
            CIUI.currentChestGetItem.lastChestUmbrellaIdx = -1;
            CIUI.currentChestGetItem.lastChestTigerIdx = -1;

            for (int i = inventoryDatas.Count - 1; i >= 0; i--)
            {
                if (inventoryDatas[i].ItemName != null && inventoryDatas[i].ItemCount > 0)
                {
                    switch (inventoryDatas[i].ItemName)
                    {
                        case "Rice(Clone)":
                            if (CIUI.currentChestGetItem.lastChestRiceIdx == -1) CIUI.currentChestGetItem.lastChestRiceIdx = i;
                            break;
                        case "Mud(Clone)":
                            if (CIUI.currentChestGetItem.lastChestMudIdx == -1) CIUI.currentChestGetItem.lastChestMudIdx = i;
                            break;
                        case "Stone(Clone)":
                            if (CIUI.currentChestGetItem.lastChestStoneIdx == -1) CIUI.currentChestGetItem.lastChestStoneIdx = i;
                            break;
                        case "Tree(Clone)":
                            if (CIUI.currentChestGetItem.lastChestWoodIdx == -1) CIUI.currentChestGetItem.lastChestWoodIdx = i;
                            break;
                        case "Syringe":
                            if (CIUI.currentChestGetItem.lastChestSyringeIdx == -1) CIUI.currentChestGetItem.lastChestSyringeIdx = i;
                            break;
                        case "Lumber":
                            if (CIUI.currentChestGetItem.lastChestLumberIdx == -1) CIUI.currentChestGetItem.lastChestLumberIdx = i;
                            break;
                        case "Brick":
                            if (CIUI.currentChestGetItem.lastChestBrickIdx == -1) CIUI.currentChestGetItem.lastChestBrickIdx = i;
                            break;
                        case "SlingShot":
                            if (CIUI.currentChestGetItem.lastChestSlingShotIdx == -1) CIUI.currentChestGetItem.lastChestSlingShotIdx = i;
                            break;
                        case "Umbrella":
                            if (CIUI.currentChestGetItem.lastChestUmbrellaIdx == -1) CIUI.currentChestGetItem.lastChestUmbrellaIdx = i;
                            break;
                        case "Tiger":
                            if (CIUI.currentChestGetItem.lastChestTigerIdx == -1) CIUI.currentChestGetItem.lastChestTigerIdx = i;
                            break;
                    }
                }
            }
            UpdateLastItemIndices("Chest");
        }
    }
    public void CheckEmptySlot(string inventoryType) // 빈 슬롯 인덱스 구해주는 함수
    {
        if (inventoryType == "Pig")
        {
            PGI.firstPigEmptyIdx = -1; // 빈 슬롯이 없는 경우를 위한 초기값 설정
            for (int i = 0; i < PGI.InventoryDatas.Count; i++)
            {
                if (PGI.InventoryDatas[i].ItemCount == 0)
                {
                    PGI.firstPigEmptyIdx = i;
                    break;
                }
            }
        }
        else if (inventoryType == "Chest")
        {
            CIUI.currentChestGetItem.firstChestEmptyIdx = -1; // 빈 슬롯이 없는 경우를 위한 초기값 설정
            for (int i = 0; i < CIUI.currentChestGetItem.chestInventoryDatas.Count; i++)
            {
                if (CIUI.currentChestGetItem.chestInventoryDatas[i].ItemCount == 0)
                {
                    CIUI.currentChestGetItem.firstChestEmptyIdx = i;
                    break;
                }
            }
        }
    }

    public void UpdateLastItemIndices(string inventoryType) // 마지막 인덱스를 구해주는 함수
    {
        if (inventoryType == "Pig")
        {
            List<int> indices = new List<int>
        {
            PGI.lastPigRiceIdx, PGI.lastPigWoodIdx, PGI.lastPigStoneIdx,
            PGI.lastPigMudIdx, PGI.lastPigSyringeIdx, PGI.lastPigLumberIdx,
            PGI.lastPigBrickIdx, PGI.lastPigSlingshotIdx, PGI.lastPigAxeIdx, PGI.lastPigShovelIdx,
            PGI.lastPigPickIdx, PGI.lastPigSickleIdx, PGI.lastPigShoesIdx,
            PGI.lastPigUmbrellaIdx, PGI.lastPigTigerIdx
        };

            PGI.lastPigItemIdx = indices.Max();
        }
        else if (inventoryType == "Chest")
        {
            List<int> indices = new List<int>
        {
            CIUI.currentChestGetItem.lastChestRiceIdx, CIUI.currentChestGetItem.lastChestWoodIdx, CIUI.currentChestGetItem.lastChestStoneIdx,
            CIUI.currentChestGetItem.lastChestMudIdx, CIUI.currentChestGetItem.lastChestSyringeIdx, CIUI.currentChestGetItem.lastChestLumberIdx,
            CIUI.currentChestGetItem.lastChestBrickIdx, CIUI.currentChestGetItem.lastChestSlingShotIdx, CIUI.currentChestGetItem.lastChestUmbrellaIdx,
            CIUI.currentChestGetItem.lastChestTigerIdx
        };

            CIUI.currentChestGetItem.lastChestItemIdx = indices.Max();
        }
    }


    //// 마지막 인덱스를 정정해주기 (물건을 뺀 경우)
    //public void RecalculateLastIndices(string inventoryType, string itemName, List<InventoryData> inventoryDataList)
    //{
    //    int lastIndex = -1;

    //    for (int i = inventoryDataList.Count - 1; i >= 0; i--)
    //    {
    //        if (inventoryDataList[i].ItemName == itemName)
    //        {
    //            lastIndex = i;
    //            break; // 이 아이템을 가진 마지막 인덱스 찾음
    //        }
    //    }
    //    UpdateSpecificIndex(inventoryType, itemName, lastIndex);
    //}

    public void PrintIndicesInfo(string inventoryType) // 인덱스 정보 뽑아보는 함수
    {
        string inventoryInfo ="";
        if (inventoryType == "Pig")
        {
            inventoryInfo = "Pig Inventory Indices:\n" +
                            $"Rice: {PGI.lastPigRiceIdx}, Wood: {PGI.lastPigWoodIdx}, " +
                            $"Stone: {PGI.lastPigStoneIdx}, Mud: {PGI.lastPigMudIdx}, " +
                        $"Syringe: {PGI.lastPigSyringeIdx}, Lumber: {PGI.lastPigLumberIdx}, " +
                        $"Brick: {PGI.lastPigBrickIdx}, SlingShot : {PGI.lastPigSlingshotIdx}," +
                        $"Axe : {PGI.lastPigAxeIdx}, Pick : {PGI.lastPigPickIdx}," + $"Shovel : { PGI.lastPigShovelIdx}" +
                        $"Sickle : {PGI.lastPigSickleIdx}, Shoes : {PGI.lastPigShoesIdx}," +
                        $"Umbrella : {PGI.lastPigUmbrellaIdx}, Tiger : {PGI.lastPigTigerIdx}"+
                        $"Syringe : {PGI.lastPigSyringeIdx}" +
                        $"Last Item: {PGI.lastPigItemIdx}" + 
                        $"Empty Slot: {PGI.firstPigEmptyIdx}";
        }
        else if (inventoryType == "Chest")
        {
            
            inventoryInfo = "Chest Inventory Indices:\n" +
                            $"Rice: {CIUI.currentChestGetItem.lastChestRiceIdx}, Wood: {CIUI.currentChestGetItem.lastChestWoodIdx}, " +
                            $"Stone: {CIUI.currentChestGetItem.lastChestStoneIdx}, Mud: {CIUI.currentChestGetItem.lastChestMudIdx}, " +
                            $"Syringe: {CIUI.currentChestGetItem.lastChestSyringeIdx}, Lumber: {CIUI.currentChestGetItem.lastChestLumberIdx}, " +
                            $"Brick: {CIUI.currentChestGetItem.lastChestBrickIdx}, SlingShot: {CIUI.currentChestGetItem.lastChestSlingShotIdx}, " +
                            $"Umbrella: {CIUI.currentChestGetItem.lastChestUmbrellaIdx}, Tiger: {CIUI.currentChestGetItem.lastChestTigerIdx}, " +
                            $"Last Item: {CIUI.currentChestGetItem.lastChestItemIdx}, " +
                            $"Empty Slot: {CIUI.currentChestGetItem.firstChestEmptyIdx}";
        }
        Debug.Log(inventoryInfo);
    }

}