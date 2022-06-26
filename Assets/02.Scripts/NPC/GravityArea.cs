using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityArea : MonoBehaviour
{
    public Collider Target;
    private Vector3 dirToEnemy;
    public float moveSpeed = 100;
    public Rigidbody rigid;

    [Space]

    public int tickDamage = 0;
    public float GravityTickTime = 0.3f;
    public GameObject owner;

    private List<GameObject> targets = new List<GameObject>();

    [Space]

    public float explodeTime = 10f;
    public float explodeRadius = 8f;
    public int explodeDamage = 0;
    public GameObject explodePartcle;

    private void Start()
    {
        StartCoroutine(StartGravity());

        Invoke("Explode", explodeTime);
    }

    private void FixedUpdate()
    {
        if (Target != null)
        {
            dirToEnemy = (Target.bounds.center - transform.position).normalized;

            rigid.velocity = (dirToEnemy * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            targets.Add(other.gameObject);
        }
        else if(other.CompareTag("Enemy"))
        {
            if(other.GetComponent<NPC_AI>().npcType == NPC_Type.Minion || other.GetComponent<NPC_AI>().npcType == NPC_Type.neutrality)
                targets.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targets.Contains(other.gameObject))
            targets.Remove(other.gameObject);
    }

    IEnumerator StartGravity()
    {
        while(true)
        {
            for(int i = 0; i < targets.Count; i++)
            {
                if(targets[i] == null)
                {
                    targets.RemoveAt(i);
                }
                else
                {
                    targets[i].GetComponent<CharacterStats>().TakeDamage(tickDamage, tickDamage, owner, true, true, false, isDebuffDamage: true, notBackAttack: true);
                }
            }

            yield return new WaitForSeconds(GravityTickTime);
        }
    }

    void Explode()
    {
        AttackEffectFunctions.explode(explodeRadius, explodeDamage, transform.position, explodePartcle, NPC_Type.enemy, owner);
        AudioManager.instance.GenerateAudioAndPlaySFX("explode1", transform.position);

        Destroy(gameObject);
    }

    private void OnDisable()
    {
        StopCoroutine(StartGravity());
    }


}
