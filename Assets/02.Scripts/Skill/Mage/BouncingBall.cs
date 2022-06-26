using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingBall : MonoBehaviour
{
    public GameObject target;
    public GameObject owner;
    public float flyingSpeed;
    public float deleteTime = 5f;
    public int damage = 0;
    public float sensingDist;
    public LayerMask TargetMask;
    public LayerMask ObstacleMask;

    public int MaxbouncingCount = 0;

    private int currentBouncedCount = 0;
    private new Rigidbody rigidbody;
    private Collider collider;
    private Collider lastAttackedNPC;


    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<BoxCollider>();
        target = GameObject.FindWithTag("playertarget");

        var dir = target.transform.position - gameObject.transform.position;

        transform.LookAt(target.transform);
        rigidbody.AddForce(dir.normalized * flyingSpeed);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<NPC_AI>() != null)
        {
            if ((other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.enemy || other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.neutrality) && lastAttackedNPC != other)
            {
                lastAttackedNPC = other;
                other.transform.GetComponentInParent<CharacterStats>().TakeDamage(damage,damage, owner, true, true, false);
                StartCoroutine(findNextTarget());
            }
        }
        else if (other.tag == "Ground" || other.tag == "Environment")
        {

            StartCoroutine(Delete());
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponentInParent<NPC_AI>() != null)
        {
            if (collision.collider.GetComponentInParent<NPC_AI>().npcType == NPC_Type.enemy)
            {
                collision.transform.GetComponentInParent<CharacterStats>().TakeDamage(damage, owner, true, true, false);
                StartCoroutine(findNextTarget(new Vector3()));
            }
        }
        else
        {
            ContactPoint cp = collision.GetContact(0);
            Vector3 dir = transform.position - cp.point; // 접촉지점에서부터 탄위치 의 방향
            rigidbody.AddForce((dir).normalized * flyingSpeed);

            StartCoroutine(findNextTarget(dir));
        }
    }*/

    IEnumerator findNextTarget()
    {
        currentBouncedCount++;

        if (currentBouncedCount >= MaxbouncingCount)
        {
            StartCoroutine(Delete());
        }

        target = null;
        
        List<Collider> collsList = new List<Collider>();
        float cloestDist = Mathf.Infinity;

        var colls = Physics.OverlapSphere(this.transform.position, sensingDist, TargetMask);

        for (int i = 0; i < colls.Length; i++)
        {
            Transform TargetTransform = colls[i].transform;
            Vector3 dirToTarget = ((TargetTransform.position + Vector3.up) - (GetComponent<Collider>().bounds.center)).normalized;
            float dstToTarget = Vector3.Distance(GetComponent<Collider>().bounds.center, TargetTransform.position + Vector3.up);
            if (!Physics.Raycast(GetComponent<Collider>().bounds.center, dirToTarget, dstToTarget, ObstacleMask))
            {
                collsList.Add(colls[i]);
            }
        }

        foreach(Collider collider in collsList)
        {
            if(Vector3.Distance(GetComponent<Collider>().bounds.center, collider.bounds.center) < cloestDist && lastAttackedNPC != collider)
            {
                if(collider.GetComponent<NPC_AI>() != null)
                {
                    if(collider.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
                    {
                        cloestDist = Vector3.Distance(GetComponent<Collider>().bounds.center, collider.bounds.center);
                        target = collider.gameObject;
                    }
                }
            }
        }

        if(target != null)
        {
            var dir = target.GetComponent<Collider>().bounds.center - gameObject.transform.position;

            rigidbody.isKinematic = true;
            rigidbody.isKinematic = false;

            transform.LookAt(target.transform);
            rigidbody.AddForce(dir.normalized * flyingSpeed);

            print(target);
        }
        else
        {
            StartCoroutine(Delete());
        }

        yield return null;
    }

    IEnumerator Delete()
    {
        Destroy(gameObject);

        yield return null;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
