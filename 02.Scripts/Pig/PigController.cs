using Highlands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PigController : PlayerController
{
    private PigStatus PST;
    private PigGetItem PGI;
    private PigSaveLife PSL;
    private PigInteractionCreateTable PICT;
    private PigInventoryUI PIUI;
    private ChestInventoryUI CIUI;
    private PigInteractionCraftingInfoUI PICUI;
    private PigKeyInfoUI PKIUI;
    // 시점 변경 트리거
    private bool mouseTrigger = false;

    public override void Awake()
    {
        base.Awake();
        PST = GetComponent<PigStatus>();
        PGI = GetComponent<PigGetItem>();
        PSL = GetComponent<PigSaveLife>();
        PICT = GetComponent<PigInteractionCreateTable>();
        PIUI = GetComponent<PigInventoryUI>();
        CIUI = GetComponent<ChestInventoryUI>();
        PICUI = GetComponent<PigInteractionCraftingInfoUI>();
        PKIUI = GetComponent <PigKeyInfoUI>();
    }
    public override void Start()
    {
        base.Start();
    }

    public override void CheckInput()
    {
        if (!PGI.isGettingItem && ! PSL.isSavingLife)
        {
            base.CheckInput();
        }
        else
        {
            base.inputH = 0;
            base.inputV = 0;
            base.inputJump = false;
        }
    }
    public override void GetMouseInput()
    {
        if (!PST.GetIsChatting() && !PICT.GetIsActiveCreateTable() && !PIUI.GetIsActiveInventory() && !PST.GetIsActiveMiniMap() && !CIUI.GetIsActiveChest()
            && !PICUI.GetIsOpenCraftingInfoUI() && !PKIUI.GetIsPigOpenKeyInfoUI())
        {
            base.GetMouseInput();
        }
        else
        {
            GetMouseZeroInput();
        }
    }


    
    

    // 집 밖인데, 마우스트리거가 true라면 3인칭으로 바꾸라는 뜻, 그 후 마우스 트리거를 false로 설정
    // 집 안이면, 마우스 트리거 상태와 관련x
    // 집 안으로 들어오면 마우스 트리거를 true로 바꾼다.
    // 집 밖일 때, 마우스 트리거가 false라면 상관 x
    public override void CameraZoom()
    {
        if (PST.GetInHouse())
        { // 집안에 있으면 항상 줌 인 상태
            MouseZoomIn();
        }
        else
        {
            if (mouseTrigger)
            {
                MouseZoomOut();
                SetMouseTrigger(false);
            }
            base.CameraZoom();  
        }
    }

    public void SetMouseTrigger(bool TF)
    {
        mouseTrigger = TF;
    }

}