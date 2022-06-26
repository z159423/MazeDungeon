using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.AI;

public class GhostSkull : MonoBehaviour
{
    public float FireBreathCoolTime = 7;
    public float FireBreathMinDist = 10f;
    public Transform FireBreathTarget;
    public VisualEffect FireBreathVFX;
    public GameObject FireBreathTick;
    public float FireBreathTickRate = .2f;
    public float FireBreathTickDamagePercent = 1f;
    public bool FireBreathReady = true;
    public bool isFireBreath = false;
    public bool extraRotation = false;
    public float extraRotationSpeed = 5f;

    [Space]

    public float fireBallCoolTime = 7;
    public float fireBallMinDist = 10;
    public float fireBallMaxDist = 20;
    public Transform fireBallFirePos;
    public GameObject fireBallProjectile;
    public bool fireBallReady = true;
    public int fireBallAmount = 1;
    public int fireBallArrowAngle = 60;

    [Space]

    public float biteCoolTime = 5;
    public float biteMaxDist = 4;
    public bool biteReady = true;

    [Space]
    public bool isAttacking = false; 

    private NPC_AI ai;
    private NPCStats stat;
    private Animator animator;
    private NPC_AttackLogic attackLogic;
    private NavMeshAgent agent;
    private AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        ai = GetComponent<NPC_AI>();
        stat = GetComponent<NPCStats>();
        animator = GetComponent<Animator>();
        attackLogic = GetComponent<NPC_AttackLogic>();
        agent = GetComponent<NavMeshAgent>();
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(extraRotation)
        {
            Vector3 lookrotation = agent.steeringTarget - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookrotation), extraRotationSpeed * Time.deltaTime);
        }

        if(isFireBreath && ai.Target != null)
        {
            SmoothLookatTarget();
        }

    }

    public IEnumerator FireBreath()
    {
        FireBreathReady = false;
        animator.SetTrigger("FireBreath");
        yield return new WaitForSeconds(FireBreathCoolTime);
        FireBreathReady = true;
    }

    public IEnumerator GenerateFireBreathTick()
    {
        while (isFireBreath)
        {
            yield return new WaitForSeconds(FireBreathTickRate);

            var flameTick = Instantiate(FireBreathTick, FireBreathVFX.transform.position, new Quaternion(0, 0, 0, 0));

            flameTick.GetComponentInChildren<FlameTick>().Set(gameObject, Mathf.RoundToInt(stat.minDamage.GetFinalStatValueAsMultiflyFloat() * FireBreathTickDamagePercent),
                Mathf.RoundToInt(stat.maxDamage.GetFinalStatValueAsMultiflyFloat() * FireBreathTickDamagePercent), FireBreathTarget);
        }

    }
    public void FireBreathStart()
    {
        FireBreathVFX.Play();
        isFireBreath = true;
        StartCoroutine(GenerateFireBreathTick());

        AudioManager.instance.PlaySFXWithAudioSource("fireGiggle1", audio);
    }

    public void FireBreathEnd()
    {
        FireBreathVFX.Stop();
        isFireBreath = false;

        audio.Stop();
    }

    private void SmoothLookatTarget()
    {
        //transform.rotation = Quaternion.Slerp(transform.rotation, q, RotationSpeed);     //canRotationWhileAttack가 true면 npc가 타겟을 바라보게 하기
        Vector3 vectorToTarget = ai.Target.transform.position - transform.position;

        vectorToTarget.y = 0;
        vectorToTarget.Normalize();

        Quaternion targetAngle = Quaternion.LookRotation(vectorToTarget);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetAngle, Time.deltaTime * 8);
    }


    public IEnumerator FireBall()
    {
        fireBallReady = false;
        animator.SetTrigger("FireBall");
        yield return new WaitForSeconds(fireBallCoolTime);
        fireBallReady = true;
    }

    public void GenerateFireBall()
    {
        int Angel = 0;

        if (fireBallAmount > 1)
            Angel = fireBallArrowAngle / (fireBallAmount - 1);

        for (int i = 0; i < fireBallAmount; i++)
        {
            var fireArrow = Instantiate(fireBallProjectile, fireBallFirePos.position, Quaternion.identity).GetComponent<ProjectileLogic>();
            fireArrow.Setting(gameObject, Mathf.RoundToInt(stat.minDamage.GetFinalStatValue() * 2),
                Mathf.RoundToInt(stat.maxDamage.GetFinalStatValue() * 2));

            fireArrow.Fire(ai.Target.bounds.center, fireBallFirePos.position, gameObject, (Angel * (i)) - (fireBallArrowAngle / 2));

            if(ai.Target != null)
                fireArrow.Target = ai.Target;
        }
    }



    public IEnumerator Bite()
    {
        biteReady = false;
        animator.SetTrigger("Bite");
        yield return new WaitForSeconds(biteCoolTime);
        biteReady = true;
    }

    public void OnDie()
    {
        FireBreathVFX.Stop();
        FireBreathVFX.transform.SetParent(null);
        Destroy(FireBreathVFX.gameObject, 10);
    }
}
