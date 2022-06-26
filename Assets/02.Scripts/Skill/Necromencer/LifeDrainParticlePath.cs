using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeDrainParticlePath : MonoBehaviour
{
    public Transform owner;
    public float disToTarget;


    public IEnumerator ChangePath()
    {
        yield return new WaitForSeconds(0.1f);
        transform.SetParent(owner);
        transform.localScale = new Vector3(1, 1, 1);
        transform.localPosition = new Vector3(0, 1, 0);
        Destroy(gameObject, disToTarget);
    }
}
