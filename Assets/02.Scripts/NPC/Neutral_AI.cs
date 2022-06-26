using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neutral_AI : MonoBehaviour {

    NPCStats enemyStats;

    public enum MonsterState { idle, trace, attack, die };

    public MonsterState monsterState = MonsterState.idle;

    //GameObject player;

    private Collider colli;

    Animator animator;

    //private Transform monsterTr;
    //private Transform playerTr;

    //private bool isDie = false;

    // Use this for initialization



    void Awake()
    {

    }
    void Start()
    {

        //player = GameObject.FindGameObjectWithTag("Player");
        enemyStats = GetComponent<NPCStats>();

        animator = GetComponent<Animator>();

        //monsterTr = this.gameObject.GetComponent<Transform>();

        //playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        //StartCoroutine(this.CheckMonsterState());

        //StartCoroutine(this.MonsterAction());
    }

    // Update is called once per frame
    void Update()
    {
        CheckMonsterState();
        //MonsterAction();
    }

    void CheckMonsterState()
    {

        //yield return new WaitForSeconds(0.2f);

        //float dist = Vector3.Distance(playerTr.position, monsterTr.position);

        
            monsterState = MonsterState.idle;
        



    }

    void MonsterAction()
    {


        switch (monsterState)
        {
            case MonsterState.idle:

                //nav.Stop();

                animator.SetBool("IsAttack", false);

                animator.SetBool("IsTrace", false);
                break;

            case MonsterState.trace:

                animator.SetBool("IsAttack", false);

                animator.SetBool("IsTrace", true);
                break;

            case MonsterState.attack:

                // nav.Stop();

                animator.SetBool("IsAttack", true);
                break;
            case MonsterState.die:

                animator.SetBool("IsDie", true);
                break;


                //yield return null;
        }
    }
}
