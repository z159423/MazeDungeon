using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RePosition : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && other.GetComponent<Rigidbody>() != null && other.GetComponent<Rigidbody>() != null)
        {
            other.GetComponent<PlayerController01>().RePositionToLastGroundedPosition();
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("ItemPickUp"))
            other.gameObject.SetActive(false);

    }

}
