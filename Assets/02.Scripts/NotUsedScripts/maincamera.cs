using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maincamera : MonoBehaviour {

    //추적할 대상
    public Transform target;            


    public float dist = 10.0f;
    public float height = 5.0f;
    public float dampRotate = 5.0f;

    private Transform tr;

	// Use this for initialization
	void Start () {
        //Cursor.lockState = CursorLockMode.Locked;

        tr = GetComponent<Transform>();
	}

    // Update is called once per frame

    void Update()
    {
       float currYangle = Mathf.LerpAngle(tr.eulerAngles.y, target.eulerAngles.y, dampRotate * Time.deltaTime);

        //float currYangleX = Mathf.LerpAngle(tr.eulerAngles.x, target.eulerAngles.x, dampRotate * Time.deltaTime);

        Quaternion rot = Quaternion.Euler(0, currYangle, 0);

        tr.position = target.position - (rot * Vector3.forward * dist) + (Vector3.up * height);

        tr.LookAt(target);
    }

    void LateUpdate () {
        
	}
}
