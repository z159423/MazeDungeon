using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTrap : MonoBehaviour
{
    public GameObject owner;
    public RogueScripts rougeScripts;
    public int damage = 0;
    public GameObject ExplosionEffect;
    public LayerMask NPCMask;
    public float T_ExplosionRadius;

    public BuffNDebuffObject Burn;

    private Collider collider;
    private Light light;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        light = GetComponentInChildren<Light>();

        StartCoroutine(LightBlink());

        Invoke("EndTrap", 60);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<NPC_AI>() != null && other.CompareTag("Enemy"))
        {
            if (other.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
            {
                Explosion();
            }
        }
    }

    private void Explosion()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, T_ExplosionRadius, NPCMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<CharacterStats>() != null)
            {
                colliders[i].GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(Burn);

                CharacterStats Cstats = colliders[i].GetComponent<CharacterStats>();

                Cstats.TakeDamage(damage, damage, owner, false, true, false, notBackAttack: true);

                StopCoroutine(LightBlink());
                light.range = 1;
            }
        }

        rougeScripts.RemoveExplosionTrap(this);

        var effect = Instantiate(ExplosionEffect, transform.position, ExplosionEffect.transform.rotation);
        collider.enabled = false;
        Destroy(gameObject);

        Destroy(effect, 3.5f);
    }

    IEnumerator LightBlink()
    {
        while(true)
        {
            yield return new WaitForSeconds(.5f);
            light.range = 0;
            yield return new WaitForSeconds(.5f);
            light.range = 1;
        }
    }

    public void EndTrap()
    {
        rougeScripts.RemoveExplosionTrap(this);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
