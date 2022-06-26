using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class SpiderQueen : MonoBehaviour
{
    public float MeleeAttackCoolTime = 3;
    public float MeleeAttackMaxDist = 5;
    public bool MeleeAttackReady = true;
    [Space]

    [SerializeField] private float randompositionX_min = -2;
    [SerializeField] private float randompositionX_max = 2;
    [SerializeField] private float randompositionZ_min = -2;
    [SerializeField] private float randompositionZ_max = 2;

    [SerializeField] GameObject eggObject;
    [SerializeField] Transform layEggPos;
    [SerializeField] Transform eggsFlyingToTarget;

    public float layEggCoolTime = 15f;
    public bool layEggReady = true;
    public int maxSpiderSpawnCount = 3;

    public List<GameObject> spawnedSpider = new List<GameObject>();

    [Space]
    public LineRenderer webLine;
    public Transform webLineStart;
    public Transform webLineEnd;
    private bool webLineRender;


    private Vector3 dirtoTarget;

    private Animator animator;
    private NPC_AI ai;
    private NavMeshAgent agent;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        ai = GetComponent<NPC_AI>();
    }

    private void Update()
    {
        if(webLineRender)
        {
            webLine.SetPosition(0, webLineStart.position);
            webLine.SetPosition(1, webLineEnd.position);
        }

        if (animator.GetBool("IsAttacking") && ai.Target != null)
        {
            agent.SetDestination(ai.Target.transform.position);
        }
    }

    public IEnumerator MeleeAttack()
    {
        MeleeAttackReady = false;
        animator.SetTrigger("MeleeAttack");

        yield return new WaitForSeconds(MeleeAttackCoolTime);
        MeleeAttackReady = true;
    }

    public void layEgg()
    {
        StartCoroutine(layEggCorutine());
    }

    public IEnumerator layEggCorutine()
    {
        layEggReady = false;
        yield return new WaitForSeconds(layEggCoolTime);
        layEggReady = true;
    }

    public void laySpiderEggs()
    {
        var X = Random.Range(randompositionX_min, randompositionX_max);
        var Z = Random.Range(randompositionZ_min, randompositionZ_max);

        eggsFlyingToTarget.position = new Vector3(transform.position.x + X, transform.position.y + 3, transform.position.z + Z);

        dirtoTarget = layEggPos.position - eggsFlyingToTarget.position;

        var egg = Instantiate(eggObject, layEggPos.position,Quaternion.identity);

        egg.GetComponentInChildren<Rigidbody>().AddForce(dirtoTarget.normalized * 150);
        egg.GetComponentInChildren<SpiderEgg>().Owner = this;
    }

    public void CheckBabySpiderDead()
    {
        for(int i = spawnedSpider.Count; i > 0; i--)
        {
            if(!spawnedSpider[i-1].activeSelf)
            {
                spawnedSpider.RemoveAt(i-1);
            }
        }
    }

    public void WebLineStart()
    {
        webLineRender = true;
        webLine.enabled = true;
    }

    public void WebLineEnd()
    {
        webLineRender = false;
        webLine.enabled = false;
    }
}
