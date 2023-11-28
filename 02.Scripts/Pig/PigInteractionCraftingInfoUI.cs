using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigInteractionCraftingInfoUI : MonoBehaviour
{
    public GameObject CraftingInfoUI;

    public bool isOpenCraftingInfoUI;

    public void PigOpenCraftingInfoUI ()
    {
        if (!CraftingInfoUI.activeSelf && Input.GetKeyDown(KeyCode.F2))
        {
            CraftingInfoUI.SetActive(true);
            isOpenCraftingInfoUI = true;
        } else if (CraftingInfoUI.activeSelf && Input.GetKeyDown(KeyCode.F2))
        {
            CraftingInfoUI.SetActive(false);
            isOpenCraftingInfoUI = false;
        }
    }

    public bool GetIsOpenCraftingInfoUI()
    {
        return CraftingInfoUI.activeSelf;
    }
}
