using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValiedChamber : MonoBehaviour
{
    public ValiedChamber ConnectionRoom = null;
    [SerializeField] ValidConnectionType connectionType;

    public void CheckStuckHall()
    {
        if(connectionType == ValidConnectionType.Hallway || connectionType == ValidConnectionType.Corner)
        {
            ConnectionRoom.CheckStuckHall();
            Destroy(gameObject);
        }
    }
}
