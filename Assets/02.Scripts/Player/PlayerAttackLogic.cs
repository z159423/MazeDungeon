using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;



public class PlayerAttackLogic : MonoBehaviour
{
    public ItemType itemType;

    private NPCStats enemystats;
    private PlayerController01 playerController;
    private PlayerStats playerStats;
    private PlayerEnchant playerEnchant;
    private PlayerSkill playerSkill;

    List<Collider> AttackedByPlayerEnemy = new List<Collider>();

    private Vector3 contract;

    //public Skill inUseSkill; 

    void Start()
    {
        playerController = GetComponentInParent<PlayerController01>();
        playerStats = GetComponentInParent<PlayerStats>();
        playerEnchant = GetComponentInParent<PlayerEnchant>();
        playerSkill = GetComponentInParent<PlayerSkill>();
    }
    /*
    void Update()
    {
        /*
        if(playerController.isAttacking == true)
        {
            playerWeaponCollider.enabled = true;
        }
        else
        {
            playerWeaponCollider.enabled = false;
        }
        
    }*/


    private void OnTriggerEnter(Collider other)
    {
        if (playerController.isAttacking.GetBoolValue() == true && other.GetComponentInParent<NPC_AI>() != null)
        {
            if (other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.enemy || other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.neutrality)
            {
                if ((other.gameObject.tag == "Enemy" || other.gameObject.tag == "enemyhitbox") && AttackedByPlayerEnemy.Contains(other) == false)
                {
                    enemystats = other.GetComponentInParent<NPCStats>();

                    //enemystats.Blink();
                    //enemystats.knockback(transform.gameObject, playerStats.knockBackPower);
                    Debug.Log(enemystats.transform + " 를 공격해서 " + playerStats.maxDamage.GetFinalStatValue() + " 의 데미지를 입혔습니다." + other);
                    AttackedByPlayerEnemy.Add(other);

                    //print("나의 각도 : " + Math.GetAngle(transform.position, transform.position + transform.forward));
                    //print("적의 각도 : " + Math.GetAngle(other.transform.position, other.transform.position + other.transform.forward));

                    var weapon = EquipmentManager.instance.currentEquipment[(int)itemType];

                    if (playerSkill.usingSkillObject == null && playerStats.playerClass == CharacterClass.Warrior)
                    {
                        playerStats.GetRage((int)Mathf.Round(playerStats.GetRageAtAutoAttackValue.GetFinalStatValueAsMultiflyFloat()));
                        print("플레이어가 공격해서 Rage를 획득함 " + playerStats.GetRageAtAutoAttackValue);
                    }

                    /*if (playerEnchant)
                        playerEnchant.CheckEnchantAttackEffect(other.GetComponent<CharacterBuffDeBuff>());*/

                    float damageMultify = 1;

                    if (playerSkill.usingSkillObject != null)
                    {
                        if(playerSkill.usingSkillObject.skillLeveling[playerSkill.GetSkillLevel(playerSkill.usingSkillObject)].buffNDebuffObject)
                        {
                            var clone = Instantiate(playerSkill.usingSkillObject.skillLeveling[playerSkill.GetSkillLevel(playerSkill.usingSkillObject)].buffNDebuffObject) as BuffNDebuffObject;
                            if(other.GetComponent<CharacterBuffDeBuff>())
                                other.GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(clone);
                        }


                        /*if (playerSkill.usingSkill.BufforDebuff.debuff == buffType.poison)
                        {
                            bufforDebuff poison = new bufforDebuff();

                            poison.Duration = playerSkill.usingSkill.BufforDebuff.Duration;
                            poison.debuff = playerSkill.usingSkill.BufforDebuff.debuff;
                            poison.fromBy = transform.GetComponentInParent<PlayerStats>().gameObject;
                            poison.poisonDamage = playerSkill.usingSkill.BufforDebuff.poisonDamage;

                            enemystats.GetBuffAndDebuff(poison);
                        }

                        if (playerSkill.usingSkill.BufforDebuff.debuff == buffType.stun)
                        {
                            print("stun!");
                            bufforDebuff stun = new bufforDebuff();

                            stun.Duration = playerSkill.usingSkill.BufforDebuff.Duration;
                            stun.debuff = playerSkill.usingSkill.BufforDebuff.debuff;
                            stun.fromBy = transform.GetComponentInParent<PlayerStats>().gameObject;

                            enemystats.GetBuffAndDebuff(stun);
                        }

                        if (playerSkill.usingSkill.BufforDebuff.debuff == buffType.Maimed)
                        {
                            print("maimed!");
                            bufforDebuff maimed = new bufforDebuff();

                            maimed.Duration = playerSkill.usingSkill.BufforDebuff.Duration;
                            maimed.debuff = playerSkill.usingSkill.BufforDebuff.debuff;
                            maimed.fromBy = transform.GetComponentInParent<PlayerStats>().gameObject;

                            enemystats.GetBuffAndDebuff(maimed);
                        }*/


                        if (playerSkill.usingSkillObject.Name == "CoupDeGrace")
                        {
                            enemystats.TakeDamage((int)Mathf.Round((int)weapon.minDamageModifier * (((playerStats.playerCurrentRage / 100) + 1) * 1.5f))
                                , (int)Mathf.Round((int)weapon.maxDamageModifier * (((playerStats.playerCurrentRage / 100) + 1) * 1.5f)), transform.GetComponentInParent<PlayerStats>().gameObject, true, true, false);

                            damageMultify = (((playerStats.playerCurrentRage / 100) + 1) * 1.5f);
                        }
                        else
                        {
                            damageMultify = playerSkill.usingSkillObject.skillLeveling[playerSkill.GetSkillLevel(playerSkill.usingSkillObject)].damageFactor;
                        }
                    }

                    enemystats.TakeDamage(Mathf.RoundToInt(playerStats.minDamage.GetFinalStatValue() * damageMultify)
                        , Mathf.RoundToInt(playerStats.maxDamage.GetFinalStatValue() * damageMultify)
                        , transform.GetComponentInParent<PlayerStats>().gameObject, true, true, false);

                    //Invoke("GenerateParticle", 0.05f);

                    ParticleGenerator.instance.GenerateHitEffect(other.ClosestPoint(GetComponent<Collider>().bounds.center), "Hit2");


                    if (weapon != null && weapon.canBreakable == true)
                    {
                        weapon.currentLimit--;
                        EquipmentManager.instance.DestroyEquipment(gameObject);
                    }

                }
            }
        }

        if(playerController.isAttacking.GetBoolValue() == true && other.GetComponent<DestroyableObject>() != null)
        {
            other.GetComponent<DestroyableObject>().GetDamage(1);
            other.GetComponent<DestroyableObject>().GenerateHitParticle(GetComponent<Collider>().bounds.center);
            AttackedByPlayerEnemy.Add(other);
        }
    }

