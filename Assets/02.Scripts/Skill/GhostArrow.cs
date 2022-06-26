using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostArrow : MonoBehaviour
{
    public GameObject target;
    public float arrowSpeed = 10f;
    public float turningSpeed = 0.1f;
    public float deleteTime = 0;
    public float sensingDist = 20f;
    public LayerMask TargetMask;
    public LayerMask ObstacleMask;
    private Rigidbody rigid;
    private Collider thisCollider;
    private Collider EnemyTarget;
    private Vector3 dirToAim;
    private Vector3 dirToEnemy;
    private Transform AimTranfrom;
    
    [HideInInspector]
    public int Damage = 0;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public float m_currentSpeed = 0f;

    [HideInInspector]
    public List<Collider> collsList = new List<Collider>();
    Collider[] colls;
    void Start()
    {
        target = GameObject.FindWithTag("playertarget");
        AimTranfrom = target.transform;
        dirToAim = (AimTranfrom.position - player.transform.position).normalized;
        rigid = GetComponent<Rigidbody>();
        thisCollider = GetComponent<Collider>();
        
        ArrowFire();

        Destroy(gameObject, 7);
    }

    void LateUpdate()
    {
        
        if (EnemyTarget != null)
        {
            if(!EnemyTarget.gameObject.activeSelf)
            {
                StartCoroutine(CheckNearEnemy());
            }
            dirToEnemy = (EnemyTarget.bounds.center - transform.position).normalized;

            rigid.velocity = transform.up * arrowSpeed;
            transform.up = Vector3.Lerp(transform.up, dirToEnemy, turningSpeed);
        }
        else
        {
            transform.up = Vector3.Lerp(transform.up, dirToAim, turningSpeed);
            //transform.position += dirToAim * 0.5f;
            rigid.velocity = dirToAim * arrowSpeed;
        }
    }

    private void OnEnable()
    {
        StartCoroutine(CheckNearEnemy());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<NPC_AI>() != null)
        {
            if (other.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
            {
                other.GetComponent<NPCStats>().TakeDamage(Damage, Damage, player, true, true, false);
                StartCoroutine(DeleteObject());
            }
        }
        /*else if (other.tag == "Ground" || other.tag == "Environment" || other.tag == "Untagged" || other.tag == "BreakableDoor")
        {

            StartCoroutine(DeleteObject());
        }*/
    }

    public void ArrowFire()
    {
        var dir = (target.transform.position) - player.transform.position;
        transform.LookAt(target.transform);

        //rigidbody.AddForce(dir.normalized * arrowSpeed);
    }

    public void GetArrowDamageValue(int playerDamage, int skillDamageFactor)
    {
        Damage = playerDamage * skillDamageFactor;
    }

    IEnumerator DeleteObject()
    {
        var Particles = GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < Particles.Length; i++)
        {
            var main = Particles[i].main;

            Particles[i].Stop();
        }
        Destroy(transform.gameObject, deleteTime);
        yield return null;
    }

    

    IEnumerator CheckNearEnemy()
    {
        WaitForSeconds delay = new WaitForSeconds(0.1f);

        while (gameObject.activeSelf)
        {
            yield return delay;
            colls = Physics.OverlapSphere(this.transform.position, sensingDist, TargetMask);

            for (int i = 0; i < colls.Length; i++)
            {
                if(colls[i].GetComponentInChildren<NPC_AI>() != null)
                {
                    if(colls[i].GetComponentInChildren<NPC_AI>().npcType == NPC_Type.enemy)
                    {
                        dirToEnemy = (colls[i].bounds.center - transform.position).normalized;
                        float dstToTarget = Vector3.Distance(GetComponent<Collider>().bounds.center, colls[i].bounds.center);
                        if (!Physics.Raycast(GetComponent<Collider>().bounds.center, dirToEnemy, dstToTarget, ObstacleMask))
                        {
                            collsList.Add(colls[i]);
                            EnemyTarget = colls[i];

                            StopCoroutine(this.CheckNearEnemy());
                        }
                    }
                }
            }
        }
    }
}
