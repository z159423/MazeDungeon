using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera01 : MonoBehaviour {

    public Transform target;

    public float distance = 10.0f;

    public float height = 5.0f;

    float heightDamping =  2.0f;

    //float rotationDamping = 3.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LateUpdate()
    {
        //float currentAngle = transform.eulerAngles.x;
        float currentHeight = transform.position.y;

        //float targetAngleY = target.eulerAngles.y;
        float targetHeight = target.position.y + height;

        //currentAngleY = Mathf.LerpAngle(0, targetAngleY, rotationDamping * Time.deltaTime);

        currentHeight = Mathf.Lerp(currentHeight, targetHeight, heightDamping * Time.deltaTime);

        //Quaternion newRotation = Quaternion.Euler(0, 0, 0);

    }
}
