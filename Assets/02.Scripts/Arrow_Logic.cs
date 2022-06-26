using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow_Logic : MonoBehaviour
{
    public GameObject target;
    public float arrowSpeed;
    public float deleteTime;
    public float DamageMultifly = 1f;

    public int minDamage;
    public int maxDamage;

    public bool Penetrate = false;
    public Vector3 DirectionAdjustment;
    public Vector3 ArrowDirection;
    public ArrowType arrowType = ArrowType.none;
    public GameObject ExplosionEffect;
    public LayerMask NPCMask;

    [Space]

    public bool canKnockback = false;
    public float knockBackForce;

    private GameObject owner;
    private GameObject player;
    private new Rigidbody rigidbody;
    private new Collider collider;
    private float distToTarget;

    private float addDamage = 0;
 
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        rigidbody = transform.GetComponent<Rigidbody>();
        target = GameObject.FindWithTag("playertarget");
        collider = transform.GetComponent<Collider>();
        distToTarget = Vector3.Distance(transform.position, target.transform.position);

        var dir = (target.transform.position + DirectionAdjustment) - player.transform.position;
        print(distToTarget);
        dir += new Vector3(0, distToTarget / 20,0);
        dir -= new Vector3(0, 1, 0);
        transform.LookAt(target.transform);
        transform.Rotate(new Vector3(90, 0, 0));

        //rigidbody.AddForce(dir.normalized * arrowSpeed);
        //ArrowDirection = dir.normalized;

        Destroy(gameObject, deleteTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<NPC_AI>() != null && other.CompareTag("Enemy"))
        {
            if (other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.enemy || other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.neutrality)
            {
                if (canKnockback)
                    other.transform.GetComponentInParent<CharacterStats>().knockback(gameObject, ArrowDirection, knockBackForce);
                if (arrowType == ArrowType.Steel)
                {
                    //DamageMultifly = 1.5f;
                    addDamage += PrefabCollect.instance.SteelArrowSkill.skillLeveling[owner.GetComponentInChildren<PlayerSkill>().GetSkillLevel(PrefabCollect.instance.SteelArrowSkill)].damageFactor;
                }
                else if (arrowType == ArrowType.Poison)
                { 

                    var buffNDebuff = CreateInstanceBuffOrDebuff.createInstnace(buffType.poison, owner, 3, 5, false,1, 0);

                    other.GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(buffNDebuff);
                }
                
                if (arrowType == ArrowType.Explosion)
                {
                    Explosion();
                }
                else
                {
                    other.transform.GetComponentInParent<CharacterStats>().TakeDamage(Mathf.RoundToInt((minDamage + addDamage) * DamageMultifly),
                        Mathf.RoundToInt((maxDamage + addDamage) * DamageMultifly), owner, true, true, false);
                }

                /*if(owner.GetComponent<PlayerEnchant>())
                    owner.GetComponent<PlayerEnchant>().CheckEnchantAttackEffect(other.GetComponent<CharacterBuffDeBuff>());*/

                if (Penetrate == false)
                {
                    transform.SetParent(other.transform);
                    rigidbody.isKinematic = true;
                    collider.enabled = false;
                    Destroy(gameObject, 5);
                }
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("obstacle"))
        {
            //transform.SetParent(other.transform);
            if (arrowType == ArrowType.Explosion)
            {
                Explosion();
            }
            rigidbody.isKinematic = true;
            collider.enabled = false;
            Destroy(transform.gameObject, deleteTime);
        }

        if (other.GetComponent<DestroyableObject>() != null)
        {
            other.GetComponent<DestroyableObject>().GetDamage(1);
            other.GetComponent<DestroyableObject>().GenerateHitParticle(GetComponent<Collider>().bounds.center);
        }
    }

    public void Fire(Vector3 target, float extraAngleY = 0, float extraAngleX = 0)
    {
        transform.LookAt(target);
        transform.Rotate(new Vector3(90, extraAngleY, 0));

        GetComponentInChildren<Rigidbody>().AddForce(transform.up * arrowSpeed);

        transform.Rotate(new Vector3(extraAngleX, 0, 0));
    }

    public void getOwner(GameObject owner)
    {
        this.owner = owner;

        if(owner.GetComponent<PlayerStats>().playerClass == CharacterClass.Rogue)
        {
            minDamage = Mathf.RoundToInt(owner.GetComponent<PlayerStats>().CrossBowMinDamage.GetFinalStatValue());
            maxDamage = Mathf.RoundToInt(owner.GetComponent<PlayerStats>().CrossBowMaxDamage.GetFinalStatValue());
        }
        else
        {
            minDamage = Mathf.RoundToInt(owner.GetComponent<PlayerStats>().minDamage.GetFinalStatValue());
            maxDamage = Mathf.RoundToInt(owner.GetComponent<PlayerStats>().maxDamage.GetFinalStatValue());
        }
    }

    IEnumerator DeleteObject()
    {
        yield return new WaitForSeconds(deleteTime);
        Destroy(transform.gameObject);
    }

    IEnumerator DeleteObject_AfterManyTime()
    {
        yield return new WaitForSeconds(10);
        Destroy(transform.gameObject);
    }

    private void Explosion()
    {
        float explosionMultiy = PrefabCollect.instance.ExplosionArrowSkill.skillLeveling[owner.GetComponentInChildren<PlayerSkill>().GetSkillLevel(PrefabCollect.instance.ExplosionArrowSkill)].damageFactor;

        int damage = Random.Range(Mathf.RoundToInt(minDamage * explosionMultiy), Mathf.RoundToInt(maxDamage * explosionMultiy));

        AttackEffectFunctions.explode(6, damage, transform.position, ExplosionEffect, NPC_Type.friendly, owner.gameObject, cameraShakeRadius:15);

        /*Collider[] colliders = Physics.OverlapSphere(transform.position, 7, NPCMask);

        float explosionMultiy = PrefabCollect.instance.ExplosionArrowSkill.skillLeveling[owner.GetComponentInChildren<PlayerSkill>().GetSkillLevel(PrefabCollect.instance.ExplosionArrowSkill)].damageFactor;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<NPC_AI>() != null)
            {
                if (colliders[i].GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
                {
                    CharacterStats Cstats = colliders[i].GetComponent<CharacterStats>();

                    Vector3 dir = Cstats.transform.position - transform.position;

                    Cstats.TakeDamage(Mathf.RoundToInt(minDamage * explosionMultiy), Mathf.RoundToInt(maxDamage * explosionMultiy), owner.gameObject, false, true, false);
                    //Cstats.transform.GetComponentInParent<CharacterStats>().knockback(gameObject, dir.normalized, 500);
                }
            }
        }
        var effect = Instantiate(ExplosionEffect, transform.position, transform.rotation);

        Destroy(effect, 3f);*/

        Destroy(transform.gameObject);
    }
}
