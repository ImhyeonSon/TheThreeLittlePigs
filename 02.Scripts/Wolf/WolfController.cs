using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class WolfController : PlayerController
{
    private WolfStatus WS;
    private WolfSkillLearning WSLearning;
    private WolfKeyInfoUI WKIF;
    public override void Awake()
    {
        base.Awake();
        WS = GetComponent<WolfStatus>();
        WSLearning = GetComponent<WolfSkillLearning>();
        WKIF = GetComponent<WolfKeyInfoUI>();
    }
    public override void Start()
    {
        base.Start();
    }

    public override void CheckInput()
    {
        if (!WS.isWindBlowing)
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
        if (!WS.GetIsChatting() && !WS.GetIsActiveMiniMap() && !WSLearning.GetIsActiveSkillSelect() && !WKIF.GetIsWolfOpenKeyInfoUI())
        {
            base.GetMouseInput();
        }
        else
        {
            GetMouseZeroInput();
        }
    }

    public override void CameraZoom()
    {
        base.CameraZoom();
    }
}
