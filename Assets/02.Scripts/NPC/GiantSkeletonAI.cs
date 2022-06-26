using System.Collections;
using UnityEngine;

public class GiantSkeletonAI : MonoBehaviour
{
    public float meleeAttackCoolTime = 5;
    public float meleeAttackmaxDist = 7;
    public bool meleeAttackReady = true;
    [Space]

    public float closeAttackCoolTime = 5;
    public float closeAttackmaxDist = 6;
    public Collider closeAttackCollider;
    public NPC_AttackLogic closeAttackLogic;
    public bool closeAttackReady = true;

    [Space]
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
    public float RangeAttackCoolTime = 5f;
    public float RangeAttackMinDistance = 10f;
    public float RangeAttackMaxDistance = 25f;
    public GameObject Skull;
    public Transform RangeAttackFirePos;
    public bool RangeAttackReady = true;
    private GameObject SpawnedSkull;

    [Space]

    public bool isAttacking = false;

    private Animator animator;
    private NPCStats npcStat;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        npcStat = GetComponent<NPCStats>();
    }
    
    public IEnumerator MeleeAttack()
    {
        meleeAttackReady = false;

        switch(Random.Range(0,2))
        {
            case 0:
                animator.SetTrigger("meleeAttack01");
                break;

            case 1:
                animator.SetTrigger("meleeAttack02");
                break;
        }

        
        yield return new WaitForSeconds(meleeAttackCoolTime);
        meleeAttackReady = true;
    }

    public IEnumerator CloseAttack01()
    {
        closeAttackReady = false;

        animator.SetTrigger("CloseAttack");
        yield return new WaitForSeconds(closeAttackCoolTime);
        closeAttackReady = true;
    }

    public void CloseAttackColliderOn()
    {
        closeAttackCollider.enabled = true;
    }

    public void CloseAttackColliderOff()
    {
        closeAttackCollider.enabled = false;
        closeAttackLogic.FunchEnd();
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

        int damage = Random.Range(Mathf.RoundToInt(npcStat.minDamage.GetFinalStatValue()), Mathf.RoundToInt(npcStat.maxDamage.GetFinalStatValue()));

        NPC_Type type = NPC_Type.enemy;
        AttackEffectFunctions.explode(JumpAttackRadius, damage, raycastHit.point + new Vector3(0,0.2f,0), ExplodeParticle, type, gameObject);
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

    public void SpawnSkull()
    {
        SpawnedSkull = Instantiate(Skull, RangeAttackFirePos.position, Quaternion.identity,RangeAttackFirePos);
    }

    public void SkullThrow()
    {
        var skull = SpawnedSkull.GetComponent<ProjectileLogic>();
        
        SpawnedSkull.transform.SetParent(null);
        skull.Setting(gameObject, Mathf.RoundToInt(GetComponent<NPCStats>().minDamage.GetFinalStatValue()), Mathf.RoundToInt(GetComponent<NPCStats>().maxDamage.GetFinalStatValue()));
        skull.Target = GetComponent<NPC_AI>().Target;

        skull.GetComponent<Rigidbody>().isKinematic = false;

        skull.Fire(GetComponent<NPC_AI>().Target.bounds.center, RangeAttackFirePos.position, gameObject);

        skull.GetComponent<Collider>().enabled = true;
        

    }

}
