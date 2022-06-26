using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallbackTest : MonoBehaviour {

    DelegateTest delegatetest;

	// Use this for initialization
	void Start () {
        delegatetest = DelegateTest.instance;
        delegatetest.happencallback += callbacktest;

        delegatetest.happencallback();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void callbacktest()
    {
        Debug.Log("CallBackTest");
    }
}
