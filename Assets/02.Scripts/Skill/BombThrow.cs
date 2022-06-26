using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombThrow : MonoBehaviour
{
    
    PlayerStats T_Pstats;
    Skill T_skill;
    public GameObject Owner;
    public float ExplosionRadius = 8f;
    public int explodeDamage = 30;
    public float explodeTime = 5;

    public LayerMask Mask;
    public NPC_Type[] TargetNpc;
    public NPC_Type ownerType;

    public BuffNDebuffObject[] debuffs;

    public GameObject explosionParticlePrefab;

    public void ExplodeCountDown(float ExplosionRadius, PlayerStats Pstats, Skill skill, float explodeTime)
    {
        this.ExplosionRadius = ExplosionRadius;
        T_Pstats = Pstats;
        T_skill = skill;
        
        Invoke("Explode", explodeTime);
    }

    private void Start()
    {
        Invoke("Explode", explodeTime);
    }

    private void Explode()
    {
        /*Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius, Mask);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<CharacterStats>() != null)
            {
                CharacterStats Cstats = colliders[i].GetComponent<CharacterStats>();

                foreach (NPC_Type nPC_Type in TargetNpc)
                {
                    if(nPC_Type == Cstats.GetComponent<NPC_AI>().npcType)
                        Cstats.TakeDamage(explodeDamage, explodeDamage, Owner, false, true, true);
                }

            }
        }*/

        AttackEffectFunctions.explode(ExplosionRadius, explodeDamage, transform.position, explosionParticlePrefab, ownerType, Owner, debuffs : debuffs);

        //T_Pstats.transform.GetComponent<PlayerController01>().BombisExpolde();

        //var effect = Instantiate(explosionParticlePrefab, transform.position, explosionParticlePrefab.transform.rotation);

        //Destroy(effect, 3f);
        Destroy(transform.gameObject);
    }
}
