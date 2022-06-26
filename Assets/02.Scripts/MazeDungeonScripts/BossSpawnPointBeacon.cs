using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnPointBeacon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MazeDungeonNpcSpawner.instance.bossBeacons.Add(this);
    }

    private void OnDestroy()
    {
        if(MazeDungeonNpcSpawner.instance)
            MazeDungeonNpcSpawner.instance.bossBeacons.Remove(this);
    }
}
