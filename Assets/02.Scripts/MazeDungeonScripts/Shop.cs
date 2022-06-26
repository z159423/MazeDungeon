using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public GameObject ShopKeeper;

    
    public void EnableShopKeeper()
    {
        ShopKeeper.SetActive(true);
    }
}
