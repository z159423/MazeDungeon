using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : MonoBehaviour
{
    private GameObject throwRockObject;
    private ProjectileLogic projectileLogic;
    public NPC_AI ai;
    public NPCStats stats;
    public Animator animator;

    [Space]

    public float meleeAttackMaxDist = 5;
    public float meleeAttackCollTime = 4;
    public bool meleeAttackReady = true;

    [Space]
    public float sAttack01MaxDist = 5;
    public float sAttack01CoolTime = 6;
    public bool sAttack01Ready = true;
    public Transform sAttack01ExplodePos;
    public float sAttackExplodeRadius = 5;

    [Space]
    public float rangeAttack01CoolTime = 10;
    public float rangeAttack01minDist = 10;
    public float rangeAttack01maxDist = 20;
    public bool rangeAttack01ready = true;
    public Transform rangeAttackFirePos;

    [Space]

    public bool isAttacking = false;

    public float wideAttackArea;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public float SmashGroundDamageMultiply;

    private void Start()
    {
        if (!ai)
            ai = GetComponent<NPC_AI>();

        if (!stats)
            stats = GetComponent<NPCStats>();

        if (!animator)
            animator = GetComponent<Animator>();
    }

    public IEnumerator meleeAttack()
    {

        meleeAttackReady = false;

        switch (Random.Range(0,2))
        {
            case 0:
                animator.SetTrigger("MeleeAttack01");
                break;

            case 1:
                animator.SetTrigger("MeleeAttack02");
                break;
        }

        yield return new WaitForSeconds(meleeAttackCollTime);
        meleeAttackReady = true;

    }


    public IEnumerator sAttack01()
    {
        sAttack01Ready = false;

        animator.SetTrigger("sAttack01");
        yield return new WaitForSeconds(sAttack01CoolTime);
        sAttack01Ready = true;
    }

    public IEnumerator throwRock()
    {
        rangeAttack01ready = false;

        animator.SetTrigger("ThrowRock");
        yield return new WaitForSeconds(rangeAttack01CoolTime);
        rangeAttack01ready = true;
    }

    public void SpawnRock()
    {
        print("돌 생성");
        throwRockObject = Instantiate(PrefabCollect.instance.ThrowRock01);
        throwRockObject.transform.SetParent(rangeAttackFirePos);
        projectileLogic = throwRockObject.GetComponent<ProjectileLogic>();
        throwRockObject.transform.localPosition = Vector3.zero;
        projectileLogic.Setting(gameObject, Mathf.RoundToInt(GetComponent<NPCStats>().minDamage.GetFinalStatValue()), Mathf.RoundToInt(GetComponent<NPCStats>().maxDamage.GetFinalStatValue()));
        Destroy(throwRockObject, 5);
    }

    public void ThrowRock()
    {
        throwRockObject.GetComponent<Collider>().enabled = true;
        throwRockObject.transform.parent = null;
        throwRockObject.transform.LookAt(GetComponent<NPC_AI>().Target.bounds.center);
        throwRockObject.GetComponentInChildren<Rigidbody>().AddForce(((GetComponent<NPC_AI>().Target.bounds.center) - throwRockObject.transform.position).normalized * 2000f);
        projectileLogic.moveForce = 1500f;
    }

    public void SmashGround()                //범위공격
    {
        RaycastHit raycastHit;
        Physics.Raycast(sAttack01ExplodePos.position, -transform.up, out raycastHit, 10f, obstacleMask);

        NPC_Type type = NPC_Type.enemy;

        int damage = Random.Range(Mathf.RoundToInt(stats.minDamage.GetFinalStatValueAsMultiflyFloat()), Mathf.RoundToInt(stats.maxDamage.GetFinalStatValueAsMultiflyFloat()));

        AttackEffectFunctions.explode(sAttackExplodeRadius, damage * 2, raycastHit.point + new Vector3(0, 0.2f, 0), PrefabCollect.instance.ExplodeParticle, type, gameObject);


        /*Collider[] colliders = Physics.OverlapSphere(transform.position, wideAttackArea, targetMask);

        foreach (Collider collider in colliders)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(transform.GetComponent<Collider>().bounds.center, -transform.up, out raycastHit, 10f, obstacleMask))
            {
                ParticleGenerator.instance.GenerateGroundParticle(raycastHit.point, "GroundCrack02", 10f);
            }

            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerStats>().TakeDamage((int)Mathf.Round(stats.minDamage.GetFinalStatValue() * SmashGroundDamageMultiply)
                    , (int)Mathf.Round(stats.maxDamage.GetFinalStatValue() * SmashGroundDamageMultiply), gameObject, true, true, false);
                EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, .1f, 1);

            }
            else if (collider.CompareTag("Enemy") && collider.gameObject != gameObject)
            {
                collider.GetComponent<NPCStats>().TakeDamage((int)Mathf.Round(stats.minDamage.GetFinalStatValue() * SmashGroundDamageMultiply)
                    , (int)Mathf.Round(stats.maxDamage.GetFinalStatValue() * SmashGroundDamageMultiply), gameObject, true, true, false);
            }
        }*/
    }
}
