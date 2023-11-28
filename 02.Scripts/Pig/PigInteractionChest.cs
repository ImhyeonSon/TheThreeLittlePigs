using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using static PigInventoryUI;

// 상호작용 할 수 있는지 판별하고, 버튼 눌렀을 때 UI 띄우는 함수
public class PigInteractionChest : MonoBehaviour
{
    //알림 메시지 관련
    public FadeController FC;

    int chestLayer = 1 << 9; // 상자 레이어
    private float interactionDistance = 2.0f;
    public Collider[] chestsInRange; // 범위 내 상자
    public ChestGetItem CGI;
    private ChestInventoryUI CIUI;

    public event Action<ChestGetItem> OnChestGetItemUpdated;
    private int houseLayer = 1 << 18;


    private void Awake()
    {
        CIUI = GetComponent<ChestInventoryUI>();
        FC = GameObject.Find("Canvas").transform.Find("NotificationUI").GetComponent<FadeController>();

    }

    public void CheckChestInteractability(Vector3 pigPosition)
    {
        chestsInRange = Physics.OverlapSphere(pigPosition, interactionDistance, chestLayer);
        Collider[] houses = Physics.OverlapSphere(transform.position, 1f, houseLayer);
        if (houses.Length > 0 )
        {
            foreach (var house in houses)
            {
                PhotonView housePV = house.GetComponent<PhotonView>();

                if (!CIUI.activeChestInventory && chestsInRange.Length > 0 && Input.GetKeyDown(KeyCode.E))
                {
                    if (!housePV.IsMine)
                    {
                        FC.StartFadeIn("본인 집 내부의 상자만 열 수 있어요!");
                    }
                    else
                    {
                        ChestGetItem localCGI = chestsInRange[0].GetComponent<ChestGetItem>();
                        CIUI.SetCurrentChest(localCGI); // 현재 연 상자를 최신 상자로
                        localCGI.SetCIUI(GetComponent<ChestInventoryUI>(), GetComponent<CheckItemIdx>());

                        CIUI.ToggleInventoryUI(true);
                    }
                }
                else if ((chestsInRange.Length == 0 && CIUI.activeChestInventory) ||
                             (CIUI.activeChestInventory && Input.GetKeyDown(KeyCode.E)))
                {
                    CIUI.ToggleInventoryUI(false); // 인벤토리 UI 닫기
                    CIUI.SetCurrentChest(null);
                }
            }   
        }
    }
}
