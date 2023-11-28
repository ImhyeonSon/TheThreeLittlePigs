using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UIElements;
using static PigEquipList;
// static UnityEditor.Progress;
//using UnityEditor.Search;

public class PigGetItem : MonoBehaviour
{

    //다른 스크립트 참조
    private PigInventoryUI PGUI;
    private PhotonView PV;
    private PigEquipList PEL;
    private GetItemProgressBar GPB;
    private PigStatus PS;
    public DurationBar DurationBar;
    private CheckItemIdx CII;

    //public CraftingManager CM;

    //애니메이션 관련
    private Animator anim;

    //소리이펙트
    public AudioSource GetTreeSFX;

    //알림 메시지 관련
    public FadeController FC;

    // 상호작용하는 아이템
    GameObject nearObject;
    // 아이템이 꽉 찼는지 확인
    public bool isFull;
    // 아이템창
    public List<InventoryData> InventoryDatas;
    // 아이템 종류
    public PigItem item;
    // 아이템 이미지
    public Sprite itemImage;
    // 입력창
    public bool InputE;
    // 현재 채집중인지 체크 (채집중이라면 이동이나 다른 액션이 불가능 하다)
    public bool isGettingItem = false;
    // 채집 쿨타임 변수
    public float getItemTimeFlow = 0f;

    // 아이템 이미지를 로드하는데 사용될 사전(dictionary)
    public Dictionary<PigItem.ItemType, string> itemSprites;

    // 파밍 오브젝트 레이어
    private int IngredientLayer = 1 << 13;

    // 인벤토리 관련 수정
    public int lastPigRiceIdx = -1;
    public int lastPigWoodIdx = -1;
    public int lastPigStoneIdx = -1;
    public int lastPigMudIdx = -1;
    public int lastPigSyringeIdx = -1;
    public int lastPigLumberIdx = -1;
    public int lastPigBrickIdx = -1;
    public int lastPigSlingshotIdx = -1;
    public int lastPigAxeIdx = -1;
    public int lastPigShovelIdx = -1;
    public int lastPigPickIdx = -1;
    public int lastPigSickleIdx = -1;
    public int lastPigShoesIdx = -1;
    public int lastPigUmbrellaIdx = -1;
    public int lastPigTigerIdx = -1;

    public int lastPigItemIdx = -1;
    public int firstPigEmptyIdx = 0;
    void Start()
    {
        // 인벤토리 UI 컴포넌트를 찾아온다
        PGUI = GetComponent<PigInventoryUI>();
        CII = GetComponent<CheckItemIdx>();
        //FC = GetComponent<>
        // 아이템 이미지 사전을 초기화한다
        itemSprites = new Dictionary<PigItem.ItemType, string>
        {
            { PigItem.ItemType.Rice, "Item/Images/Rice" },
            { PigItem.ItemType.Wood, "Item/Images/Wood" },
            { PigItem.ItemType.Stone, "Item/Images/Stone" },
            { PigItem.ItemType.Mud, "Item/Images/Mud" },
            { PigItem.ItemType.Syringe, "Item/Images/Syringe"},
            { PigItem.ItemType.Lumber, "Item/Images/Lumber" },
            { PigItem.ItemType.Brick, "Item/Images/Brick" },
            { PigItem.ItemType.SlingShot, "Item/Images/SlingShot" },
            { PigItem.ItemType.Axe, "Item/Images/Axe" },
            { PigItem.ItemType.Shovel, "Item/Images/Shovel" },
            { PigItem.ItemType.Pick, "Item/Images/Pick" },
            { PigItem.ItemType.Sickle, "Item/Images/Sickle" },
            { PigItem.ItemType.Shoes, "Item/Images/Shoes" },
            { PigItem.ItemType.Umbrella, "Item/Images/Umbrella" },
            { PigItem.ItemType.Tiger, "Item/Images/Tiger" },
        };
    }


    void Awake()
    {
        //알림 메시지 관련
        FC = GameObject.Find("Canvas").transform.Find("NotificationUI").GetComponent<FadeController>();

        // 포톤뷰를 초기화한다
        PV = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        PEL = GetComponent<PigEquipList>();
        GPB = GetComponent<GetItemProgressBar>();
        PS= GetComponent<PigStatus>();

        // 처음 시작했을 때 비어있는 인벤토리 12칸을 만든다
        InventoryDatas = new List<InventoryData>();
        for (int i = 0; i < 12; i++)
        {
            InventoryData inventoryData = new InventoryData
            {
                ItemName = null,
                ItemCount = 0,
                ItemImage = null,
            };
            InventoryDatas.Add(inventoryData);
        }
    }

