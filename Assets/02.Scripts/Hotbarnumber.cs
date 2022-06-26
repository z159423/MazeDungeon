using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbarnumber : MonoBehaviour {

    //InventorySlot[] Slot;
    Text[] HotbarNum;

	// Use this for initialization
	void Start () {
        HotbarNum = GetComponentsInChildren<Text>();
        //Slot = GetComponentsInChildren<InventorySlot>();
        GetHotbarnumber();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void GetHotbarnumber()
    {
        for(int i = 0; i < HotbarNum.Length; i++)
        {
            HotbarNum[i].text = (i+1).ToString();
        }
    }
}
