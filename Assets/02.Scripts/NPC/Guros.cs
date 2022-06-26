using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guros : MonoBehaviour
{
    public float JumpAttackCoolTime = 10f;
    public float JumpAttackMaxDistance;
    public float JumpAttackMinDistance;
    public float JumpAttackRadius = 10;
    public LayerMask ObstacleMask;
    public Transform JumpAttackExplodePosition;
    public GameObject ExplodeParticle;
    public bool JumpAttackReady = true;
    [Space]
    public float Sattack1CoolTime = 10f;
    public float Sattack1Distance = 6f;
    public bool Sattack1Ready = true;
    [Space]
    public float Sattack2CoolTime = 10f;
    public float Sattack2Distance = 6f;
    public bool Sattack2Ready = true;
    [Space]
    public float RangeAttackCoolTime = 10f;
    public float RangeAttackMinDistance = 10f;
    public float RangeAttackMaxDistance = 25f;
    public GameObject Spear;
    public GurosSpear gurosSpear;
    public Transform SpearHand;
    public bool RangeAttackReady = true;
    public bool SpearReady = true;
    public Quaternion OriginSpearRotation;
    public Vector3 OriginSpearPotition;

    [Space]

    public MeleeWeaponTrail SwordTrail;
    public MeleeWeaponTrail AxeTrail;
    public MeleeWeaponTrail MaceTrail;
    public ParticleSystem SwordParticleTrail;
    public ParticleSystem AxeParticleTrail;
    public ParticleSystem MaceParticleTrail;

    private Animator animator;
    private NPCStats stat;

    private void Start()
    {
        animator = GetComponent<Animator>();
        stat = GetComponent<NPCStats>();
    }

    public IEnumerator JumpAttack()
    {
        JumpAttackReady = false;

        animator.SetTrigger("jumpAttack01");
        yield return new WaitForSeconds(JumpAttackCoolTime);
        JumpAttackReady = true;

    }

    public void jumpAttack()
    {
        RaycastHit raycastHit;
        Physics.Raycast(JumpAttackExplodePosition.position, -transform.up, out raycastHit, 10f, ObstacleMask);


        float damage = Random.Range(stat.minDamage.GetFinalStatValue(), stat.maxDamage.GetFinalStatValue());

        NPC_Type type = NPC_Type.enemy;
        AttackEffectFunctions.explode(JumpAttackRadius, Mathf.RoundToInt(damage), raycastHit.point + new Vector3(0,0.2f,0), ExplodeParticle, type, gameObject);
    }

    public IEnumerator Sattack1()
    {
        Sattack1Ready = false;

        animator.SetTrigger("Sattack01");
        yield return new WaitForSeconds(Sattack1CoolTime);
        Sattack1Ready = true;
    }

    public IEnumerator Sattack2()
    {
        Sattack2Ready = false;
        yield return new WaitForSeconds(Sattack2CoolTime);
        Sattack2Ready = true;
    }

    public IEnumerator RangeAttack()
    {
        RangeAttackReady = false;
        
        animator.SetTrigger("RangeAttack");

        yield return new WaitForSeconds(RangeAttackCoolTime);
        RangeAttackReady = true;
    }

    public void ThrowSpear()
    {
        OriginSpearPotition = Spear.transform.localPosition;
        OriginSpearRotation = Spear.transform.localRotation;

        Spear.transform.SetParent(null);
        var spear = Spear.GetComponent<ProjectileLogic>();
        spear.Setting(gameObject, Mathf.RoundToInt(GetComponent<NPCStats>().minDamage.GetFinalStatValue()), Mathf.RoundToInt(GetComponent<NPCStats>().maxDamage.GetFinalStatValue()));
        spear.Target = GetComponent<NPC_AI>().Target;

        spear.GetComponent<Rigidbody>().isKinematic = false;

        spear.Fire(GetComponent<NPC_AI>().Target.transform.position, Spear.transform.position, gameObject, 0, -90);

        spear.GetComponent<Collider>().enabled = true;
        spear.GetComponentInChildren<TrailRenderer>().emitting = true;

        AudioManager.instance.GenerateAudioAndPlaySFX("throw1", Spear.transform.position);

        SpearReady = false;
    }

    public IEnumerator PullSpear()
    {
        Spear.GetComponent<GurosSpear>().isReadyPull = false;

        animator.SetTrigger("PullSpear");
        Spear.GetComponent<Rigidbody>().isKinematic = false;
        
        yield return null;
    }

    public void StartPull()
    {
        Spear.GetComponent<GurosSpear>().isPulling = true;
    }

    public void StartRightWeaponTrailStart()
    {
        //SwordTrail.Use = true;
        //SwordTrail.Emit = true;
        //AxeTrail.Emit = true;

        SwordParticleTrail.Play();
        AxeParticleTrail.Play();
    }

    public void StartRightWeaponTrailEnd()
    {
        //SwordTrail.Use = false;
        //SwordTrail.Emit = false;
        //AxeTrail.Emit = false;

        SwordParticleTrail.Stop();
        AxeParticleTrail.Stop();
    }

    public void StartLeftWeaponTrailStart()
    {
        MaceParticleTrail.Play();
        //MaceTrail.Emit = true;

    }

    public void StartLeftWeaponTrailEnd()
    {
        MaceParticleTrail.Stop();
        //MaceTrail.Emit = false;
    }
}