    public void PigInput()
    {
        // 채팅중이 아니면 
        InputE = Input.GetKeyDown(KeyCode.E);
    }

    public void GetItem()
    {
        // 0. 채집을 하려면 현재 채집중이지 않아야 한다.
        if (InputE && !isGettingItem )
        {


            //SearchItem 동작확인용
            SearchItem("Rice");
            SearchItem("Mud");
            SearchItem("Stone");
            SearchItem("Wood");
            // 1. 가까이에 먹을 수  있는 오브젝트가 있으면
            if (nearObject != null)
            {
                // 2-1. 인벤토리가 꽉차지 않았으면
                if (!isFull)
                {
                    // 3. 여기서 아이템 종류에 따라서 다른 애니메이션을 실행하게할 분기가 필요할 듯

                    // 3-1 여기서 채집진행 불린형 변수를 변경
                    isGettingItem = true;
                    // 3-2 채집 쿨타임 설정
                    
                    getItemTimeFlow = 4.0f;
                    PEL.isSlingshotEquiped = false;
                    PV.RPC("UsePigItem", RpcTarget.All, PigItemType.NULL); // 새총을 들고 채집하는 버그 해결하기위해 장착해제 

                    item = nearObject.GetComponent<PigItem>();
                    switch (item.type)
                    {
                        case PigItem.ItemType.Rice:
                            {
                                PV.RPC("PigGetItemAnimaionRPC", RpcTarget.All, isGettingItem); //애니메이션 동기화
                                //인벤토리에 해당 도구 아이템이 있는지 검사하는 로직
                                if (SearchItem("Sickle"))
                                {
                                    PV.RPC("UsePigItem", RpcTarget.All, PigItemType.SICKLE);  // 낫은 채취 이벤트일때만 활성화 시킵니다.
                                }
                                DurationBar.SetDurBar(PEL.SelectedPigItem == PigItemType.NULL ? 4.0f : 2.0f);
                                FC.StartFadeIn("벼를 채집중입니다.");
                                PV.RPC("GetTreeAudioPlay", RpcTarget.All, true);
                                break;
                            }
                        case PigItem.ItemType.Wood:
                            {
                                PV.RPC("PigGetItemAnimaionRPC", RpcTarget.All, isGettingItem); //애니메이션 동기화
                                //인벤토리에 해당 도구 아이템이 있는지 검사하는 로직
                                if (SearchItem("Axe"))
                                {
                                    PV.RPC("UsePigItem", RpcTarget.All, PigItemType.AXE);  // 도끼는 채취 이벤트일때만 활성화 시킵니다.
                                    Debug.Log("Tree 탐색을 하는데에 성공 하나요?");
                                }
                                DurationBar.SetDurBar(PEL.SelectedPigItem == PigItemType.NULL ? 4.0f : 2.0f);
                                FC.StartFadeIn("나무를 채집중입니다.");
                                PV.RPC("GetTreeAudioPlay", RpcTarget.All, true);
                                break;
                            }
                        case PigItem.ItemType.Stone:
                            {
                                PV.RPC("PigGetItemAnimaionRPC", RpcTarget.All, isGettingItem); //애니메이션 동기화
                                //인벤토리에 해당 도구 아이템이 있는지 검사하는 로직
                                if (SearchItem("Pick"))
                                {
                                    PV.RPC("UsePigItem", RpcTarget.All, PigItemType.PICKAXE);  // 곡괭이는 채취 이벤트일때만 활성화 시킵니다.
                                }
                                DurationBar.SetDurBar(PEL.SelectedPigItem == PigItemType.NULL ? 4.0f : 2.0f);
                                FC.StartFadeIn("돌을 채집중입니다.");
                                PV.RPC("GetTreeAudioPlay", RpcTarget.All, true);
                                break;
                            }
                        case PigItem.ItemType.Mud:
                            {
                                PV.RPC("PigGetItemAnimaionRPC", RpcTarget.All, isGettingItem); //애니메이션 동기화
                                //인벤토리에 해당 도구 아이템이 있는지 검사하는 로직
                                if (SearchItem("Shovel"))//삽이 없어요
                                {
                                    PV.RPC("UsePigItem", RpcTarget.All, PigItemType.SHOVEL);  // 삽은 채취 이벤트일때만 활성화 시킵니다.
                                }
                                DurationBar.SetDurBar(PEL.SelectedPigItem == PigItemType.NULL ? 4.0f : 2.0f);
                                FC.StartFadeIn("진흙을 채집중입니다.");
                                PV.RPC("GetTreeAudioPlay", RpcTarget.All, true);
                                break;
                            }
                        case PigItem.ItemType.Leather:
                            {
                                break;
                            }
                        case PigItem.ItemType.Syringe:
                            {
                                break;
                            }
                    }
                    DecideAddItemIdx(nearObject.name);
                }
                // 2-2. 인벤토리가 꽉 찼으면
                else
                {
                    Debug.Log("아이템창이 꽉 찼습니다!");
                    FC.StartFadeIn("아이템창이 꽉 찼습니다!");
                }
                nearObject = null;
                item.DestroyGetItemObject(PEL.SelectedPigItem.ToString()); // 해당 오브젝트를 비활성화 합니다.
            }            
        }
    }

/*    // 가까이에 들어온 Item이 trigger에 반응하면 nearObject를 변경
    void OnTriggerStay(Collider other)  
    {
        if (other.tag == "Ingredients")
        {
            nearObject = other.gameObject;
        }
    }

    // trigger 범위 밖으로 나가면 nearObject를 null로 변경
    void OnTriggerExit(Collider other)
    {   
        if (other.tag == "Ingredients")
        {
            nearObject = null;
        }
    }*/

