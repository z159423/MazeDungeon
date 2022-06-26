using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    PlayerController01 player;
    Collider shieldCollider;
    
    void Start()
    {
        player = GetComponentInParent<PlayerController01>();
        shieldCollider = GetComponent<Collider>();
    }

    
    void Update()
    {
        if(player.isBlocking == true)
        {
            shieldCollider.enabled = true;
        }
        else
        {
            shieldCollider.enabled = false;
        }
        
    }
}
