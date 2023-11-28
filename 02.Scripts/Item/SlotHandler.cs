using UnityEngine.EventSystems;
using UnityEngine;

public class SlotHandler : MonoBehaviour, IPointerClickHandler
{
    private PigInventoryUI PIUI;
    private ChestInventoryUI CIUI;
    private ManagerInventory MI;
    public int slotIdx; // 인스펙터에서 내가 넣어준 값

    private bool isChestActive = false;

    private void Start()
    {
        PIUI = transform.root.GetComponentInChildren<PigInventoryUI>();
        CIUI = transform.root.GetComponentInChildren<ChestInventoryUI>();
        MI = transform.root.GetComponentInChildren<ManagerInventory>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isChestActive = CIUI.activeChestInventory; // 클릭할 때마다 상자 활성화 여부 조회

        // 상자 인벤 활성화 및 우클릭 이벤트 발생시 (상자 상호작용 조건 충족)
        if (eventData.button == PointerEventData.InputButton.Right && isChestActive)
        {
            if (gameObject.tag == "ChestInventory")
            {
                MI.SlotClicked(slotIdx, "Chest");        
            }
            else if (gameObject.tag == "PigInventory")
            {
                MI.SlotClicked(slotIdx, "Pig");
            }
        }
    }
}
