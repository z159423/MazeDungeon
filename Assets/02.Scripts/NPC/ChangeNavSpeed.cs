using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChangeNavSpeed : StateMachineBehaviour
{
    public bool IndividualSpeed = true;
    public float MoveSpeed;

    public MonsterState monsterStatel;

    private NavMeshAgent agent;
    private NPC_AI ai;



     //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.GetComponent<NavMeshAgent>();
        ai = animator.GetComponent<NPC_AI>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (IndividualSpeed)
        {
            agent.speed = MoveSpeed;
        }
        else
        {
            switch (monsterStatel)
            {
                case MonsterState.idle:
                    agent.speed = 0;
                    break;

                case MonsterState.trace:
                    agent.speed = ai.moveSpeed.GetFinalStatValue();
                    break;

                case MonsterState.meleeAttack:
                    agent.speed = ai.meleeAttackMoveSpeed;
                    break;

                case MonsterState.rangeAttack:
                    agent.speed = ai.meleeAttackMoveSpeed;
                    break;

                default:
                    agent.speed = 1;
                    break;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
