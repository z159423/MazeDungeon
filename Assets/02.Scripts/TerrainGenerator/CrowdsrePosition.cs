using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdsrePosition : MonoBehaviour
{
    Transform[] ObjectCrowd;

    void Start()
    {
        ObjectCrowd = GetComponentsInChildren<Transform>();

        rePosition();
    }

    void rePosition()
    {
        foreach (Transform trans in ObjectCrowd)
        {
            RaycastHit hit;
            if (Physics.Raycast(trans.position + new Vector3(0, 100f, 0), Vector3.down, out hit, 200) && hit.transform.tag == "Ground")
            {
                trans.position = hit.point;
            }
            else
            {
                Destroy(trans.gameObject);
            }
        }
    }
}
