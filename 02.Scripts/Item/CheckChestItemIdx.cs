using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckChestItemIdx : MonoBehaviour
{
    // 상자 인벤토리
    public int lastChestRiceIdx = -1;
    public int lastChestWoodIdx = -1;
    public int lastChestStoneIdx = -1;
    public int lastChestMudIdx = -1;
    public int lastChestSyringe = -1;
    public int lastChestItemIdx = -1; // 전체 아이템의 마지막 인덱스 기록 예정

    public void UpdateLastIndex(string itemName, int idx)
    {
    switch (itemName)
        {
            case "Rice(Clone)": lastChestRiceIdx = idx; break;
            case "Mud(Clone)": lastChestMudIdx = idx; break;
            case "Stone(Clone)": lastChestStoneIdx = idx; break;
            case "Wood(Clone)": lastChestWoodIdx = idx; break;
        }
    }

    public void UpdateLastItemIndices()
    {
        lastChestItemIdx = Mathf.Max(lastChestRiceIdx, lastChestWoodIdx, lastChestStoneIdx, lastChestMudIdx, lastChestSyringe);
    }

    public void PrintIndicesInfo()
    {
        string chestInventoryInfo = "Chest Inventory Indices:\n" +
                                    $"Rice: {lastChestRiceIdx}, Wood: {lastChestWoodIdx}, " +
                                    $"Stone: {lastChestStoneIdx}, Mud: {lastChestMudIdx}, " +
                                    //$"Leather: {lastChestLeatherIdx}, " + // Uncomment when leather is added
                                    $"Syringe: {lastChestSyringe}, " +
                                    $"Last Item: {lastChestItemIdx}";

        Debug.Log(chestInventoryInfo);
    }
}
