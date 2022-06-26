using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHeadAim : MonoBehaviour
{
    [SerializeField] NPC_AI Ai;

    private void Start()
    {
        Ai = GetComponentInParent<NPC_AI>();
    }

    void Update()
    {
        if(Ai.Target == null)
        {
            if(Ai.LookingTarget)
                transform.position = Ai.LookingTarget.transform.position;
        }
        else if(Ai.Target)
        {
            transform.position = Ai.Target.transform.position;
        }
        else
        {
            transform.position = Vector3.zero;
        }
            
    }
}
