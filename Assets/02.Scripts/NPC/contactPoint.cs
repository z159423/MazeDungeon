using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class contactPoint : MonoBehaviour
{
    NPC_AI npcAI;

    private void Start()
    {
        if (GetComponentInParent<NPC_AI>() != null)
        {
            npcAI = GetComponentInParent<NPC_AI>();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            npcAI.contactPoint = collision.contacts[0].point;
            print("충돌지점 : " );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            npcAI.contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(GetComponent<Collider>().bounds.center);
    }
}