    public void CheckFarmingObject()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.5f, IngredientLayer);
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Ingredients"))
                {
                    nearObject = hitCollider.gameObject;
                }
            }
        }
        else
        {
            nearObject = null;
        }
    }

    // 채집 쿨타임 체크 
    public void CheckGetItemCooltime()
    {
        float checkTime;
        checkTime = PEL.SelectedPigItem == PigItemType.NULL ? 0f : 2f; // 무기장착여부에 따라 소요시간이 바뀐다

        if (isGettingItem) // true로 변했다면
        {
            getItemTimeFlow -= Time.deltaTime;
            if (getItemTimeFlow <= checkTime)
            {
                isGettingItem = false;
                PV.RPC("PigGetItemAnimaionRPC", RpcTarget.All, isGettingItem); //애니메이션 동기화
                PV.RPC("UsePigItem", RpcTarget.All, PigItemType.NULL); // 사용했던 아이템을 다시 비활성화
                PGUI.UpdateInventoryUI(); // 모든 변동사항이 반영되고 나서 인벤토리 ui를 갱신 
                PV.RPC("GetTreeAudioPlay", RpcTarget.All, false);
            }

        }
    }

    [PunRPC]
    public void PigGetItemAnimaionRPC(bool value)
    {
        anim.SetBool("isMining",value);
    }



    // 인벤토리의 현재 상태를 콘솔에 출력하는 메서드
    public void PrintInventory()
    {
        Debug.Log("=== 인벤토리 상태 ===");
        for (int i = 0; i < InventoryDatas.Count; i++)
        {
            string itemName = InventoryDatas[i].ItemName ?? "비어있음";
            int itemCount = InventoryDatas[i].ItemCount;
            string itemImage = InventoryDatas[i].ItemImage;
            Debug.Log($"슬롯 {i + 1}: 아이템명 - {itemName}, 수량 - {itemCount} , 이미지 경로 - {itemImage}");
        }
    }

    // 인벤토리에 현재 인자로 받은 아이템이 존재하는지 검사하는 메소드

    public bool SearchItem(string value)
    {
        bool isExist = false;
        for (int i = 0; i < InventoryDatas.Count; i++)
        {
            if (value == InventoryDatas[i].ItemName)
            {
                isExist = true;
                Debug.Log($"인벤토리에 아이템 {value}가 존재 합니다");
            }
        }
        return isExist;
    }

    // 아이템 추가할 때 인덱스
    public void DecideAddItemIdx(string farmingObjectName)
    {
        int addIdx = -1; // 물건 더해줄 인덱스
        switch (farmingObjectName)
        {
            case "Tree(Clone)":
                if (lastPigWoodIdx != -1 && InventoryDatas[lastPigWoodIdx].ItemCount != 10)
                {
                    addIdx = lastPigWoodIdx;
                    break;
                } else
                {
                    addIdx = firstPigEmptyIdx;
                    break;
                }
            case "Mud(Clone)":
                if (lastPigMudIdx != -1 && InventoryDatas[lastPigMudIdx].ItemCount != 10)
                {
                    addIdx = lastPigMudIdx;
                    break;
                } else
                {
                    addIdx = firstPigEmptyIdx;
                    break;
                }
            case "Stone(Clone)":
                if (lastPigStoneIdx != -1 && InventoryDatas[lastPigStoneIdx].ItemCount != 10)
                {
                    addIdx = lastPigStoneIdx;
                    break;
                } else
                {
                    addIdx = firstPigEmptyIdx;
                    break;
                }
            case "Rice(Clone)":
                if (lastPigRiceIdx != -1 && InventoryDatas[lastPigRiceIdx].ItemCount != 10)
                {
                    addIdx = lastPigRiceIdx;
                    break;
                } else
                {
                    addIdx = firstPigEmptyIdx;
                    break;
                }
            case "Syringe":
                if (lastPigSyringeIdx != -1 && InventoryDatas[lastPigSyringeIdx].ItemCount != 10)
                {
                    addIdx = lastPigSyringeIdx;
                    break;
                } else
                {
                    addIdx = firstPigEmptyIdx;
                    break;
                }
        }
        Debug.Log(addIdx + "애드아이디엑스");
        if (addIdx != 12 && addIdx != -1)
        {
            AddFarmingObject(farmingObjectName, addIdx);
        }
        else
        {
            FC.StartFadeIn("인벤토리가 다 찼습니다!");
            Debug.Log("파밍 중 추가할 슬롯이 없어요~");
        }
    }

    void AddFarmingObject(string farmingObjectName, int addIdx)
    {
        Debug.Log("추가할 아이템은, " + farmingObjectName + " 인덱스는, " + addIdx);

        if (InventoryDatas[addIdx].ItemName != null) // 슬롯이 비워지지 않은 경우,
        {
            InventoryDatas[addIdx].ItemCount++;
        }
        else
        {
            InventoryDatas[addIdx].ItemName = farmingObjectName;
            InventoryDatas[addIdx].ItemCount = 1;

            string spritePath = null;

            // 파밍 오브젝트가 아닌 경우와 파밍 오브젝트인 경우 다른 로직으로 처리
            if (farmingObjectName == "Syringe")
            {
                PigItem.ItemType? itemType = ConvertStringToEnum(farmingObjectName);
                if (itemType.HasValue && itemSprites.TryGetValue(itemType.Value, out spritePath))
                {
                    InventoryDatas[addIdx].ItemImage = spritePath;
                }
            }
            else
            {
                if (item.type != null && itemSprites.TryGetValue(item.type, out spritePath))
                {
                    InventoryDatas[addIdx].ItemImage = spritePath;
                }
            }
            CII.CheckIndex("Pig", InventoryDatas);
            CII.CheckEmptySlot("Pig");
        }
    }


    // 정원
    // 인벤토리에 채집외의 경로의 아이템을 추가하는 로직 
    public void AddItem(string name)
    {
        if (!isFull)
        {
            //인벤토리 리스트를 전부 순회하면서
            for (int i = 0; i < InventoryDatas.Count; i++)
            {
                // 아이템명이 null이 아니라면
                if (InventoryDatas[i].ItemName != null)
                {
                    // 인벤토리의 아이템과 획득한 아이템의 이름이 일치하고 아이템의 개수가 10개보다 작다면
                    if (InventoryDatas[i].ItemName == name && InventoryDatas[i].ItemCount < 10)
                    {
                        InventoryDatas[i].ItemCount++;

                        break;
                    }
                }
                //아이템명이 null이라면 새롭게 생성
                else
                {
                    InventoryDatas[i].ItemName = name;
                    InventoryDatas[i].ItemCount = 1;
                    // name을 기반으로 아이템 열거형 값을 가져옴
                    if (Enum.TryParse<PigItem.ItemType>(name, out PigItem.ItemType itemType))
                    {
                        // 이제 itemType을 사용하여 아이템의 열거형 값을 얻을 수 있습니다.
                        // 이미지의 이름을 아이템의 열거형 값으로 사용하여 sprite를 찾습니다.
                        if (itemSprites.TryGetValue(itemType, out string spritePath))
                        {
                            // 찾았다면, 스프라이트 경로를 InventoryData에 할당합니다.
                            InventoryDatas[i].ItemImage = spritePath;
                        }
                        break;
                    }
                }
                CII.CheckIndex("Pig", InventoryDatas);
                //CII.UpdateSpecificIndex("Pig", InventoryDatas[i].ItemName, i);
            }
            // for문 끝난 후 마지막 인덱스 업데이트 
            PGUI.UpdateInventoryUI(); // 모든 변동사항이 반영되고 나서 인벤토리 ui를 갱신 

        }
        //  인벤토리가 꽉 찼으면
        else
        {
            FC.StartFadeIn("아이템창이 꽉 찼습니다!");
            //Debug.Log("아이템창이 꽉 찼습니다!");
        }
    }


    // 인벤토리에서 특정 아이템의 개수를 1 줄이는 함수
    public void DeleteItem(string name)
    {
        CII.PrintIndicesInfo("Pig");
        Debug.Log("슬롯 줄이기 전");

        int checkIdx = -1;
        switch (name)
        {
            case "SlingShot":
                checkIdx = lastPigSlingshotIdx; break;
            case "Umbrella":
                checkIdx = lastPigUmbrellaIdx; break;
            case "Tiger":
                checkIdx = lastPigTigerIdx; break;
            case "Rice(Clone)":
                checkIdx = lastPigRiceIdx; break;
            case "Tree(Clone)":
                checkIdx = lastPigWoodIdx; break;
            case "Mud(Clone)":
                checkIdx = lastPigMudIdx; break;
            case "Stone(Clone)":
                checkIdx = lastPigStoneIdx; break;
            case "Syringe":
                checkIdx = lastPigSyringeIdx; break;
        }
        if (InventoryDatas[checkIdx].ItemCount > 0)
        {
            InventoryDatas[checkIdx].ItemCount--;
        }
        if (InventoryDatas[checkIdx].ItemCount == 0) // 슬롯 초기화
        {
            InventoryDatas[checkIdx].ItemName = null;
            InventoryDatas[checkIdx].ItemImage = null;
            CII.CheckIndex("Pig", InventoryDatas);
            CII.CheckEmptySlot("Pig");
        }
        CII.PrintIndicesInfo("Pig");
        ////인벤토리 리스트를 전부 순회하면서
        //for (int i = 0; i < InventoryDatas.Count; i++)
        //{
        //    // 아이템명이 일치한다면
        //    if (InventoryDatas[i].ItemName == name)
        //    {
        //        //아이템의 개수가 1개이상이라면
        //        if (InventoryDatas[i].ItemCount >= 1)
        //        {
        //            InventoryDatas[i].ItemCount--;
        //            break;
        //        }
        //        //아이템의 개수가 1개라면
        //        else
        //        {
        //            InventoryDatas[i].ItemName = null;
        //            InventoryDatas[i].ItemCount = 0;
        //            InventoryDatas[i].ItemImage = null;
        //            break;
        //        }
        //    }
        //    //CII.UpdateSpecificIndex("Pig", InventoryDatas[i].ItemName, i);
        //}
        // for문 끝난 후 마지막 인덱스 업데이트 
        PGUI.UpdateInventoryUI(); // 모든 변동사항이 반영되고 나서 인벤토리 ui를 갱신 
    }

    // 인벤토리에서 특정 아이템의 개수를 반환하는 함수
    public int CountItem(string name)
    {
        int cnt = 0;
        //인벤토리 리스트를 전부 순회하면서
        for (int i = 0; i < InventoryDatas.Count; i++)
        {
            // 아이템명이 일치한다면
            if (InventoryDatas[i].ItemName == name)
            {
                //아이템의 개수가 1개이상이라면
                if (InventoryDatas[i].ItemCount >= 1)
                {
                    cnt += InventoryDatas[i].ItemCount;
                }
            }
        }
        return cnt;
    }

    public PigItem.ItemType? ConvertStringToEnum(string name) // String을 Enum으로 바꿔주는 코드로, 파밍 오브젝트 외 다른 오브젝트의 경우(주사기, ...)에 사용
    {
        // Enum으로 변환 시도
        if (Enum.TryParse<PigItem.ItemType>(name, true, out PigItem.ItemType itemType))
        {
            return itemType; // 변환된 Enum 타입 반환
        }

        // 변환 실패 시 null 반환
        return null;
    }


    //치트키
    public void ItemCheet()
    {

 

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            AddItem("Sickle");
            AddItem("Axe");
            AddItem("Pick");
            AddItem("Shoes");
            PGUI.UpdateInventoryUI();
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            AddItem("SlingShot");
            AddItem("Tiger");
            AddItem("Umbrella");
            PGUI.UpdateInventoryUI();
        }
        


    }



    [PunRPC]
    public void GetTreeAudioPlay(bool TF)
    {
        if (TF)
        {
            GetTreeSFX.Play();
        }
        else
        {
            GetTreeSFX.Stop();
        }
    }
}
