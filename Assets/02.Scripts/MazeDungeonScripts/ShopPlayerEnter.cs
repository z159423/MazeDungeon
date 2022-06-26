using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPlayerEnter : MonoBehaviour
{
    public ShopKeeper shopKeeper;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !DungeonGenerator.instance.isShopKeeperDead)
        {
            shopKeeper.SpawnHelloText();
        }
    }
}
