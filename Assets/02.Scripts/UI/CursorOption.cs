using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorOption : MonoBehaviour {

    UIManager allUI;

	// Use this for initialization
	void Start () {
        allUI = UIManager.instance;
    }
	
	// Update is called once per frame
	void Update () {
		/*if(!allUI.IsAnyUiOn())
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }*/
	}
}
