using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contactPointTrigger : MonoBehaviour
{
    NPC_AI npcAI;

    private void Start()
    {
        npcAI = GetComponentInParent<NPC_AI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        npcAI.contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
    }
}
