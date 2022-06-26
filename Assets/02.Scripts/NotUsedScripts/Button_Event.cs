using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Event : MonoBehaviour {

    public GameObject unityBall;

    GameObject shootPoint;
    GameObject player;


    //Animator animator;
    public Vector3 shootingDegree;
    bool isPlayerEnter;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        shootPoint = GameObject.FindGameObjectWithTag("BallShootPoint");
        //animator = GetComponent<Animator>();

        isPlayerEnter = false;
    }

    // Update is called once per frame
    void Update () {
		if(isPlayerEnter && Input.GetButtonDown("Fire1"))
        {
            //animator.SetTrigger("PushButton");

            BallShoot();

            Invoke("BallShoot", 1.5f);
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            isPlayerEnter = true;
        }    
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject == player)
        {
            isPlayerEnter = false;
        }
    }

    void BallShoot()
    {
        GameObject instantItem = (GameObject)Instantiate(unityBall, shootPoint.transform.position, shootPoint.transform.rotation);

        Rigidbody rigidbody = instantItem.GetComponent<Rigidbody>();
        rigidbody.AddForce(shootingDegree * 100f, ForceMode.Impulse);
    }
}
