using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SkeletonGuardian : MonoBehaviour
{
    public Collider SwordCollider;
    public MeleeWeaponTrail SwordTrail;
    public NPC_AttackLogic attackLogic;

    private Animator animator;
    private NavMeshAgent agent;
    private NPCStats stat;

    [Space]

    public bool attack01Ready = true;
    public float attack01CoolTime = 5;
    public float attack01MaxDist = 7;

    [Space]
    public bool attack02Ready = true;
    public float attack02CoolTime = 5;
    public float attack02MaxDist = 7;
    public Transform attack02Explosiontransform;
    public float attack02ExplosionRadius = 5;

    [Space]
    public bool attack03Ready = true;
    public float attack03CoolTime = 7;
    public float attack03MinDist = 7;
    public float attack03MaxDist = 10;

    [Space]
    public bool jumpAttackReady = true;
    public float jumpAttackCoolTime = 10;
    public float jumpAttackMinDist = 10;
    public float jumpAttackMaxDist = 14;
    public float jumpAttackRadius = 5;
    public Transform jumpAttackExplosiontransform;
    public float jumpAttackExplosionRadius = 5;

    [Space]
    public bool WhirlWindReady = true;
    public float WhirlWindCoolTime = 15;
    public float WhirlWindMaxDist = 10;

    [Space]
    public bool WhirlWindForwardReady = true;
    public float WhirlWindForwardCoolTime = 10;
    public float WhirlWindForwardMaxDist = 10;

    [Space]

    public LayerMask ObstacleMask;
    public bool isAttacking = false;
    public Transform Neck;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stat = GetComponent<NPCStats>();
    }

    public IEnumerator Attack01Start()
    {
        attack01Ready = false;
        animator.SetTrigger("Attack01");
        yield return new WaitForSeconds(attack01CoolTime);
        attack01Ready = true;
    }


    public IEnumerator Attack02Start()
    {
        attack02Ready = false;
        animator.SetTrigger("Attack02");
        yield return new WaitForSeconds(attack02CoolTime);
        attack02Ready = true;
    }

    public void Attack02Explosion()
    {
        int damage = Random.Range(Mathf.RoundToInt(stat.minDamage.GetFinalStatValueAsMultiflyFloat()), Mathf.RoundToInt(stat.maxDamage.GetFinalStatValueAsMultiflyFloat()));

        RaycastHit raycastHit;
        Physics.Raycast(attack02Explosiontransform.position, -transform.up, out raycastHit, 10f, ObstacleMask);

        NPC_Type type = NPC_Type.enemy;

        AttackEffectFunctions.explode(attack02ExplosionRadius, damage, raycastHit.point, PrefabCollect.instance.ExplodeParticle, type, gameObject);
    }


    public IEnumerator Attack03Start()
    {
        attack03Ready = false;
        animator.SetTrigger("Attack03");
        yield return new WaitForSeconds(attack03CoolTime);
        attack03Ready = true;
    }


    public IEnumerator JumpAttackStart()
    {
        jumpAttackReady = false;
        animator.SetTrigger("JumpAttack");
        yield return new WaitForSeconds(jumpAttackCoolTime);
        jumpAttackReady = true;
    }

    public void JumpAttackExplosion()
    {
        int damage = Random.Range(Mathf.RoundToInt(stat.minDamage.GetFinalStatValueAsMultiflyFloat()), Mathf.RoundToInt(stat.maxDamage.GetFinalStatValueAsMultiflyFloat()));

        RaycastHit raycastHit;
        Physics.Raycast(jumpAttackExplosiontransform.position, -transform.up, out raycastHit, 10f, ObstacleMask);

        NPC_Type type = NPC_Type.enemy;

        AttackEffectFunctions.explode(jumpAttackExplosionRadius, damage, raycastHit.point, PrefabCollect.instance.ExplodeParticle, type,gameObject);
    }


    public IEnumerator WhirlWindStart()
    {
        WhirlWindReady = false;
        animator.SetTrigger("WhirlWind");
        yield return new WaitForSeconds(WhirlWindCoolTime);
        WhirlWindReady = true;
    }


    public IEnumerator WhirlWindForwardStart()
    {
        WhirlWindForwardReady = false;
        animator.SetTrigger("WhirlWindForward");
        yield return new WaitForSeconds(WhirlWindForwardCoolTime);
        WhirlWindForwardReady = true;
    }


    public void SwordColliderOn()
    {
        SwordCollider.enabled = true;
        SwordTrail.Emit = true;
    }


    public void SwordColliderOff()
    {
        SwordCollider.enabled = false;
        SwordTrail.Emit = false;
        attackLogic.FunchEnd();
    }



    

    public void TrailOn()
    {
        SwordTrail.Emit = true;
    }

    public void TrailOff()
    {
        SwordTrail.Emit = false;
    }

    public void ClearNpcAttackList()
    {
        attackLogic.FunchEnd();
    }

    public void ChangeNavSpeed(float speed)
    {
        agent.speed = speed;
    }

    public void ChangeAngulerApeed(float speed)
    {
        agent.angularSpeed = speed;
    }
}
