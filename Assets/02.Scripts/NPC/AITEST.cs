using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class AITEST : MonoBehaviour
{
    private NavMeshAgent agent;

    private Vector3 dir;

    public Transform target;
    public float moveSpeed;

    public Vector3[] corners;
    public Vector3 nextPosition;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.isStopped = false;

        agent.SetDestination(target.position);

        StartCoroutine(warp());
    }

    // Update is called once per frame
    void Update()
    {
        //agent.Warp(transform.position);

        agent.velocity = Vector3.zero;

        if (target)
            agent.SetDestination(target.position);

        transform.LookAt(target.position);

        corners = agent.path.corners; 

        if(corners.Length > 1)
        {
            if(Vector3.Distance(transform.position, corners[1]) > agent.stoppingDistance)
            {
                dir = corners[1] - transform.position;

                dir.y = 0;
                nextPosition = agent.nextPosition;

                transform.position += transform.TransformDirection(dir.normalized) * moveSpeed * Time.deltaTime;
            }
            
        }
        else
        {
            dir = target.position;
        }

    }

    IEnumerator warp()
    {
        agent.Warp(transform.position);

        yield return new WaitForSeconds(0.1f);

        StartCoroutine(warp());
    }

    private void OnDrawGizmos()
    {
        foreach (Vector3 corner in corners)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(corner, 0.5f);
        }
    }
}
