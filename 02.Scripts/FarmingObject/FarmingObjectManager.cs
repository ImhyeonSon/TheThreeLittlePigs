using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class FarmingObjectManager : MonoBehaviourPunCallbacks
{
    GameObject[] TestObjects;
    RespawnTimer[] TestRespawnTimer;

    GameObject[] TreeObjects;
    RespawnTimer[] TreeRespawnTimer;

    GameObject[] MudObjects;
    RespawnTimer[] MudRespawnTimer;

    GameObject[] StoneObjects;
    RespawnTimer[] StoneRespawnTimer;

    GameObject[] RiceObjects;
    RespawnTimer[] RiceRespawnTimer;

    GameObject[] ChickenObjects;
    RespawnTimer[] ChickenRespawnTimer;
    AnimalStatus[] ChickenStatus;

    GameObject[] GoatObjects;
    RespawnTimer[] GoatRespawnTimer;
    AnimalStatus[] GoatStatus;

    GameObject[] CowObjects;
    RespawnTimer[] CowRespawnTimer;
    AnimalStatus[] CowStatus;

    public PhotonView PV;

    void Awake()
    {
        // 맵 시작시 파밍 오브젝트 생성
        PV = GetComponent<PhotonView>();
        
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            //(TestObjects, TestRespawnTimer) = CreateFarmingObjects("FarmingObjectTest", "Test", 4f);
            (TreeObjects, TreeRespawnTimer) = CreateFarmingObjects("Tree", "Tree", 60f);  // 앞에는 파밍 오브젝트 프리팹 이름 적어줘야 함. (resources 폴더 안에 위치)    
            (MudObjects, MudRespawnTimer) = CreateFarmingObjects("Mud", "Mud", 60f);
            (StoneObjects, StoneRespawnTimer) = CreateFarmingObjects("Stone", "Stone", 60f);
            (RiceObjects, RiceRespawnTimer) = CreateFarmingObjects("Rice", "Rice", 60f);

            (ChickenObjects, ChickenRespawnTimer) = CreateFarmingObjects("Chicken", "Chicken", 15f);
            ChickenStatus = GetObjectAnimalStatus(ChickenObjects);
            (GoatObjects, GoatRespawnTimer) = CreateFarmingObjects("Goat", "Goat", 40f); // 리스폰 체크
            GoatStatus = GetObjectAnimalStatus(GoatObjects);
            (CowObjects, CowRespawnTimer) = CreateFarmingObjects("Cow", "Cow", 60f);
            CowStatus = GetObjectAnimalStatus(CowObjects);
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
    }
    void Update()  // 타이머 체크
    {
        if (PhotonNetwork.IsMasterClient && PV.IsMine)
        {
            //if (TestObjects != null && TestRespawnTimer != null)
            //{
            //    FarmingObjectRespawnCheck(TestObjects, TestRespawnTimer);
            //}

            if (TreeObjects != null && TreeRespawnTimer != null)
            {
                FarmingObjectRespawnCheck(TreeObjects, TreeRespawnTimer);
            }
            if (MudObjects != null && MudRespawnTimer != null)
            {
                FarmingObjectRespawnCheck(MudObjects, MudRespawnTimer);
            }
            if (StoneObjects != null && StoneRespawnTimer != null)
            {
                FarmingObjectRespawnCheck(StoneObjects, StoneRespawnTimer);
            }
            if (RiceObjects != null && RiceRespawnTimer != null)
            {
                FarmingObjectRespawnCheck(RiceObjects, RiceRespawnTimer);
            }

            if (ChickenObjects != null && ChickenRespawnTimer != null)
            {
                FarmingObjectRespawnCheck(ChickenObjects, ChickenRespawnTimer);
            }
            if (GoatObjects != null && GoatRespawnTimer != null)
            {
                FarmingObjectRespawnCheck(GoatObjects, GoatRespawnTimer);
            }
            if (CowObjects != null && CowRespawnTimer != null)
            {
                FarmingObjectRespawnCheck(CowObjects, CowRespawnTimer);
            }
        }
    }

    public (GameObject[] FarmingObjects, RespawnTimer[] FarmingObjectRespawnTimer) CreateFarmingObjects(string farmingObjectName, string farmingObjectPosition, float defaultRespawnTime)  // 맵에 파밍 오브젝트를 설치하는 함수
    {
        if (farmingObjectName == "Mud")
        {
            Debug.Log("진흙진흙진흙");
        }
        GameObject[] RespawnPosition = GetRespawnPosition(farmingObjectPosition);  // 위치를 가져와 리스트에 저장하고

        // 파밍 오브젝트 관련 리스트 초기화 // 왜인지 모르겠지만 부모 오브젝트까지 리스트에 포함되므로 하나 빼줌
        GameObject[] FarmingObjects = new GameObject[RespawnPosition.Length - 1];
        RespawnTimer[] FarmingObjectRespawnTimer = new RespawnTimer[RespawnPosition.Length - 1];

        // test용 코드

        //GameObject[] FarmingObjects = new GameObject[1];
        //RespawnTimer[] FarmingObjectRespawnTimer = new RespawnTimer[1];
        //for (int idx = 1; idx < 2; idx++)  // 리스트를 돌면서 위치마다 파밍 오브젝트 설치하고 리스트에 저장

        for (int idx = 1; idx < RespawnPosition.Length; idx++)  // 리스트를 돌면서 위치마다 파밍 오브젝트 설치하고 리스트에 저장
        {
            GameObject farmingObject = PhotonNetwork.Instantiate(farmingObjectName, RespawnPosition[idx].transform.position, RespawnPosition[idx].transform.rotation, 0);
            FarmingObjects[idx - 1] = farmingObject;
            FarmingObjectRespawnTimer[idx - 1] = new RespawnTimer(defaultRespawnTime);  // 타이머 생성해서 리스트에 저장
        }

        return (FarmingObjects, FarmingObjectRespawnTimer);
    }
    // FarmingObject PV를 미리 저장/
    public AnimalStatus[] GetObjectAnimalStatus(GameObject[] FarmingObjects)
    {
        AnimalStatus[] returnStatus = new AnimalStatus[FarmingObjects.Length];
        for (int idx = 0; idx < FarmingObjects.Length; idx++)  // 리스트를 돌면서 위치마다 파밍 오브젝트 설치하고 리스트에 저장
        {
            AnimalStatus farmingObject = FarmingObjects[idx].GetComponent<AnimalStatus>();
            returnStatus[idx] = farmingObject; // 타이머 생성해서 리스트에 저장
        }
        return returnStatus;
    }

    public void ObjectSyncronize()
    {
        for (int idx = 0; idx < ChickenObjects.Length; idx++)  // 리스트를 돌면서 위치마다 파밍 오브젝트 설치하고 리스트에 저장
        {
            ChickenStatus[idx].UseSyncPosition(); // 타이머 생성해서 리스트에 저장
        }
        for (int idx = 0; idx < GoatObjects.Length; idx++)  // 리스트를 돌면서 위치마다 파밍 오브젝트 설치하고 리스트에 저장
        {
            GoatStatus[idx].UseSyncPosition(); // 타이머 생성해서 리스트에 저장
        }
        for (int idx = 0; idx < CowObjects.Length; idx++)  // 리스트를 돌면서 위치마다 파밍 오브젝트 설치하고 리스트에 저장
        {
            CowStatus[idx].UseSyncPosition(); // 타이머 생성해서 리스트에 저장
        }
    }

    public GameObject[] GetRespawnPosition(string farmingObjectPosition)  // 미리 설정해둔 파밍 오브젝트의 위치를 가져와 리스트를 반환하는 함수
    {
        GameObject RespawnPositionList = GameObject.Find(farmingObjectPosition);
        return RespawnPositionList.GetComponentsInChildren<Transform>().Select(t => t.gameObject).ToArray();
    }

    public void FarmingObjectRespawnCheck(GameObject[] FarmingObjects, RespawnTimer[] FarmingObjectRespawnTimer)  // 파밍 오브젝트의 활성화 상태에 따라 타이머를 처리하는 함수
    {
        //Debug.Log("파밍 오브젝트 리스폰 체크");
        for (int i = 0; i < FarmingObjects.Length; i++)
        {
            if (!FarmingObjects[i].activeSelf)  // 파밍 오브젝트가 활성화되어있지 않다면
            {
                if (FarmingObjectRespawnTimer[i].CheckRespawnTime(PV, FarmingObjects[i]))  // 리스폰 시간이 다 흘렀다면
                {
                    PV.RPC("FarmingObjectSetActiveRPC", RpcTarget.All, FarmingObjects[i].GetComponent<PhotonView>().ViewID, true);
                }
            }
        }
    }

    [PunRPC]
    public void FarmingObjectSetActiveRPC(int PVID, bool active)
    {
        PhotonView.Find(PVID).gameObject.SetActive(active);
    }

    //public void InitializeList(int listLength)  // 리스트 초기화 함수
    //{
    //    FarmingObjects = new GameObject[listLength];
    //    FarmingObjectRespawnTimer = new RespawnTimer[listLength];
    //}
}
