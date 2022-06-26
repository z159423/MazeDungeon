using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    public List<PlayerInfo> Players = new List<PlayerInfo>();
    
    void Start()
    {
        instance = this;
    }

    [System.Serializable]
    public class PlayerInfo
    {
        public GameObject playerobject;
        public GameObject Skillobject;
    }
}
