using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPull : MonoBehaviour
{
    public float pullPower = 200f;
    [SerializeField]private Transform parent;
    private List<Collider> targets = new List<Collider>();

    private void Update()
    {
        transform.position = parent.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null)
            {
                targets.RemoveAt(i);
            }
            else
            {
                Vector3 dir = (transform.position - targets[i].bounds.center).normalized;

                targets[i].GetComponentInChildren<Rigidbody>().velocity = (dir * pullPower * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Rigidbody>() != null)
        {
            targets.Add(other);
        }
        else if (other.CompareTag("Enemy") && other.GetComponent<Rigidbody>() != null)
        {
            targets.Add(other);
        }
        else if(other.GetComponentInChildren<DestroyableObject>() && other.GetComponent<Rigidbody>() != null)
        {
            targets.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targets.Contains(other))
            targets.Remove(other);
    }
}
