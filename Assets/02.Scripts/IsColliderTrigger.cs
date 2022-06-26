using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsColliderTrigger : MonoBehaviour
{
    public bool isTrigged;

    private void OnTriggerEnter(Collider other)
    {
        isTrigged = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isTrigged = false;
    }
}
