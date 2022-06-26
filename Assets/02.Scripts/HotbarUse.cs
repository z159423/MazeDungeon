using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HotbarUse : MonoBehaviour {

    public GameObject[] Hotbars;

    // Use this for initialization
    private void Awake()
    {
        Hotbars = new GameObject[transform.childCount];
    }

    void Start () {
        UpdateHotKey();
    }

    // Update is called once per frame
    void Update () {
        /*
        if (Input.GetButtonDown("Keybored-1"))
        {
            HotKeyUsed(0);
        }
        else if (Input.GetButtonDown("Keybored-2"))
        {
            HotKeyUsed(1);
        }
        else if (Input.GetButtonDown("Keybored-3"))
        {
            HotKeyUsed(2);
        }
        else if (Input.GetButtonDown("Keybored-4"))
        {
            HotKeyUsed(3);
        }
        else if (Input.GetButtonDown("Keybored-5"))
        {
            HotKeyUsed(4);
        }
        else if (Input.GetButtonDown("Keybored-6"))
        {
            HotKeyUsed(5);
        }
        else if (Input.GetButtonDown("Keybored-7"))
        {
            HotKeyUsed(6);
        }
        else if (Input.GetButtonDown("Keybored-8"))
        {
            HotKeyUsed(7);
        }
        else if (Input.GetButtonDown("Keybored-9"))
        {
            HotKeyUsed(8);
        }
        else if (Input.GetButtonDown("Keybored-0"))
        {
            HotKeyUsed(9);
        }
        */
	}

    void HotKeyUsed(int Keycode )
    {
        Hotbars[Keycode].GetComponentInChildren<InventorySlot>().UseItem();

        //Destroy(Hotbar[0].transform.GetChild(2).gameObject);
    }

    void UpdateHotKey()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Hotbars[i] = transform.GetChild(i).gameObject;
        }
    }
}
