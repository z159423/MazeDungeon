using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DungeonGenerator.instance.playerSpawnPoints.Add(this);
    }

    private void OnDestroy()
    {
        DungeonGenerator.instance.playerSpawnPoints.Remove(this);
    }

    public void AddPlayerSpawnPoint()
    {
        DungeonGenerator.instance.playerSpawnPoints.Add(this);
    }
}
