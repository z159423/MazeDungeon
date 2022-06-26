using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNPC : MonoBehaviour
{

    #region Singleton

    public static SpawnNPC instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public int maxNPCSpawnCount = 5;
    public float SpawnRange;
    public float SpawnRate;
    public float NPCDstCheckLate = 2;
    public float MaxNPCViewDist = 500;

    private Transform playerTransform;
    private NPCObjectPool NPCPool;
    private MapGenerator mapGenerator;
    private HealthUI healthUI;

    

    public class PoolValue
    {
        public List<GameObject> NPCObject;
        public string tag;
    }

    public Dictionary<string, PoolValue> PlacedNPC;
    public List<GameObject> SpawnedNpc = new List<GameObject>();

    void Start()
    {
        PlacedNPC = new Dictionary<string, PoolValue>();
        playerTransform = GameObject.FindWithTag("Player").transform;
        NPCPool = NPCObjectPool.Instance;
        mapGenerator = MapGenerator.instance;

        foreach (NPCObjectPool.Pool Pools in NPCPool.pools)
        {
            PoolValue value = new PoolValue();
            value.NPCObject = new List<GameObject>();
            value.tag = Pools.tag;

            PlacedNPC.Add(Pools.tag, value);
        }

        StartCoroutine(SpawnNPCFromBiome());
        StartCoroutine(CheckNPCDstFromPlayer2());
        //StartCoroutine(CheckNpcPlaced());
        //StartCoroutine(CheckNPCDstFromPlayer());
    }

    IEnumerator SpawnNPCFromBiome()
    {
        yield return new WaitForSeconds(0.5f);

        if(SpawnedNpc.Count < maxNPCSpawnCount)
        {
            StartCoroutine(CheckBiomeSurrundingPlayerAndSpawn());
        }else
        {
            print("스폰 최대수를 초과하였습니다. NPC 스폰 중지중 " + SpawnedNpc.Count);
        }

        yield return new WaitForSeconds(SpawnRate);
        StartCoroutine(SpawnNPCFromBiome());
    }

    IEnumerator CheckBiomeSurrundingPlayerAndSpawn()
    {

        Vector3 Rvector = new Vector3(Random.Range(-SpawnRange, SpawnRange), 0, Random.Range(-SpawnRange, SpawnRange));
        RaycastHit hit;

        if (Physics.Raycast(playerTransform.position + Rvector + new Vector3(0, 500f, 0), Vector3.down, out hit, 2000f, 5))
        {
            BiomePointInfo biomePoint = GetCloestBiomeInfo(hit.point);

            StartCoroutine(NPCreplaceAsBiome(NPCPool.GetTagFromPool(biomePoint.biomeType), hit.point));
        }
        else
        {
            print("레이케스트 감지 실패 npc스폰 실패");
        }

        yield return null;
    }

    IEnumerator NPCreplaceAsBiome(string tag, Vector3 spawnPosition)
    {
        print(tag);
        GameObject obj = NPCPool.SpawnFromPool(tag, spawnPosition + Vector3.up, new Quaternion(0, 0, 0, 0));
        NPCStats Stats = obj.GetComponentInChildren<NPCStats>();

        Stats = SettingNPCStats(Stats);

        SpawnedNpc.Add(obj);

        LocalNavMeshBuilder localNav = obj.GetComponentInChildren<LocalNavMeshBuilder>();
        localNav.enabled = false;
        localNav.enabled = true;

        obj.SetActive(false);
        obj.SetActive(true);

        obj.GetComponentInChildren<NPC_AI>().ResetNPCAi();

        print("추가된 npc " + obj.name + " Count " + SpawnedNpc.Count + " Level : " + Stats.npcLevel + " ID: " + obj.GetInstanceID());

        yield return null;
    }

    IEnumerator CheckNPCDstFromPlayer2() //플레이어와 스폰된 npc의 거리를 측정해 거리가 MaxNPCViewDist 보다 멀어지면 플레이어 근처로 리스폰 
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < SpawnedNpc.Count; i++)
        {
            float dist = Vector3.Distance(SpawnedNpc[i].transform.position, playerTransform.position);
            //print(list.NPCObject[i].gameObject + " " + dist);
            if (dist > MaxNPCViewDist)
            {
                SpawnedNpc[i].GetComponentInChildren<NPCStats>().resetNPC();
                SpawnedNpc.Remove(SpawnedNpc[i]);
            }
        }

        yield return new WaitForSeconds(NPCDstCheckLate);
        StartCoroutine(CheckNPCDstFromPlayer2());
    }

    IEnumerator CheckNPCDstFromPlayer() //플레이어와 스폰된 npc의 거리를 측정해 거리가 MaxNPCViewDist 보다 멀어지면 플레이어 근처로 리스폰 
    {
        yield return new WaitForSeconds(0.5f);

        foreach (PoolValue list in PlacedNPC.Values)
        {
            for(int i = 0; i < list.NPCObject.Count; i++)
            {
                float dist = Vector3.Distance(list.NPCObject[i].transform.position, playerTransform.position);
                //print(list.NPCObject[i].gameObject + " " + dist);
                if(dist > MaxNPCViewDist)
                {
                    list.NPCObject[i].GetComponentInChildren<NPCStats>().resetNPC();
                    list.NPCObject.Remove(list.NPCObject[i]);
                }

            }
        }
        
        yield return new WaitForSeconds(NPCDstCheckLate);
        StartCoroutine(CheckNPCDstFromPlayer());
    }


    IEnumerator CheckNpcPlaced()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (PoolValue list in PlacedNPC.Values)
        {
            //print(NPCPool.poolDictionary[list.tag].Count);
            if (list.NPCObject.Count < NPCPool.poolDictionary[list.tag].Count)
            {
                print("list.NPCObject.Count " + list.NPCObject.Count + " NPCPool.poolDictionary[list.tag].Count " + NPCPool.poolDictionary[list.tag].Count);
                
                //print(list.NPCObject.Count);
                StartCoroutine(NPCreplace(list.tag));
            }
        }

        yield return new WaitForSeconds(SpawnRate);
        StartCoroutine(CheckNpcPlaced());
    }

    IEnumerator NPCreplace(string tag)
    {
        Vector3 Rvector = new Vector3(Random.Range(-SpawnRange, SpawnRange), 0, Random.Range(-SpawnRange, SpawnRange));
        RaycastHit hit;

        if (Physics.Raycast(playerTransform.position + Rvector + new Vector3(0, 500f, 0), Vector3.down, out hit, 2000f, 5))
        {
           // print(tag);
            GameObject obj =  NPCPool.SpawnFromPool(tag, hit.point, new Quaternion(0, 0, 0, 0));

            NPCStats Stats = obj.GetComponentInChildren<NPCStats>();
            

            Stats = SettingNPCStats(Stats);

            PlacedNPC[tag].NPCObject.Add(obj);

            LocalNavMeshBuilder localNav = obj.GetComponentInChildren<LocalNavMeshBuilder>();
            localNav.enabled = false;
            localNav.enabled = true;

            obj.SetActive(false);
            obj.SetActive(true);

            obj.GetComponentInChildren<NPC_AI>().ResetNPCAi();

            print("npc 추가 tag :  " + tag + " count : " + PlacedNPC[tag].NPCObject.Count + " Level : " + Stats.npcLevel + " ID: " + obj.GetInstanceID());
        }

        yield return null;
    }

    NPCStats SettingNPCStats(NPCStats stats)
    {
        stats.npcLevel = GetLevelFormBiomeLevel(stats.transform.position);
        stats.npcLevel = (stats.npcLevel <= 0) ? 1 : stats.npcLevel;
        healthUI = stats.GetComponent<HealthUI>();
        //healthUI.GetLvlText(stats.npcLevel);

        stats.maxDamage.ClearIntModifier();
        stats.armor.ClearIntModifier();

        stats.maxHealth.ClearIntModifier();

        stats.maxHealth.AddIntModifier(stats.npcLevel * 35);
        stats.maxDamage.AddIntModifier(stats.npcLevel * 3);
        stats.armor.AddIntModifier(stats.npcLevel * 1);
        stats.IndividualDropTable.EXPDropAmount = stats.npcLevel * 5;

        return stats;
    }

    int GetLevelFormBiomeLevel(Vector3 vector)
    {
        Vector2Int temp = new Vector2Int((int)vector.x, (int)vector.y);
        BiomePointInfo biomePoint = mapGenerator.GetClosestCentroidIndex(temp);
        return biomePoint.BiomeLevel;
    }

    BiomePointInfo GetCloestBiomeInfo(Vector3 vector)
    {
        Vector2Int temp = new Vector2Int((int)vector.x, (int)vector.y);
        BiomePointInfo biomePoint = mapGenerator.GetClosestCentroidIndex(temp);
        return biomePoint;
    }
}
