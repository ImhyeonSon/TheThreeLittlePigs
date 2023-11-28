using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PigEquipList;

public class PigAttack : MonoBehaviour
{

    //다른스크립트 참조
    private PhotonView PV; // 포톤 뷰
    private PigStatus PS;
    private PigGetItem PGI;
    private PigInventoryUI PIUI;

    //알림 메시지 관련
    public FadeController FC;


    //애니메이션 관련
    private Animator anim;


    //소리이펙트
    public AudioSource AttackSFX;
    public AudioSource HitSFX;
    public AudioSource NoticeSFX;

    // 기본공격 관련 변수
    public GameObject AttackRange;
    public bool isActiveAttacking = true;
    public float attackingTimeFlow = 0f;
    private int AttackLayer = 1 << 17 | 1 << 8;


    private void Awake()
    {
        // 포톤뷰 가져온다
        PV = GetComponent<PhotonView>();
        PS = GetComponent<PigStatus>();
        PGI = GetComponent<PigGetItem>();
        PIUI = GetComponent<PigInventoryUI>();
        //알림
        FC = GameObject.Find("Canvas").transform.Find("NotificationUI").GetComponent<FadeController>();
    }



    void Start()
    {
        anim = GetComponent<Animator>();
    }


    // 일반공격 
    public void UseAttack()
    {
        if (isActiveAttacking && Input.GetMouseButton(0))
        {

            attackingTimeFlow = 1.0f;
            Debug.Log("돼지 기본 공격");
            isActiveAttacking = false;
            PV.RPC("UsePigAttackAnimationRPC", RpcTarget.All,true);
            //anim.SetBool("isAttack", true);
            Attack();
            //소리이펙트
            PV.RPC("AttackAudioPlay",RpcTarget.All,true);
        }
    }

    public void Attack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(AttackRange.transform.position, 1.5f, AttackLayer);
        if (hitColliders.Length > 0)
        {
            foreach (var hitCollider in hitColliders)
            {

                if (hitCollider.CompareTag("Wolf")) 
                {
                    Debug.Log("돼지 공격 범위에서 늑대를 감지.");
                    PV.RPC("HitAudioPlay", RpcTarget.All, true); //소리
                    hitCollider.GetComponent<PhotonView>().RPC("ReceiveAttack", RpcTarget.All);

                }
                else if (hitCollider.CompareTag("Chicken") || hitCollider.CompareTag("Goat") || hitCollider.CompareTag("Cow")) // 야생동물 공격
                {
                    Debug.Log("돼지 공격범위에서 Animal 을 감지.");
                    PV.RPC("HitAudioPlay", RpcTarget.All, true); //소리
                    Debug.Log(hitCollider);
                    int animalDie = hitCollider.GetComponent<AnimalStatus>().TakeDamage(PS.GetDamage());
                    if (animalDie > 0 && Random.value < 0.5f)
                    {
                        Debug.Log("50% 확률로 아이템 획득을 실행했습니다");
                        NoticeSFX.Play(); // 소리;
                        FC.StartFadeIn("주사기를 획득했어요!");
                        PGI.DecideAddItemIdx("Syringe");
                        PIUI.UpdateInventoryUI();
                    }
                }
            }
        }
    }


    // 공격 쿨타임 체크 
    public void CheckAttackCooltime()
    {
        if (!isActiveAttacking)
        {
            attackingTimeFlow -= Time.deltaTime;
            if (attackingTimeFlow <= 0.7f)
            {
                PV.RPC("UsePigAttackAnimationRPC", RpcTarget.All, false);
                //anim.SetBool("isAttack", false);
            }

            if (attackingTimeFlow <= 0f)
            {
                isActiveAttacking = true;
            }
        }
    }

    [PunRPC]
    public void UsePigAttackAnimationRPC(bool TF)
    {
        anim.SetBool("isAttack", TF);
    }


    //소리이펙트
    [PunRPC]
    public void AttackAudioPlay(bool TF)
    {
        if (TF)
        {
            AttackSFX.Play();
        }
        else
        {
            AttackSFX.Stop();
        }
    }

    [PunRPC]
    public void HitAudioPlay(bool TF)
    {
        if (TF)
        {
            HitSFX.Play();
        }
        else
        {
            HitSFX.Stop();
        }
    }

    [PunRPC]
    public void NoticeAudioPlay(bool TF)
    {
        if (TF)
        {
            NoticeSFX.Play();
        }
        else
        {
            NoticeSFX.Stop();
        }
    }
}
