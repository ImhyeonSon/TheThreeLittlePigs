using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class HouseManager : MonoBehaviour
{
    private int maxLevel = 3;
    public int currentLevel = 1;

    public Transform[] longWalls;
    public Transform[] shortWalls;
    public Transform[] houseFoundations;
    public Transform[] houseRoofs;

    public Material[] longWallMaterials;
    public Material[] shortWallMaterials;
    public Material houseFoundationMaterial;
    public Material[] houseRoofMaterials;

    private Renderer[] longWallRenderers;
    private Renderer[] shortWallRenderers;
    private Renderer[] foundationRenderers;
    private Renderer[] roofRenderers;

    public GameObject upgradeParticle;

    public DurabilityManager durabilityManager;

    private PhotonView PV;
    private DurabilityManager DM;

    // 게임 승리 조건
    private PhotonGameManager PGM;
    public ChestHeirloom CH; // 퍼블릭으로 그대로 사용

    public GameObject prfBuiltBar; // 유저들에게 보여질 건설 상태 바
    public GameObject canvas;
    public RectTransform builtBar;
    private Image curBuiltBar;
    public Transform sitePosition; // 부지의 위치
    public float halfExtents; // 부지의 변의 길이의 절반

    bool isUpgrading = false;
    bool isUpable = false;

    private void Awake()
    {
        PGM = GameObject.Find("PhotonGameManager").GetComponent<PhotonGameManager>();
        PV = GetComponent<PhotonView>();
        DM = GetComponent<DurabilityManager>();
    }

    void Start()
    {
        longWallRenderers = InitializeRenderers(longWalls, longWallMaterials[0]);
        shortWallRenderers = InitializeRenderers(shortWalls, shortWallMaterials[0]);
        foundationRenderers = InitializeRenderers(houseFoundations, houseFoundationMaterial);
        roofRenderers = InitializeRenderers(houseRoofs, houseRoofMaterials[0]);
        durabilityManager.Initialize();
        //builtBar = Instantiate(prfBuiltBar, canvas.transform).GetComponent<RectTransform>();
        curBuiltBar = builtBar.transform.GetChild(0).GetComponent<Image>();
    }

    private void Update()
    {
        //HandleLevel();
        UpgradeableSite();
    }

    private void UpdateHouseInfo()
    {
        // 외형 업데이트
        if (currentLevel == 2)
        {
            UpdateMaterials(longWallRenderers, longWallMaterials[1]);
            UpdateMaterials(shortWallRenderers, shortWallMaterials[1]);
            UpdateMaterials(roofRenderers, houseRoofMaterials[1]);
        }
        else if (currentLevel == 3)
        {
            UpdateMaterials(longWallRenderers, longWallMaterials[2]);
            UpdateMaterials(shortWallRenderers, shortWallMaterials[2]);
            UpdateMaterials(roofRenderers, houseRoofMaterials[2]);
        }
    }
    
    void UpgradeableSite()
    {
        // 부지의 중심 위치와 크기를 설정
        Vector3 center = sitePosition.position;
        Vector3 size = new Vector3(halfExtents * 2, 8f, halfExtents * 2); // 부지의 너비, 높이, 길이

        // 돼지 플레이어가 있는 레이어 마스크 설정 (레이어 16)
        int pigLayer = 1 << 16;

        // Physics.OverlapBox를 사용하여 해당 영역 내에 돼지 플레이어가 있는지 확인
        Collider[] hitColliders = Physics.OverlapBox(center, size / 2, Quaternion.identity, pigLayer);

        if (hitColliders.Length > 0)
        {
            isUpable = true;
        }
        else
        {
            isUpable = false;
        }
    }

    [PunRPC]
    void ReadyToLevelUp()
    {
        //LevelUp();
        StartCoroutine(LevelUp());
        ShowBuildBar(); // 건설 상태 바를 보여주는 함수 호출
    }

    private IEnumerator LevelUp()
    {
        if (currentLevel < maxLevel)
        {
            isUpgrading = true; // 업그레이드 시작
            upgradeParticle.SetActive(true);

            float duration = currentLevel == 1 ? 10.0f : 13.0f; // 업그레이드 지속 시간
            float time = 0;

            while (time < duration)
            {
                time += Time.deltaTime;
                float normalizedTime = time / duration; // 0.0에서 1.0 사이의 진행도
                curBuiltBar.fillAmount = normalizedTime; // UI 바 업데이트
                yield return null;
            }

            currentLevel+=1;
            UpdateHouseInfo();
            Debug.Log("업그레이드는 끝났는데...");
            isUpgrading = false; // 업그레이드 완료
            upgradeParticle.SetActive(false);
            durabilityManager.UpdateStats(currentLevel);
            Debug.Log(DM.curDurability + "현재 내구도! 초기화 되어야 함!" + DM.maxDurability);

            DM.UpdateDurabilityBar();
            if (currentLevel == maxLevel)
            {
                if (CH.GetHaveHeirloom())
                { //벽돌집이 완성 됐을 때 가보가 들어있다면 승리
                    PGM.PigWin();
                    //PGM.PigWinRPC(); // RPC를 사용하지 않아도 됨.
                }
            }

            upgradeParticle.SetActive(false); // 업그레이드 완료시 이펙트 제거
            builtBar.gameObject.SetActive(false); // 업그레이드가 완료되면 상태 바 비활성화

        }
    }


    private void ShowBuildBar()
    {
        builtBar.gameObject.SetActive(true); // 건설 상태 바 활성화
        curBuiltBar.fillAmount = 0; // 건설 상태 바의 진행도를 0으로 초기화
        upgradeParticle.SetActive(true); // 업그레이드 이펙트 활성화
    }
    // 재질을 업데이트
    private void UpdateMaterials(Renderer[] renderers, Material material)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material = material;
        }
    }

    // Renderer 배열을 초기화
    private Renderer[] InitializeRenderers(Transform[] transforms, Material initialMaterial)
    {
        Renderer[] renderers = new Renderer[transforms.Length];
        for (int i = 0; i < transforms.Length; i++)
        {
            renderers[i] = transforms[i].GetComponent<Renderer>();
            renderers[i].material = initialMaterial;
        }
        return renderers;
    }

    public void IsHouseHaveHeirloom()
    {
        if (currentLevel == maxLevel && CH.GetHaveHeirloom()) // 벽돌집에 가보가 있다면
        {
            PGM.PigWin(); // 상위에서 RPC로 보내주기 때문에 PigWin으로 체크하면 됨.
        }
     }

}
