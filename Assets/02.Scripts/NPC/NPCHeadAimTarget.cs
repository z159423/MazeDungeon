using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHeadAimTarget : MonoBehaviour
{
    NPC_AI AI;
    Vector3 originPosition;

    void Start()
    {
        originPosition = transform.localPosition;
        AI = GetComponentInParent<NPC_AI>();
    }

    void Update()
    {
        if(AI.Target != null)
        {
            transform.position = AI.Target.bounds.center;
        } else
        {
            transform.localPosition = originPosition;
        }
        
    }
}
