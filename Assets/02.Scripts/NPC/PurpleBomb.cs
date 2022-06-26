using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurpleBomb : MonoBehaviour
{
    public float ExplotionTime = 4;
    public float ExplosionRadius = 5;
    public int Damage = 10;
    public GameObject Particle;
    public NPC_Type npcType;
    public GameObject Owner;

    private void Start()
    {
        Invoke("explode", ExplotionTime);
    }

    public void explode()
    {
        AttackEffectFunctions.explode(ExplosionRadius, Damage, transform.position, Particle, npcType, Owner);
        Destroy(gameObject);
    }
}
