using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNPCInRoom : MonoBehaviour
{
    public NPCSpawnPointBeacon[] SpawnPoints;
    private List<Transform> NPCSpawnPoint = new List<Transform>();

    private void Start()
    {
        MazeDungeonNpcSpawner.instance.spawnnpcinroom.Add(this);
    }

    public void SpawnNpcInRoom()
    {
        SpawnPoints = transform.GetComponentsInChildren<NPCSpawnPointBeacon>();

        for (int i = 0; i < SpawnPoints.Length; i++)
        {
            NPCSpawnPoint.Add(SpawnPoints[i].transform);
        }

        MazeDungeonNpcSpawner.instance.SpawnNpc(NPCSpawnPoint);

        /*var room = GetComponentInParent<AddRoom>();

        if (room.roomType != RoomType.boss && room.roomType != RoomType.treasure && room.roomType != RoomType.shop)
        {
            
        }*/
    }

    private void OnDestroy()
    {
        MazeDungeonNpcSpawner.instance.spawnnpcinroom.Remove(this);
    }
}
