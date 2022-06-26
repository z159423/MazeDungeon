using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestPointBeacon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DungeonGenerator.instance.chestPointBeacons.Add(this);
    }

    private void OnDestroy()
    {
        DungeonGenerator.instance.chestPointBeacons.Remove(this);
    }
}
