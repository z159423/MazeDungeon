using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCObjectPool : MonoBehaviour
{

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public BiomeType 스폰되는지형;
    }

    #region Singleton

    public static NPCObjectPool Instance;
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    SpawnNPC spawnNPC;

    public bool dontSpawn = false;
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        spawnNPC = SpawnNPC.instance;

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        if (dontSpawn == false)
        {
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }
    }

    public string GetTagFromPool(BiomeType type)        //BiomeType으로 NPCPool에서 일치하는 Tag중 하나 가져오기
    {
        List<string> tagList = new List<string>();

        foreach (Pool pool in pools)
        {
            if(pool.스폰되는지형 == type)
            {
                tagList.Add(pool.tag);
            }
        }

        int RandomValue = Random.Range(0, tagList.Count);

        return tagList[RandomValue];
    }

    public GameObject SpawnFromPool (string tag, Vector3 position, Quaternion rotation)
    {
        /*
        foreach(string tag2 in poolDictionary.Keys)
        {
            print(tag2);
        }*/

        if(!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("엔피씨 오브젝트 풀 안에 " + tag + " 가 없습니다.");
            return null;
        }

        GameObject objectToSpawn = null;

        //이미 생성된 npc 인스턴스의 개수를 초과하여 npc를 생성할시 새로운 npc인스턴스를 생성해서 추가함

        for (int i = 0; i < poolDictionary[tag].Count; i++)
        {
            objectToSpawn = poolDictionary[tag].Dequeue();

            if (!spawnNPC.SpawnedNpc.Contains(objectToSpawn))
            {

                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                objectToSpawn.SetActive(true);

                poolDictionary[tag].Enqueue(objectToSpawn);
                return objectToSpawn;
            }
            else
            {
                poolDictionary[tag].Enqueue(objectToSpawn);
            }
        }

        if (spawnNPC.SpawnedNpc.Count >= poolDictionary[tag].Count)     
        {
            foreach (Pool pool in pools)
            {
                print(tag + " " + pool.tag);
                if (pool.tag == tag)
                {
                    GameObject addedObject = Instantiate(pool.prefab);
                    addedObject.SetActive(false);
                    poolDictionary[tag].Enqueue(addedObject);

                    addedObject.transform.position = position;
                    addedObject.transform.rotation = rotation;
                    addedObject.SetActive(true);

                    return addedObject;
                }
            }
        }


        return null;
        
    }

}
