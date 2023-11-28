using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftingMaterial : MonoBehaviour
{
    public CraftingSystem CS;
    public TextMeshProUGUI materialCountText;


    private void OnEnable()
    {
        CS = GetComponentInParent<Transform>().gameObject.GetComponentInParent<CraftingSystem>();
        materialCountText = transform.Find("Count").gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void SetMaterialCount(int requiredCount, int userCount)
    {
        if (requiredCount > userCount)
        {
            materialCountText.color = Color.red;
            //CS.SetIsActive(false);
        }
        else  // 재료가 충분함
        {
            materialCountText.color = Color.black;
            //CS.SetIsActive(true);
        }
        materialCountText.text = userCount + " / " + requiredCount;
    }
}
