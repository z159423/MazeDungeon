using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{

    [SerializeField] float ExplosionRadius;
    [SerializeField] int ExplosionDamage;
    public GameObject Summoner;

    NPC_Type npcType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<NPC_AI>() != null)
        {
            if (other.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
            {
                Destroy(transform.root.gameObject);
                Explosion();
            }
        }
        else if (other.tag == "Ground" || other.tag == "Environment" || other.tag == "Untagged" || other.tag == "BreakableDoor")
        {

            Destroy(transform.root.gameObject);
            Explosion();
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        Explosion();
        Destroy(transform.root.gameObject);
    }*/

    private void Explosion()
    {
        if(Summoner == null)
        {
            npcType = NPC_Type.enemy;
        } else
        {
            npcType = NPC_Type.friendly;
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);

        transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (npcType == NPC_Type.friendly || npcType == NPC_Type.Minion)
            {
                if (colliders[i].GetComponentInChildren<NPC_AI>() != null)
                {
                    if (colliders[i].GetComponentInChildren<NPC_AI>().npcType == NPC_Type.enemy)
                    {
                        CharacterStats Cstats = colliders[i].GetComponentInChildren<CharacterStats>();

                        Cstats.TakeDamage(ExplosionDamage,ExplosionDamage, Summoner.gameObject, false, true, false);
                    }
                }
            }
            else if (npcType == NPC_Type.enemy)
            {
                if (colliders[i].CompareTag("Player"))
                {
                    colliders[i].GetComponentInChildren<PlayerStats>().TakeDamage(ExplosionDamage,ExplosionDamage, Summoner.gameObject, false, true, false);
                }
                else if (npcType == NPC_Type.friendly || npcType == NPC_Type.Minion)
                {
                    CharacterStats Cstats = colliders[i].GetComponent<CharacterStats>();

                    Cstats.TakeDamage(ExplosionDamage,ExplosionDamage, Summoner.gameObject, false, true, false);
                }
            }

        }

        ParticleGenerator.instance.ExplosionParticle(transform.position, "Explosion Small");
    }
}
