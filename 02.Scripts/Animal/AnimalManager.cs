using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviourPunCallbacks
{
    //// 특정 플레이어의 Animal Manager만 SetActive(true)가 되도록 한다.
    //// 테스트 용
    //int BearCount = 10;
    //PhotonView PV;
    //GameObject[] BearObjects;
    //int numberOfBearObjects;
    //int sequenceIndex = 0;

    //float DefaultRespawnTime = 4f;
    //float RespawnTime = 3;

    //Transform[] RespawnTransformList;
    //List<List<GameObject>> objectPools;
    //Dictionary<string, int> animalCodeDict;

    GameObject[] ChickenObjects;
    RespawnTimer[] ChickenRespawnTimer;

    GameObject[] GoatObjects;
    RespawnTimer[] GoatRespawnTimer;

    GameObject[] CowObjects;
    RespawnTimer[] CowRespawnTimer;

    private void Awake()
    {
        //InitializePool();
        //// 곰 프리펩 생성, 곰 포지션 추가, 10개
        //CreateObjects("BearPrefab", "RespawnBearPositionList", sequenceIndex, 10);
        ////FindMyPhotonView();

        //ChickenObjects = CreateAnimalObjects("BearPrefab", "Chicken", 4f);
        //GoatObjects = CreateAnimalObjects("BearPrefab", "Goat", 4f);
        //CowObjects = CreateAnimalObjects("BearPrefab", "Cow", 4f);
    }

    void Update()
    {
        //CheckRespawnTime();

        //if (ChickenObjects != null && ChickenRespawnTimer != null)
        //{
        //    FarmingObjectRespawnCheck(ChickenObjects, ChickenRespawnTimer);
        //}
        //if (GoatObjects != null && GoatRespawnTimer != null)
        //{
        //    FarmingObjectRespawnCheck(GoatObjects, GoatRespawnTimer);
        //}
        //if (CowObjects != null && CowRespawnTimer != null)
        //{
        //    FarmingObjectRespawnCheck(CowObjects, CowRespawnTimer);
        //}
    }

    //// 동물들 생성 주기는 항상 같음
    //public void CheckRespawnTime()
    //{
    //    if (RespawnTime > 0f)
    //    {
    //        RespawnTime -= Time.deltaTime;
    //    }
    //    else
    //    {
    //        Debug.Log("동물들 리스폰!");
    //        RespawnTime = DefaultRespawnTime;
    //        // 나중에 동물별로 시간을 다르게 리스폰 할 수도 있으므로 분리를 미리 해둠
    //        foreach (var idx in animalCodeDict)
    //        {
    //            int animalCode = idx.Value;
    //            RespawnObject(animalCode);
    //        }
    //    }

    //}

    //// 특정 object들에 접근해서 전부 리스폰 하는 함수
    //public void RespawnObject(int animalCode)
    //{
    //    int animalCount = objectPools[animalCode].Count;
    //    for (int i = 0; i < animalCount; i++)
    //    {
    //        if (!objectPools[animalCode][i].activeSelf)
    //        {
    //            objectPools[animalCode][i].SetActive(true);
    //        }
    //    }
    //}

    //public void InitializePool()
    //{

    //    objectPools = new List<List<GameObject>>();
    //    animalCodeDict = new Dictionary<string, int>();
    //}



    //public void CreateObjects(string animal, string animalPosition, int aCode, int animalCount)
    //{
    //    GetRespawnPosition(animalPosition);
    //    // List를 새로 추가
    //    objectPools.Add(new List<GameObject>());
    //    animalCodeDict.Add(animal, aCode);
    //    for (int idx = 0; idx < animalCount; idx++)
    //    {
    //        GameObject ppp = PhotonNetwork.Instantiate(animal, RespawnTransformList[idx].position, RespawnTransformList[idx].rotation, 0);
    //        objectPools[aCode].Add(ppp);
    //    }
    //    sequenceIndex += 1;
    //}

    //// 리스폰 위치 할당
    //public void GetRespawnPosition(string animalPosition)
    //{
    //    GameObject RespawnPositionList = GameObject.Find(animalPosition);
    //    RespawnTransformList = RespawnPositionList.GetComponentsInChildren<Transform>();
    //}

    //// 사용안함
    //public void FindMyPhotonView()
    //{
    //    GameObject[] PVList = GameObject.FindGameObjectsWithTag("Player");
    //    foreach (GameObject player in PVList)
    //    {
    //        PhotonView playerPV = player.GetComponent<PhotonView>();
    //        if (playerPV.IsMine)
    //        {
    //            PV = playerPV;
    //            break;
    //        }
    //    }
    //}


    // Start is called before the first frame update
    void Start()
    {

    }


    //if (animalCodeDict.ContainsKey("Cat"))
    //{
    //    int c = animalCodeDict["Cat"];
    //// 여기에서 c를 사용할 수 있음
    //}


    //public void RespawnBear()
    //{
    //    BearObjects = GameObject.FindGameObjectsWithTag("Bear");
    //    if (BearObjects == null)
    //    {
    //        Debug.Log("비어있음");
    //    }
    //    numberOfBearObjects = BearObjects.Length;
    //    Debug.Log("몇마ㅣ린데?" + numberOfBearObjects);
    //    if (numberOfBearObjects < BearCount)
    //    {
    //        for (int i = numberOfBearObjects; i < BearCount; i++)
    //        {
    //            // 부족하면 RespawnTransform에 저장된 정보를 바탕으로 생성한다.
    //            //PhotonNetwork.Instantiate("BearPreFabs", RespawnTransformList[i].position, Quaternion.identity);
    //            if (RespawnTransformList == null)
    //            {
    //                GetRespawnPosition();
    //            }
    //            // 생성이 아니라 SetActive True로 변경하기
    //            PhotonNetwork.Instantiate("BearPreFabs", RespawnTransformList[i].position, RespawnTransformList[i].rotation, 0);
    //            //PhotonNetwork.Instantiate("BearPreFabs", new Vector3(0,0,0), Quaternion.Euler( new Vector3(0,0,0)),0);
    //        }
    //    }
    //    else
    //    {   // Object Pulling으로 해결
    //        foreach (GameObject bear in BearObjects)
    //        {
    //            if (!bear.activeSelf)
    //            {
    //                bear.SetActive(true);
    //            }
    //        }
    //    }
    //}


}
