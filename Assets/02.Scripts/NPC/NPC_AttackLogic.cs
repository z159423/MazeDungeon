using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_AttackLogic : MonoBehaviour
{

    List<Collider> AttackedByThisNpc = new List<Collider>();

    private NPC_AI ThisNpc_ai;
    private NPCStats Stats;
    private Animator animator;
    private Vector3 contactPoint;

    public MeleeWeaponTrail Trail;
    public ParticleSystem weaponTrail;
    public List<Collider> thisCollider = new List<Collider>();
    public LayerMask ShieldMask;

    public float DamageMultifly = 1f;

    [Space]

    public BuffNDebuffObject[] attackDebuff;

    void Start()
    {
        ThisNpc_ai = GetComponentInParent<NPC_AI>();
        Stats = GetComponentInParent<NPCStats>();
        animator = GetComponentInParent<Animator>();

        if(!Trail)
        {
            if(GetComponentInChildren<MeleeWeaponTrail>())
            {
                Trail = GetComponentInChildren<MeleeWeaponTrail>();
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (ThisNpc_ai != null)
        {
            int minDamage = Mathf.RoundToInt(Stats.minDamage.GetFinalStatValue() * DamageMultifly);
            int maxDamage = Mathf.RoundToInt(Stats.maxDamage.GetFinalStatValue() * DamageMultifly);

            List<BuffNDebuffObject> tempList = new List<BuffNDebuffObject>();

            for(int i = 0; i < attackDebuff.Length; i++)
            {
                var clone = Instantiate(attackDebuff[i]);
                clone.buffOrDebuff.Owner = gameObject;

                tempList.Add(clone);
            }

            var cloneDebuffs = tempList.ToArray();

            if (ThisNpc_ai.npcType == NPC_Type.enemy || ThisNpc_ai.npcType == NPC_Type.neutrality)
            {
                if ((other.gameObject.tag == "Player" && !AttackedByThisNpc.Contains(other)))       //플레이어일 경우
                {
                    RaycastHit hit;
                    Debug.DrawLine(GetComponent<Collider>().bounds.center, other.gameObject.GetComponent<Collider>().bounds.center, Color.green, 2f);
                    if (!Physics.Raycast(GetComponent<Collider>().bounds.center, (other.gameObject.GetComponent<Collider>().bounds.center - GetComponent<Collider>().bounds.center),
                        out hit, Vector3.Distance(other.gameObject.GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.center), ShieldMask))
                    {
                        other.GetComponent<PlayerStats>().TakeDamage(minDamage, maxDamage, transform.gameObject, true, true, false, debuff : cloneDebuffs);

                        Vector3 dir = other.transform.position - transform.position;

                        other.transform.GetComponent<PlayerStats>().knockback(transform.gameObject, dir.normalized, Stats.knockBackPower);
                        AttackedByThisNpc.Add(other);

                        //contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                        Invoke("GenerateParticle", 0.01f);
                    }
                    else
                    {
                        if (hit.collider.CompareTag("Shield"))     //사이에 방패가 있을경우
                        {
                            print("공격이 방패에 막힘");
                            AudioManager.instance.GenerateAudioAndPlaySFX("block1", transform.position);
                            //FloatingTextController.CreateFloatingText("Block", transform, transform.gameObject.tag, 25, gameObject,true, owner: Stats);
                            FloatingTextController.GenerateDebuffFloatingText("Block", transform, 15, ThisNpc_ai.npcType, owner: gameObject);
                            Invoke("GenerateParticle", 0.01f);
                            other.GetComponent<PlayerStats>().UseShieldPower(Mathf.RoundToInt(Stats.minDamage.GetFinalStatValue())
                                , Mathf.RoundToInt(Stats.maxDamage.GetFinalStatValue()));
                        }
                    }
                }
                else if (other.GetComponentInParent<NPC_AI>() != null)      //아군일 경우
                {
                    if ((other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.friendly 
                        || other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.Minion 
                        || other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.neutrality
                        || ThisNpc_ai.npcType == NPC_Type.neutrality)   
                        && other.GetComponent<NPC_AI>() != null && !AttackedByThisNpc.Contains(other)
                        )
                    {
                        //Debug.Log(other);
                        other.transform.GetComponent<NPCStats>().TakeDamage(minDamage, maxDamage, transform.gameObject, true, true, false, debuff: cloneDebuffs);

                        Vector3 dir = other.transform.position - transform.position;
                        other.transform.GetComponent<NPCStats>().knockback(transform.gameObject, dir.normalized, Stats.knockBackPower);
                        AttackedByThisNpc.Add(other);
                    }
                }
            }
            else if (ThisNpc_ai.npcType == NPC_Type.friendly || ThisNpc_ai.npcType == NPC_Type.Minion || ThisNpc_ai.npcType == NPC_Type.neutrality)       //이 npc가 아군일때
            {
                if (!AttackedByThisNpc.Contains(other) && other.GetComponent<NPC_AI>() != null
                    && other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.enemy)       //적군일 경우
                {
                    other.GetComponentInParent<NPCStats>().TakeDamage(minDamage, maxDamage, transform.gameObject, true, true, false, debuff: cloneDebuffs);

                    Vector3 dir = other.transform.position - transform.position;

                    other.transform.GetComponent<NPCStats>().knockback(transform.gameObject, dir.normalized, Stats.knockBackPower);
                    AttackedByThisNpc.Add(other);
                }
            }
        }
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (ThisNpc_ai != null)
        {
            if (ThisNpc_ai.npcType == NPC_AI.NPC_Type.enemy)
            {
                if ((collision.collider.gameObject.tag == "Player" && !AttackedByThisNpc.Contains(collision.collider)))
                {
                    collision.collider.GetComponent<PlayerStats>().TakeDamage(Stats.damage.GetValue(), transform.gameObject, true, true, false);
                    print(Stats.knockBackPower);
                    collision.collider.transform.GetComponent<PlayerStats>().knockback(transform.gameObject, Stats.knockBackPower);
                    AttackedByThisNpc.Add(collision.collider);
                }
                else if (collision.collider.GetComponentInParent<NPC_AI>() != null)
                {
                    if (collision.collider.GetComponentInParent<NPC_AI>().npcType == NPC_AI.NPC_Type.friendly && collision.collider.GetComponent<NPC_AI>() != null
                            && !AttackedByThisNpc.Contains(collision.collider))
                    {
                        //Debug.Log(other);
                        collision.collider.transform.GetComponent<NPCStats>().TakeDamage(Stats.damage.GetValue(), transform.gameObject, true, true, false);
                        collision.collider.transform.GetComponent<NPCStats>().knockback(transform.gameObject, Stats.knockBackPower);
                        AttackedByThisNpc.Add(collision.collider);
                    }
                }
            }
            else if (ThisNpc_ai.npcType == NPC_AI.NPC_Type.friendly)
            {
                if (!AttackedByThisNpc.Contains(collision.collider) && collision.collider.GetComponent<NPC_AI>() != null
                    && collision.collider.GetComponentInParent<NPC_AI>().npcType == NPC_AI.NPC_Type.enemy)
                {
                    collision.collider.GetComponentInParent<NPCStats>().TakeDamage(Stats.damage.GetValue(), transform.gameObject, true, true, false);
                    collision.collider.transform.GetComponent<NPCStats>().knockback(transform.gameObject, Stats.knockBackPower);
                    AttackedByThisNpc.Add(collision.collider);
                }
            }
        }
    }
    */

    public void FunchEnd()
    {
        AttackedByThisNpc.Clear();
    }

    public void ColliderOn()
    {
        foreach(Collider coll in thisCollider)
        {
            coll.enabled = true;
        }
    }

    public void ColliderOff()
    {
        foreach (Collider coll in thisCollider)
        {
            coll.enabled = false;
        }
    }

    public void WeaponTrailOn()
    {
        /*if (Trail)
        {
            Trail.Emit = true;
        }*/
        if(weaponTrail != null)
            weaponTrail.Play();


    }

    public void WeaponTrailOff()
    {
        /*if (Trail)
        {
            Trail.Emit = false;
        }*/
        if (weaponTrail != null)
            weaponTrail.Stop();

        AttackedByThisNpc.Clear();
    }

    public void AttackOn()
    {
        foreach (Collider coll in thisCollider)
        {
            coll.enabled = true;
        }
        //Trail.Emit = true;
    }

    public void AttackOff()
    {
        foreach (Collider coll in thisCollider)
        {
            coll.enabled = false;
        }
        //Trail.Emit = false;

        AttackedByThisNpc.Clear();
    }

    public void GenerateParticle()
    {
        ParticleGenerator.instance.GenerateHitEffect(GetComponentInParent<NPC_AI>().contactPoint, "Hit2");
    }

    public void ChangeDamageMultyfly(float percent)
    {
        DamageMultifly = percent;
    }
}
