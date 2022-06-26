using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueSpawnManager : MonoBehaviour
{
    public List<StatueSpawnPoint> spawnPoints = new List<StatueSpawnPoint>();

    public List<GameObject> spawnedStatues = new List<GameObject>();

    public GameObject[] statuePrefabs;


    public static StatueSpawnManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void SpawnStatues()
    {
        for(int i = 0; i < spawnPoints.Count; i++)
        {
            if(spawnPoints[i] != null)
            {
                if (AttackEffectFunctions.GetRandomResultAsInt(spawnPoints[i].spawnPercent))
                {
                    var statue = Instantiate(statuePrefabs[Random.Range(0, statuePrefabs.Length)], spawnPoints[i].transform);
                    statue.transform.localRotation = Quaternion.Euler(spawnPoints[i].spawnRotate);
                    spawnPoints[i].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));

                    spawnedStatues.Add(statue);
                }
            }
        }
    }
}

