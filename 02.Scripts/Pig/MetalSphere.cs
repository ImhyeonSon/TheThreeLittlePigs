using UnityEngine;
using Photon.Pun;

public class MetalSphere : MonoBehaviour
{

    private int attackLayer = 1 << 17 | 1 << 8; // 늑대와 땅, 동물 레이어를 지정하기 위한 LayerMask
    private int planeLayer = 1 << 7 | 1 << 13;
    private PhotonView PV;
    public GameObject boomEffect;
    private int animalHitDemage = 2;

    //소리이펙트
    public AudioSource BoomSFX;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        Destroy(gameObject, 2f);
    }

    private void Start()
    {
    }

    private void Update()
    {
        CheckWolfAnimal();
        CheckPlaneIngredient();
    }


    
    public void CheckWolfAnimal()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1.5f, attackLayer);
        foreach (var hitCollider in hitColliders)
        {
            
            if (hitCollider.CompareTag("Wolf"))
            {
                Debug.Log("늑대의 속도를 줄이는 메소드를 호출");
                hitCollider.GetComponent<PhotonView>().RPC("ReduceWolfSpeed", RpcTarget.All);
                PV.RPC("BoomAudioPlay", RpcTarget.All, true);
                boomEffect.SetActive(true);
            }
            else if (hitCollider.CompareTag("Chicken") || hitCollider.CompareTag("Goat") || hitCollider.CompareTag("Cow"))
            {
                //hitCollider.GetComponent<PhotonView>().RPC("ReduceWolfSpeed", RpcTarget.All);
                int animalDie = hitCollider.GetComponent<AnimalStatus>().TakeDamage(animalHitDemage);
                PV.RPC("BoomAudioPlay", RpcTarget.All, true);
                boomEffect.SetActive(true);
            }
            else
            {
                PV.RPC("BoomAudioPlay", RpcTarget.All, true);
                boomEffect.SetActive(true);
            }
        }   
    }

    public void CheckPlaneIngredient()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f, planeLayer);
        foreach (var hitCollider in hitColliders)
        {
            PV.RPC("BoomAudioPlay", RpcTarget.All, true);
            boomEffect.SetActive(true);
        }
    }


    //소리이펙트
    [PunRPC]
    public void BoomAudioPlay(bool TF)
    {
        if (TF && !BoomSFX.isPlaying)
        {
            BoomSFX.Play();
        }
        else if (!TF)
        {
            BoomSFX.Stop();
        }
    }

}
