using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CharacterBuffDeBuff : MonoBehaviour
{
    public List<BuffNDebuffObject> buffNDebuffObjectsList = new List<BuffNDebuffObject>();

    private CharacterStats Stats;
    private NPC_AI Ai;
    private NPC_Type npcType;
    public SkinnedMeshRenderer skinned;

    [Space]
    public Transform stunParticlePos;

    void Start()
    {
        Stats = GetComponent<CharacterStats>();
        Ai = GetComponent<NPC_AI>();

        if(Ai != null)
        {
            npcType = Ai.npcType;
        }
        else
        {
            if (GetComponent<PlayerStats>() != null)
                npcType = NPC_Type.friendly;
        }
        if(!skinned)
            skinned = GetComponentInChildren<SkinnedMeshRenderer>();
    }


    public void AddBuffOrDebuff(BuffNDebuffObject newBuffOrDebuff)
    {
        var clone = Instantiate(newBuffOrDebuff) as BuffNDebuffObject;

        if (!clone.buffOrDebuff.enableStack)
        {
            foreach (BuffNDebuffObject buffNDebuffObject in buffNDebuffObjectsList)
            {
                if (buffNDebuffObject.buffOrDebuff.buffDebuff == clone.buffOrDebuff.buffDebuff)
                {
                    buffNDebuffObject.buffOrDebuff.UpdateThis(clone.buffOrDebuff);
                    return;
                }
            }
        }

        if (GetComponent<PlayerStats>() != null)
            BuffDebuffIcon.GenerateNewIcon(UIManager.instance.buffDebuffIconParent, clone);

        StartCoroutine(StartTimeRunning(clone));
    }

    IEnumerator StartTimeRunning(BuffNDebuffObject Object)
    {
        bool success = GetStartEffect(Object);

        if(success)
        {
            while (Object.buffOrDebuff.GetCurrentRunningTime() > 0)
            {
                yield return new WaitForSeconds(Object.buffOrDebuff.RepeatTime);
                GetWhileEffect(Object);
                Object.buffOrDebuff.TickRunningTime(Object.buffOrDebuff.RepeatTime);
                
            }

            EndEffect(Object);
        }
    }

    private bool GetStartEffect(BuffNDebuffObject Object)
    {
        buffNDebuffObjectsList.Add(Object);
        Object.buffOrDebuff.SetCurrentRunningTimeToStartTime();

        bool succese = true;

        switch (Object.buffOrDebuff.buffDebuff)
        {
            case buffType.poison:
                PoisonStartEffect(Object);

                break;

            case buffType.Burn:
                BurnStartEffect(Object);
                break;

            case buffType.stun:
                succese = StunStartEffect(Object);
                break;

            case buffType.Restraint:

                break;

            case buffType.Maimed:
                MaimedStartEffect(Object);
                break;

            case buffType.SlowDown:
                succese = SlowDownStartEffect(Object);
                break;

            case buffType.HealthRegenerate:
                HealthRegenerationStartEffect(Object);
                break;

            case buffType.DamageIncrease:
                DamageIncreaseStartEffect(Object);
                break;

            case buffType.StunImmunity:
                StunImmunityStartEffect(Object);
                break;

            case buffType.SlowDownImmunity:
                SlowDownImmunityStartEffect(Object);
                break;

            case buffType.DecreaseDefense:
                DecreseDefenseStartEffect(Object);
                break;

            case buffType.DefenceIncrease:
                IncreseDefenseStartEffect(Object);
                break;

            case buffType.ManaRegenerateSpeedUp:
                IncreseDefenseStartEffect(Object);
                break;

            case buffType.IncreaseAttackSpeed:
                IncreseAttackSpeedStartEffect(Object);
                break;

            case buffType.DecreaseAttackSpeed:
                DecreseAttackSpeedStartEffect(Object);
                break;

            case buffType.DecreaseDamage:
                DecreseDamageStartEffect(Object);
                break;

            case buffType.MoveSpeedIncrease:
                IncreaseMoveSpeedStartEffect(Object);
                break;

            case buffType.MoveSpeedDecrease:
                DecreaseMoveSpeedStartEffect(Object);
                break;
        }

        return succese;
    }

    private void GetWhileEffect(BuffNDebuffObject Object)
    {
        switch(Object.buffOrDebuff.buffDebuff)
        {
            case buffType.poison:
                PoisonWhileEffect(Object);

                break;

            case buffType.Burn:
                BurnWhileEffect(Object);
                break;

            case buffType.HealthRegenerate:
                HealthRegenerationWhileEffect(Object);
                break;
        }
    }

    private void EndEffect(BuffNDebuffObject Object)
    {
        Debug.Log(Object.buffOrDebuff.buffDebuff + " 의 효과가 끝남");
        buffNDebuffObjectsList.Remove(Object);

        BuffDebuffIcon.DeleteIcon(Object);

        switch (Object.buffOrDebuff.buffDebuff)
        {
            case buffType.poison:
                PoisonEndEffect(Object);
                break;

            case buffType.Burn:
                BurnEndEffect(Object);
                break;

            case buffType.stun:
                StunEndEffect(Object);
                break;

            case buffType.Restraint:

                break;

            case buffType.Maimed:
                MaimedEndEffect(Object);
                break;

            case buffType.SlowDown:
                SlowDownEndEffect(Object);
                break;

            case buffType.HealthRegenerate:
                HealthRegenerationEndEffect(Object);
                break;

            case buffType.DamageIncrease:
                DamageIncreaseEndEffect(Object);
                break;

            case buffType.StunImmunity:
                StunImmunityEndEffect(Object);
                break;

            case buffType.SlowDownImmunity:
                SlowDownImmunityEndEffect(Object);
                break;

            case buffType.DecreaseDefense:
                DecreseDefenseEndEffect(Object);
                break;

            case buffType.DefenceIncrease:
                IncreseDefenseEndEffect(Object);
                break;

            case buffType.IncreaseAttackSpeed:
                IncreseAttackSpeedEndEffect(Object);
                break;

            case buffType.DecreaseAttackSpeed:
                DecreseAttackSpeedEndEffect(Object);
                break;

            case buffType.DecreaseDamage:
                DecreseDamageEndEffect(Object);
                break;

            case buffType.MoveSpeedIncrease:
                IncreseMoveSpeedEndEffect(Object);
                break;

            case buffType.MoveSpeedDecrease:
                DecreaseMoveSpeedEndEffect(Object);
                break;
        }
    }

    private void PoisonStartEffect(BuffNDebuffObject Object)
    {
        var Particle = Instantiate(PrefabCollect.instance.PoisonParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned; 

        Object.buffOrDebuff.ParticleEffect = Particle;

        //FloatingTextController.CreateFloatingText("Poison", transform, transform.gameObject.tag, 15, gameObject, true, owner : Stats);
        FloatingTextController.GenerateDebuffFloatingText("Poison", transform, 15, npcType, owner: gameObject);
    }

    private void PoisonWhileEffect(BuffNDebuffObject Object)
    {
        Stats.TakeDamage(Mathf.RoundToInt(Object.buffOrDebuff.Damage)
                    ,Mathf.RoundToInt(Object.buffOrDebuff.Damage)
                    , Object.buffOrDebuff.Owner, false, false, true, true,true);
    }

    private void PoisonEndEffect(BuffNDebuffObject Object)
    {
        Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);

        Destroy(Object.buffOrDebuff.ParticleEffect, 5); 
    }

    private void BurnStartEffect(BuffNDebuffObject Object)
    {
        var Particle = Instantiate(PrefabCollect.instance.BurnParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        Object.buffOrDebuff.ParticleEffect = Particle;

        //FloatingTextController.CreateFloatingText("Burn", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
        FloatingTextController.GenerateDebuffFloatingText("Burn", transform, 15, npcType, owner: gameObject);
    }

    private void BurnWhileEffect(BuffNDebuffObject Object)
    {
        Stats.TakeDamage((int)Mathf.Round(Object.buffOrDebuff.Damage)
                    , (int)Mathf.Round(Object.buffOrDebuff.Damage)
                    , Object.buffOrDebuff.Owner, false, false, true, true, true);
    }

    private void BurnEndEffect(BuffNDebuffObject Object)
    {
        if (Object.buffOrDebuff.ParticleEffect)
        {
            Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);
            Destroy(Object.buffOrDebuff.ParticleEffect, 5);
        }
    }


    private bool StunStartEffect(BuffNDebuffObject Object)
    {
        if(GetComponent<CharacterStats>().StunImmunity.GetBoolValue())
        {
            FloatingTextController.GenerateBuffFloatingText("StunImmunization", transform, 15, npcType, owner: gameObject);
            //FloatingTextController.GenerateDebuffFloatingText("Stun", transform, 15, npcType, owner: gameObject);
            BuffDebuffIcon.DeleteIcon(Object);
            buffNDebuffObjectsList.Remove(Object);
            return false;
        }

        GameObject Particle;

        if(GetComponent<HealthUI>())
        {
            Particle = Instantiate(PrefabCollect.instance.StunParticle, GetComponent<HealthUI>().target.position, Quaternion.identity, transform);
        }
        else
        {
            Particle = Instantiate(PrefabCollect.instance.StunParticle, stunParticlePos.position, Quaternion.identity, transform);
        }
            
        Object.buffOrDebuff.ParticleEffect = Particle;

        Stats.isStun.AddBoolModifier();

        if(GetComponent<UnityEngine.Animations.Rigging.RigBuilder>())
        {
            if (GetComponent<PlayerController01>())
            {

            }
            else
            {
                if (GetComponent<Animator>().applyRootMotion)
                {

                }
                else
                {
                    GetComponent<UnityEngine.Animations.Rigging.RigBuilder>().enabled = false;
                }

            }
            
        }

        //FloatingTextController.CreateFloatingText("Stun", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
        FloatingTextController.GenerateDebuffFloatingText("Stun", transform, 15, npcType, owner: gameObject);

        GetComponent<Animator>().SetBool("Stun", true);

        return true;
    }

    private void StunEndEffect(BuffNDebuffObject Object)
    {
        Stats.isStun.RemoveBoolModifier();

        if (GetComponent<UnityEngine.Animations.Rigging.RigBuilder>())
        {
            GetComponent<UnityEngine.Animations.Rigging.RigBuilder>().enabled = true;
        }

        if(Object.buffOrDebuff.ParticleEffect)
        {
            Object.buffOrDebuff.ParticleEffect.GetComponentInChildren<ParticleSystem>().Stop(true);
            Destroy(Object.buffOrDebuff.ParticleEffect);
        }
            
        GetComponent<Animator>().SetBool("Stun", false);
    }

    private bool SlowDownStartEffect(BuffNDebuffObject Object)
    {
        if (GetComponent<CharacterStats>().SlowDownImmunity.GetBoolValue())
        {
            //FloatingTextController.CreateFloatingText("SlowDownImmunization", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            //FloatingTextController.GenerateDebuffFloatingText("SlowDown", transform, 15, npcType, owner: gameObject);
            //BuffDebuffIcon.DeleteIcon(Object);

            FloatingTextController.GenerateBuffFloatingText("SlowDownImmunization", transform, 15, npcType, owner: gameObject);
            //FloatingTextController.GenerateDebuffFloatingText("Stun", transform, 15, npcType, owner: gameObject);
            BuffDebuffIcon.DeleteIcon(Object);
            buffNDebuffObjectsList.Remove(Object);
            return false;
        }

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().MoveSpeedStat.AddPercentModifier(Object.buffOrDebuff.Percent);

            //FloatingTextController.CreateFloatingText("SlowDown", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateDebuffFloatingText("SlowDown", transform, 15, npcType, owner: gameObject);
        }
        else
        {
            Ai.moveSpeed.AddPercentModifier(Object.buffOrDebuff.Percent);
            Ai.AddMoveAnimationSpeed(Object.buffOrDebuff.Percent);
            //FloatingTextController.CreateFloatingText("SlowDown", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateDebuffFloatingText("SlowDown", transform, 15, npcType, owner: gameObject);
        }

        var Particle = Instantiate(PrefabCollect.instance.SlowDownParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        Object.buffOrDebuff.ParticleEffect = Particle;

        return true;
    }

    private void SlowDownEndEffect(BuffNDebuffObject Object)
    {
        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().MoveSpeedStat.RemovePercentModifier(Object.buffOrDebuff.Percent);

        }
        else
        {
            Ai.moveSpeed.RemovePercentModifier(Object.buffOrDebuff.Percent);
            Ai.RemoveMoveAnimationSpeed(Object.buffOrDebuff.Percent);
        }

        Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);
        Destroy(Object.buffOrDebuff.ParticleEffect, 5);
    }

    private void MaimedStartEffect(BuffNDebuffObject Object)
    {
        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().MoveSpeedStat.AddPercentModifier(Object.buffOrDebuff.Percent);
            GetComponent<PlayerStats>().AttackSpeed.AddPercentModifier(Object.buffOrDebuff.Percent);

            //FloatingTextController.CreateFloatingText("Maimed", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateDebuffFloatingText("Maimed", transform, 15, npcType, owner: gameObject);
        }
        else
        {
            Ai.moveSpeed.AddPercentModifier(Object.buffOrDebuff.Percent);
            Ai.AddMoveAnimationSpeed(Object.buffOrDebuff.Percent);
            Ai.AddAttackAnimationSpeed(Object.buffOrDebuff.Percent);
            //FloatingTextController.CreateFloatingText("Maimed", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateDebuffFloatingText("Maimed", transform, 15, npcType, owner: gameObject);
        }

        var Particle = Instantiate(PrefabCollect.instance.MaimedParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        Object.buffOrDebuff.ParticleEffect = Particle;
    }

    private void MaimedEndEffect(BuffNDebuffObject Object)
    {
        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().MoveSpeedStat.RemovePercentModifier(Object.buffOrDebuff.Percent);
            GetComponent<PlayerStats>().AttackSpeed.RemovePercentModifier(Object.buffOrDebuff.Percent);
        }
        else
        {
            Ai.moveSpeed.RemovePercentModifier(Object.buffOrDebuff.Percent);
            Ai.RemoveMoveAnimationSpeed(Object.buffOrDebuff.Percent);
            Ai.RemoveAttackAnimationSpeed(Object.buffOrDebuff.Percent);
        }
        Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);
        Destroy(Object.buffOrDebuff.ParticleEffect, 5);
    }

    private bool DecreseDefenseStartEffect(BuffNDebuffObject Object)
    {

        var Particle = Instantiate(PrefabCollect.instance.defenseDecreaseParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        Object.buffOrDebuff.ParticleEffect = Particle;

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().armor.AddIntModifier((int)Object.buffOrDebuff.value1);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateDebuffFloatingText("DecreaseDefense", transform, 15, npcType, owner: gameObject);
        }
        else
        {
            Stats.armor.AddIntModifier((int)Object.buffOrDebuff.value1);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateDebuffFloatingText("DecreaseDefense", transform, 15, npcType, owner: gameObject);
        }
        return true;
    }

    private void DecreseDefenseEndEffect(BuffNDebuffObject Object)
    {
        if (Object.buffOrDebuff.ParticleEffect)
        {
            Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);
            Destroy(Object.buffOrDebuff.ParticleEffect, 5);
        }

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().armor.RemoveIntModifier((int)Object.buffOrDebuff.value1);
        }
        else
        {
            Stats.armor.RemoveIntModifier((int)Object.buffOrDebuff.value1);
        }
    }







    private void HealthRegenerationStartEffect(BuffNDebuffObject Object)
    {
        var Particle = Instantiate(PrefabCollect.instance.HealthGenerationParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        Object.buffOrDebuff.ParticleEffect = Particle;

        //FloatingTextController.CreateFloatingText("HealthRegeneration", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
        FloatingTextController.GenerateBuffFloatingText("HealthRegeneration", transform, 15, npcType, owner: gameObject);
    }

    private void HealthRegenerationWhileEffect(BuffNDebuffObject Object)
    {
        Stats.Heal(Mathf.RoundToInt(Object.buffOrDebuff.Damage));
    }

    private void HealthRegenerationEndEffect(BuffNDebuffObject Object)
    {
        Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);

        Destroy(Object.buffOrDebuff.ParticleEffect, 5);
    }

    private void DamageIncreaseStartEffect(BuffNDebuffObject Object)
    {
        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().minDamage.AddPercentModifier(Object.buffOrDebuff.Percent);
            GetComponent<PlayerStats>().maxDamage.AddPercentModifier(Object.buffOrDebuff.Percent);

            //FloatingTextController.CreateFloatingText("DamageIncrease", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateBuffFloatingText("DamageIncrease", transform, 15, npcType, owner: gameObject);
        }
        else
        {
            Stats.minDamage.AddPercentModifier(Object.buffOrDebuff.Percent);
            Stats.maxDamage.AddPercentModifier(Object.buffOrDebuff.Percent);
            FloatingTextController.GenerateBuffFloatingText("DamageIncrease", transform, 15, npcType, owner: gameObject);
        }

        var Particle = Instantiate(PrefabCollect.instance.DamageIncreaseParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        Object.buffOrDebuff.ParticleEffect = Particle;
    }

    private void DamageIncreaseEndEffect(BuffNDebuffObject Object)
    {
        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().minDamage.RemovePercentModifier(Object.buffOrDebuff.Percent);
            GetComponent<PlayerStats>().maxDamage.RemovePercentModifier(Object.buffOrDebuff.Percent);
        }
        else
        {
            Stats.minDamage.RemovePercentModifier(Object.buffOrDebuff.Percent);
            Stats.maxDamage.RemovePercentModifier(Object.buffOrDebuff.Percent);
        }

        Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);
        Destroy(Object.buffOrDebuff.ParticleEffect, 5);
    }

    private void StunImmunityStartEffect(BuffNDebuffObject Object)
    {
        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().StunImmunity.AddBoolModifier();

            FloatingTextController.GenerateBuffFloatingText("StunImmunization", transform, 15, npcType, owner: gameObject);
        }
        else
        {
            Stats.StunImmunity.AddBoolModifier();
        }
    }

    private void StunImmunityEndEffect(BuffNDebuffObject Object)
    {
        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().StunImmunity.RemoveBoolModifier();
        }
        else
        {
            Stats.StunImmunity.RemoveBoolModifier();
        }
    }

    private void SlowDownImmunityStartEffect(BuffNDebuffObject Object)
    {
        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().SlowDownImmunity.AddBoolModifier();

            FloatingTextController.GenerateBuffFloatingText("SlowDownImmunization", transform, 15, npcType, owner: gameObject);
        }
        else
        {
            Stats.SlowDownImmunity.AddBoolModifier();
        }
    }

    private void SlowDownImmunityEndEffect(BuffNDebuffObject Object)
    {
        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().SlowDownImmunity.RemoveBoolModifier();
        }
        else
        {
            Stats.SlowDownImmunity.RemoveBoolModifier();
        }
    }

    private bool IncreseDefenseStartEffect(BuffNDebuffObject Object)
    {

        var Particle = Instantiate(PrefabCollect.instance.defenseIncreseParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        Object.buffOrDebuff.ParticleEffect = Particle;

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().armor.AddIntModifier((int)Object.buffOrDebuff.value1);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateBuffFloatingText("IncreseDefense", transform, 15, npcType, owner: gameObject);
        }
        else
        {
            Stats.armor.AddIntModifier((int)Object.buffOrDebuff.value1);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateBuffFloatingText("IncreseDefense", transform, 15, npcType, owner: gameObject);
        }
        return true;
    }

    private void IncreseDefenseEndEffect(BuffNDebuffObject Object)
    {
        if (Object.buffOrDebuff.ParticleEffect)
        {
            Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);
            Destroy(Object.buffOrDebuff.ParticleEffect, 5);
        }

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().armor.RemoveIntModifier((int)Object.buffOrDebuff.value1);
        }
        else
        {
            Stats.armor.RemoveIntModifier((int)Object.buffOrDebuff.value1);
        }
    }

    private bool DecreseDamageStartEffect(BuffNDebuffObject Object)
    {

        var Particle = Instantiate(PrefabCollect.instance.damageDecreaseParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        Object.buffOrDebuff.ParticleEffect = Particle;

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().minDamage.AddPercentModifier(Object.buffOrDebuff.value1);
            GetComponent<PlayerStats>().maxDamage.AddPercentModifier(Object.buffOrDebuff.value1);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateDebuffFloatingText("DecreaseDamage", transform, 15, npcType, owner: gameObject);
        }
        else
        {
            Stats.minDamage.AddPercentModifier(Object.buffOrDebuff.value1);
            Stats.maxDamage.AddPercentModifier(Object.buffOrDebuff.value1);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateDebuffFloatingText("DecreaseDamage", transform, 15, npcType, owner: gameObject);
        }
        return true;
    }

    private void DecreseDamageEndEffect(BuffNDebuffObject Object)
    {
        if (Object.buffOrDebuff.ParticleEffect)
        {
            Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);
            Destroy(Object.buffOrDebuff.ParticleEffect, 5);
        }

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().minDamage.RemovePercentModifier(Object.buffOrDebuff.value1);
            GetComponent<PlayerStats>().maxDamage.RemovePercentModifier(Object.buffOrDebuff.value1);
        }
        else
        {
            Stats.minDamage.RemovePercentModifier(Object.buffOrDebuff.value1);
            Stats.maxDamage.RemovePercentModifier(Object.buffOrDebuff.value1);
        }
    }

    private bool IncreseAttackSpeedStartEffect(BuffNDebuffObject Object)
    {

        var Particle = Instantiate(PrefabCollect.instance.DamageIncreaseParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        Object.buffOrDebuff.ParticleEffect = Particle;

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().AttackSpeed.AddPercentModifier(Object.buffOrDebuff.value1);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateBuffFloatingText("IncreaseAttackSpeed", transform, 15, npcType, owner: gameObject);
        }
        else
        {
            Stats.GetComponent<NPC_AI>().AddAttackAnimationSpeed(Object.buffOrDebuff.value1);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateBuffFloatingText("IncreaseAttackSpeed", transform, 15, npcType, owner: gameObject);
        }
        return true;
    }

    private void IncreseAttackSpeedEndEffect(BuffNDebuffObject Object)
    {
        if (Object.buffOrDebuff.ParticleEffect)
        {
            Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);
            Destroy(Object.buffOrDebuff.ParticleEffect, 5);
        }

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().AttackSpeed.RemovePercentModifier(Object.buffOrDebuff.value1);
        }
        else
        {
            Stats.GetComponent<NPC_AI>().RemoveAttackAnimationSpeed(Object.buffOrDebuff.value1);
        }
    }

    private bool DecreseAttackSpeedStartEffect(BuffNDebuffObject Object)
    {

        var Particle = Instantiate(PrefabCollect.instance.damageDecreaseParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        Object.buffOrDebuff.ParticleEffect = Particle;

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().AttackSpeed.AddPercentModifier(Object.buffOrDebuff.value1);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateDebuffFloatingText("DecreaseAttackSpeed", transform, 15, npcType, owner: gameObject);
        }
        else
        {
            Stats.GetComponent<NPC_AI>().AddAttackAnimationSpeed(Object.buffOrDebuff.value1);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateDebuffFloatingText("DecreaseAttackSpeed", transform, 15, npcType, owner: gameObject);
        }
        return true;
    }

    private void DecreseAttackSpeedEndEffect(BuffNDebuffObject Object)
    {
        if (Object.buffOrDebuff.ParticleEffect)
        {
            Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);
            Destroy(Object.buffOrDebuff.ParticleEffect, 5);
        }

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().AttackSpeed.RemovePercentModifier(Object.buffOrDebuff.value1);

        }
        else
        {
            Stats.GetComponent<NPC_AI>().RemoveAttackAnimationSpeed(Object.buffOrDebuff.value1);
        }
    }

    private bool IncreaseMoveSpeedStartEffect(BuffNDebuffObject Object)
    {

        var Particle = Instantiate(PrefabCollect.instance.MoveSpeedIncreaseParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        Object.buffOrDebuff.ParticleEffect = Particle;

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().MoveSpeedStat.AddPercentModifier(Object.buffOrDebuff.value1);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateBuffFloatingText("IncreaseMoveSpeed", transform, 15, npcType, owner: gameObject);
        }
        else
        {
            Stats.GetComponent<NPC_AI>().moveSpeed.AddPercentModifier(Object.buffOrDebuff.value1);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateBuffFloatingText("IncreaseMoveSpeed", transform, 15, npcType, owner: gameObject);
        }
        return true;
    }

    private void IncreseMoveSpeedEndEffect(BuffNDebuffObject Object)
    {
        if (Object.buffOrDebuff.ParticleEffect)
        {
            Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);
            Destroy(Object.buffOrDebuff.ParticleEffect, 5);
        }

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().MoveSpeedStat.RemovePercentModifier(Object.buffOrDebuff.value1);
        }
        else
        {
            Stats.GetComponent<NPC_AI>().moveSpeed.RemovePercentModifier(Object.buffOrDebuff.value1);
        }
    }

    private bool DecreaseMoveSpeedStartEffect(BuffNDebuffObject Object)
    {

        var Particle = Instantiate(PrefabCollect.instance.moveSpeedDecreaseParticle);
        var shape = Particle.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        Object.buffOrDebuff.ParticleEffect = Particle;

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().MoveSpeedStat.AddPercentModifier(Object.buffOrDebuff.Percent);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateDebuffFloatingText("DecreaseMoveSpeed", transform, 15, npcType, owner: gameObject);
        }
        else
        {
            Stats.GetComponent<NPC_AI>().moveSpeed.AddPercentModifier(Object.buffOrDebuff.Percent);

            //FloatingTextController.CreateFloatingText("DecreaseDefense", transform, transform.gameObject.tag, 15, gameObject, true, owner: Stats);
            FloatingTextController.GenerateDebuffFloatingText("DecreaseMoveSpeed", transform, 15, npcType, owner: gameObject);
        }
        return true;
    }

    private void DecreaseMoveSpeedEndEffect(BuffNDebuffObject Object)
    {
        if (Object.buffOrDebuff.ParticleEffect)
        {
            Object.buffOrDebuff.ParticleEffect.GetComponent<ParticleSystem>().Stop(true);
            Destroy(Object.buffOrDebuff.ParticleEffect, 5);
        }

        if (GetComponent<PlayerStats>())
        {
            GetComponent<PlayerStats>().MoveSpeedStat.RemovePercentModifier(Object.buffOrDebuff.value1);
        }
        else
        {
            Stats.GetComponent<NPC_AI>().moveSpeed.RemovePercentModifier(Object.buffOrDebuff.value1);
        }
    }



    public void UntilStageEndBuffEnd()
    {
        for(int i = 0; i < buffNDebuffObjectsList.Count; i++)
        {
            if(buffNDebuffObjectsList[i].buffOrDebuff.untilTheNextStage)
            {
                buffNDebuffObjectsList[i].buffOrDebuff.SetCurrentRunningTimeToEndTime();
            }
        }
    }

    private void OnDisable()
    {
        foreach (BuffNDebuffObject Object in buffNDebuffObjectsList)
        {
            if(Object.buffOrDebuff.ParticleEffect)
            {
                Destroy(Object.buffOrDebuff.ParticleEffect);
            }
        }
    }
}


