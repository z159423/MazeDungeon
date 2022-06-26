using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDungeonNpcSpawner : MonoBehaviour
{
    public static MazeDungeonNpcSpawner instance;

    public bool STOP_NPC_Spawn = false;

    public Transform NPCPool;

    public GameObject[] NPCs;
    public NPCStages[] NPCs2;
    public NPCStages[] Boss;

    public NPCPoolByStageLevel[] Npcs3;
    public NPCPoolByStageLevel[] Bosses;
    public NPCPoolByStageLevel[] miniBosses;

    [Space]
    public bool SpawnShopGuards = false;
    public GameObject[] ShopGuards;

    [Space]
    public bool spawnMimic = false;
    public GameObject mimicPrefab;
    [Range(0,100)]
    public int mimicSpawnStartPercent = 10;
    [Range(0, 100)]
    public int mimicSpawnPercentGrowth = 30;
    public GameObject mimic;

    [Space]
    public List<SpawnNPCInRoom> spawnnpcinroom = new List<SpawnNPCInRoom>();
    public List<BossSpawnPointBeacon> bossBeacons = new List<BossSpawnPointBeacon>();

    public List<GameObject> spawnedNpc = new List<GameObject>();
    public List<NPCStats> spawnedBoss = new List<NPCStats>();

    private List<GameObject> bossSpawnList = new List<GameObject>();
    private List<GameObject> removeBosses = new List<GameObject>();

    public int CurrentStageLevel = 1;

    [Space]

    [Range(0, 100)]
    public float eliteMonsterSpawnChance = 25;
    [Range(0, 1)]
    public float eliteHpGrowthMultify = .30f;
    [Range(0, 1)]
    public float eliteArmorGrowthMultify = .30f;
    [Range(0, 1)]
    public float eliteDamageGrowthMultify = .30f;

    [Space]

    [Range(0,1)]
    public float NPCHpGrowthMultify = .33f;
    [Range(0, 1)]
    public float NPCArmorGrowthMultify = .33f;
    [Range(0, 1)]
    public float NPCDamageGrowthMultify = .33f;

    private DungeonGenerator dungeonGenerator;

    private void Start()
    {
        instance = this;

        dungeonGenerator = GetComponent<DungeonGenerator>();
    }

    [System.Serializable]
    public class NPC
    {
        public string NPCname;
        public GameObject NPCPrefab;

    }
    [System.Serializable]
    public class NPCStages
    {
        public Stages appearanceStages;
        public NPC[] npcs;
    }

    [System.Serializable]
    public class NPCPoolByStageLevel
    {
        public int StageLevel;
        public GameObject[] NPCs;
    }

    public void SpawnNPCAtTime()
    {
        foreach (SpawnNPCInRoom inRoom in spawnnpcinroom)
        {
            inRoom.SpawnNpcInRoom();
        }

        foreach (BossSpawnPointBeacon beacon in bossBeacons)
        {
            foreach (NPCPoolByStageLevel nPCPoolByStageLevel in Bosses)
            {
                if (nPCPoolByStageLevel.StageLevel == DungeonGenerator.instance.CurrentStageNumber)
                {
                    bossSpawnList.Clear();
                    bossSpawnList.AddRange(nPCPoolByStageLevel.NPCs);

                    for (int j = 0; j < removeBosses.Count; j++)
                    {
                        for (int i = 0; i < bossSpawnList.Count; i++)
                        {
                            if (bossSpawnList.Contains(removeBosses[j]))
                            {
                                bossSpawnList.Remove(removeBosses[j]);

                                //Debug.LogError(removeBosses[j]);
                                break;
                            }
                        }
                    }

                    int value = Random.Range(0, bossSpawnList.Count);

                    var Boss = Instantiate(bossSpawnList[value], beacon.transform.position, Quaternion.Euler(0, dungeonGenerator.BossChamber.GetComponentInChildren<ChamberChecking>().parentRoomConnectionPoint.ConnectionRotation - 90, 0), NPCPool);

                    removeBosses.Add(bossSpawnList[value]);
                    spawnedBoss.Add(Boss.GetComponentInChildren<NPCStats>());
                }
            }

            /*if (DungeonGenerator.instance.CurrentStage == npc.appearanceStages)
            {
                int value = Random.Range(3, npc.npcs.Length);
                print(value + " " + npc.npcs.Length);
                spawnedBoss.Add(Instantiate(npc.npcs[value].NPCPrefab, beacon.transform.position, Quaternion.identity, NPCPool).GetComponentInChildren<NPCStats>());
            }*/
        }
        if(SpawnShopGuards)
        SpawnShopGuard();
        SpawnMiniBoss();

        NpcLevelAdjustment();
    }

    public void SpawnNpc(List<Transform> trans)
    {
        if(STOP_NPC_Spawn == false)
        {
            foreach (Transform tran in trans)
            {
                foreach(NPCPoolByStageLevel nPCPoolByStageLevel in Npcs3)
                {
                    if(nPCPoolByStageLevel.StageLevel == DungeonGenerator.instance.CurrentStageNumber)
                    {
                        int value = Random.Range(0, nPCPoolByStageLevel.NPCs.Length);
                        var npc = Instantiate(nPCPoolByStageLevel.NPCs[value], tran.position, Quaternion.identity, NPCPool);

                        var EliteMonterChnace = Random.Range(0, 100);

                        if (EliteMonterChnace <= eliteMonsterSpawnChance)
                        {
                            ChnageToEliteMonster(npc);
                        }

                        spawnedNpc.Add(npc);
                    }
                }
            }
        }
    }

    public void SpawnShopGuard()
    {
        var roomCount = Random.Range(0, spawnnpcinroom.Count);

        var spawnPointCount = spawnnpcinroom[roomCount].SpawnPoints.Length;

        //Debug.LogError("roomCount : " + roomCount + " spawnPointCount " + spawnPointCount);
        Vector3 pos = spawnnpcinroom[roomCount].SpawnPoints[Random.Range(0, spawnPointCount)].transform.position;

        GameObject leader = null;
        for (int i = 0; i < ShopGuards.Length; i++)
        {
            var guard = Instantiate(ShopGuards[i], pos, Quaternion.identity, NPCPool);
            spawnedNpc.Add(guard);

            if(i > 0)
            {
                guard.GetComponentInChildren<NPC_AI>().followTarget = leader.transform.GetChild(0).gameObject;
            }
            else
            {
                leader = guard;
            }
        }
    }

    public void SpawnMiniBoss()
    {
        for (int i = 0; i < ChamberManager.Instantce.miniBossCount[DungeonGenerator.instance.CurrentStageNumber].miniBossCount; i++)
        {
            var roomCount = Random.Range(0, spawnnpcinroom.Count);

            var spawnPointCount = spawnnpcinroom[roomCount].SpawnPoints.Length;

            Vector3 pos = spawnnpcinroom[roomCount].SpawnPoints[Random.Range(0, spawnPointCount)].transform.position;

            var miniboss = Instantiate(miniBosses[DungeonGenerator.instance.CurrentStageNumber].NPCs[Random.Range(0, miniBosses[DungeonGenerator.instance.CurrentStageNumber].NPCs.Length)], pos, Quaternion.identity, NPCPool);
            spawnedNpc.Add(miniboss);
        }
    }

    public void SpawnMimic()
    {
        if(mimicSpawnStartPercent > Random.Range(0,100))
        {
            var roomCount = Random.Range(0, spawnnpcinroom.Count);

            var spawnPointCount = spawnnpcinroom[roomCount].SpawnPoints.Length;

            Vector3 pos = spawnnpcinroom[roomCount].SpawnPoints[Random.Range(0, spawnPointCount)].transform.position;

            mimic = Instantiate(mimicPrefab, pos, Quaternion.identity, NPCPool);

            mimicSpawnStartPercent -= 30;

            mimicSpawnStartPercent = Mathf.Clamp(mimicSpawnStartPercent, 0, 100);

            print("미믹 스폰됨");
        }
        else
        {
            mimicSpawnStartPercent += 30;

            mimicSpawnStartPercent = Mathf.Clamp(mimicSpawnStartPercent, 0, 100);
        }

    }

    public void ClearStage()
    {
        DestroyAllNPC();
        DestroyAllBoss();
        spawnnpcinroom.Clear();
    }

    private void DestroyAllNPC()
    {
        foreach(GameObject NPC in spawnedNpc)
        {
            Destroy(NPC);
        }
        spawnedNpc.Clear();

        Destroy(mimic);
    }

    private void DestroyAllBoss()
    {
        foreach (NPCStats Boss in spawnedBoss)
        {
            Destroy(Boss.gameObject);
        }
        spawnedBoss.Clear();
    }

    public void CheckAllBossDead()
    {
        foreach(NPCStats boss in spawnedBoss)
        {
            if (!boss.isDead)
                return;
        }

        print("모든 보스가 죽었습니다.");

        AudioManager.instance.FindBGM(isPreviousBgm: true);

        if (DungeonGenerator.instance.BossChamber.GetComponentInChildren<ExitIronbar>())
            DungeonGenerator.instance.BossChamber.GetComponentInChildren<ExitIronbar>().OpenExit();

        if (DungeonGenerator.instance.BossChamber.GetComponentInChildren<Door>())
            DungeonGenerator.instance.BossChamber.GetComponentInChildren<Door>().OpenDoor();
    }

    private void NpcLevelAdjustment()
    {
        foreach(GameObject npcObject in spawnedNpc)
        {
            npcObject.GetComponentInChildren<NPCStats>().npcLevel = dungeonGenerator.CurrentStageNumber + 1;
            npcObject.GetComponentInChildren<NPCStats>().StatFitToLevel();
        }

        foreach (NPCStats bossStats in spawnedBoss)
        {
            bossStats.GetComponentInChildren<NPCStats>().npcLevel = dungeonGenerator.CurrentStageNumber + 1;
            bossStats.GetComponentInChildren<NPCStats>().StatFitToLevel();
        }
    }


    private void ChnageToEliteMonster(GameObject npc)
    {
        var stat = npc.GetComponentInChildren<NPCStats>();
        var ai = npc.GetComponentInChildren<NPC_AI>();

        stat.transform.localScale *= 1.25f;

        stat.maxHealth.AddIntModifier(Mathf.RoundToInt((float)stat.maxHealth.GetFinalStatValueAsMultiflyFloat() * eliteHpGrowthMultify));
        stat.fullHealth();

        float damage = stat.minDamage.GetFinalStatValue() * eliteDamageGrowthMultify;
        stat.minDamage.AddIntModifier(Mathf.RoundToInt(damage));
        stat.maxDamage.AddIntModifier(Mathf.RoundToInt(damage));

        stat.armor.AddIntModifier(Mathf.RoundToInt(stat.armor.GetFinalStatValue() * eliteArmorGrowthMultify));

        ai.meleeAttackDist *= 1.35f;

        stat.EliteNPC = true;
    }

    public Vector3 GetRandomSpawnPoint()
    {
        var roomCount = Random.Range(0, spawnnpcinroom.Count);

        var spawnPointCount = spawnnpcinroom[roomCount].SpawnPoints.Length;

        //Debug.LogError("roomCount : " + roomCount + " spawnPointCount " + spawnPointCount);
        Vector3 pos = spawnnpcinroom[roomCount].SpawnPoints[Random.Range(0, spawnPointCount)].transform.position;

        return pos;
    }
}

public enum Stages { Jail, AbandonedCastle, FlowerGarden, Library, KingsParlor, ChaosRift }
