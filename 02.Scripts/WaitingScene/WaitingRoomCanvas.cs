using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomCanvas : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject FailLackPersonPopup;
    public void FailLackPersonPopupClose()
    {
        FailLackPersonPopup.SetActive(false);
    }
}
