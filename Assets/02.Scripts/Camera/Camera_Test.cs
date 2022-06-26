using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Test : MonoBehaviour {

    public Transform target;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float dist = 4f;

    public float xSpeed = 220.0f;
    public float ySpeed = 100.0f;

    //private float x = 0.0f;
    //private float y = 0.0f;

    public float dampRotate = 5.0f;
    public float scrollSpeed = 20.0f;

    

    // Use this for initialization
    void Start () {
        
    }

    // Update is called once per frame
    void Update()
    {

        

            //transform.RotateAround(target.position, Vector3.up, 40 * Time.deltaTime);
            transform.RotateAround(target.position, Vector3.up, Input.GetAxis("Mouse X") * Time.deltaTime * 40);

            transform.LookAt(target);
      }
    
}
