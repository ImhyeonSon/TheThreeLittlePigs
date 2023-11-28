using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HouseOpenDoor : MonoBehaviour
{
    public DoorOpen[] doors;
    public float interactionDistance = 5.0f;
    public AudioSource oepnAudioSource;
    public AudioSource closeAudioSource;
    private PhotonView PV;
    int pigLayer = 1 << 16; // 돼지 플레이어 레이어

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        doors = GetComponentsInChildren<DoorOpen>();
    }

    private void Update()
    {
        TryOpenDoor();
    }


    // 현재 나(플레이어)가 문에 가까운지 판별해주는 함수
    private Collider CheckIfPlayerIsNearAnyDoor()
    {
        foreach (var door in doors)
        {
            Collider[] hitColliders = Physics.OverlapSphere(door.transform.position, interactionDistance, pigLayer);
            foreach (var hitCollider in hitColliders)
            {
                // 현재 로컬 플레이어만 반환
                if (hitCollider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    return hitCollider;
                }
            }
        }
        return null;
    }

    // 플레이어가 가장 가까운 문을 열어주는 함수
    private void CheckAndOpenClosestDoor(Collider player)
    {
        float minDistance = float.MaxValue; // 최소 거리를 저장할 변수. 초기값은 무한대로 설정.
        DoorOpen closestDoor = null; // 가장 가까운 문을 저장할 변수.

        foreach (var door in doors)
        {
            float distance = Vector3.Distance(player.transform.position, door.transform.position); // 플레이어와 문 사이의 거리를 계산.
            if (distance < minDistance) // 만약 현재 계산한 거리가 이전 최소 거리보다 작으면
            {
                minDistance = distance; // 현재 거리를 최소 거리로 설정.
                closestDoor = door; // 현재 문을 가장 가까운 문으로 설정.
            }
        }

        if (closestDoor != null && minDistance < interactionDistance && !closestDoor.isOpening) // 가장 가까운 문이 있는지, 그리고 해당 문이 열리지 않았는지 검사.
        {
            PV.RPC("ReadyToOpenDoor", RpcTarget.All, closestDoor.name); // 문을 여는 RPC 함수 호출.
        }

    }

    [PunRPC]
    void ReadyToOpenDoor(string doorName)
    {
        DoorOpen door = FindDoorByName(doorName);
        if (door != null)
        {
            if (door.isOpen)
            {
                oepnAudioSource.Play();
            }
            else
            {
                closeAudioSource.Play();
            }
            StartCoroutine(RotateDoor(door, door.isOpen ? door.OpenRotation : door.OriginalRotation, door.isOpen ? door.OriginalRotation : door.OpenRotation));
        }
    }

    void TryOpenDoor()
    {
        if(Input.GetKeyDown(KeyCode.F)){
            Collider localPlayerCollider = CheckIfPlayerIsNearAnyDoor();
            if (localPlayerCollider != null)
            {
                CheckAndOpenClosestDoor(localPlayerCollider);
            }
        }
    }
    private DoorOpen FindDoorByName(string doorName)
    {
        foreach (var door in doors)
        {
            if (door.name == doorName)
            {
                return door;
            }
        }
        return null;
    }

    private IEnumerator RotateDoor(DoorOpen door, Quaternion from, Quaternion to)
    {
        yield return StartCoroutine(door.RotateDoor(from, to));

        //if (door.isOpen) => 컨벤션 지키기
        //    audioSource.PlayOneShot(openSound);
        //else
        //    audioSource.PlayOneShot(closeSound);
    }
}
