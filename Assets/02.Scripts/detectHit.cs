using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class detectHit : MonoBehaviour {

    public Slider healthbar;

    void OnTriggerEnter(Collider other)
    {
        healthbar.value -= 5;
        Debug.Log("Hit");
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
