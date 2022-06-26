using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupMassage : MonoBehaviour {

    //PlayerController01 player;

    public Text ItemPickUpText;
    void PickUpMassage(string Itemname)
    {
        ItemPickUpText.text = Itemname += "을 주으시려면 e키를 누루십시요.";
    }

    
}
