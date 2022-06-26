using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CharacterStats {

    [Header("=============================================")]

    [Space]

    public Stat AttackSpeed = new Stat();
    public Stat MoveSpeedStat = new Stat();
    public Stat RunningSpeedStat = new Stat();
    public Stat MaxMana = new Stat();
    public Stat MaxStamina = new Stat();
    public Stat SteminaRecoveryRate = new Stat();
    public Stat CrossBowMinDamage = new Stat();
    public Stat CrossBowMaxDamage = new Stat();
    public Stat CritialChange = new Stat();
    public Stat SneakGagueGainValue = new Stat();
    public Stat RollRequireStamina = new Stat();
    public Stat rollInvincibleTime = new Stat();
    public float bow_BaseAttackSpeed = 1f;            // 활 기본 공격속도

    [Space]

    public CharacterClass playerClass;
    public int PlayerLvl = 1;
    public int playerMaxExp = 10;
    public int playerRestEXP = 0;
    public int skillLvlPoint = 0;
    public int skillUpgradePoint = 0;
    
    //public float playerMaxMana = 100;
    public float playerMaxSoul = 200;
    public float playerMaxRage = 100;
    public float playerMaxSneakGague = 100;
    public Stat playerMaxShieldPower;                       //방패 강도

    [Space]

    public Stat MPGenerationValue;                          //마나 재생량
    public Stat MPGenerationSpeedMulti;                //마나 재생 속도(초당)

    [Space]

    public Stat GetRageAtAutoAttackValue;                   //공격시 분노 획득량
    public Stat GetRageAtGetBlockValue;                       //방어시 분노 획득량
    public Stat RageDecreaseTickRate;
    public Stat RageDecreaseTickValue;
    public Stat RageDecreaseStopTime;
    private BoolStat rageDecrease = new BoolStat();

    public float playerCurrentStamina { get;  set; }
    public float maxExp = 10;
    public float playerCurrentExp { get; set; }
    public float playerCurrentMp { get; set; }
    public float playerCurrentSoul { get; set; }
    public float playerCurrentRage { get; set; }
    public float PlayerCurrentCharge { get; set; }
    public float PlayerCurrentSneakGuage { get; set; }
    public float PlayerCurrentShieldPower { get; set; }

    public float HpPotionCoolTime = 3f;
    public float CurrentHpPotionCoolTime = 0;

    public delegate void OnExpChange();
    public OnExpChange OnStatChangeCallBack;

    private UIManager UIManager;
    private PlayerController01 playerController;
    private PlayerArtifact playerArtifact;
    private PlayerEnchant playerEnchant;

    private Slider healthBarSlider;
    private Slider shieldBarSlider;
    private Slider EXPBarSlider;
    private Slider manaBarSlider;
    private Slider soulBarSlider;
    private Slider rageBarSlider;
    private Slider staminaBarSlider;
    private Slider chargeBarSlider;
    private Slider sneakBarSlider;
    private Slider shieldPowerBarSlider;

    private Text hpText;
    private Text manaText;
    private Text soulText;
    private Text rageText;
    private Text AttackStatText;
    private Text DeffendStatText;
    private Text CriticalChanceText;
    private Text SpeedStatText;
    private Text DodgeChangeText;
    private Text EXPText;
    private Text LvlText;
    private Text staminaText;
    private Text sneakText;

    private float lerp = 0;

    private new void Start()
    {
        base.Start();

        StageManager.instance.Player = gameObject;

        playerController = GetComponent<PlayerController01>();
        playerArtifact = GetComponent<PlayerArtifact>();
        playerEnchant = GetComponent<PlayerEnchant>();

        UIManager = UIManager.instance;
        healthBarSlider = UIManager.Hpbar;
        shieldBarSlider = UIManager.ShieldBar;
        EXPBarSlider = UIManager.Expbar;
        manaBarSlider = UIManager.Manabar;
        soulBarSlider = UIManager.Soulbar;
        rageBarSlider = UIManager.Ragebar;
        staminaBarSlider = UIManager.StaminaBarSlider;
        chargeBarSlider = UIManager.ChargeBar;
        sneakBarSlider = UIManager.SneakBar;
        shieldPowerBarSlider = UIManager.WarriorShieldBar;

        hpText = UIManager.HpText;
        manaText = UIManager.ManaText;
        soulText = UIManager.SoulText;
        rageText = UIManager.RageText;
        AttackStatText = UIManager.AttackStat;
        DeffendStatText = UIManager.DeffendStat;
        DodgeChangeText = UIManager.DodgeChangeStat;
        CriticalChanceText = UIManager.CriticalChanceStat;

        SpeedStatText = UIManager.SpeedStat;
        EXPText = UIManager.ExpText;
        LvlText = UIManager.LvlText;
        staminaText = UIManager.StaminaText;

		healthBarSlider.maxValue = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());
        staminaBarSlider.maxValue = MaxStamina.GetFinalStatValueAsMultiflyFloat();
        currentHealth = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());
        playerCurrentStamina = MaxStamina.GetFinalStatValueAsMultiflyFloat();
        playerCurrentMp = MaxMana.GetFinalStatValueAsMultiflyFloat();
        playerCurrentSoul = 0;
        playerCurrentRage = 0;

        shieldPowerBarSlider.maxValue = playerMaxShieldPower.GetFinalStatValueAsMultiflyFloat();
        PlayerCurrentShieldPower = playerMaxShieldPower.GetFinalStatValueAsMultiflyFloat();

        EXPBarSlider.maxValue = maxExp;
        manaBarSlider.maxValue = MaxMana.GetFinalStatValueAsMultiflyFloat();
        soulBarSlider.maxValue = playerMaxSoul;
        rageBarSlider.maxValue = playerMaxRage;
        chargeBarSlider.maxValue = playerController.maxChargeCount;
        chargeBarSlider.minValue = playerController.minChargeCount;

        EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
        staminaText.text = "";

        skinned = transform.Find("Body").GetComponentInChildren<SkinnedMeshRenderer>();

        StartCoroutine(restorationMP());

        OnStatChangeCallBack += OnStatChange;

        lerp = Time.deltaTime * 10;

        StartCoroutine(RageDecreaseTick());

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    private void Update()
    {
        /*healthBarSlider.value = currentHealth;
        staminaBarSlider.value = playerCurrentStamina;
        EXPBarSlider.value = playerCurrentExp;
        manaBarSlider.value = playerCurrentMp;
        soulBarSlider.value = playerCurrentSoul;
        rageBarSlider.value = playerCurrentRage;
        chargeBarSlider.value = playerController.ChargeCount;*/

        if (PlayerCurrentShieldPower < playerMaxShieldPower.GetFinalStatValueAsMultiflyFloat() && !playerController.isBlocking)
            PlayerCurrentShieldPower += Time.deltaTime * 5;

        if(CurrentHpPotionCoolTime > 0)
        {
            CurrentHpPotionCoolTime -= Time.deltaTime;
        }

        

        healthBarSlider.value = Mathf.Lerp(healthBarSlider.value, currentHealth, lerp);
        shieldBarSlider.maxValue = maxHealth.GetFinalStatValueAsMultiflyFloat();
        shieldBarSlider.value = Mathf.Lerp(shieldBarSlider.value, shieldStat.GetTotalShieldValue(), lerp);
        staminaBarSlider.value = Mathf.Lerp(staminaBarSlider.value, playerCurrentStamina, lerp);
        EXPBarSlider.value = Mathf.Lerp(EXPBarSlider.value, playerCurrentExp, lerp);
        manaBarSlider.value = Mathf.Lerp(manaBarSlider.value, playerCurrentMp, lerp);
        //print("manaBarSlider.value : " + manaBarSlider.value + "playerCurrentMp : " + playerCurrentMp + "Time.deltaTime * 2f : " + Time.deltaTime * 2f);
        soulBarSlider.value = Mathf.Lerp(soulBarSlider.value, playerCurrentSoul, lerp);
        rageBarSlider.value = Mathf.Lerp(rageBarSlider.value, playerCurrentRage, lerp);
        chargeBarSlider.value = Mathf.Lerp(chargeBarSlider.value, playerController.ChargeCount, lerp);
        shieldPowerBarSlider.value = Mathf.Lerp(shieldPowerBarSlider.value, PlayerCurrentShieldPower, lerp);
        sneakBarSlider.value = Mathf.Lerp(sneakBarSlider.value, playerController.SneakGague, lerp);
    }

    IEnumerator StatsValueUI()
    {
        healthBarSlider.value = Mathf.Lerp(healthBarSlider.value, currentHealth, 0.05f);
        staminaBarSlider.value = Mathf.Lerp(staminaBarSlider.value, playerCurrentStamina, 0.05f);
        EXPBarSlider.value = Mathf.Lerp(EXPBarSlider.value, playerCurrentExp, 0.05f);
        manaBarSlider.value = Mathf.Lerp(0, 100, 0.05f);
        soulBarSlider.value = Mathf.Lerp(soulBarSlider.value, playerCurrentSoul, 0.05f);
        rageBarSlider.value = Mathf.Lerp(rageBarSlider.value, playerCurrentRage, 0.05f);
        chargeBarSlider.value = Mathf.Lerp(chargeBarSlider.value, playerController.ChargeCount, 0.05f);

        hpText.text = currentHealth.ToString();             //  HP바 업데이트
        EXPText.text = playerCurrentExp.ToString() + "/" + EXPBarSlider.maxValue.ToString();
        manaText.text = playerCurrentMp.ToString();
        soulText.text = playerCurrentSoul.ToString();
        rageText.text = playerCurrentRage.ToString();
        //staminaText.text = currentStamina.ToString();
        AttackStatText.text = maxDamage.GetFinalStatValue().ToString();
        DeffendStatText.text = armor.GetFinalStatValue().ToString();
        LvlText.text = PlayerLvl.ToString();

        yield return null;
    }

    void OnEquipmentChanged (Item newItem, Item oldItem)
    {
        if (newItem != null)
        {
            armor.AddIntModifier((int)newItem.armorModifier);
            minDamage.AddIntModifier((int)newItem.minDamageModifier);
            maxDamage.AddIntModifier((int)newItem.maxDamageModifier);
            MoveSpeedStat.AddIntModifier((int)newItem.speedModifier);
            CrossBowMinDamage.AddIntModifier((int)newItem.minHandCrossBowDamage);
            CrossBowMaxDamage.AddIntModifier((int)newItem.maxHandCrossBowDamage);
            playerMaxShieldPower.AddIntModifier((int)newItem.ShieldPointModifier);
        }

        if (oldItem != null)
        {
            armor.RemoveIntModifier((int)oldItem.armorModifier);
            minDamage.RemoveIntModifier((int)oldItem.minDamageModifier);
            maxDamage.RemoveIntModifier((int)oldItem.maxDamageModifier);
            MoveSpeedStat.RemoveIntModifier((int)oldItem.speedModifier);
            CrossBowMinDamage.RemoveIntModifier((int)oldItem.minHandCrossBowDamage);
            CrossBowMaxDamage.RemoveIntModifier((int)oldItem.maxHandCrossBowDamage);
            playerMaxShieldPower.RemoveIntModifier((int)oldItem.ShieldPointModifier);
        }

        if(OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }

    }

    public void DecreaseStamina(float staminavalue)
    {
        staminavalue = Mathf.Clamp(staminavalue, 0, int.MaxValue);
        playerCurrentStamina -= staminavalue;

        playerCurrentStamina = Mathf.Clamp(playerCurrentStamina, 0, MaxStamina.GetFinalStatValueAsMultiflyFloat());

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    public void RegenStamina(float regenstamina)
    {
        playerCurrentStamina += regenstamina;
        playerCurrentStamina = Mathf.Clamp(playerCurrentStamina, 0, MaxStamina.GetFinalStatValueAsMultiflyFloat());

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    public void GetMana(int manaValue)
    {
        playerCurrentMp += manaValue;

        playerCurrentMp = Mathf.Clamp(playerCurrentMp, 0, MaxMana.GetFinalStatValueAsMultiflyFloat());

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    public void UseMana(int manaValue)
    {
        playerCurrentMp -= manaValue;

        playerCurrentMp = Mathf.Clamp(playerCurrentMp, 0, MaxMana.GetFinalStatValueAsMultiflyFloat());

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    public void GetSoul(int soulValue)
    {
        if ((playerCurrentSoul + soulValue) > playerMaxSoul)
        {
            playerCurrentSoul = playerMaxSoul;
        }
        else
        {
            playerCurrentSoul += soulValue;
        }

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    public void UseSoul(int soulValue)
    {
        playerCurrentSoul -= soulValue;

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    public void GetRage(int rageValue)
    {
        if ((playerCurrentRage + rageValue) > playerMaxRage)
        {
            playerCurrentRage = playerMaxRage;
        }
        else
        {
            playerCurrentRage += rageValue;
        }

        StartCoroutine(RageDecreaseStop());

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    public void UseShieldPower(int minDamage, int maxDamage)
    {
        int damage = Random.Range(minDamage, maxDamage);

        PlayerCurrentShieldPower -= damage;

        if(PlayerCurrentShieldPower < 0)
        {
            StartCoroutine(playerController.StopBlock());
        }

        Mathf.Clamp(PlayerCurrentShieldPower, 0, playerMaxShieldPower.GetFinalStatValueAsMultiflyFloat());

        GetRage(Mathf.RoundToInt(GetRageAtGetBlockValue.GetFinalStatValueAsMultiflyFloat()));

        playerEnchant.ShieldBlockEnchantEffect();

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    public void UseRage(int rageValue)
    {
        playerCurrentRage -= rageValue;

        playerCurrentRage = Mathf.Clamp(playerCurrentRage, 0, playerMaxRage);

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    IEnumerator RageDecreaseTick()
    {
        while(true)
        {
            if(!rageDecrease.GetBoolValue())
            {
                UseRage(Mathf.RoundToInt(RageDecreaseTickValue.GetFinalStatValue()));
                //Debug.LogError(Mathf.RoundToInt(RageDecreaseTickValue.GetFinalStatValue()));
            }

            yield return new WaitForSeconds(RageDecreaseTickRate.GetFinalStatValue());

            if (OnStatChangeCallBack != null)
            {
                OnStatChangeCallBack();
            }
        }
    }

    IEnumerator RageDecreaseStop()
    {
        rageDecrease.AddBoolModifier();
        yield return new WaitForSeconds(RageDecreaseStopTime.GetFinalStatValue());
        rageDecrease.RemoveBoolModifier();
    }

    public void GetExp(int Exp)
    {
        Debug.Log("플레이어가 경험치 획득 " + Exp);
        playerCurrentExp += Exp;
        playerRestEXP = (int)Mathf.Round((playerCurrentExp - EXPBarSlider.maxValue));

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    public override void TakeDamage(int minDamage, int maxDamage, GameObject other, bool playSound, bool playBlink
        , bool ignoreArmor, bool NotCritical = false, bool isDebuffDamage = false, bool notBackAttack = false
        , bool ownerAttack = true, BuffNDebuffObject[] debuff = null)
    {
        /*
        int damage = Random.Range(minDamage, maxDamage);

        if(PlayerCurrentShield > 0)
        {
            int leftDamage = PlayerCurrentShield - damage;

            if(leftDamage < 0)
            {
                base.TakeDamage(leftDamage,leftDamage, other, playSound, playBlink, ignoreArmor);
                FloatingTextController.CreateFloatingText(PlayerCurrentShield.ToString(), transform, "Shield", 25, other);
                PlayerCurrentShield = 0;

                if (PlayerLeftShield > 0)
                {
                    PlayerLeftShield = 0;
                }

            }
            else
            {
                if(PlayerLeftShield > 0)
                {
                    PlayerLeftShield -= damage;
                }

                PlayerCurrentShield = PlayerCurrentShield - damage;
                FloatingTextController.CreateFloatingText(damage.ToString(), transform, "Shield", 25, other);
            }
        }*/
        //else
        //{
        base.TakeDamage(minDamage, maxDamage, other, playSound, playBlink, ignoreArmor, NotCritical, isDebuffDamage, Debuff : debuff);

        if (playerController.SneakGague > 0)
        {
            playerController.SneakGague -= 35;
            playerController.SneakGague = Mathf.Clamp(playerController.SneakGague, 0, 100);
        }

        if (GetComponent<PlayerEnchant>())
        {
            if (other)
            {
                GetComponent<PlayerEnchant>().CheckEnchantAttackedByEffect(other.GetComponent<CharacterBuffDeBuff>());
            }
        }

        if (playerController.SneakGague > 0)
        {
            playerController.SneakGague -= 35;
            playerController.SneakGague = Mathf.Clamp(playerController.SneakGague, 0, 100);
        }
        //}


        //if (playerClass == CharacterClass.Warrior)
        //GetRage((int)Mathf.Round(GetRageAtGetBlockValue.GetFinalStatValueAsMultiflyFloat()));



        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    public void LevelUp()
    {
        if (PlayerLvl > 30)
            return;

        print("플레이어 래벨업");
        PlayerLvl++;            //레벨업

        playerCurrentExp = 0;
        playerCurrentExp += playerRestEXP;
        
        EXPBarSlider.maxValue = (int)Mathf.Round(EXPBarSlider.maxValue + 10);        //필요 경험치 변경
        EXPBarSlider.maxValue = Mathf.Clamp(EXPBarSlider.maxValue, 0, 10000);

        playerCurrentExp = Mathf.Clamp(playerCurrentExp, 0, EXPBarSlider.maxValue);
        playerMaxExp = (int)Mathf.Round(EXPBarSlider.maxValue);

        foreach(PlayerModel.PlayerClassStatPreset statPreset in PlayerModel.instance.playerClassStatPreset)
        {
            if(statPreset.characterClass == playerClass)
            {
                maxHealth.AddIntModifier(statPreset.StartHpGrowthRate);
                Heal(statPreset.StartHpGrowthRate);
            }
        }

        if(playerArtifact.ArtifactContain(ArtifactType.GriffinFeather))
        {
            currentHealth = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());
            healthBarSlider.maxValue = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());
        }

        SkillLvlPointUp(1);
        UIManager.instance.skillParent.OnSkillPointChange.Invoke();

        AudioManager.instance.GenerateAudioAndPlaySFX("levelUp1", transform.position + (Vector3.up * 2));

        //FloatingTextController.CreateFloatingText("levelUp", transform, transform.gameObject.tag, 25, null, owner: this, effectString: true);
        FloatingTextController.GenerateBuffFloatingText("levelUp", transform, 15, NPC_Type.friendly, owner: gameObject);

        var particle = Instantiate(PrefabCollect.instance.levelUpParticle, GetComponent<Collider>().bounds.center, new Quaternion(0, 0, 0, 0));
        Destroy(particle, 5);
    }

    public void SkillLvlPointUp(int value)
    {
        skillLvlPoint+= value;

        SkillUI.instance.SkillLvlPoint.text = skillLvlPoint.ToString();

        QuickUI.instance.OnChangeSkillPoint(skillLvlPoint);
    }

    public void SkillLvlPointUse(int value)
    {
        skillLvlPoint -= value;

        SkillUI.instance.SkillLvlPoint.text = skillLvlPoint.ToString();

        QuickUI.instance.OnChangeSkillPoint(skillLvlPoint);

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    protected override void Die(GameObject other)
    {
        base.Die(other);

        if(other != null)
        {
            if(other.GetComponentInChildren<HealthUI>() != null)
                PlayStat.instance.DiedBy(other.GetComponentInChildren<HealthUI>().GetName());
        }

        PlayStat.instance.PlayerDiedEvent();

        UIManager.DeathMessage.SetActive(true);
        gameObject.SetActive(false);

        if(UIManager.DeathMessage.GetComponentInChildren<Button>() != null)
        {
            Button DeathButton = UIManager.DeathMessage.GetComponentInChildren<Button>();
            DeathButton.onClick.AddListener(Respawn);
        }
        

        print("플레이어 사망함수 발동");
    }

    public void Respawn()
    {
        UIManager.DeathMessage.SetActive(false);

        Button DeathButton = UIManager.DeathMessage.GetComponentInChildren<Button>();
        DeathButton.onClick.RemoveAllListeners();

        gameObject.SetActive(true);

        currentHealth = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());
        RaycastHit hit;
        Vector3 randomV3 = new Vector3(Random.Range(100, 500), 500, Random.Range(100, 500));
        Physics.Raycast(transform.position + randomV3, Vector3.down, out hit, 1000f);

        transform.position = hit.point;

        print("플레이어 리스폰 : " + hit.point + "에 리스폰됨");
    }

    public bool Usingpotion(int Hp, int Mp)
    {
        if(currentHealth >= (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat()))
        {
            print("HP가 풀이라 물약 사용 실패");
            return false;
        }
        else if(CurrentHpPotionCoolTime > 0)
        {
            print("물약 쿨타임 사용 실패" + CurrentHpPotionCoolTime);
            return false;
        }
        else
        {
            print("물약 사용 성공 " + CurrentHpPotionCoolTime);
            Heal(Hp);
            int random = Random.Range(1, 3);
            AudioManager.instance.GenerateAudioAndPlaySFX("drink" + random.ToString(), GetComponentInChildren<Collider>().bounds.center);
            CurrentHpPotionCoolTime = HpPotionCoolTime;
            return true;
        }
        
    }

    public void ChangeMaxHp(int maxhp)
    {
        maxHealth.ClearIntModifier();
        maxHealth.AddIntModifier(maxhp);
        currentHealth = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    public void ChangeMaxMP(int maxmp)
    {
        MaxMana.ClearIntModifier();
        MaxMana.AddIntModifier(maxmp);
        playerCurrentMp = (int)Mathf.Round(MaxMana.GetFinalStatValueAsMultiflyFloat());

        if (OnStatChangeCallBack != null)
        {
            OnStatChangeCallBack();
        }
    }

    public Slider getHpSlider()
    {
        return healthBarSlider;
    }
    public Slider getMpSlider()
    {
        return manaBarSlider;
    }
    public Slider getExpSlider()
    {
        return EXPBarSlider;
    }

    IEnumerator restorationMP()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / MPGenerationSpeedMulti.GetFinalStatValueAsMultiflyFloat());
            if ((playerCurrentMp + MPGenerationValue.GetFinalStatValueAsMultiflyFloat()) > 100)
            {
                float value = playerCurrentMp + MPGenerationValue.GetFinalStatValueAsMultiflyFloat();
                playerCurrentMp = Mathf.Clamp(value, 0, MaxMana.GetFinalStatValueAsMultiflyFloat());
            }
            else
                playerCurrentMp += MPGenerationValue.GetFinalStatValueAsMultiflyFloat();

            if (OnStatChangeCallBack != null)
            {
                OnStatChangeCallBack();
            }
        }
    }

    public void StageClear()
    {
        gameObject.transform.Translate(new Vector3(0, 0, 0));
    }

    public void SettingPlayerSpeedStat()
    {
        switch (playerClass)                     //플레이어의 기본 이동속도 스탯를 설정
        {
            case CharacterClass.Warrior:
                MoveSpeedStat.AddIntModifier(8);
                break;

            case CharacterClass.Archer:
                MoveSpeedStat.AddIntModifier(10);
                break;

            case CharacterClass.Rogue:
                MoveSpeedStat.AddIntModifier(12);
                break;

            case CharacterClass.Wizard:
                MoveSpeedStat.AddIntModifier(9);
                break;

            case CharacterClass.Necromancer:
                MoveSpeedStat.AddIntModifier(9);
                break;

            default:
                MoveSpeedStat.AddIntModifier(10);
                print("기본이동속도로 설정");
                break;
        }
    }

    void OnChangeHealth()
    {
        if (GetComponent<PlayerArtifact>())
        {
            if (GetComponent<PlayerArtifact>().ArtifactContain(ArtifactType.BerserkerEyePatch))
            {
                if (GetComponent<PlayerStats>().currentHealth < (GetComponent<PlayerStats>().maxHealth.GetFinalStatValueAsMultiflyFloat() / 2))
                {
                    
                }
            }
        }
    }

    public void OnStatChange()
    {
        if (playerCurrentExp >= EXPBarSlider.maxValue)             //경험치가 맥스경험치에 도달하면
        {
            LevelUp();
        }

        if (currentHealth > (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat()))
        {
            currentHealth = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());
        }

        if (playerCurrentStamina >= MaxStamina.GetFinalStatValueAsMultiflyFloat())
        {
            staminaBarSlider.gameObject.SetActive(false);
        }
        else
        {
            staminaBarSlider.gameObject.SetActive(true);
        }

        if (sneakBarSlider.value <= 1)
        {
            sneakBarSlider.gameObject.SetActive(false);
        }
        else
        {
            sneakBarSlider.gameObject.SetActive(true);
        }

        if (PlayerCurrentShieldPower >= playerMaxShieldPower.GetFinalStatValueAsMultiflyFloat() * .999f)
        {
            shieldPowerBarSlider.gameObject.SetActive(false);
        }
        else
        {
            shieldPowerBarSlider.gameObject.SetActive(true);
        }

        healthBarSlider.maxValue = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());
        manaBarSlider.maxValue = (int)Mathf.Round(MaxMana.GetFinalStatValueAsMultiflyFloat());
        staminaBarSlider.maxValue = MaxStamina.GetFinalStatValueAsMultiflyFloat();
        manaText.text = ((int)Mathf.Round(playerCurrentMp)).ToString();
        soulText.text = ((int)Mathf.Round(playerCurrentSoul)).ToString();
        rageText.text = ((int)Mathf.Round(playerCurrentRage)).ToString();
        var currentHeath = currentHealth;
        hpText.text = Mathf.Clamp(currentHeath,0, currentHeath).ToString();           //  HP바 업데이트
        EXPText.text = playerCurrentExp.ToString() + "/" + EXPBarSlider.maxValue.ToString();
        //staminaText.text = currentStamina.ToString();
        AttackStatText.text = Mathf.RoundToInt(minDamage.GetFinalStatValue()).ToString() + " ~ " + Mathf.RoundToInt(maxDamage.GetFinalStatValue()).ToString();
        DeffendStatText.text = Mathf.RoundToInt(armor.GetFinalStatValue()).ToString();
        CriticalChanceText.text = Mathf.RoundToInt(((CritialChange.GetFinalStatValueAsAddFloat() * 100))).ToString() + "%";
        DodgeChangeText.text = Mathf.RoundToInt(dodge.GetFinalStatValueAsAddFloat()).ToString() + "%";
        SpeedStatText.text = string.Format("{0:N1}",(MoveSpeedStat.GetFinalStatValue() + playerController.moveSpeed));
        UIManager.instance.MaxHpStat.text = Mathf.RoundToInt(maxHealth.GetFinalStatValueAsMultiflyFloat()).ToString();
        UIManager.instance.MaxSteaminaStat.text = MaxStamina.GetFinalStatValueAsMultiflyFloat().ToString();
        UIManager.instance.AttackSpeed.text = string.Format("{0:N2}", (AttackSpeed.GetFinalStatValueAsMultiflyFloat()));

        LvlText.text = PlayerLvl.ToString();

        shieldPowerBarSlider.maxValue = playerMaxShieldPower.GetFinalStatValueAsMultiflyFloat();

        //print("OnStatChange");
    }
}
