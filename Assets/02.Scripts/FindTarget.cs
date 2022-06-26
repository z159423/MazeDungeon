using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTarget : MonoBehaviour
{
    Transform target;
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("playertarget").transform;
    }

    void Update()
    {
        transform.position = target.position;
    }
}
