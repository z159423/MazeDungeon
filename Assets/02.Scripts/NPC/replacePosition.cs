using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class replacePosition : MonoBehaviour
{
    
    void FixedUpdate()
    {
        if(transform.position.y < -500)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.up,out hit, 1000f);

            transform.gameObject.SetActive(false);
            transform.position = hit.point + Vector3.up;
            transform.gameObject.SetActive(true);
        }
    }
}
