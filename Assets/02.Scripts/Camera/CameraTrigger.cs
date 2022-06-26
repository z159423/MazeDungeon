using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Vector3 triggerPoint;
    public Vector3 avgPoint;
    public LayerMask obstacleMask;
    public Collider collider;
    public bool triggered = false;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("obstacle"))
        {
            triggerPoint = other.ClosestPoint(collider.bounds.center);

            //float dist = Vector3.Distance(triggerPoint, collider.bounds.center);

            avgPoint = (triggerPoint + collider.bounds.center) / 2;

            triggered = true;
        }
        else
        {
            triggered = false;
        }

    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(triggerPoint, .2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(avgPoint, .1f);
    }
}
