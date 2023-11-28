using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAllocate : MonoBehaviour
{
    private CanvasController canvasController;
    // 제작대를 위한 수동 GameObject 할당
    public GameObject DimedPanel=null;


    private void Awake()
    {
        canvasController = GameObject.Find("CanvasController").gameObject.GetComponent<CanvasController>();
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        //if (canvasController != null)
        //{
            // 캔버스를 키면 자동으로 CanvasManager에 등록
            canvasController.CanvasAllocate(gameObject);
        //}
    }

    void OnDisable()
    {
        if (DimedPanel != null)
        {
            DimedPanel.SetActive(false);
        }
        for (int idx = canvasController.canvases.Count-1; idx >= 0; idx--)
        {
            if (canvasController.canvases[idx] == gameObject)
            {
                canvasController.canvases.RemoveAt(idx);
            }
        }
    }

    // Update is called once per frame
    public void CanvasClose()// 혹시 몰라서 만들어둔 함수
    {
        gameObject.SetActive(false);   
    }
}
