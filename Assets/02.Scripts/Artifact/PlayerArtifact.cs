using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArtifact : MonoBehaviour
{
    public List<Artifact> playerArtifact = new List<Artifact>();

    public LayerMask enemyMask;

    private PlayerStats playerStat;

    private int brokenDirkHitCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerStat = GetComponent<PlayerStats>();
    }

    public void OnGetArtifact(Artifact artifact, GameObject player)
    {
        playerArtifact.Add(artifact);
        ArtifactEquipEffect(artifact, player);

        UIManager.instance.Artifact.GetComponent<ArtifactCollection>().GetNewArtifact(artifact);
        PlayStat.instance.GetNewArtifact();
    }

    void ArtifactEquipEffect(Artifact artifact, GameObject player)
    {
        switch(artifact.artifactType)
        {
            case ArtifactType.SilverRing:
                player.GetComponentInChildren<PlayerStats>().armor.AddPercentModifier(.1f);

                break;

            case ArtifactType.SilverNecklace:
                player.GetComponentInChildren<PlayerStats>().AttackSpeed.AddPercentModifier(.1f);
                break;

            case ArtifactType.RubyRing:
                player.GetComponentInChildren<PlayerStats>().maxHealth.AddIntModifier(20 /*(int)Mathf.Round(player.GetComponentInChildren<PlayerStats>().maxHealth.GetFinalStatValueAsMultiflyFloat() / .2f)*/);
                break;

            case ArtifactType.ToothNecklace:
                player.GetComponentInChildren<PlayerStats>().CritialChange.AddPercentModifier(.05f);
                break;

            case ArtifactType.GoldRing:
                player.GetComponentInChildren<PlayerStats>().armor.AddPercentModifier(.1f);
                player.GetComponentInChildren<PlayerStats>().maxHealth.AddPercentModifier(.1f);
                break;

            case ArtifactType.GoldNecklace:
                player.GetComponentInChildren<PlayerStats>().AttackSpeed.AddPercentModifier(.1f);
                player.GetComponentInChildren<PlayerStats>().CritialChange.AddPercentModifier(.05f);
                break;

            case ArtifactType.BlueCrystal:
                var blue = Instantiate(artifact.FollowerObject, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
                blue.GetComponentInChildren<FollowLogic>().SetFollower(gameObject);
                blue.GetComponentInChildren<TurretLogic>().owner = gameObject;
                break;

            case ArtifactType.RedCrystal:
                var red = Instantiate(artifact.FollowerObject, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
                red.GetComponentInChildren<FollowLogic>().SetFollower(gameObject);
                red.GetComponentInChildren<TurretLogic>().owner = gameObject;
                break;

            case ArtifactType.YellowCrystal:
                var yellow = Instantiate(artifact.FollowerObject, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
                yellow.GetComponentInChildren<FollowLogic>().SetFollower(gameObject);
                yellow.GetComponentInChildren<TurretLogic>().owner = gameObject;
                break;

            case ArtifactType.FrozenHeart:
                StartCoroutine(StartArtifactEffect_FrozenHeart());
                break;

            case ArtifactType.Feather:
                player.GetComponentInChildren<PlayerStats>().MoveSpeedStat.AddPercentModifier(.1f);
                break;

            case ArtifactType.ElfBread:
                player.GetComponentInChildren<PlayerStats>().MaxStamina.AddPercentModifier(.2f);
                player.GetComponentInChildren<PlayerStats>().SteminaRecoveryRate.AddPercentModifier(.1f);
                break;

            case ArtifactType.StoneGauntlet:
                player.GetComponentInChildren<PlayerStats>().MoveSpeedStat.AddPercentModifier(-.1f);
                player.GetComponentInChildren<PlayerStats>().minDamage.AddPercentModifier(.1f);
                player.GetComponentInChildren<PlayerStats>().maxDamage.AddPercentModifier(.1f);
                break;

            case ArtifactType.Icicle:
                player.GetComponentInChildren<PlayerStats>().minDamage.AddPercentModifier(.1f);
                player.GetComponentInChildren<PlayerStats>().maxDamage.AddPercentModifier(.1f);
                break;

            case ArtifactType.ShieldCharm:
                player.GetComponentInChildren<PlayerStats>().armor.AddIntModifier(5);
                break;

            case ArtifactType.DodgeCharm:
                //player.GetComponentInChildren<PlayerStats>().RollRequireStamina.AddPercentModifier(-.2f);
                player.GetComponentInChildren<PlayerStats>().rollInvincibleTime.AddPercentModifier(.2f);
                break;

            case ArtifactType.ThirdEye:
                player.GetComponentInChildren<PlayerStats>().dodge.AddIntModifier(30);
                break;

            case ArtifactType.MoneyBag:
                int randomValue = Random.Range(50, 200);
                Inventory.instance.GetCoin(randomValue);
                break;

            case ArtifactType.ChickenLeg:
                player.GetComponentInChildren<PlayerStats>().maxHealth.AddIntModifier(20);
                player.GetComponentInChildren<PlayerStats>().Heal((int)Mathf.Round(player.GetComponentInChildren<PlayerStats>().maxHealth.GetFinalStatValueAsMultiflyFloat() / 2));
                break;

            case ArtifactType.LostFairy:
                var fairy = Instantiate(artifact.FollowerObject, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
                fairy.GetComponentInChildren<FollowLogic>().SetFollower(gameObject);
                fairy.GetComponentInChildren<TurretLogic>().owner = gameObject;
                break;

            case ArtifactType.OgreClub:
                player.GetComponentInChildren<PlayerStats>().minDamage.AddPercentModifier(.5f);
                player.GetComponentInChildren<PlayerStats>().maxDamage.AddPercentModifier(.5f);
                player.GetComponentInChildren<PlayerStats>().CritialChange.SetMaxValue(0);
                break;
        }
    }

    public void ArtifactAttackEffect(GameObject other)
    {
        if (other.GetComponentInChildren<CharacterStats>())
        {
            foreach (Artifact artifact in playerArtifact)
            {
                int value;
                switch (artifact.artifactType)
                {

                    case ArtifactType.Stone:
                        value = Random.Range(0, 100);

                        if(value < 10)
                        {
                            foreach (BuffNDebuffObject buffNDebuffObject in artifact.Debuff)
                            {
                                var instance = CreateInstanceBuffOrDebuff.CopyInstnace(buffNDebuffObject);
                                instance.buffOrDebuff.Owner = gameObject;
                                other.GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(instance);
                            }
                        }
                        break;

                    case ArtifactType.SnakeFang:
                        value = Random.Range(0, 100);

                        if (value < 10)
                        {
                            foreach (BuffNDebuffObject buffNDebuffObject in artifact.Debuff)
                            {
                                var instance = CreateInstanceBuffOrDebuff.CopyInstnace(buffNDebuffObject);
                                instance.buffOrDebuff.Owner = gameObject;
                                other.GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(instance);
                            }
                        }
                        break;

                    case ArtifactType.Icicle:
                        value = Random.Range(0, 100);

                        if (value < 20)
                        {
                            foreach (BuffNDebuffObject buffNDebuffObject in artifact.Debuff)
                            {
                                var instance = CreateInstanceBuffOrDebuff.CopyInstnace(buffNDebuffObject);
                                instance.buffOrDebuff.Owner = gameObject;
                                other.GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(instance);
                            }
                        }
                        break;

                    case ArtifactType.ThunderBeastClaw:
                        value = Random.Range(0, 10);

                        if (value < 2)
                        {
                            AttackEffectFunctions.ElectricStatic(15, Mathf.RoundToInt(playerStat.minDamage.GetFinalStatValueAsMultiflyFloat()), Mathf.RoundToInt(playerStat.maxDamage.GetFinalStatValueAsMultiflyFloat()),
                                other.GetComponent<Collider>().bounds.center,PrefabCollect.instance.LightingRenderer, NPC_Type.friendly, gameObject, enemyMask, other);
                        }

                        break;

                    case ArtifactType.FighterGlove:
                        StartCoroutine(CriticalChanceUp(.01f));

                        break;

                    case ArtifactType.BerserkerHeadBand:
                        StartCoroutine(AttackSpeedUp(.01f));

                        break;

                    case ArtifactType.VampirePendant:

                        if(Random.Range(0,100) < 25)
                        {
                            playerStat.Heal(Mathf.RoundToInt(playerStat.maxHealth.GetFinalStatValueAsMultiflyFloat() * 0.01f));
                            ParticleGenerator.instance.LifeDrainParticleGenerate(other.GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.center, gameObject,other);
                        }
                        
                        break;

                    case ArtifactType.BrokenDirk:
                        if (brokenDirkHitCount == 6)
                            brokenDirkHitCount = 0;
                        else
                            brokenDirkHitCount++;
                        break;
                }
            }
        }
    }

    public void ArtifactAttackedEffect(GameObject other)
    {
        if (other)
        {
            if (other.GetComponentInChildren<CharacterStats>())
            {
                foreach (Artifact artifact in playerArtifact)
                {
                    switch (artifact.artifactType)
                    {
                        case ArtifactType.MagmaStone:

                            foreach (BuffNDebuffObject buffNDebuffObject in artifact.Debuff)
                            {
                                var instance = CreateInstanceBuffOrDebuff.CopyInstnace(buffNDebuffObject);
                                instance.buffOrDebuff.Owner = gameObject;

                                int randomValue = Random.Range(0, 10);

                                if (randomValue < 5)
                                    other.GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(instance);
                            }
                            break;

                        case ArtifactType.IceCube:
                            foreach (BuffNDebuffObject buffNDebuffObject in artifact.Debuff)
                            {
                                var instance = CreateInstanceBuffOrDebuff.CopyInstnace(buffNDebuffObject);
                                instance.buffOrDebuff.Owner = gameObject;

                                int randomValue = Random.Range(0, 10);

                                if (randomValue < 5)
                                    other.GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(instance);
                            }
                            break;

                        case ArtifactType.ThunderBeastFoot:
                            int value = Random.Range(0, 10);

                            if (value < 3)
                            {
                                AttackEffectFunctions.ElectricStatic(15, Mathf.RoundToInt(playerStat.minDamage.GetFinalStatValueAsMultiflyFloat()), Mathf.RoundToInt(playerStat.maxDamage.GetFinalStatValueAsMultiflyFloat()),
                                    GetComponent<Collider>().bounds.center, PrefabCollect.instance.LightingRenderer, NPC_Type.friendly, gameObject, enemyMask, other);
                            }
                            break;
                    }
                }
            }
        }
    }

    public void ArtifactAttackedConditionalEffect()
    {

    }

    public void ArtifactKillEffect(GameObject other)
    {
        foreach(Artifact artifact in playerArtifact)
        {
            switch (artifact.artifactType)
            {
                case ArtifactType.SoulCollector:
                    ParticleGenerator.instance.SoulDrainParticleGenerate(other.GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.center, gameObject, other);
                    playerStat.shieldStat.AddShield("SoulCollector", 3, 50, playerStat);

                    break;

                case ArtifactType.ParasiteSpiderEgg:
                    int random = Random.Range(0, 10);

                    if (random < 5)
                    {
                        StartCoroutine(SpawnSpider(artifact, other));
                    }
                    break;

                case ArtifactType.BloodyGem:

                    ParticleGenerator.instance.LifeDrainParticleGenerate(other.GetComponent<Collider>().bounds.center, GetComponent<Collider>().bounds.center, gameObject,other);
                    playerStat.Heal(3);

                    break;

                case ArtifactType.NecromancerOldBook:
                    int random2 = Random.Range(0, 10);

                    if (random2 < 3)
                    {
                        StartCoroutine(SpawnSkeleton(artifact, other));
                    }

                    break;
            }
        }
        
    }

    public void ArtifactRollEffect()
    {
        foreach (Artifact artifact in playerArtifact)
        {
            switch (artifact.artifactType)
            {
                case ArtifactType.BombBag:
                    StartCoroutine(SpawnRollBomb());
                    break;

                case ArtifactType.TrapBag:
                    StartCoroutine(SpawnRollTrap());
                    break;
            }
        }
    }

    public bool ArtifactContain(ArtifactType artifactType)
    {
        foreach(Artifact artifact in playerArtifact)
        {
            if(artifact.artifactType == artifactType)
            {
                return true;
            }
        }

        return false;
    }

    public Artifact GetArtifact(ArtifactType artifactType)
    {
        foreach (Artifact artifact in playerArtifact)
        {
            if (artifact.artifactType == artifactType)
            {
                return artifact;
            }
        }

        return null;
    }

    IEnumerator SpawnSpider(Artifact artifact, GameObject other)
    {
        int random = Random.Range(0, artifact.SummonMonsters.Length);

        var spider = Instantiate(artifact.SummonMonsters[random], other.transform.position, other.transform.rotation);
        spider.GetComponentInChildren<NPC_AI>().Summoner = gameObject;

        yield return new WaitForSeconds(15);

        if (!spider.GetComponentInChildren<NPCStats>().isDead)
            spider.GetComponentInChildren<NPCStats>().KillThisNPC();
    }

    IEnumerator SpawnSkeleton(Artifact artifact, GameObject other)
    {
        int random = Random.Range(0, artifact.SummonMonsters.Length);

        var mob = Instantiate(artifact.SummonMonsters[random], other.transform.position, other.transform.rotation);
        mob.GetComponentInChildren<NPC_AI>().Summoner = gameObject;

        mob.GetComponentInChildren<Animator>().SetTrigger("Summon");
        ParticleGenerator.instance.GenerateGroundParticle(other.transform.position, "SummonCircle", 10f);

        yield return new WaitForSeconds(30);

        if(!mob.GetComponentInChildren<NPCStats>().isDead)
            mob.GetComponentInChildren<NPCStats>().KillThisNPC();
    }

    IEnumerator CriticalChanceUp(float percent)
    {
        GetComponent<PlayerStats>().CritialChange.AddPercentModifier(percent);
        yield return new WaitForSeconds(10);
        GetComponent<PlayerStats>().CritialChange.RemovePercentModifier(percent);
    }

    IEnumerator AttackSpeedUp(float percent)
    {
        GetComponent<PlayerStats>().AttackSpeed.AddPercentModifier(percent);
        yield return new WaitForSeconds(10);
        GetComponent<PlayerStats>().AttackSpeed.RemovePercentModifier(percent);
    }

    IEnumerator StartArtifactEffect_FrozenHeart()
    {
        while(true)
        {
            yield return new WaitForSeconds(5);

            playerStat.shieldStat.AddShield("FrozenHeart", 5, (int)(playerStat.maxHealth.GetFinalStatValueAsMultiflyFloat() * .3f), playerStat);
        }
    }

    public int GetBrokenDirkHitCount()
    {
        return brokenDirkHitCount;
    }


    IEnumerator SpawnRollBomb()
    {
        var bomb = Instantiate(PrefabCollect.instance.miniBomb, transform.position, Quaternion.identity);
        var stat = GetComponent<PlayerStats>();
        bomb.GetComponent<BombThrow>().explodeDamage = Random.Range(Mathf.RoundToInt(stat.minDamage.GetFinalStatValue()), Mathf.RoundToInt(stat.maxDamage.GetFinalStatValue()));
        bomb.GetComponent<BombThrow>().Owner = gameObject;

        yield return null;
    }

    IEnumerator SpawnRollTrap()
    {
        var trap = Instantiate(PrefabCollect.instance.miniTrap, transform.position, Quaternion.identity);
        trap.GetComponent<BearTrap>().owner = gameObject;
        trap.GetComponent<BearTrap>().endTrapTime = 10;

        yield return null;
    }
}
