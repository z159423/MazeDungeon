using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFieldOfView : MonoBehaviour
{
    public float viewRadius;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public Collider[] TargetsInViewRadius;

    public List<Transform> visibleTargets = new List<Transform>();

    void Start()
    {
        StartCoroutine(StartFinding(.2f));
    }

    IEnumerator StartFinding(float delay)
    {
        while(true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    public void FindVisibleTargets()
    {
        TargetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        visibleTargets.Clear();

        for (int i = 0; i < TargetsInViewRadius.Length; i++)
        {
            Transform TargetTransform = TargetsInViewRadius[i].transform;
            Vector3 dirToTarget = (TargetTransform.position - transform.position).normalized;
            float dstToTarget = Vector3.Distance(transform.position + Vector3.up, TargetTransform.position + Vector3.up);
            if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
            {
                visibleTargets.Add(TargetTransform);
            }
        }
    }
}



