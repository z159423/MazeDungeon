using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningOrbCenter : MonoBehaviour
{
    public Rigidbody rootRigidBody;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("obstacle"))
        {
            Destroy(rootRigidBody.gameObject);
            
        }
    }
}
