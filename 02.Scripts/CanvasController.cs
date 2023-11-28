using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    // 양방향 List를 쓰면 더 빠르지만 크기가 크지 않으므로 그냥 둠
    public List<GameObject> canvases; // 순차적으로 끌 Canvas들의 배열
    //public GameObject[] canvases= new GameObject[10];

    //private int currentCanvasIndex = -1; // 현재 Canvas 인덱스

    private bool InputESC = false;
    void Update()
    {
        CanvasInput();
        CanvasClose();
    }
    public void CanvasInput()
    {
        InputESC = Input.GetKeyDown(KeyCode.Escape);
    }


    public void CanvasClose()
    {
        if (InputESC)
        {
            if (canvases.Count > 0)
            {
                // 현재 인덱스에 해당하는 Canvas를 끄기
                canvases[canvases.Count-1].SetActive(false);
                // 인덱스를 1 감소해 다음 캔버스를 가리킨다.           
            }
        }
    }
    // 캔버스가 다 꺼졌을 때, Esc 키를 누르면 게임 나가기 창이 뜨도록 변경

    public void CanvasAllocate(GameObject newCanvas)
    {
        canvases.Add(newCanvas);
        //canvases.Add(newCanvas);
    }

    // 꺼진 캔버스에서 자동으로 호출 해 사용
    public void CanvasRemove()
    {
        canvases.RemoveAt(canvases.Count - 1);
    }

}