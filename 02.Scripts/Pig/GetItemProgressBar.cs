using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetItemProgressBar : MonoBehaviour
{
    private PhotonView PV; // 포톤 뷰
    public GameObject prfBuiltBar; // 유저들에게 보여질 건설 상태 바
    RectTransform builtBar;
    private Image curBuiltBar;
    

    void Start()
    {
        PV = GetComponent<PhotonView>();
        builtBar = Instantiate(prfBuiltBar, prfBuiltBar.transform).GetComponent<RectTransform>();
        curBuiltBar = builtBar.transform.GetChild(0).GetComponent<Image>();
    }

    public void ReadyToGetItem()
    {
        Debug.Log("채집 ReadyToGetItem 함수 호출");
        StartCoroutine(StartBuild());
        ShowBuildBar(); // 건설 상태 바를 보여주는 함수 호출
    }


    private void ShowBuildBar()
    {
        Debug.Log("채집 ShowBuildBar 함수 호출");
        builtBar.gameObject.SetActive(true); // 건설 상태 바 활성화
        curBuiltBar.fillAmount = 0; // 건설 상태 바의 진행도를 0으로 초기화
    }

    public IEnumerator StartBuild()
    {

        Debug.Log("채집 StartBuild 함수 호출");
        float buildDuration = 4f; // 맨손이라면 채집 시간을 4초로 설정
        float buildTime = 0; // 현재 채집 시간을 0으로 초기화

        

        // 진행 상태를 업데이트하는 루프
        while (buildTime < buildDuration)
        {
            buildTime += Time.deltaTime;
            curBuiltBar.fillAmount = buildTime / buildDuration; // 진행도 업데이트
            yield return null;
        }

        
        curBuiltBar.fillAmount = 1; // 건설 상태 바를 100%로 설정

        // 완료 후 상태 바 숨기기
        builtBar.gameObject.SetActive(false);

    }
}
