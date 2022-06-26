using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonGas : MonoBehaviour
{
    public GameObject owner;
    public int minDamage;
    public int maxDamage;
    public BuffNDebuffObject poison;
    public float DeleteTime = 7;
    public float damageTickLate = 1.5f;

    public ParticleSystem posionBulkParticle;

    void Start()
    {
        var main = posionBulkParticle.main;
        main.startLifetime = DeleteTime;
        StartCoroutine(StopPoison());
        StartCoroutine(Poison());
    }
    IEnumerator StopPoison()
    {
        yield return new WaitForSeconds(DeleteTime);
        Destroy(gameObject);
    }

    IEnumerator Poison()
    {
        new WaitForSeconds(damageTickLate);

        float time = 0;
        Collider[] colls;
        LayerMask targetMask = (1 << LayerMask.NameToLayer("NPC"));


        while (true)
        {
            colls = Physics.OverlapSphere(transform.position, 8, targetMask);

            foreach (Collider NPCCollider in colls)
            {
                if (NPCCollider.GetComponentInChildren<NPC_AI>() != null)
                {
                    if (NPCCollider.GetComponentInChildren<NPC_AI>().npcType == NPC_Type.enemy || NPCCollider.GetComponentInChildren<NPC_AI>().npcType == NPC_Type.neutrality)
                    {
                        NPCCollider.GetComponentInChildren<NPCStats>().TakeDamage(minDamage, maxDamage, owner, false, true, true, NotCritical: true, isDebuffDamage: true, notBackAttack: true);
                        NPCCollider.GetComponentInChildren<CharacterBuffDeBuff>().AddBuffOrDebuff(poison);
                    }
                        
                }
            }
            time += damageTickLate;
            yield return new WaitForSeconds(damageTickLate);
        }
    }
}
