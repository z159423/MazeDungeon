using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contactPoint_Player : MonoBehaviour
{
    PlayerController01 player;

    private void Start()
    {
        player = GetComponentInParent<PlayerController01>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
            player.contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(GetComponent<Collider>().bounds.center);
    }
}
