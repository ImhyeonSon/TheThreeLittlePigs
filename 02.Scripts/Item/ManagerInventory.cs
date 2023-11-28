using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerInventory : MonoBehaviour
{
    private PhotonView PV;
    public GameObject UserInventory;
    public GameObject ChestInventory;

    private ChestInventoryUI CIUI;
    private PigInventoryUI PIUI;
    private PigGetItem PGI;
    private CheckItemIdx CII;
    private ChestGetItem CGI;
    private PigInteractionChest PIC;

    //알림 메시지 관련
    public FadeController FC;

    private float interactionDistance = 2.0f;
    int chestLayer = 1 << 9; // 상자 레이어
    private Collider[] chestInRange; // 범위 내 상자

    // 이 스크립트는 Pig에 있음

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        CIUI = GetComponent<ChestInventoryUI>();
        PIUI = GetComponent<PigInventoryUI>();
        PGI = GetComponent<PigGetItem>();
        CII = GetComponent<CheckItemIdx>();
        PIC = GetComponent<PigInteractionChest>();
        FC = GameObject.Find("Canvas").transform.Find("NotificationUI").GetComponent<FadeController>();
        PhotonPeer.RegisterType(typeof(InventoryData), (byte)'I', SerializeInventoryData, DeserializeInventoryData); PIC = GetComponent<PigInteractionChest>();
        if (PIC != null)
        {
            PIC.OnChestGetItemUpdated += UpdateCGIReference;
        }
    }
    private void OnDestroy()
    {
        if (PIC != null)
        {
            PIC.OnChestGetItemUpdated -= UpdateCGIReference;
        }
    }

    private void UpdateCGIReference(ChestGetItem newCGI)
    {
        CGI = newCGI;
    }
    public void SlotClicked(int idx, string InventoryType)
    {
        // 돼지는 RPC 안 쏜다. 상자의 경우에만 모든 유저에게 반영되도록 RPC 처리
        if (InventoryType == "Pig")
        {
            InventoryData data = PGI.InventoryDatas[idx];
            byte[] dataBytes = SerializeInventoryData(data); // RPC로 보내기 위해 data json화

            string jsonData = Encoding.UTF8.GetString(dataBytes); // byte[]를 문자열로 변환
            Debug.Log("돼지가 잃는 직렬화된 데이터 (JSON): " + jsonData); // 문자열 형태의 JSON 데이터 출력
            int addIdx = decideAddItemIdx(data.ItemName, "Chest"); // 애초에 슬롯이 꽉 찼는지 판별할 임의의 인덱스

            if (data.ItemCount != 0)
            {
                if (data.ItemName == "Axe" || data.ItemName == "Shovel" || data.ItemName == "Pick" || data.ItemName == "Sickle" || data.ItemName == "Shoes")
                {
                    FC.StartFadeIn("해당 아이템은 옮길 수 없어요!");
                }
                else if (addIdx == -1)
                {
                    FC.StartFadeIn("상자가 다 찼어요!");
                }
                else // 상자에 넣을 수 있는 아이템
                {
                    if (PGI.InventoryDatas[idx].ItemCount > 0)
                    {
                        LostItemFromPig(idx);
                        AddItemToChest(dataBytes, addIdx);
                    }
                }
            }
        }
        else if (InventoryType == "Chest")
        {
            InventoryData data = CIUI.currentChestGetItem.chestInventoryDatas[idx];
            byte[] dataBytes = SerializeInventoryData(data); // 상자가 데이터 잃는단 걸 보여주기 위해 data json화

            string jsonData = Encoding.UTF8.GetString(dataBytes); // byte[]를 문자열로 변환
            Debug.Log("상자가 잃는 직렬화된 데이터 (JSON): " + jsonData); // 문자열 형태의 JSON 데이터 출력
            int addIdx = decideAddItemIdx(data.ItemName, "Pig"); // 애초에 슬롯이 꽉 찼는지 판별할 임의의 인덱스

            if (data.ItemCount !=  0)
            {
                if (addIdx == -1)
                {
                    FC.StartFadeIn("인벤토리가 다 찼어요!");
                }
                else if (data.ItemCount != 0)
                {
                    if (CIUI.currentChestGetItem.chestInventoryDatas[idx].ItemCount > 0)
                    {
                        AddItemToPig(data.ItemName, data.ItemImage, addIdx);
                        LostItemFromChest(dataBytes, idx);
                    }
                }
            }
        }
        UpdateInventoryUI(); // 데이터 오간 후 인벤토리 업데이트 해준다.
    }

    
    void LostItemFromPig(int index)
    {
        PGI.InventoryDatas[index].ItemCount--;
        if (PGI.InventoryDatas[index].ItemCount == 0)
        {
            // 슬롯이 빈 경우 슬롯을 초기화해준다.
            PGI.InventoryDatas[index].ItemName = null;
            PGI.InventoryDatas[index].ItemImage = "Item/Images/NoItem";
        }
        CII.CheckIndex("Pig", PGI.InventoryDatas); // 항목별 마지막 인덱스 체크해주기
        CII.CheckEmptySlot("Pig"); // 빈 슬롯 찾아주기
    }

    public void LostItemFromChest(byte[] serializedData, int idx)
    {
        InventoryData data = DeserializeInventoryData(serializedData); // json화 풀어볼까

        string jsonData = JsonUtility.ToJson(data);
        // 변환된 JSON 문자열을 로그로 출력
        Debug.Log("상자에서 잃을 역직렬화된 데이터 (JSON): " + jsonData);

        // 체스트 인벤토리 내의 아이템 수량 감소
        //CIUI
        CIUI.currentChestGetItem.chestInventoryDatas[idx].ItemCount--;
        if (CIUI.currentChestGetItem.chestInventoryDatas[idx].ItemCount == 0)
        {
            // 슬롯 초기화
            CIUI.currentChestGetItem.chestInventoryDatas[idx].ItemName = null;
            CIUI.currentChestGetItem.chestInventoryDatas[idx].ItemImage = "Item/Images/NoItem";
        }
        CII.CheckEmptySlot("Chest");
        CII.CheckIndex("Chest", CIUI.currentChestGetItem.chestInventoryDatas);

    }

    // 다른 인벤토리에 아이템을 더해준다
    void AddItemToPig(string itemName, string itemImage, int idx)
    {
        Debug.Log("돼지에 아이템 넣어줘볼까?");
        Debug.Log("클릭한 상자의 이름 : " + itemName + "클릭한 상자의 이미지 정보 : " + itemName);
            if (PGI.InventoryDatas[idx].ItemCount == 0)
            {
            // 새 슬롯임
            Debug.Log("새 슬롯이라구!");
                PGI.InventoryDatas[idx].ItemName = itemName;
                PGI.InventoryDatas[idx].ItemImage = itemImage;
            }
            PGI.InventoryDatas[idx].ItemCount++; // 새 슬롯이건 기존의 슬롯이건 아이템 올려주기
            CII.CheckEmptySlot("Pig");
            CII.CheckIndex("Pig", PGI.InventoryDatas);
     }    

    [PunRPC]
    public void AddItemToChest(byte[] serializedData, int addIdx)
    {
        Debug.Log(serializedData);
        InventoryData data = DeserializeInventoryData(serializedData) as InventoryData;

        string jsonData = JsonUtility.ToJson(data);
        // 변환된 JSON 문자열을 로그로 출력
        Debug.Log("상자에 넣어줄 역직렬화된 데이터 (JSON): " + jsonData);


        string itemName = data.ItemName;
        //int idx = decideAddItemIdx(itemName, "Chest"); // 여기에 추가해줄래

        if (addIdx != -1)
        {
            if (CIUI.currentChestGetItem.chestInventoryDatas[addIdx].ItemCount == 0)
            {
                CIUI.currentChestGetItem.chestInventoryDatas[addIdx].ItemName = itemName;
                CIUI.currentChestGetItem.chestInventoryDatas[addIdx].ItemImage = data.ItemImage;
                CII.PrintIndicesInfo("Chest");
            }
            CIUI.currentChestGetItem.chestInventoryDatas[addIdx].ItemCount ++;
        }
        else
        {
            if (addIdx == -2)
            {
                FC.StartFadeIn("해당 아이템은 옮길 수 없습니다.");
            }
            else
            {
                FC.StartFadeIn("아이템 슬롯을 확인해주세요!");
            }
        }
        CII.CheckIndex("Chest", CIUI.currentChestGetItem.chestInventoryDatas);
        CII.CheckEmptySlot("Chest");
    }

    // 물건 옮기면 무조건 UI 갱신
    void UpdateInventoryUI()
    {
        PIUI.UpdateInventoryUI();
        CIUI.UpdateChestInventoryUI();
    }

    // 직렬화 함수. RPC에 담아 보내는 data는 json 형태
    public static byte[] SerializeInventoryData(object InventoryData)
    {
        string json = JsonUtility.ToJson(InventoryData);
        return Encoding.UTF8.GetBytes(json);
    }


    // 역직렬화 함수. RPC에서 받은 json data를 inventoryData 형태로 변환시킨다
    public static InventoryData DeserializeInventoryData(byte[] bytes)
    {
        string json = Encoding.UTF8.GetString(bytes);
        return JsonUtility.FromJson<InventoryData>(json);
    }

    int decideAddItemIdx(string itemName, string inventoryType) // 아이템 더해질 인벤토리의 인덱스 구하는 함수
    {
        Debug.Log("넣어줄 아이템은요," + itemName + "어디에 넣어 줄거면," + inventoryType);
        if (inventoryType == "Pig")
        {
            CII.PrintIndicesInfo("Pig");
        } else
        {
            CII.PrintIndicesInfo("Chest");
        }
        int addIdx = -1;
        Debug.Log("*****************" + itemName);
        // 해당 아이템의 마지막 카운트가 10 이하라면 더할 수 있고 10이라면 Emptyslot에 넣어야 한다
        switch (itemName)
        {
            case "Rice(Clone)":
                if (inventoryType == "Chest")
                {
                    if (CIUI.currentChestGetItem.lastChestRiceIdx != -1 && CIUI.currentChestGetItem.chestInventoryDatas[CIUI.currentChestGetItem.lastChestRiceIdx].ItemCount < 10)
                    {
                        addIdx = CIUI.currentChestGetItem.lastChestRiceIdx;
                    }
                    else
                    {
                        addIdx = CIUI.currentChestGetItem.firstChestEmptyIdx;
                    }

                } else if (inventoryType == "Pig")
                {
                    if (PGI.lastPigRiceIdx != -1 && PGI.InventoryDatas[PGI.lastPigRiceIdx].ItemCount < 10)
                    {
                        addIdx = PGI.lastPigRiceIdx;
                    }
                    else
                    {
                        addIdx = PGI.firstPigEmptyIdx;
                    }
                }
                break;
            case "Mud(Clone)":
                if (inventoryType == "Chest")
                {
                    if (CIUI.currentChestGetItem.lastChestMudIdx != -1 && CIUI.currentChestGetItem.chestInventoryDatas[CIUI.currentChestGetItem.lastChestMudIdx].ItemCount < 10)
                    {
                        addIdx = CIUI.currentChestGetItem.lastChestMudIdx;
                    }
                    else
                    {
                        addIdx = CIUI.currentChestGetItem.firstChestEmptyIdx;
                    }
                }
                else if (inventoryType == "Pig")
                {
                    if (PGI.lastPigMudIdx != -1 && PGI.InventoryDatas[PGI.lastPigMudIdx].ItemCount < 10)
                    {
                        addIdx = PGI.lastPigMudIdx;
                    }
                    else
                    {
                        addIdx = PGI.firstPigEmptyIdx;
                    }
                }
                break;
            case "Stone(Clone)":
                if (inventoryType == "Chest")
                {
                    if (CIUI.currentChestGetItem.lastChestStoneIdx != -1 && CIUI.currentChestGetItem.chestInventoryDatas[CIUI.currentChestGetItem.lastChestStoneIdx].ItemCount < 10)
                    {
                        addIdx = CIUI.currentChestGetItem.lastChestStoneIdx;
                    }
                    else
                    {
                        addIdx = CIUI.currentChestGetItem.firstChestEmptyIdx;
                    }
                }
                else if (inventoryType == "Pig")
                {

                    if (PGI.lastPigStoneIdx != -1 && PGI.InventoryDatas[PGI.lastPigStoneIdx].ItemCount < 10)
                    {
                        addIdx = PGI.lastPigStoneIdx;
                    }
                    else
                    {
                        addIdx = PGI.firstPigEmptyIdx;
                    }
                }
                break;
            case "Tree(Clone)":
                if (inventoryType == "Chest")
                {
                    if (CIUI.currentChestGetItem.lastChestWoodIdx != -1 && CIUI.currentChestGetItem.chestInventoryDatas[CIUI.currentChestGetItem.lastChestWoodIdx].ItemCount < 10)
                    {
                        addIdx = CIUI.currentChestGetItem.lastChestWoodIdx;
                    }
                    else
                    {
                        addIdx = CIUI.currentChestGetItem.firstChestEmptyIdx;
                    }
                }
                else if (inventoryType == "Pig")
                {
                    if (PGI.lastPigWoodIdx != -1 && PGI.InventoryDatas[PGI.lastPigWoodIdx].ItemCount < 10)
                    {
                        addIdx = PGI.lastPigWoodIdx;
                    }
                    else
                    {
                        addIdx = PGI.firstPigEmptyIdx;
                    }
                }
                break;
            case "Syringe":
                if (inventoryType == "Chest")
                {
                    if (CIUI.currentChestGetItem.lastChestSyringeIdx != -1 && CIUI.currentChestGetItem.chestInventoryDatas[CIUI.currentChestGetItem.lastChestSyringeIdx].ItemCount < 10)
                    {
                        addIdx = CIUI.currentChestGetItem.lastChestSyringeIdx;
                    }
                    else
                    {
                        addIdx = CIUI.currentChestGetItem.firstChestEmptyIdx;
                    }
                }
                else if (inventoryType == "Pig")
                {
                    if (PGI.lastPigSyringeIdx != -1 && PGI.InventoryDatas[PGI.lastPigSyringeIdx].ItemCount < 10)
                    {
                        addIdx = PGI.lastPigSyringeIdx;
                    }
                    else
                    {
                        addIdx = PGI.firstPigEmptyIdx;
                    }
                }
                break;
            case "Lumber":
                if (inventoryType == "Chest")
                {
                    if (CIUI.currentChestGetItem.lastChestLumberIdx != -1 && CIUI.currentChestGetItem.chestInventoryDatas[CIUI.currentChestGetItem.lastChestLumberIdx].ItemCount < 10)
                    {
                        addIdx = CIUI.currentChestGetItem.lastChestLumberIdx;
                    }
                    else
                    {
                        addIdx = CIUI.currentChestGetItem.firstChestEmptyIdx;
                    }
                }
                else if (inventoryType == "Pig")
                {
                    if (PGI.lastPigLumberIdx != -1 && PGI.InventoryDatas[PGI.lastPigLumberIdx].ItemCount < 10)
                    {
                        addIdx = PGI.lastPigLumberIdx;
                    }
                    else
                    {
                        addIdx = PGI.firstPigEmptyIdx;
                    }
                }
                break;
            case "Brick":
                if (inventoryType == "Chest")
                {
                    if (CIUI.currentChestGetItem.lastChestBrickIdx != -1 && CIUI.currentChestGetItem.chestInventoryDatas[CIUI.currentChestGetItem.lastChestBrickIdx].ItemCount < 10)
                    {
                        addIdx = CIUI.currentChestGetItem.lastChestBrickIdx;
                    }
                    else
                    {
                        addIdx = CIUI.currentChestGetItem.firstChestEmptyIdx;
                    }
                }
                else if (inventoryType == "Pig")
                {
                    if (PGI.lastPigBrickIdx != -1 && PGI.InventoryDatas[PGI.lastPigBrickIdx].ItemCount < 10)
                    {
                        addIdx = PGI.lastPigBrickIdx;
                    }
                    else
                    {
                        addIdx = PGI.firstPigEmptyIdx;
                    }
                }
                break;
            case "SlingShot":
                if (inventoryType == "Chest")
                {
                    if (CIUI.currentChestGetItem.lastChestSlingShotIdx != -1 && CIUI.currentChestGetItem.chestInventoryDatas[CIUI.currentChestGetItem.lastChestSlingShotIdx].ItemCount < 10)
                    {
                        addIdx = CIUI.currentChestGetItem.lastChestSlingShotIdx;
                    }
                    else
                    {
                        addIdx = CIUI.currentChestGetItem.firstChestEmptyIdx;
                    }
                }
                else if (inventoryType == "Pig")
                {
                    if (PGI.lastPigSlingshotIdx != -1 && PGI.InventoryDatas[PGI.lastPigSlingshotIdx].ItemCount < 10)
                    {
                        addIdx = PGI.lastPigSlingshotIdx;
                    }
                    else
                    {
                        addIdx = PGI.firstPigEmptyIdx;
                    }
                }
                break;
            case "Umbrella":
                if (inventoryType == "Chest")
                {
                    if (CIUI.currentChestGetItem.lastChestUmbrellaIdx != -1 && CIUI.currentChestGetItem.chestInventoryDatas[CIUI.currentChestGetItem.lastChestUmbrellaIdx].ItemCount < 10)
                    {
                        addIdx = CIUI.currentChestGetItem.lastChestUmbrellaIdx;
                    }
                    else
                    {
                        addIdx = CIUI.currentChestGetItem.firstChestEmptyIdx;
                    }
                }
                else if (inventoryType == "Pig")
                {
                    if (PGI.lastPigUmbrellaIdx != -1 && PGI.InventoryDatas[PGI.lastPigUmbrellaIdx].ItemCount < 10)
                    {
                        addIdx = PGI.lastPigUmbrellaIdx;
                    }
                    else
                    {
                        addIdx = PGI.firstPigEmptyIdx;
                    }
                }
                break;
            case "Tiger":
                if (inventoryType == "Chest")
                {
                    if (CIUI.currentChestGetItem.lastChestTigerIdx != -1 && CIUI.currentChestGetItem.chestInventoryDatas[CIUI.currentChestGetItem.lastChestTigerIdx].ItemCount < 10)
                    {
                        addIdx = CIUI.currentChestGetItem.lastChestTigerIdx;
                    }
                    else
                    {
                        addIdx = CIUI.currentChestGetItem.firstChestEmptyIdx;
                    }
                }
                else if (inventoryType == "Pig")
                {
                    if (PGI.lastPigUmbrellaIdx != -1 && PGI.InventoryDatas[PGI.lastPigTigerIdx].ItemCount < 10)
                    {
                        addIdx = PGI.lastPigTigerIdx;
                    }
                    else
                    {
                        addIdx = PGI.firstPigEmptyIdx;
                    }
                }
                break;
            case "Axe":
            case "Pick":
            case "Sickle":
            case "Shoes":
                return -2; // 넣을 수 없는 오브젝트의 경우 -2 리턴.
        }
        return addIdx;
    }
}
