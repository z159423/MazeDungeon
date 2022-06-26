using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickUp : MonoBehaviour
{

    public bool isOnSale = true;
    public int PurchaseValue = 50;


    public bool PickUp()
    {
        if (isOnSale)
        {
            if (Inventory.instance.coinAmount < PurchaseValue)
            {
                InfoMessageManager.instance.PopupInfoMessage("NotEnoughMoney");
                Debug.Log("코인이 부족하여 구매할 수 없습니다 코인갯수 :  " + Inventory.instance.coinAmount);
                return false;
            }
            else
            {
                Inventory.instance.LoseCoin(PurchaseValue);
                Inventory.instance.GetKey(1);
                AudioManager.instance.GenerateAudioAndPlaySFX("purchase", GetComponentInChildren<Collider>().bounds.center);
                Destroy(gameObject);
                return true;
            }
        }
        else
        {
            Inventory.instance.GetKey(1);
            Destroy(gameObject);
            return true;
        }

    }

}
