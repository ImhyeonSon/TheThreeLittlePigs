using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    //알림 메시지 관련
    public FadeController FC;

    //소리이펙트
    public AudioSource SuccesSFX;


    // 해당 아이템을 만드는데 필요한 재료 개수(직접 할당)
    public int requiredWood = 0;
    public int requiredMud = 0;
    public int requiredStone = 0;
    public int requiredRice = 0;  // 지푸라기
    public int requiredLumber = 0;  // 나무판자
    public int requiredBrick = 0;

    // 유저가 가진 재료 개수
    public GameObject CTUI;  // CraftingTableUI
    public CraftingManager CM;
    public CheckItemIdx CII;
    private PigGetItem PGI;
    private PigInteractionCreateTable PICT;
    private PigInventoryUI PIUI;

    public string finishedProduct;

    public GameObject woodMaterial;
    public GameObject mudMaterial;
    public GameObject stoneMaterial;
    public GameObject riceMaterial;
    public GameObject lumberMaterial;
    public GameObject brickMaterial;

    public TextMeshProUGUI woodCount;
    public TextMeshProUGUI mudCount;
    public TextMeshProUGUI stoneCount;
    public TextMeshProUGUI riceCount;
    public TextMeshProUGUI lumberCount;
    public TextMeshProUGUI brickCount;

    public GameObject craftBtn;
    public GameObject disabledBtn;
    public bool isActive;
    public GameObject craftingPopupUI;
    public GameObject panelPopupUI;

    public bool isTreeHouse = false;
    public bool isBrickHouse = false;



    private Dictionary<string, int> requiredMaterials = new Dictionary<string, int>();

    void Awake()
    {
        CM = CTUI.GetComponent<CraftingManager>();
        GameObject checkItemIdxHolder = GameObject.Find("Pig_test");
        FC = GameObject.Find("Canvas").transform.Find("NotificationUI").GetComponent<FadeController>();
        PGI = CM.PGI;
        PICT = GetComponentInParent<Transform>().gameObject.GetComponentInParent<PigInteractionCreateTable>();
        CII = GetComponentInParent<Transform>().gameObject.GetComponentInParent<CheckItemIdx>();
        PIUI = GetComponentInParent<Transform>().gameObject.GetComponentInParent<PigInventoryUI>();
        UpdateMaterialCountsUI();
    }
    private void OnEnable()
    {
        UpdateMaterialCountsUI(); // 현재 가지고 있는 재료 업데이트
        CheckMaterialsAndSetButtonState(); // 버튼 업데이트
        SettingRequiredMaterials(); // 필요한 재료 세팅
    }
    public void UpdateMaterialCountsUI()
    {
        if (requiredWood > 0)
        {
            woodMaterial.SetActive(true);
            woodMaterial.GetComponent<CraftingMaterial>().SetMaterialCount(requiredWood, CM.userWood);  // 텍스트 설정
        }
        if (requiredMud > 0)
        {
            mudMaterial.SetActive(true);
            mudMaterial.GetComponent<CraftingMaterial>().SetMaterialCount(requiredMud, CM.userMud);
        }
        if (requiredStone > 0)
        {
            stoneMaterial.SetActive(true);
            stoneMaterial.GetComponent<CraftingMaterial>().SetMaterialCount(requiredStone, CM.userStone);
        }
        if (requiredRice > 0)
        {
            riceMaterial.SetActive(true);
            riceMaterial.GetComponent<CraftingMaterial>().SetMaterialCount(requiredRice, CM.userRice);
        }
        if (requiredLumber > 0)
        {
            lumberMaterial.SetActive(true);
            lumberMaterial.GetComponent<CraftingMaterial>().SetMaterialCount(requiredLumber, CM.userLumber);
        }
        if (requiredBrick > 0)
        {
            brickMaterial.SetActive(true);
            brickMaterial.GetComponent<CraftingMaterial>().SetMaterialCount(requiredBrick, CM.userBrick);
        }
    }
    public void CheckMaterialsAndSetButtonState()
    {
        // 필요한 모든 재료가 충분한지 확인
        bool canCraft = true;
        canCraft &= (requiredWood <= CM.userWood);
        canCraft &= (requiredMud <= CM.userMud);
        canCraft &= (requiredStone <= CM.userStone);
        canCraft &= (requiredRice <= CM.userRice);
        canCraft &= (requiredLumber <= CM.userLumber);
        canCraft &= (requiredBrick <= CM.userBrick);

        SetIsActive(canCraft);
    }

    public void SetIsActive(bool canCraft)
    {
        isActive = canCraft;
        if (finishedProduct == "Axe" && CM.userAxe > 0)
        {
            SetActiveBtn(false);
        } else if (finishedProduct == "Shovel" && CM.userShovel > 0)
        {
            SetActiveBtn(false);
        } else if (finishedProduct == "Pick" && CM.userPick > 0)
        {
            SetActiveBtn(false);
        } else if (finishedProduct == "Sickle" && CM.userSickle > 0)
        {
            SetActiveBtn(false);
        } else if (finishedProduct == "Shoes" && CM.userShoes > 0)
        {
            SetActiveBtn(false);
        }
        else
        {
            if (isTreeHouse || isBrickHouse)
            {
                GameObject[] houses = GameObject.FindGameObjectsWithTag("House");

                foreach (var house in houses)
                {
                    // 집 객체의 Photon View를 가져옴
                    PhotonView housePV = house.GetComponent<PhotonView>();
                    if (housePV.IsMine)
                    {
                        // 집 객체의 HouseManager 스크립트를 가져옴
                        HouseManager HM = house.GetComponent<HouseManager>();
                        // HouseManager 스크립트에 접근하여 필요한 작업 수행
                        if (HM.currentLevel == 1 && canCraft)
                        {
                            if (isTreeHouse)
                            {
                                SetActiveBtn(true);
                            }
                            else if (isBrickHouse)
                            {
                                SetActiveBtn(false);
                            }
                        }
                        else if (HM.currentLevel == 2 && canCraft)
                        {
                            if (isTreeHouse)
                            {
                                SetActiveBtn(false);
                            }
                            else if (isBrickHouse)
                            {
                                SetActiveBtn(true);
                            }
                        }
                        else
                        {
                            SetActiveBtn(false);
                        }
                    } 
                }
            }
            else
            {
                SetActiveBtn(isActive);
            }
        }

    }

    public void SetActiveBtn(bool active)  // 재료 여부에 따라 버튼 활성화
    {
        craftBtn.SetActive(active);
        disabledBtn.SetActive(!active);
    }

    private void SettingRequiredMaterials() // 딕셔너리 형태로 필요한 재료들을 넣어줄 예정
    {
        requiredMaterials.Clear(); // 기존 재료 목록을 클리어
        requiredMaterials.Add("Tree(Clone)", requiredWood);
        requiredMaterials.Add("Mud(Clone)", requiredMud);
        requiredMaterials.Add("Stone(Clone)", requiredStone);
        requiredMaterials.Add("Rice(Clone)", requiredRice);
        requiredMaterials.Add("Lumber", requiredLumber);
        requiredMaterials.Add("Brick", requiredBrick);
    }
    public void OnCraftButtonClick() // 제작 버튼 클릭 시 실행할 로직
    {
        GameObject[] houses = GameObject.FindGameObjectsWithTag("House");
        if (finishedProduct == "TreeHouse" || finishedProduct == "BrickHouse")
        {
            foreach (var house in houses)
            {
                PhotonView housePV = house.GetComponent<PhotonView>();
                if (housePV.IsMine)
                {
                    HouseManager HM = house.GetComponent<HouseManager>();
                    if (HM.isUpgrading && (finishedProduct == "TreeHouse" || finishedProduct == "BrickHouse")) // 업그레이드 중의 경우
                    {
                        FC.StartFadeIn("현재 집을 업그레이드 중입니다!\n조금 기다려 주세요!");
                        craftingPopupUI.SetActive(false);
                        panelPopupUI.SetActive(false);
                        PICT.ToggleCreateTable(); // 제작대 UI 비활성화
                    }
                    else
                    {
                        ConsumeMaterials();
                    }
                }
            }
        } else {
            ConsumeMaterials();
        }
    }

    private void ConsumeMaterials()
    {
        if (PGI.firstPigEmptyIdx != -1) {
            foreach (var material in requiredMaterials)
            {
                string itemName = material.Key;
                int requiredAmount = material.Value;

                // 인벤토리에서 해당 아이템을 찾아 필요한 수량만큼 소비
                for (int i = PGI.InventoryDatas.Count - 1; i >= 0 && requiredAmount > 0; i--)
                {
                    if (PGI.InventoryDatas[i].ItemName == itemName)
                    {
                        int itemCount = PGI.InventoryDatas[i].ItemCount;

                        // 현재 슬롯에서 필요한 수량을 모두 소비할 수 있는 경우
                        if (itemCount >= requiredAmount)
                        {
                            PGI.InventoryDatas[i].ItemCount -= requiredAmount;
                            requiredAmount = 0;
                        }
                        else // 현재 슬롯에 있는 수량만큼만 소비하고 다음 슬롯을 확인
                        {
                            requiredAmount -= itemCount;
                            PGI.InventoryDatas[i].ItemCount = 0;
                        }
                        if (PGI.InventoryDatas[i].ItemCount == 0)
                        {
                            PGI.InventoryDatas[i].ItemName = null;
                            PGI.InventoryDatas[i].ItemImage = null;
                        }

                        // 마지막 인덱스 업데이트 필요
                        CII.CheckEmptySlot("Pig");
                        CII.PrintIndicesInfo("Pig");
                        CII.CheckIndex("Pig", PGI.InventoryDatas);
                    }
                }
                CII.PrintIndicesInfo("Pig");
            }
            // 제작 성공
            craftingPopupUI.SetActive(false);
            panelPopupUI.SetActive(false);
            PICT.ToggleCreateTable(); // 제작 성공 + 제작대 UI 비활성화
            DecideAddItemIdx(finishedProduct); // 인덱스 찾을 함수임
        }
        else
        {
            craftingPopupUI.SetActive(false);
            panelPopupUI.SetActive(false);
            PICT.ToggleCreateTable(); // 제작대 UI 비활성화
            FC.StartFadeIn("인벤토리를 확인해주세요!");
        }
    }

    void DecideAddItemIdx(string product)
    {
        int addIdx = -1; // 물건 더해줄 인덱스

        switch (product)
        {
            case "Lumber":
                if (PGI.lastPigLumberIdx != -1 && PGI.InventoryDatas[PGI.lastPigLumberIdx].ItemCount != 10) // 이미 존재하고, 해당 아이템 슬롯이 차지 않음
                {
                    addIdx = PGI.lastPigLumberIdx;
                    break;
                } else
                {
                    addIdx = PGI.firstPigEmptyIdx;
                    break;
                }
            case "Brick":
                if (PGI.lastPigBrickIdx != -1 && PGI.InventoryDatas[PGI.lastPigBrickIdx].ItemCount != 10)
                {
                    addIdx = PGI.lastPigBrickIdx;
                    break;
                }
                else
                {
                    addIdx = PGI.firstPigEmptyIdx;
                    break;
                }
            case "SlingShot":
                if (PGI.lastPigSlingshotIdx != -1 && PGI.InventoryDatas[PGI.lastPigSlingshotIdx].ItemCount != 10)
                {
                    addIdx = PGI.lastPigSlingshotIdx;
                    break;
                }
                else
                {
                    addIdx = PGI.firstPigEmptyIdx;
                    break;
                }
            case "Axe":
                if (PGI.lastPigAxeIdx != -1 && PGI.InventoryDatas[PGI.lastPigAxeIdx].ItemCount != 10)
                {
                    addIdx = PGI.lastPigAxeIdx;
                    break;
                }
                else
                {
                    addIdx = PGI.firstPigEmptyIdx;
                    break;
                }
            case "Shovel":
                if (PGI.lastPigShovelIdx != -1 && PGI.InventoryDatas[PGI.lastPigShovelIdx].ItemCount != 10)
                {
                    addIdx = PGI.lastPigShovelIdx;
                    break;
                }
                else
                {
                    addIdx = PGI.firstPigEmptyIdx;
                    break;
                }
            case "Pick":
                if (PGI.lastPigPickIdx != -1 && PGI.InventoryDatas[PGI.lastPigPickIdx].ItemCount != 10)
                {
                    addIdx = PGI.lastPigPickIdx;
                    break;
                }
                else
                {
                    addIdx = PGI.firstPigEmptyIdx;
                    break;
                }
            case "Sickle":
                if (PGI.lastPigSickleIdx != -1 && PGI.InventoryDatas[PGI.lastPigSickleIdx].ItemCount != 10)
                {
                    addIdx = PGI.lastPigSickleIdx;
                    break;
                }
                else
                {
                    addIdx = PGI.firstPigEmptyIdx;
                    break;
                }
            case "Shoes":
                if (PGI.lastPigShoesIdx != -1 && PGI.InventoryDatas[PGI.lastPigShoesIdx].ItemCount != 10)
                {
                    addIdx = PGI.lastPigShoesIdx;
                    break;
                }
                else
                {
                    addIdx = PGI.firstPigEmptyIdx;
                    break;
                }
            case "Umbrella":
                if (PGI.lastPigUmbrellaIdx != -1 && PGI.InventoryDatas[PGI.lastPigUmbrellaIdx].ItemCount != 10)
                {
                    addIdx = PGI.lastPigUmbrellaIdx;
                    break;
                }
                else
                {
                    addIdx = PGI.firstPigEmptyIdx;
                    break;
                }
            case "Tiger":
                if (PGI.lastPigTigerIdx != -1 && PGI.InventoryDatas[PGI.lastPigTigerIdx].ItemCount != 10)
                {
                    addIdx = PGI.lastPigTigerIdx;
                    break;
                }
                else
                {
                    addIdx = PGI.firstPigEmptyIdx;
                    break;
                }
            case "TreeHouse":
                //Debug.Log("나무 집 생성");
                break;

            case "BrickHouse":
                //Debug.Log("벽돌 집 생성");
                break;
        }

        if (product == "TreeHouse" || product == "BrickHouse")
        {
            // 모든 집 객체를 찾아옴
            GameObject[] houses = GameObject.FindGameObjectsWithTag("House");

            foreach (var house in houses)
            {
                // 집 객체의 Photon View를 가져옴
                PhotonView housePV = house.GetComponent<PhotonView>();
                if (housePV.IsMine)
                {
                    // 집 객체의 HouseManager 스크립트를 가져옴
                    HouseManager HM = house.GetComponent<HouseManager>();
                    // HouseManager 스크립트에 접근하여 필요한 작업 수행
                    if (!HM.isUpgrading) // 업그레이드 중이 아닌 경우
                    {
                        if (product == "TreeHouse")
                        {
                            FC.StartFadeIn("지었던 집이 나무집으로 업그레이드 됩니다.\n지었던 집을 확인해보세요!");
                            HM.GetComponent<PhotonView>().RPC("ReadyToLevelUp", RpcTarget.All);
                        }
                        else if (product == "BrickHouse")
                        {
                            FC.StartFadeIn("지었던 집이 벽돌집으로 업그레이드 됩니다.\n지었던 집을 확인해보세요!");
                            HM.GetComponent<PhotonView>().RPC("ReadyToLevelUp", RpcTarget.All);
                        }
                    }
                }
            }
        } else if (addIdx == -1 || addIdx == 12) {
            FC.StartFadeIn("아이템을 추가할 빈 슬롯이 없어요!");
        } else {
            AddCreatedItem(product, addIdx);
        }
        CII.CheckEmptySlot("Pig");
        CII.CheckIndex("Pig", PGI.InventoryDatas);
    }

    void AddCreatedItem(string item, int idx)
    {
        if (Enum.TryParse(item, out PigItem.ItemType itemType))
        {
            if (PGI.itemSprites.TryGetValue(itemType, out string spritePath))
            {
                if (PGI.InventoryDatas[idx].ItemCount == 0) // 새로운 슬롯인 경우
                {
                    PGI.InventoryDatas[idx].ItemName = item;
                    if (item == "SlingShot")
                    {
                        PGI.InventoryDatas[idx].ItemCount = 10; // 새총의 경우 아이템 카운트 10개로 정해준다
                    }
                    else if (item == "Tiger" || item == "Umbrella")
                    {
                        PGI.InventoryDatas[idx].ItemCount = 5; // 우산이나 호랑이 허수아비의 경우 5개
                    }
                    else
                    {
                        PGI.InventoryDatas[idx].ItemCount = 1;
                    }
                    PGI.InventoryDatas[idx].ItemImage = spritePath;
                    CII.CheckEmptySlot("Pig"); // 비어있는 첫 슬롯 확인해주기
                }
                else
                {
                    if (item == "SlingShot") // 기존 슬롯에 9발이 있고, 내가 10발을 저장해야 하는 경우. 원하는 것? 기존 슬롯 + 1발 = 10발, 새 슬롯 = 9발
                    {
                        int addSlingShotCount = 10; // 10발을 추가해야 한다.
                        addSlingShotCount -= PGI.InventoryDatas[idx].ItemCount; // 새 슬롯에 더해줘야 하는 새총알의 수는 10 - 추가할 기존 슬롯
                        PGI.InventoryDatas[idx].ItemCount = 10;
                        PGI.InventoryDatas[PGI.firstPigEmptyIdx].ItemCount = addSlingShotCount; // 새 슬롯에는 빠진 만큼 더해주면 된다.
                        PGI.InventoryDatas[PGI.firstPigEmptyIdx].ItemName = item;
                        PGI.InventoryDatas[PGI.firstPigEmptyIdx].ItemImage = spritePath;
                        CII.CheckEmptySlot("Pig"); // 비어있는 첫 슬롯 확인해주기
                    }
                    else if ((item == "Tiger" || item == "Umbrella") && PGI.InventoryDatas[idx].ItemCount > 5) // 호.허 우산 로직. 5개 이상인 경우는 어떻게 만들지?
                    {
                        int addItemCount = 5; // 5개를 추가해야 한다.
                        addItemCount = PGI.InventoryDatas[idx].ItemCount - 5; // 새 슬롯에 더해줘야 하는 아이템 수는 5 - 추가할 기존 슬롯
                        PGI.InventoryDatas[idx].ItemCount = 10;
                        PGI.InventoryDatas[PGI.firstPigEmptyIdx].ItemCount = addItemCount; // 새 슬롯에는 빠진 만큼 더해주면 된다.
                        PGI.InventoryDatas[PGI.firstPigEmptyIdx].ItemName = item;
                        PGI.InventoryDatas[PGI.firstPigEmptyIdx].ItemImage = spritePath;
                        CII.CheckEmptySlot("Pig"); // 비어있는 첫 슬롯 확인해주기
                    }
                    else
                    {
                        if (item == "Tiger" || item == "Umbrella")
                        {
                            PGI.InventoryDatas[idx].ItemCount += 5;
                        }
                        else
                        {
                            PGI.InventoryDatas[idx].ItemCount++;
                        }
                    }
                }
            }
        }
        // 인덱스 정보를 출력하고 인벤토리를 다시 확인
        CII.CheckEmptySlot("Pig");
        CII.CheckIndex("Pig", PGI.InventoryDatas);
        CII.PrintIndicesInfo("Pig");
        PIUI.UpdateInventoryUI();

        SuccesSFX.Play();

    }

}
