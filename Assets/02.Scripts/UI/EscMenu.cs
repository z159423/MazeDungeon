using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscMenu : MonoBehaviour {

    public GameObject ESCMENU;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    }

    public void OnResumeButton()
    {
        ESCMENU.SetActive(false);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
