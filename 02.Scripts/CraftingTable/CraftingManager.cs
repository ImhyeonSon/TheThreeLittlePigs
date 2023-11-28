using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public PigGetItem PGI;

    public int userWood = 0;
    public int userMud = 0;
    public int userStone = 0;
    public int userRice = 0;
    public int userLumber = 0;
    public int userBrick = 0;
    public int userSlingshot = 0;
    public int userAxe = 0;
    public int userShovel = 0;
    public int userPick = 0;
    public int userSickle = 0;
    public int userShoes = 0;
    public int userUmbrella = 0;
    public int userTiger = 0;

    public TextMeshProUGUI userLumberCountText;
    public TextMeshProUGUI userBrickCountText;
    public TextMeshProUGUI userSlingshotCountText;
    public TextMeshProUGUI userAxeCountText;
    public TextMeshProUGUI userShovelCountText;
    public TextMeshProUGUI userPickCountText;
    public TextMeshProUGUI userSickleCountText;
    public TextMeshProUGUI userShoesCountText;
    public TextMeshProUGUI userUmbrellaCountText;
    public TextMeshProUGUI userTigerCountText;

    public TextMeshProUGUI userLumberCountPopupText;
    public TextMeshProUGUI userBrickCountPopupText;
    public TextMeshProUGUI userSlingshotCountPopupText;
    public TextMeshProUGUI userAxeCountPopupText;
    public TextMeshProUGUI userShovelCountPopupText;
    public TextMeshProUGUI userPickCountPopupText;
    public TextMeshProUGUI userSickleCountPopupText;
    public TextMeshProUGUI userShoesCountPopupText;
    public TextMeshProUGUI userUmbrellaCountPopupText;
    public TextMeshProUGUI userTigerCountPopupText;

    private CraftingMaterial CM;

    void Awake()
    {
        SetUserCountText();
        CM = GetComponentInChildren<CraftingMaterial>();
    }
    public void SetUserMaterialCount()
    {
        userWood = 0;
        userMud = 0;
        userStone = 0;
        userRice = 0;
        userLumber = 0;
        userBrick = 0;
        userSlingshot = 0;
        userAxe = 0;
        userShovel = 0;
        userPick = 0;
        userSickle = 0;
        userShoes = 0;
        userUmbrella = 0;
        userTiger = 0;
        // 반복문 돌면서 이름 검사하면서 더해주기
        for (int i = 0; i < PGI.InventoryDatas.Count; i++)
        {
            switch (PGI.InventoryDatas[i].ItemName)
            {
                case "Tree(Clone)":
                    userWood += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Mud(Clone)":
                    userMud += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Stone(Clone)":
                    userStone += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Rice(Clone)":
                    userRice += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Lumber":
                    userLumber += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Brick":
                    userBrick += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Slingshot":
                    userSlingshot += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Axe":
                    userAxe += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Shovel":
                    userShovel += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Pick":
                    userPick += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Sickle":
                    userSickle += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Shoes":
                    userShoes += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Umbrella":
                    userUmbrella += PGI.InventoryDatas[i].ItemCount;
                    break;
                case "Tiger":
                    userTiger += PGI.InventoryDatas[i].ItemCount;
                    break;
            }
        }
        SetUserCountText();
        
    }

    public void SetUserCountText()
    {
        userLumberCountText.text = userLumber.ToString();
        userBrickCountText.text = userBrick.ToString();
        userSlingshotCountText.text = userSlingshot.ToString();
        userAxeCountText.text = userAxe.ToString();
        userShovelCountText.text = userShovel.ToString(); // 삽!!
        userPickCountText.text = userPick.ToString();
        userSickleCountText.text = userSickle.ToString();
        userShoesCountText.text = userShoes.ToString();
        userUmbrellaCountText.text = userUmbrella.ToString();
        userTigerCountText.text = userTiger.ToString();

        userLumberCountPopupText.text = userLumber.ToString();
        userBrickCountPopupText.text = userBrick.ToString();
        userSlingshotCountPopupText.text = userSlingshot.ToString();
        userAxeCountPopupText.text = userAxe.ToString();
        userShovelCountPopupText.text = userShovel.ToString(); // 삽
        userPickCountPopupText.text = userPick.ToString();
        userSickleCountPopupText.text = userSickle.ToString();
        userShoesCountPopupText.text = userShoes.ToString();
        userUmbrellaCountPopupText.text = userUmbrella.ToString();
        userTigerCountPopupText.text = userTiger.ToString();
    }
}
