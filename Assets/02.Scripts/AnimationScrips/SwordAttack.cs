using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : StateMachineBehaviour
{
    public bool nextAttakReady = false;

    public PlayerAttackLogic attackLogic;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isAttack", true);
        if(animator.GetComponent<PlayerController01>())
            animator.GetComponent<PlayerController01>().isAttacking.AddBoolModifier();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isAttack", false);
        if (animator.GetComponent<PlayerController01>())
        {
            animator.GetComponent<PlayerController01>().rightSwordAttackLogic.attackedByPlayerEnemyClear();
            animator.GetComponent<PlayerController01>().SwordTrailOff();
            animator.GetComponent<PlayerController01>().isAttacking.RemoveBoolModifier();
        }
            

        //if(nextAttakReady)
            //animator.GetComponent<PlayerController01>().NextAttackReady();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
