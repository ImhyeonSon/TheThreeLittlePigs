using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PigInteractionCreateTable : MonoBehaviour
{
    int createTableLayer = 1 << 10; // 제작대 레이어
    private float interactionDistance = 2.0f;
    public GameObject craftingTableUI;
    bool activeCreateTable = false;
    private CraftingManager CM; // CraftingManager 참조 추가
    public GameObject inventoryUI;

    public Collider[] ObjCheck;

    private void Awake()
    {
        // CraftingTableUI의 자식 컴포넌트에서 CraftingManager를 찾습니다.
        CM = craftingTableUI.GetComponentInChildren<CraftingManager>();
    }
    public void CheckCreateTableInteractability(Vector3 pigPosition)
    {
        Collider[] craftingTablesInRange = Physics.OverlapSphere(pigPosition, interactionDistance, createTableLayer); // 제작대가 범위 안에 있는지 검사
        ObjCheck = craftingTablesInRange;

        if (craftingTablesInRange.Length > 0 && Input.GetKeyDown(KeyCode.E))
        {
            ToggleCreateTable();
            CM.SetUserMaterialCount();
            if (inventoryUI.activeSelf) // 인벤토리 UI가 활성화되어 있으면 비활성화합니다.
            {
                inventoryUI.SetActive(false);
            }
        }else if ((craftingTablesInRange.Length == 0 && activeCreateTable) || (activeCreateTable && Input.GetKeyDown(KeyCode.E)))
        {
            ToggleCreateTable();
        }
    }


    public void ToggleCreateTable()
    {
        activeCreateTable = !craftingTableUI.activeSelf; // 제작대 상태 토글
        craftingTableUI.SetActive(activeCreateTable); // UI 활성화/비활성화
    }

    public bool GetIsActiveCreateTable()
    {
        return craftingTableUI.activeSelf;
    }
}
