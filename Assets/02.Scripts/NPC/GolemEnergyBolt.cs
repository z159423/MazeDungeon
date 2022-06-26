using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemEnergyBolt : ProjectileLogic
{
    public GameObject ChargingParticle;

    public float explosionTime = 8;

    private void Start()
    {
        base.Start();
        StartCoroutine(ExplodeInTime());
    }

    IEnumerator ExplodeInTime()
    {
        yield return new WaitForSeconds(explosionTime);

        Explode();
        Destroy(gameObject);
    }

    public void Fire(Collider target, Transform firePos, GameObject Owner)
    {
        GetComponent<Rigidbody>().isKinematic = false;
        base.Fire(target.bounds.center, firePos.position, Owner);
        GetComponent<Collider>().enabled = true;
        
        transform.SetParent(null);

        ChargingParticle.GetComponent<ParticleSystem>().Stop();
    }
}
