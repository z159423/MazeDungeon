using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class NPCStats : CharacterStats
{
    [Header("=============================================")]

    public int npcLevel;
    public string NPC_NameKey;
    public AudioClip DeadSound;

    [Space]
    public DropItem.DropTable IndividualDropTable;
    public bool IndividualEXP = false;
    public bool IndividualCoinDrop = false;
    public bool IndividualEquipmentDropPercent = false;
    public bool IndividualConsumableDropPercent = false;
    public bool IndividualKeyDropPercent = false;
    public bool IndividualArtifactDropPercent = false;
    public bool IndividualHeartDropPercent = false;


    [Space]

    public int soulDropRatio = 10;

    public HealthUI healthUI;

    [Space]

    public bool EliteNPC = false;

    private SpawnNPC spawnNPC;
    private NPC_AI Ai;

    public UnityEvent OnDeathEvent;

    private new void Start()
    {
        base.Start();

        Ai = GetComponent<NPC_AI>();
        spawnNPC = SpawnNPC.instance;

        healthUI = GetComponent<HealthUI>();
    }

    public override void TakeDamage(int minDamage, int maxDamage, GameObject other, bool playSound, bool playBlink, bool ignoreArmor
        , bool NotCritical = false, bool isDebuffDamage = false, bool notBackAttack = false, bool ownerAttack = true, BuffNDebuffObject[] debuff = null)
    {
        if(Ai.Target == null && other != null)
        {
            Ai.AnnounceNearNpc(gameObject,other);
        }

        base.TakeDamage(minDamage, maxDamage, other, playSound, playBlink, ignoreArmor, NotCritical, isDebuffDamage, notBackAttack, ownerAttack, Debuff : debuff);
    }

    protected override void Die(GameObject other)
    {
        base.Die(other);

        OnDeathEvent.Invoke();

        print(gameObject.name + " 죽음 NPCStsts Die 함수 발동함");

        currentHealth = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());

        //====================================  코인 드랍  ========================================

        int coinValue;

        if(IndividualCoinDrop)
        {
            coinValue = Random.Range(IndividualDropTable.DropCoinMinAmount, IndividualDropTable.DropCoinMaxAmount);
        }
        else
        {
            if(Ai.monsterType == MonsterType.Boss)
            {
                coinValue = Random.Range(DropItem.instance.bossMonsterDropTable.DropCoinMinAmount , DropItem.instance.bossMonsterDropTable.DropCoinMaxAmount);
            }
            else
            {
                coinValue = Random.Range(DropItem.instance.normalMonsterDropTable.DropCoinMinAmount, DropItem.instance.normalMonsterDropTable.DropCoinMaxAmount);
            }
        }

        if(coinValue != 0)
           DropItem.instance.DropCoin(coinValue, GetComponent<Collider>().bounds.center, Vector3.up, 0.1f);

        //====================================  경험치 드랍  ========================================

        int EXPValue;

        if (IndividualEXP)
        {
            EXPValue = IndividualDropTable.EXPDropAmount;
        }
        else
        {
            if(Ai.monsterType == MonsterType.Boss)
            {
                EXPValue = DropItem.instance.bossMonsterDropTable.EXPDropAmount;
            }
            else
            {
                EXPValue = DropItem.instance.normalMonsterDropTable.EXPDropAmount;
            }
        }

        if (other)
        {
            if (other.transform.GetComponent<PlayerStats>() != null)
            {
                other.transform.GetComponent<PlayerStats>().GetExp(EXPValue);

                other.GetComponent<PlayerArtifact>().ArtifactKillEffect(gameObject);

                PlayStat.instance.EnemyKilled();

                //if (EXPValue > 0)
                    //FloatingTextController.CreateFloatingText("경험치 + " + EXPValue.ToString(), transform, transform.gameObject.tag, 15, gameObject);
            }
        }

        if (other)
        {
            if (other.transform.GetComponent<NPC_AI>())
            {
                if (other.transform.GetComponent<NPC_AI>().Summoner != null)
                {
                    if(other.transform.GetComponent<NPC_AI>().Summoner.GetComponent<PlayerStats>())
                        other.transform.GetComponent<NPC_AI>().Summoner.GetComponent<PlayerStats>().GetExp(EXPValue);
                }
            }
        }
        //====================================  장비,소모품,열쇠 드랍  ========================================

        var playernumber = Random.Range(0, PlayerManager.instance.Players.Count);
        DropItem.instance.dropItem(DataMannager.instance.itemDataBase, transform, this, Ai, PlayerManager.instance.Players[playernumber].playerobject.GetComponentInChildren<PlayerStats>());

        //var player = GameObject.FindGameObjectWithTag("Player");
        //DropItem.instance.CreateEquipmentDropItem(transform, player.GetComponent<PlayerStats>().playerClass, 1);    //정해진 아이템 스폰

        //====================================  소울 드랍  ========================================

        foreach (GameObject Players in GameObject.FindGameObjectsWithTag("Player"))                                      //플레이어중에 네크로멘서가 있으면 영혼수집용 영혼이 소환됨
        {
            if(Players.GetComponentInChildren<PlayerStats>().playerClass == CharacterClass.Necromancer && Players.GetComponentInChildren<PlayerSkill>().CheckPlayerHaveSkill(SkillManager.instance.SoulRecovery))
            {
                GameObject Skull = Instantiate(PrefabCollect.instance.SoulSkull, transform.position, Quaternion.identity);
                SoulSkull soulSkull = Skull.GetComponentInChildren<SoulSkull>();
                soulSkull.TargetPlayer = Players.transform;

                if (soulDropRatio != 0)
                {
                    soulSkull.soulValue = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat()) / soulDropRatio;
                    soulSkull.soulValue = (int)Mathf.Round(soulSkull.soulValue * (.5f + (.5f * Players.GetComponentInChildren<PlayerSkill>().GetSkillLevel(SkillManager.instance.SoulRecovery))));
                }
            }
        }

        if(Ai.Summoner != null && Ai.npcType == NPC_Type.Minion)
        {
            Ai.Summoner.GetComponentInChildren<PlayerController01>().DeleteMinion(transform.root.gameObject);
        }

        AudioManager.instance.GenerateAudioAndPlaySFX("dead1", GetComponent<Collider>().bounds.center);

        transform.parent.gameObject.SetActive(false);

        //print("tag " + spawnNPC.SpawnedNpc.Count);
        spawnNPC.SpawnedNpc.Remove(transform.parent.gameObject);
        //print("tag " + spawnNPC.SpawnedNpc.Count);

        if(Ai.monsterType == MonsterType.Boss && !Ai.miniBoss)
            MazeDungeonNpcSpawner.instance.CheckAllBossDead();

        if(GetComponent<ShopKeeper>())
        {
            GetComponent<ShopKeeper>().FreeAllItem();
            DungeonGenerator.instance.isShopKeeperDead = true;
        }

        if (Ai.bossType == BossType.FallenKing && DungeonGenerator.instance.FinalStageNumber == DungeonGenerator.instance.CurrentStageNumber)
        {
            UIManager.instance.GameEnding();
        }

        NpcKilledStackData(other);
    }

    public void NpcKilledStackData(GameObject other)
    {
        if(other != null)
        {
            if (NPC_NameKey == "SkeletonArcher" && other.transform.GetComponent<PlayerStats>())
            {
                CharacterClassManager.AddSkeletonArcherKillCount();
                //Debug.LogError("해골궁수 처치");
            }

            if (Ai.bossType == BossType.Lich && other.transform.GetComponent<PlayerStats>())
            {
                CharacterClassManager.KillTheLich();
            }
        }
        
    }

    public void AttackSoundPlay()
    {
        base.NpcAttackSoundPlay();
    }


    public void resetNPC()
    {
        print(" Npc 재배치됨");
        currentHealth = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());
        Ai.LocalNav.m_Size = new Vector3(0, 0, 0);
        transform.parent.gameObject.SetActive(false);
        isStun.ClearBoolStat();
        //spawnNPC.PlacedNPC[tag].NPCObject.Remove(transform.parent.gameObject);
        //print("tag " + spawnNPC.PlacedNPC[tag].NPCObject.Count);
    }

    public void fullHealth()
    {
        currentHealth = (int)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat());
    }

    public void StatFitToLevel()        //NPC 스탯을 레벨에 맞추는 함수
    {
        if (npcLevel == 1)
            return;

        //float multiple = (npcLevel - 1) * MazeDungeonNpcSpawner.instance.NPCGrowthMultify;

        float hpGrowth = MazeDungeonNpcSpawner.instance.NPCHpGrowthMultify * (npcLevel-1);
        float armorGrowth = MazeDungeonNpcSpawner.instance.NPCArmorGrowthMultify * (npcLevel-1);
        float damageGrowth = MazeDungeonNpcSpawner.instance.NPCDamageGrowthMultify * (npcLevel-1);

        var tempHealth = (float)Mathf.Round(maxHealth.GetFinalStatValueAsMultiflyFloat()) * hpGrowth;
        maxHealth.AddIntModifier(Mathf.RoundToInt(tempHealth));

        //var tempEXP = (float)IndividualDropTable.EXPDropAmount * multiple;
        //IndividualDropTable.EXPDropAmount += (int)Mathf.Round(tempEXP);

        var tempMinDamage = (float)minDamage.GetFinalStatValue() * damageGrowth;
        var tempMaxDamage = (float)maxDamage.GetFinalStatValue() * damageGrowth;
        maxDamage.AddIntModifier(Mathf.RoundToInt(tempMaxDamage));
        minDamage.AddIntModifier(Mathf.RoundToInt(tempMinDamage));

        var tempArmor = (float)armor.GetFinalStatValue() * armorGrowth;
        armor.AddIntModifier(Mathf.RoundToInt(tempArmor));

        fullHealth();
    }

    public void ChangeIndividualDrops()
    {
        IndividualEXP = true;
        IndividualCoinDrop = true;
        IndividualEquipmentDropPercent = true;
        IndividualConsumableDropPercent = true;
        IndividualKeyDropPercent = true;
        IndividualArtifactDropPercent = true;
    }

    public void KillThisNPC()
    {
        Die(null);
    }

    public void setLevel(int Level)
    {
        npcLevel = Level;
    }

}
