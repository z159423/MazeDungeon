using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnchant : MonoBehaviour
{
   
    public List<Enchant> playerEnchants = new List<Enchant>();

    public List<Enchants> StackEnchants = new List<Enchants>();
    
    private PlayerStats playerStats;
    private PlayerController01 playerController;

    [Space]

    public EnchantSetting[] enchantSettings;

    [Space]

    const float DamageIncreaseInitialValue = .1f;
    public float DamageIncreaseValue = 1;

    [Space]
    const float AttackSpeedIncreaseInitialValue = .1f;
    public float AttackSpeedIncreseValue = 1;

    [Space]
    const float LeechListInitialValue = 2;
    public float LeechLifeValue = 0;

    [Space]
    const float SlowDownInitialPercent = .1f;
    const float SlowDownInitialValue = .2f;
    public float SlowDownPercent = 0;
    public float SlowDownValue = 0;

    [Space]
    const float BindingInitialPercent = .05f;
    const float BindingInitialTime = 1;
    public float BindingPercent = 0;
    public float BindingTime = 0;

    [Space]
    const float PoisoningInitialPercent = .2f;
    const float PoisoningInitialTime = 2;
    public float PoisoningPercent = 0;
    public float PoisoningTime = 0;

    [Space]
    const float BurnInitialPercent = .2f;
    const float BurnInitialTime = 2;
    public float BurnPercent = 0;
    public float BurnTime = 0;

    [Space]
    const float StunInitialPercent = 0.05f;
    const float StunInitialTime = 1;
    public float StunPercent = 0;
    public float StunTime = 0;

    [Space]
    const float ElectricShockInitialPercent = .1f;
    const float ElectricShockDamageInitialValue = .25f;
    public float ElectricShockPercent = 0;
    public float ElectricShockDamageValue = 0;

    [Space]
    const float ExtraGainGoldInitialValue = .15f;
    public float ExtraGainGoldValue = 1;

    [Space]
    const float CriticalChanceIncreaseInitialValue = .05f;
    public float CriticalChanceIncreaseValue = 0;

    [Space]
    const float WideAttackInitialPercent = .05f;
    const float WideAttackDamageInitialValue = .3f;
    public float WideAttackPercent = 0;
    public float WideAttackDamageValue = 0;

    [Space]
    const float DoubleShotInitialPercent = .1f;
    public float DoubleShotPercent = 0;

    [Space]
    const float PenetrateShotInitialPercent = .15f;
    public float PenetrateShotPercent = 0;

    [Space]
    const float AssassinationInitialValue = .5f;
    public float AssassinationValue = 1;

    [Space]
    const float SturdyShieldInitialValue = .15f;
    public float SturdyShieldValue = 1;

    [Space]
    const float IncreaseArmorInitialValue = .1f;
    public float IncreaseArmorValue = 1;

    [Space]
    const float IncreaseMaxHealthInitialValue = .05f;
    public float IncreaseMaxHealthValue = 1;

    [Space]
    const float IncreaseStaminaInitialValue = .1f;
    public float IncreaseStaminaValue = 1;

    [Space]
    const float IncreaseMaxManaInitialValue = .1f;
    public float IncreaseMaxManaValue = 1;

    [Space]
    const float QuickStealthInitialValue = .15f;
    public float QuickStealthValue = 1;

    [Space]
    const float BerserkerInitialValue = .1f;
    public float BerserkerValue = 1;

    [Space]
    const float IncreaseManaRecoveryInitialValue = .1f;
    public float IncreaseManaRecoveryValue = 1;

    [Space]
    const float ThornInitialPercent = .25f;
    const float ThornInitialValue = .33f;
    public float ThornPercent = 0;
    public float ThornValue = 0;

    [Space]
    const float FrostArmorInitialValue = 1;
    const float MaxFrostArmorInitialValue = 10;
    public float FrostArmorValue = 0;
    public float MaxFrostArmorValue = 0;

    [Space]
    const float MoveSpeedIncreaseInitialValue = .1f;
    public float MoveSpeedIncreaseValue = 1;

    [Space]
    const float sneakMoveSpeedIncreaseInitialValue = .15f;
    public float sneakMoveSpeedIncreaseValue = 1;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        playerController = GetComponent<PlayerController01>();
        EquipmentManager.instance.onEquipmentChanged += OnEnchantChange;
    }

    public void OnEnchantChange(Item newItem, Item oldItem)
    {
        if(oldItem)
        {
            foreach (Enchant enchant in oldItem.enchants)
            {
                playerEnchants.Remove(enchant);

                UpdatePlayerEnchant(enchant.enchants.enchantType);
                UnApplayStackEnchantToPlayer(enchant);
            }
        }
        
        if(newItem)
        {
            foreach (Enchant enchant in newItem.enchants)
            {
                playerEnchants.Add(enchant);

                UpdatePlayerEnchant(enchant.enchants.enchantType);
                ApplayEnchantToPlayer(enchant);
            }
        }
    }

    void UpdatePlayerEnchant(EnchantType enchantType)
    {
        foreach (EnchantSetting enchant in enchantSettings)
        {
            if (enchantType == enchant.enchantType)
            {
                enchant.Percent = GetEnchantLevel(enchantType) * enchant.InitialPercent;
                enchant.Value = GetEnchantLevel(enchantType) * enchant.InitialValue;
                enchant.Time = GetEnchantLevel(enchantType) * enchant.InitialTime;



                return;
            }
        }
    }

    float CalculateEnchantPercent(Enchant enchant)
    {
        foreach (EnchantSetting setting in enchantSettings)
        {
            if (enchant.enchants.enchantType == setting.enchantType)
            {
                float percent = 0;

                percent += (enchant.enchants.EnchantCurrentLevel * GetEnchantInitialPercent(enchant.enchants.enchantType));

                return percent;
            }
        }

        Debug.LogError("인첸트 기본값을 못찾아서 기본값을 반환하였습니다.");
        return 1;
    }

    float CalculateEnchantValue(Enchant enchant)
    {
        foreach (EnchantSetting setting in enchantSettings)
        {
            if (enchant.enchants.enchantType == setting.enchantType)
            {
                float value = 0;

                value += (enchant.enchants.EnchantCurrentLevel * GetEnchantInitialValue(enchant.enchants.enchantType));


                return value;
            }
        }

        Debug.LogError("인첸트 기본값을 못찾아서 0을 반환하였습니다.");
        return 0;
    }


    public float GetEnchantInitialPercent(EnchantType enchantType)
    {
        foreach (EnchantSetting enchant in enchantSettings)
        {
            if (enchantType == enchant.enchantType)
            {
                return enchant.InitialPercent;
            }
        }

        print(enchantType + " 의 반환값이 없어서 0을 반환함");
        return 0;
    }

    public float GetEnchantInitialValue(EnchantType enchantType)
    {
        foreach (EnchantSetting enchant in enchantSettings)
        {
            if (enchantType == enchant.enchantType)
            {
                return enchant.InitialValue;
            }
        }

        //Debug.LogError(enchantType + " 의 반환값이 없어서 0을 반환함");
        return 0;
    }

    public float GetEnchantInitialTime(EnchantType enchantType)
    {
        foreach (EnchantSetting enchant in enchantSettings)
        {
            if (enchantType == enchant.enchantType)
            {
                return enchant.InitialTime;
            }
        }

        print(enchantType + " 의 반환값이 없어서 0을 반환함");
        return 0;
    }

    public float GetEnchantInitialRepeatTime(EnchantType enchantType)
    {
        foreach (EnchantSetting enchant in enchantSettings)
        {
            if (enchantType == enchant.enchantType)
            {
                return enchant.InitialRepeatTime;
            }
        }

        print(enchantType + " 의 반환값이 없어서 0을 반환함");
        return 0;
    }


    public int GetEnchantLevel(EnchantType enchantType)
    {
        int level = 0;
        foreach (Enchant enchant in playerEnchants)
        {
            if (enchantType == enchant.enchants.enchantType)
            {
                level += enchant.enchants.EnchantCurrentLevel;
            }
        }

        return level;
    }

    public float GetEnchantPercent(EnchantType enchantType)
    {
        foreach (EnchantSetting enchant in enchantSettings)
        {
            if (enchantType == enchant.enchantType)
            {
                return enchant.Percent;
            }
        }

        print(enchantType + " 의 반환값이 없어서 0을 반환함");
        return 0;
    }

    public float GetEnchantValue(EnchantType enchantType)
    {
        foreach (EnchantSetting enchant in enchantSettings)
        {
            if (enchantType == enchant.enchantType)
            {
                return enchant.Value;
            }
        }

        print(enchantType + " 의 반환값이 없어서 0을 반환함");
        return 0;
    }

    public float GetEnchantTime(EnchantType enchantType)
    {
        foreach (EnchantSetting enchant in enchantSettings)
        {
            if (enchantType == enchant.enchantType)
            {
                return enchant.Time;
            }
        }

        print(enchantType + " 의 반환값이 없어서 0을 반환함");
        return 0;
    }

    void ApplayEnchantToPlayer(Enchant enchant)
    {
        switch(enchant.enchants.enchantType)
        {
            case EnchantType.DamageIncrease:

                playerStats.minDamage.AddPercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                playerStats.maxDamage.AddPercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                break;

            case EnchantType.AttackSpeedIncrease:

                playerStats.AttackSpeed.AddPercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                break;

            case EnchantType.IncreaseCriticalChance:

                playerStats.CritialChange.AddPercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                break;

            case EnchantType.ArmorIncrease:

                playerStats.armor.AddPercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                break;

            case EnchantType.MaxHealthIncrease:

                playerStats.maxHealth.AddPercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                break;

            case EnchantType.MaxStaminaIncrease:

                playerStats.MaxStamina.AddPercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                break;

            case EnchantType.MaxManaIncrease:

                playerStats.MaxMana.AddPercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value / 100);

                break;

            case EnchantType.ManaRecoveryIncrease:
                playerStats.MPGenerationValue.AddIntModifier((int)enchant.value1.element[enchant.enchants.EnchantCurrentLevel-1].value);
                break;

            case EnchantType.ManaRecoverySpeedIncrease:
                playerStats.MPGenerationSpeedMulti.AddPercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                break;

            case EnchantType.Poisoning:
                //완

                break;

            case EnchantType.SlowDown:
                //완
                break;

            case EnchantType.Binding:

                break;

            case EnchantType.Burn:
                //완
                break;

            case EnchantType.Stun:
                //완
                break;

            case EnchantType.ElectricShock:

                break;

            case EnchantType.WideAttack:

                break;

            case EnchantType.DoubleShot:
                //완
                break;

            case EnchantType.PenetrateShot:
                //완
                break;

            case EnchantType.Assassination:

                break;

            case EnchantType.SturdyShield:
                playerStats.playerMaxShieldPower.AddPercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                break;

            case EnchantType.QuickStealth:
                playerStats.SneakGagueGainValue.AddPercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value / 100);
                break;

            case EnchantType.Berserker:
                playerStats.GetRageAtAutoAttackValue.AddIntModifier((int)enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                break;

            case EnchantType.Thorn:
                //완
                break;

            case EnchantType.FrostArmor:

                break;

            case EnchantType.MoveSpeedIncrease:
                playerStats.MoveSpeedStat.AddPercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                break;

            case EnchantType.sneakMoveSpeedIncrease:
                
                break;
        }
    }

    void UnApplayStackEnchantToPlayer(Enchant enchant)
    {
        switch (enchant.enchants.enchantType)
        {
            case EnchantType.DamageIncrease:

                playerStats.minDamage.RemovePercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                playerStats.maxDamage.RemovePercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                break;

            case EnchantType.AttackSpeedIncrease:

                playerStats.AttackSpeed.RemovePercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                break;

            case EnchantType.IncreaseCriticalChance:

                playerStats.CritialChange.RemovePercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                break;

            case EnchantType.ArmorIncrease:

                playerStats.armor.RemovePercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                break;

            case EnchantType.MaxHealthIncrease:

                playerStats.maxHealth.RemovePercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                break;

            case EnchantType.MaxStaminaIncrease:

                playerStats.MaxStamina.RemovePercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                break;

            case EnchantType.MaxManaIncrease:

                playerStats.MaxMana.RemovePercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value / 100);

                break;

            case EnchantType.ManaRecoveryIncrease:
                playerStats.MPGenerationValue.RemoveIntModifier((int)enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                break;

            case EnchantType.ManaRecoverySpeedIncrease:
                playerStats.MPGenerationSpeedMulti.RemovePercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                break;

            case EnchantType.Poisoning:
                //완

                break;

            case EnchantType.SlowDown:
                //완
                break;

            case EnchantType.Binding:

                break;

            case EnchantType.Burn:
                //완
                break;

            case EnchantType.Stun:
                //완
                break;

            case EnchantType.ElectricShock:

                break;

            case EnchantType.WideAttack:
                //??
                break;

            case EnchantType.DoubleShot:
                //완
                break;

            case EnchantType.PenetrateShot:
                //완
                break;

            case EnchantType.Assassination:

                break;

            case EnchantType.SturdyShield:
                playerStats.playerMaxShieldPower.RemovePercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                break;

            case EnchantType.QuickStealth:
                playerStats.SneakGagueGainValue.RemovePercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value / 100);
                break;

            case EnchantType.Berserker:
                playerStats.GetRageAtAutoAttackValue.RemoveIntModifier((int)enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                break;

            case EnchantType.Thorn:
                //완
                break;

            case EnchantType.FrostArmor:

                break;

            case EnchantType.MoveSpeedIncrease:
                playerStats.MoveSpeedStat.RemovePercentModifier(enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                break;

            case EnchantType.sneakMoveSpeedIncrease:

                break;
        }
    }

    public IEnumerator ArrowFireEnchantEffect(GameObject arrow, bool applyDoubleShot = true, bool applyPenerateShot = true)
    {
        foreach(Enchant enchant in playerEnchants)
        {
            int level = 0;
            float percent = 0;

            switch(enchant.enchants.enchantType)
            {
                case EnchantType.DoubleShot:

                    if(applyDoubleShot)
                    {
                        level = enchant.enchants.EnchantCurrentLevel;

                        percent = enchant.value1.element[level - 1].value;

                        if (RandomValueFloat(percent))
                        {
                            //FloatingTextController.CreateFloatingText("DoubleShot", transform, transform.gameObject.tag, 15, null,true, owner: game);
                            FloatingTextController.GenerateBuffFloatingText("DoubleShot", transform, 15, NPC_Type.friendly, owner: gameObject);

                            //StartCoroutine(playerController.ArrowFire());
                            yield return new WaitForSeconds(0.1f);
                            playerController.ArrowFire(1, 0);
                        }
                        else
                        {
                            //StartCoroutine(playerController.ArrowFire());
                        }
                    }
                    break;

                case EnchantType.PenetrateShot:

                    if(applyPenerateShot)
                    {
                        level = enchant.enchants.EnchantCurrentLevel;

                        percent = enchant.value1.element[level - 1].value;

                        if (RandomValueFloat(percent))
                        {
                            arrow.GetComponent<Arrow_Logic>().Penetrate = true;
                            //FloatingTextController.CreateFloatingText("PenetrateShot", transform, transform.gameObject.tag, 15, null,true, owner : playerStats);
                            FloatingTextController.GenerateBuffFloatingText("PenetrateShot", transform, 15, NPC_Type.friendly, owner: gameObject);
                        }
                    }
                    
                    break;
            }
        }
    }

    public void FiredArrowEnchantEffect(Arrow_Logic arr)
    {
        foreach (Enchant enchant in playerEnchants)
        {
            int level = 0;
            float percent = 0;

            switch (enchant.enchants.enchantType)
            {
                case EnchantType.PenetrateShot:
                    level = enchant.enchants.EnchantCurrentLevel;

                    percent = enchant.value1.element[level-1].value;

                    if (RandomValueFloat(percent))
                    {
                        arr.Penetrate = true;
                        //FloatingTextController.CreateFloatingText("PenetrateShot", transform, transform.gameObject.tag, 15, null,true, owner : playerStats);
                        FloatingTextController.GenerateBuffFloatingText("DoubleShot", transform, 15, NPC_Type.friendly, owner: gameObject);
                    }
                    break;

            }
        }
    }

    public void CheckEnchantAttackEffect(CharacterBuffDeBuff enemy)
    {
        foreach(Enchant enchant in playerEnchants)
        {
            int level = 0;
            float percent = 0;

            switch (enchant.enchants.enchantType)
            {

                case EnchantType.Poisoning:
                    /*level = GetEnchantLevel(EnchantType.Poisoning);

                    percent = GetEnchantInitialPercent(EnchantType.Poisoning) * level;

                    Debug.LogError(percent + " " + GetEnchantValue(EnchantType.Poisoning) * level + " " + GetEnchantTime(EnchantType.Poisoning) * level);*/

                    if (RandomValueFloat(enchant.value3.element[enchant.enchants.EnchantCurrentLevel - 1].value / 100))
                    {
                        var instance = CreateInstanceBuffOrDebuff.createInstnace(buffType.poison, gameObject
                            , enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value
                            , enchant.value2.element[enchant.enchants.EnchantCurrentLevel - 1].value, false, 1f, 0);

                        enemy.AddBuffOrDebuff(instance);

                        //FloatingTextController.CreateFloatingText("Poison", enemy.transform, transform.gameObject.tag, 15, enemy.gameObject,true);

                    }

                    break;

                case EnchantType.Binding:

                    break;

                case EnchantType.Burn:
                    level = enchant.enchants.EnchantCurrentLevel;

                    percent = enchant.value3.element[enchant.enchants.EnchantCurrentLevel - 1].value / 100;

                    //Debug.LogError(percent + " " + GetEnchantValue(EnchantType.Burn) * level + " " + GetEnchantTime(EnchantType.Burn) * level);

                    if (RandomValueFloat(percent))
                    {
                        var instance = CreateInstanceBuffOrDebuff.createInstnace(buffType.Burn, gameObject
                            , enchant.value1.element[level - 1].value, enchant.value2.element[level - 1].value, false, 1f, 0);

                        enemy.AddBuffOrDebuff(instance);

                        //FloatingTextController.CreateFloatingText("Burn", enemy.transform, transform.gameObject.tag, 15, enemy.gameObject,true);

                    }
                    break;

                case EnchantType.SlowDown:
                    level = enchant.enchants.EnchantCurrentLevel;

                    percent = enchant.value3.element[enchant.enchants.EnchantCurrentLevel - 1].value / 100;

                    if (RandomValueFloat(percent))
                    {
                        var instance = CreateInstanceBuffOrDebuff.createInstnace(buffType.SlowDown, gameObject
                            , 0, enchant.value5.element[enchant.enchants.EnchantCurrentLevel - 1].value, false, 0, enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);

                        enemy.AddBuffOrDebuff(instance);

                        //FloatingTextController.CreateFloatingText("SlowDown", transform, transform.gameObject.tag, 15, enemy.gameObject,true);

                    }
                    break;

                case EnchantType.ElectricShock:

                    break;

                case EnchantType.Stun:

                    level = enchant.enchants.EnchantCurrentLevel;

                    percent = enchant.value2.element[level - 1].value;

                    if (RandomValueFloat(percent))
                    {
                        var instance = CreateInstanceBuffOrDebuff.createInstnace(buffType.stun, gameObject
                            , 0, enchant.value1.element[level - 1].value, false, 1f, 0);

                        enemy.AddBuffOrDebuff(instance);

                        //FloatingTextController.CreateFloatingText("Stun", transform, transform.gameObject.tag, 15, enemy.gameObject,true);

                    }
                    break;

                case EnchantType.DecreaseDefense:

                    level = enchant.enchants.EnchantCurrentLevel;

                    percent = enchant.value3.element[level - 1].value / 100;

                    if (RandomValueFloat(percent))
                    {
                        var instance = CreateInstanceBuffOrDebuff.createInstnace(buffType.DecreaseDefense, gameObject
                            , 0, enchant.value2.element[level - 1].value, false, 1f, 0, value1: enchant.value1.element[level - 1].value);

                        enemy.AddBuffOrDebuff(instance);

                        //FloatingTextController.CreateFloatingText("Stun", transform, transform.gameObject.tag, 15, enemy.gameObject,true);

                    }
                    break;
            }
        }
    }

    public void CheckEnchantAttackedByEffect(CharacterBuffDeBuff enemy)
    {
        foreach (Enchant enchant in playerEnchants)
        {
            int level = 0;
            float percent = 0;
            float value = 0;

            switch (enchant.enchants.enchantType)
            {
                case EnchantType.Thorn:

                    level = enchant.enchants.EnchantCurrentLevel;

                    value = enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value;

                    int mindamage = (int)Mathf.Round(playerStats.minDamage.GetFinalStatValueAsMultiflyFloat() * value);
                    int maxdamage = (int)Mathf.Round(playerStats.maxDamage.GetFinalStatValueAsMultiflyFloat() * value);

                    enemy.GetComponent<CharacterStats>().TakeDamage(mindamage, maxdamage, transform.gameObject, false, false, false,NotCritical : true, notBackAttack : true, isDebuffDamage : true);

                    break;
                
            }
        }
    }

    public void CheckNPCKillEnchantEffect(GameObject other)
    {
        foreach (Enchant enchant in playerEnchants)
        {
            int level = 0;
            float percent = 0;
            float value = 0;

            switch (enchant.enchants.enchantType)
            {
                case EnchantType.LeechLife:

                    value = enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value;
                    ParticleGenerator.instance.LifeDrainParticleGenerate(other.GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.center, gameObject, other);

                    playerStats.Heal((int)value);
                    break;

                case EnchantType.ManaDrain:

                    playerStats.GetMana((int)enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                    ParticleGenerator.instance.ManaDrainParticleGenerate(other.GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.center, gameObject);

                    break;
            }
        }
    }

    public void ShieldBlockEnchantEffect()
    {
        foreach (Enchant enchant in playerEnchants)
        {

            switch (enchant.enchants.enchantType)
            {
                case EnchantType.AttackAbsorption:

                    playerStats.GetRage((int)enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value);
                    break;
            }
        }
    }

    public bool EnchantContain(EnchantType type)
    {
        foreach(Enchant enchant in playerEnchants)
        {
            if (enchant.enchants.enchantType == type)
                return true;
        }
        return false;
    }

    public bool RandomValueFloat(float percent)
    {
        int result = Random.Range(0, 101);

        if(result < (int)(percent * 100))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}



[System.Serializable]
public class EnchantSetting
{
    public EnchantType enchantType;

    public float MaxLevel = 10;
    public float InitialPercent = 0;
    public float InitialValue = 0;
    public float InitialTime = 0;
    public float InitialRepeatTime = 0;

    [Space]
    public float Percent = 0;
    public float Value = 0;
    public float Time = 0;
    public float RepeatTime = 0;
}
