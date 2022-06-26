using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball_Movement : MonoBehaviour {

    public float speed = 1f;

    GameObject player;
    GameObject playerEquipPoint;

    Rigidbody rigdbody;
    Vector3 forceDirection;
    Animator animator;

    bool isPlayerEnter;
    //bool isPicking;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerEquipPoint = GameObject.FindGameObjectWithTag("EquipPoint");

        animator = GetComponent<Animator>();

        rigdbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && isPlayerEnter)
        {
            transform.SetParent(playerEquipPoint.transform);
            transform.localPosition = Vector3.zero;
            transform.rotation = new Quaternion(0, 0, 0, 0);

            isPlayerEnter = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == player)
        {
            if (other.gameObject == player)
            {
                forceDirection = transform.position;

                forceDirection.x = player.transform.position.x > forceDirection.x ? -1f : 1f;
                forceDirection.y = 0;
                forceDirection.z = player.transform.position.z > forceDirection.z ? -1f : 1f;

                rigdbody.AddForce(forceDirection * speed, ForceMode.Impulse);
            }
        }
    }

    public void pickup (GameObject item)
    {
        SetEquip(item, true);

        animator.SetTrigger("doPickup");
        //isPicking = true;
    }

    void Drop()
    {
        GameObject item = playerEquipPoint.GetComponentInChildren<Rigidbody>().gameObject;
        SetEquip(item, false);

        playerEquipPoint.transform.DetachChildren();
        //isPicking = false;
    }

    void SetEquip(GameObject item, bool isEquip)
    {
        Collider[] itemColliders = item.GetComponents<Collider>();
        Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();

        foreach(Collider itemCollider in itemColliders)
        {
            itemCollider.enabled = !isEquip;
        }
        itemRigidbody.isKinematic = isEquip;
    }
}
