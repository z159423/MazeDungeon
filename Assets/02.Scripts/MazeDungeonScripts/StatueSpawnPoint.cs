using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueSpawnPoint : MonoBehaviour
{
    [Range(0,100)]
    public int spawnPercent = 0;

    public Vector3 spawnRotate;

    private void OnEnable()
    {
        StatueSpawnManager.instance.spawnPoints.Add(this);
    }

    private void OnDestroy()
    {
        StatueSpawnManager.instance.spawnPoints.Remove(this);
    }
}
