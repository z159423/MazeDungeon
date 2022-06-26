using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour {

    const float locomationAnimationSmootTime = .1f;

    //NavMeshAgent agent;
    protected Animator animator;

    // Use this for initialization
    protected virtual void Start()
    {
        //agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //float speedPercent = agent.velocity.magnitude / agent.speed;
        //animator.SetFloat("speedPercent", speedPercent, locomationAnimationSmootTime, Time.deltaTime);
    }
}
