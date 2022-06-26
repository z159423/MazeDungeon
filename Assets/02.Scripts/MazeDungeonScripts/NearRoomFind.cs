using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearRoomFind : MonoBehaviour
{
    public GameObject Wall;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerCheck"))
        {
            Destroy(Wall);
            Destroy(gameObject);
        }
    }
}
