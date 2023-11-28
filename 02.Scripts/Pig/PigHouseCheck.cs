using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigHouseCheck : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform HouseCheckTransfrom;
    private int houseCheckLayer= 1<<18;
    private PigStatus PS;
    private PigController PC;

    private void Awake()
    {
        PS = GetComponent<PigStatus>();
        PC = GetComponent<PigController>();
    }
    public void HouseCheck()
    {
        Collider[] hitColliders = Physics.OverlapSphere(HouseCheckTransfrom.position, 0.1f, houseCheckLayer);
        if (hitColliders.Length > 0)
        {
            PS.SetInHouse(true);
            // 마우스 트리거를 true로 바꾼다.
            PC.SetMouseTrigger(true);
            return;
        }
        PS.SetInHouse(false);
    }
}
