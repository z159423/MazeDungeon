using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenKingFall : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<NPC_AttackLogic>().ColliderOff();
        animator.GetComponent<NPC_AttackLogic>().WeaponTrailOff();
        animator.GetComponent<NPC_AttackLogic>().FunchEnd();

        animator.GetComponent<FallenKing>().attackReady = false;
        animator.GetComponent<FallenKing>().fall = true;
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<FallenKing>().attackReady = true;
        animator.GetComponent<FallenKing>().fall = false;
        animator.GetComponent<FallenKing>().phaseTwoStart();
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
