using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class WolfSkillRPC : MonoBehaviour
{

    // 실제로 사용할 스킬들은 여기서 정의해서 사용
    private PlayerCharacter character;
    private WolfStatus WS;
    private PlayerStatus PS;
    private PhotonView PV;
    private Animator anim;
    public GameObject AttackRange;
    public Transform WindBlowingTransform;
    public GameObject MeshObject; // 클로킹에 사용
    private Renderer meshRenderer; // 클로킹에 사용
    public GameObject ClockingAura; // 클로킹 시 발동

    // 대지의 울림
    public GameObject WolfShockEffect;

    // 소리 소스
    public AudioSource audioSource;
    public AudioClip wolfClawAudioClip;
    public AudioClip WolfClockingClip;

    public AudioSource WolfDashSFX;

    public ParticleSystem WolfAttackVFX;
    public AudioSource WolfAttackSFX;

    private int pigLayer = 1 << 16;

    public int AttackLayer = 1 << 16 | 1 << 18 | 1 << 8;
    private int UmbrellaLayer = 1 << 19;

    public float HowlingDurationTime = 12.0f; // Howling eye effect의 지속시간을 HowlingDurationTime - 3.5초로 설정... 인줄 알았지만 12초 일때 10초가 맞음

    private float windBlowingTimeFlow;

    // Wolf Skill Effect
    public Transform WolfClawRange;
    public GameObject[] WolfClawEffects; // WolfClawEffect안의 오브젝트를 할당
    Vector3 WolfClawBoxRange = new Vector3 (14.5f, 2.3f, 14.5f);

    // Wolf Clocking
    private bool isClocking = false;
    private float ClockingTime =0f;
    private float defaultClockingTime = 20f; // 클로킹 시간 20초


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        PV = GetComponent<PhotonView>();
        WS = GetComponent<WolfStatus>();
        PS = GetComponent<PlayerStatus>();
        anim = GetComponent<Animator>();
        character = GetComponent<PlayerCharacter>();
        meshRenderer = MeshObject.GetComponent<Renderer>();
    }

    void Update()
    {
        if (isClocking)
        {
            ClockingTimer();
        }    
    }

    private void OnDrawGizmos() //goundCheck 범위확인용
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(AttackRange.transform.position, 1.5f);


        Gizmos.color = Color.red;


        // Wind blow 범위 기즈모 체크
        //Vector3 a = WindBlowingTransform.localPosition;
        //Vector3 b = WindBlowingTransform.localPosition + 5 * Vector3.forward;
        //Vector3 c = WindBlowingTransform.parent.TransformPoint(a);
        //Vector3 d = WindBlowingTransform.parent.TransformPoint(b);

        //Vector3 capsuleStart = c;
        //Vector3 capsuleEnd = d;

        //// Capsule의 반지름 설정
        //float capsuleRadius = 2f;

        //// Capsule 그리기
        //Gizmos.DrawWireSphere(capsuleStart, capsuleRadius);
        //Gizmos.DrawWireSphere(capsuleEnd, capsuleRadius);

        //// 선 그리기
        //Gizmos.DrawLine(capsuleStart + Vector3.up * capsuleRadius, capsuleEnd + Vector3.up * capsuleRadius);
        //Gizmos.DrawLine(capsuleStart + Vector3.down * capsuleRadius, capsuleEnd + Vector3.down * capsuleRadius);

        //// Physics.OverlapCapsule 호출하여 Collider 배열 얻기
        //Collider[] colliders = Physics.OverlapCapsule(capsuleStart, capsuleEnd, capsuleRadius, AttackLayer);

        //// 각 Collider의 위치에 빨간색 기즈모 그리기
        //Gizmos.color = Color.red;
        //foreach (Collider collider in colliders)
        //{
        //    Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
        //}

        // WolfClaw
        Gizmos.color = Color.cyan;

        // 회전을 적용한 기즈모 그리기
        Matrix4x4 originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(WolfClawRange.position, transform.rotation * Quaternion.Euler(0f, 25f, -20f), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, WolfClawBoxRange);
        Gizmos.matrix = originalMatrix; // 기존 행렬로 복원

        originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(WolfClawRange.position+new Vector3(0,0.4f,0), transform.rotation * Quaternion.Euler(0f, 25f, -20f), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, WolfClawBoxRange);
        Gizmos.matrix = originalMatrix; // 기존 행렬로 복원
        
        originalMatrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(WolfClawRange.position-new Vector3(0, 0.4f, 0), transform.rotation * Quaternion.Euler(0f, 25f, -20f), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, WolfClawBoxRange);
        Gizmos.matrix = originalMatrix; // 기존 행렬로 복원

    }

    public void Attack()
    {
        Debug.Log("늑대 공격");
        if (isClocking)
        {
            PV.RPC("ResetClockingRPC", RpcTarget.All);
        }
        Collider[] hitColliders = Physics.OverlapSphere(AttackRange.transform.position, 1.8f, AttackLayer);
        Debug.Log(hitColliders.Length);
        if (hitColliders.Length > 0 ) {
            PV.RPC("AttackFX_RPC", RpcTarget.All, 0.25f);
            foreach (var hitCollider in hitColliders)
            {
                Debug.Log("뭘 때렸지? : "+hitCollider.gameObject.name);
                if (hitCollider.CompareTag("Pig"))
                {
                    Debug.Log("늑대 공격범위에서 돼지를 감지.");
                    Debug.Log(hitCollider);
                    hitCollider.GetComponent<PigStatus>().ReceiveAttack(PS.GetDamage());
                }
                else if (hitCollider.CompareTag("House"))
                {
                    Debug.Log("늑대 공격범위에서 집을 감지.");
                    Debug.Log(hitCollider);
                    hitCollider.GetComponent<PhotonView>().RPC("ReceiveWolfAttack", RpcTarget.All, WS.GetDamage());
                    WS.PlusExp(1);
                }
                else if (hitCollider.CompareTag("Chicken") || hitCollider.CompareTag("Goat") || hitCollider.CompareTag("Cow"))
                {
                    Debug.Log("늑대 공격범위에서 Animal 을 감지.");
                    Debug.Log(hitCollider);
                    int exp = hitCollider.GetComponent<AnimalStatus>().TakeDamage(WS.GetDamage());
                    
                    if (exp > 0 )
                    {
                        WS.PlusExp(exp);
                    }
                }
            }
        }
    }

    [PunRPC]
    public void AttackFX_RPC(float delay)
    {
        WolfAttackVFX.Play();
        WolfAttackSFX.PlayDelayed(delay);
    }


    public void Dash()
    {
        Debug.Log("대쉬");
        float curSpeed = PS.GetSpeed();
        WS.ChangeSpeed(curSpeed * 3, 2f);
        WolfDashSFX.Play();
    }

    public void ActiveSuperJump()
    {
        float defaultJumpSpeed = character.GetDefaultJumpSpeed();
        WS.ChangeJumpSpeed(defaultJumpSpeed * 2.5f, 8.0f);
    }
    
    public void WindBlowing()
    {
        WS.WindBlowing();
        Debug.Log("바람 불기 시작");

    }

    public void WindBlowingAttack()
    {
        Vector3 windStart = WindBlowingTransform.parent.TransformPoint(WindBlowingTransform.localPosition);
        Vector3 windEnd = WindBlowingTransform.parent.TransformPoint(WindBlowingTransform.localPosition + 5 * Vector3.forward);
        if (isClocking)
        {
            PV.RPC("ResetClockingRPC", RpcTarget.All);
        }
        Collider[] umbrellarCollider =Physics.OverlapCapsule(windStart, windEnd, 2f, UmbrellaLayer);
        if (umbrellarCollider.Length > 0)
        {
            return;
        }
        Collider[] colliders =Physics.OverlapCapsule(windStart, windEnd, 2f, AttackLayer);
        foreach (Collider collider in colliders)
        {
            // 특정 Collider에 충돌했을 때의 처리
            if (collider.CompareTag("Pig"))
            {
                Debug.Log("Pig"); // 데미지 조정 필요
                // 돼지대상 데미지 = 1
                collider.GetComponent<PigStatus>().ReceiveAttack(1);
            }
            else if (collider.CompareTag("House"))
            {
                // 집대상 데미지 = 공격력 * 0.7
                collider.GetComponent<PhotonView>().RPC("ReceiveWolfAttack", RpcTarget.All, (int)(WS.GetDamage() * 0.7f));
                Debug.Log("House");
            }
            else if (collider.CompareTag("Chicken") || collider.CompareTag("Goat") || collider.CompareTag("Cow"))
            {
                Debug.Log("Animal");
                // 동물대상 데미지 = 공격력 * 0.5
                int exp = collider.GetComponent<AnimalStatus>().TakeDamage((int)(WS.GetDamage() * 0.5f));
                if (exp > 0)
                {
                    WS.PlusExp(exp);
                }
            }
        }

    }

    public void WolfClawAttack()
    {
        PV.RPC("WolfClawEffectRPC",RpcTarget.All);
        //anim.SetTrigger("isClawAttack"); // RPC로 변경
        if (isClocking)
        {
            PV.RPC("ResetClockingRPC",RpcTarget.All);
        }
        // 45도 회전을 기준
        Collider[] colliders = Physics.OverlapBox(WolfClawRange.position, WolfClawBoxRange / 2, transform.rotation * Quaternion.Euler(0f, 45f, -15f), AttackLayer);
        if (colliders != null)
        {
            WolfClawDamage(colliders);
        }
        colliders= Physics.OverlapBox(WolfClawRange.position + new Vector3(0, 0.4f, 0), WolfClawBoxRange / 2, transform.rotation * Quaternion.Euler(0f, 45f, -15f), AttackLayer);
        if (colliders != null)
        {
            WolfClawDamage(colliders);
        }
        colliders = Physics.OverlapBox(WolfClawRange.position - new Vector3(0, 0.4f, 0), WolfClawBoxRange / 2, transform.rotation * Quaternion.Euler(0f, 45f, -15f), AttackLayer);
        if (colliders != null)
        {
            WolfClawDamage(colliders);
        }
    }
    public void WolfClawDamage(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            // 특정 Collider에 충돌했을 때의 처리
            if (collider.CompareTag("Pig"))
            {
                Debug.Log("Pig");
                // 돼지대상 데미지 = 공격력 * 0.3 + 1
                PigStatus PS = collider.GetComponent<PigStatus>();
                if (!PS.GetInHouse())
                {
                    PS.ReceiveAttack((int)(WS.GetDamage() * 0.3) + 1);
                    PS.OnClawDebuffEffect();
                }
            }
            else if (collider.CompareTag("House"))
            {
                Debug.Log("House");
                // 집대상 데미지 = 공격력
                collider.GetComponent<PhotonView>().RPC("ReceiveWolfAttack", RpcTarget.All, WS.GetDamage());
            }
            else if (collider.CompareTag("Chicken") || collider.CompareTag("Goat") || collider.CompareTag("Cow"))
            {
                Debug.Log("Animal");
                // 동물대상 데미지 = 공격력
                int exp = collider.GetComponent<AnimalStatus>().TakeDamage(WS.GetDamage());
                if (exp > 0)
                {
                    WS.PlusExp(exp);
                }
            }
        }
    }

    [PunRPC]
    public void WolfClawEffectRPC()
    {
        anim.SetTrigger("isClawAttack");
        audioSource.clip = wolfClawAudioClip;
        audioSource.Play();
        if (isClocking)
        {
            ResetClocking();
        }
        for (int i = 0; i < WolfClawEffects.Length; i++)
        {
            WolfClawEffects[i].SetActive(true);
            for (int idx = 0; idx < WolfClawEffects[i].transform.childCount; idx++)
            {
                WolfClawEffects[i].transform.GetChild(idx).gameObject.SetActive(true);
            }
        }
        
    }

    public void Howling()
    {
        Debug.Log("늑대 하울링");
        WS.isHowling = true;
        WS.howlingTimeFlow = HowlingDurationTime;
        // particlesystem play
        float curSpeed = PS.GetSpeed();
        float curAttackSpeed = 1.0f;
        WS.ChangeSpeed(curSpeed * 2.5f, HowlingDurationTime);
        WS.ChangeAttackSpeed(curAttackSpeed * 0.7f, HowlingDurationTime);
        //PV.RPC("HowlingRPC", RpcTarget.All, timeFlow);
    }

    public void UseWolfClocking()
    {
        anim.SetTrigger("isClocking"); // s
        WS.SetClocking(true);
        if (meshRenderer != null)
        {
            // 렌더링을 끄거나 킬 때 사용할 함수 호출
            PV.RPC("UseWolfClockingRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    public void UseWolfClockingRPC()
    {
        audioSource.clip = WolfClockingClip;
        audioSource.Play();
        ClockingAura.SetActive(true);
        isClocking = true;
        ClockingTime = defaultClockingTime;
        WS.ChangeSpeed(18f, defaultClockingTime); // 속도 변경
    }

    public void WolfClocking()
    {
        meshRenderer.enabled = false;
    }

    public void ClockingTimer()
    {
        if (ClockingTime > 0)
        {
            ClockingTime -= Time.deltaTime;
        }
        else
        {
            ResetClocking();
        }
    }

    public void ResetClocking()
    {
        ClockingAura.SetActive(false);
        isClocking = false;
        ClockingTime = 0;
        meshRenderer.enabled = true;
        WS.ResetSpeed();
    }

    [PunRPC]
    public void ResetClockingRPC()
    {
        ResetClocking();
    }

    public void UseWolfShock()
    {
        //anim.SetTrigger("isStunning");
        PV.RPC("UseWolfShockRPC", RpcTarget.All);
        Collider[] hitColliders = Physics.OverlapSphere(AttackRange.transform.position, 15f, pigLayer);
        foreach(Collider collider in hitColliders)
        {
            PigStatus PS = collider.GetComponent<PigStatus>();
            if (!PS.GetInHouse())
            {
                collider.GetComponent<PhotonView>().RPC("RecieveShockRPC", RpcTarget.All, 2f);
            }
        }
    }



    [PunRPC]
    public void UseWolfShockRPC()
    {
        anim.SetTrigger("isStunning");
        WolfShockEffect.SetActive(true);
    }


}
