using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Skulder : MonoBehaviour
{
    public float FireBreathCoolTime = 10;
    public float FireBreathMinDist = 15f;
    public Transform FireBreathTarget;
    public VisualEffect FireBreathVFX;
    public GameObject FireBreathTick;
    public float FireBreathTickRate = .5f;
    public float FireBreathTickDamagePercent = .33f;
    public bool FireBreathReady = true;
    public bool isFireBreath = false;

    [Space]

    public float meleeAttackCoolTime = 10f;
    public float meleeAttackMinDist = 5;
    public bool meleeAttackReady = true;
    public float meleeAttackDamagePercenet = 2f;
    public float meleeAttackDurationTime = 1.5f;

    [Space]

    public float FireMagicArrowCoolTime = 7;
    public float FireMagicArrowMinDist = 7f;
    public float FireMagicArrowMaxDist = 20f;
    public GameObject FireMagicArrowProjectile;
    public Transform FireMagicArrowFirePos;
    public float FireMagicArrowDamagePercent = 1f;
    public int FireMagicArrowAmount = 5;
    public int FireMagicArrowAngle = 40;
    public bool FireMagicArrowReady = true;

    [Space]

    public float BlastFireBallCoolTime = 8;
    public float BlastFireBallMinDist = 7f;
    public int BlastFireBallAmount = 8;
    public int BlastFireBallAngle = 45;
    public float BlastFireBallDamagePercent = 1f;
    public bool BlastFireBallReady = true;

    [Space]

    public float FireExplosionFireBallCoolTime = 10f;
    public float FireExplosionFireBallMinDist = 7f;
    public float FireExplosionFireBallMaxDist = 20f;
    public GameObject FireExplosionFireBallProjectile;
    public bool FireExplosionFireBallReady = true;
    public int FireExplosionFireBallAmount = 1;
    public int FireExplosionFireBallAngle = 0;

    public bool isAttacking = false;

    private NPC_AI ai;
    private NPCStats stat;
    private Animator animator;
    private NPC_AttackLogic attackLogic;

    // Start is called before the first frame update
    void Start()
    {
        ai = GetComponent<NPC_AI>();
        stat = GetComponent<NPCStats>();
        animator = GetComponent<Animator>();
        attackLogic = GetComponent<NPC_AttackLogic>();
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
        while(isFireBreath)
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
        
    }

    public void FireBreathEnd()
    {
        FireBreathVFX.Stop();
        isFireBreath = false;
    }






    public IEnumerator MeleeAttack()
    {
        meleeAttackReady = false;
        animator.SetTrigger("MeleeAttack");
        yield return new WaitForSeconds(meleeAttackCoolTime);
        meleeAttackReady = true;
    }

    public void MeleeAttackWhile()
    {
        StartCoroutine(MeleeAttackColliderOn());
    }

    IEnumerator MeleeAttackColliderOn()
    {
        attackLogic.ColliderOn();
        attackLogic.ChangeDamageMultyfly(2);

        yield return new WaitForSeconds(meleeAttackDurationTime);

        MeleeAttackWhileEnd();
    }

    public void MeleeAttackWhileEnd()
    {
        attackLogic.ColliderOff();
        attackLogic.ChangeDamageMultyfly(1);
        attackLogic.FunchEnd();
        
    }






    public IEnumerator FireMagicArrow()
    {
        FireMagicArrowReady = false;
        animator.SetTrigger("FireMagicArrow");
        yield return new WaitForSeconds(FireMagicArrowCoolTime);
        FireMagicArrowReady = true;
    }


    public void GenerateMagicArrow()
    {
        int Angel = 0;

        if (FireMagicArrowAmount > 1)
            Angel = FireMagicArrowAngle / (FireMagicArrowAmount - 1);

        for (int i = 0; i < FireMagicArrowAmount; i++)
        {
            var fireArrow = Instantiate(FireMagicArrowProjectile, FireMagicArrowFirePos.position, Quaternion.identity).GetComponent<ProjectileLogic>();
            fireArrow.Setting(gameObject, Mathf.RoundToInt(stat.minDamage.GetFinalStatValue() * FireMagicArrowDamagePercent), 
                Mathf.RoundToInt(stat.maxDamage.GetFinalStatValue() * FireMagicArrowDamagePercent));

            fireArrow.Fire(ai.Target.bounds.center, FireMagicArrowFirePos.position, gameObject, (Angel * (i)) - (FireMagicArrowAngle / 2));
        }

        AudioManager.instance.GenerateAudioAndPlaySFX("fireBall1", FireMagicArrowFirePos.position);
    }





    public IEnumerator BlastFireBall()
    {
        BlastFireBallReady = false;
        animator.SetTrigger("BlastFireBall");
        yield return new WaitForSeconds(BlastFireBallCoolTime);
        BlastFireBallReady = true;
    }

    public void GenerateBlastFireBall()
    {
        int Angel = 0;

        if (BlastFireBallAmount > 1)
            Angel = BlastFireBallAngle / (BlastFireBallAmount - 1);

        for (int i = 0; i < BlastFireBallAmount; i++)
        {
            var fireArrow = Instantiate(FireMagicArrowProjectile, FireMagicArrowFirePos.position, Quaternion.identity).GetComponent<ProjectileLogic>();
            fireArrow.Setting(gameObject, Mathf.RoundToInt(stat.minDamage.GetFinalStatValue() * FireMagicArrowDamagePercent),
                Mathf.RoundToInt(stat.maxDamage.GetFinalStatValue() * BlastFireBallDamagePercent));

            fireArrow.Fire(ai.Target.bounds.center, FireMagicArrowFirePos.position, gameObject, (Angel * (i)) - (BlastFireBallAngle / 2));
        }

        AudioManager.instance.GenerateAudioAndPlaySFX("fireBall1", FireMagicArrowFirePos.position);
    }



    public IEnumerator FireExplosionFireBall()
    {
        FireExplosionFireBallReady = false;
        animator.SetTrigger("FireExplosionFireBall");
        yield return new WaitForSeconds(FireExplosionFireBallCoolTime);
        FireExplosionFireBallReady = true;
    }

    public void GenerateExplosionFireBall()
    {
        int Angel = 0;

        if (FireExplosionFireBallAmount > 1)
            Angel = FireExplosionFireBallAngle / (FireExplosionFireBallAmount - 1);

        for (int i = 0; i < FireExplosionFireBallAmount; i++)
        {
            var fireArrow = Instantiate(FireExplosionFireBallProjectile, FireMagicArrowFirePos.position, Quaternion.identity).GetComponent<ProjectileLogic>();
            fireArrow.Setting(gameObject, Mathf.RoundToInt(stat.minDamage.GetFinalStatValue()),
                Mathf.RoundToInt(stat.maxDamage.GetFinalStatValue()));

            fireArrow.Fire(ai.Target.transform.position, FireMagicArrowFirePos.position, gameObject, (Angel * (i)) - (FireExplosionFireBallAngle / 2));
        }
    }



    public void StartAttackAnimation()
    {
        isAttacking = true;
    }

    public void EndAttackAnimation()
    {
        isAttacking = false;
    }

    public void OnDie()
    {
        FireBreathVFX.Stop();
        FireBreathVFX.transform.SetParent(null);
        Destroy(FireBreathVFX.gameObject, 10);
    }
}
