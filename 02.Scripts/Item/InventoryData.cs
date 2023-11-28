using System;
using UnityEngine;

[Serializable]
public class InventoryData
{
    public string ItemName;
    public int ItemCount;
    public string ItemImage;

    public InventoryData()
    {
    }

    public InventoryData(string itemName, int itemCount, string itemImage)
    {
        ItemName = itemName;
        ItemCount = itemCount;
        ItemImage = itemImage;
    }
}
