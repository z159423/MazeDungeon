using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shackle : MonoBehaviour
{
    private new Rigidbody rigidbody;
    private new Collider collider;
    public bool isFire = false;
    public bool isTrigged = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<NPC_AI>() != null)
        {
            if (other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.enemy)
            {
                transform.SetParent(other.transform);
                rigidbody.useGravity = false;
                rigidbody.isKinematic = true;
                isTrigged = true;
                collider.enabled = false;
            }
        }
        else if (other.tag == "Ground" || other.tag == "Environment")
        {
            transform.SetParent(other.transform);
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            //isTrigged = true;
            collider.enabled = false;
        }
    }
}