    /*
    private void OnCollisionEnter(Collision other)
    {
        if (playerController.isAttacking == true && other.collider.GetComponent<NPC_AI>() != null)
        {
            if (other.collider.GetComponentInParent<NPC_AI>().npcType == NPC_AI.NPC_Type.enemy)
            {
                if ((other.gameObject.tag == "Enemy" || other.gameObject.tag == "enemyhitbox") && AttackedByPlayerEnemy.Contains(other.collider) == false)
                {
                    enemystats = other.collider.GetComponentInParent<EnemyStats>();

                    //enemystats.Blink();
                    enemystats.knockback(transform.gameObject, playerStats.knockBackPower);
                    Debug.Log(enemystats.transform + " 를 공격해서 " + playerStats.damage.GetValue() + " 의 데미지를 입혔습니다." + other.collider);
                    AttackedByPlayerEnemy.Add(other.collider);

                    var weapon = EquipmentManager.instance.currentEquipment[(int)itemType];

                    if (inUseSkill != null)
                    {

                        if (inUseSkill.BufforDebuff.debuff == Debuff.poison)
                        {
                            bufforDebuff poison = new bufforDebuff();

                            poison.Duration = inUseSkill.BufforDebuff.Duration;
                            poison.debuff = inUseSkill.BufforDebuff.debuff;
                            poison.fromBy = transform.GetComponentInParent<PlayerStats>().gameObject;
                            poison.poisonDamage = inUseSkill.BufforDebuff.poisonDamage;

                            enemystats.GetBuffAndDebuff(poison);
                        }
                        enemystats.TakeDamage((int)((int)weapon.damageModifier * inUseSkill.damageFactor), transform.GetComponentInParent<PlayerStats>().gameObject, true, true);
                    }
                    else
                    {
                        enemystats.TakeDamage((int)weapon.damageModifier, transform.GetComponentInParent<PlayerStats>().gameObject, true, true);
                    }



                }
            }
        }
    }*/

    
    public void GenerateParticle()
    {
        ParticleGenerator.instance.GenerateHitEffect(GetComponentInParent<PlayerController01>().contactPoint, "Hit2");
    }

    public void attackedByPlayerEnemyClear()
    {
        AttackedByPlayerEnemy.Clear();
    }
}
