using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public GameObject target;
    public float arrowSpeed = 3500;
    public float deleteTime = 1;
    public float damageMultifly = 1;

    public Light magicArrowLight;
    public GameObject ExplosionEffect;

    public GameObject player;
    public BuffNDebuffObject burn;
    private GameObject owner;
    private Rigidbody rigidbody;
    private Collider collider;
    private PlayerStats PStat;

    private int Damage;
    private float T_ExplosionRadius;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<NPC_AI>() != null)
        {
            if (other.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
            {
                StartCoroutine(DeleteObject());
                Explode();
            }
        }
        else if (other.tag == "Ground" || other.tag == "Environment" || other.tag == "Untagged" || other.tag == "BreakableDoor")
        {

            StartCoroutine(DeleteObject());
            Explode();
        }
    }

    public void ColliderOn()
    {
        collider.enabled = true;
    }

    public void FireBallFire()
    {
        var dir = (target.transform.position) - transform.position;
        transform.LookAt(target.transform);

        rigidbody.AddForce(dir.normalized * arrowSpeed);
        StartCoroutine(DeleteObject_AfterManyTime());
    }

    public void SetInfo(float damageMulti, float Radius, GameObject player, BuffNDebuffObject burn)
    {
        damageMultifly = damageMulti;
        T_ExplosionRadius = Radius;
        this.player = player;
        PStat = player.GetComponent<PlayerStats>();
        this.burn = burn;

        target = GameObject.FindWithTag("playertarget");
        rigidbody = transform.GetComponent<Rigidbody>();
        collider = transform.GetComponent<Collider>();
        magicArrowLight = transform.GetComponent<Light>();
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, T_ExplosionRadius);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<CharacterStats>() != null)
            {
                if(colliders[i].gameObject == player)
                {
                    //사용자는 피해 없음
                }
                else
                {
                    CharacterStats Cstats = colliders[i].GetComponent<CharacterStats>();

                    Cstats.TakeDamage(Mathf.RoundToInt(PStat.minDamage.GetFinalStatValue() * damageMultifly)
                        , Mathf.RoundToInt(PStat.maxDamage.GetFinalStatValue() * damageMultifly), PStat.gameObject, false, true, false);

                    Cstats.GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(burn);
                }
            }
        }

        //T_Pstats.transform.GetComponent<PlayerController01>().BombisExpolde();

        var effect = Instantiate(ExplosionEffect, transform.position, ExplosionEffect.transform.rotation);
        collider.enabled = false;
        Destroy(rigidbody);

        Destroy(effect, 3.5f);
    }

    IEnumerator DeleteObject()
    {
        var Particles = GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < Particles.Length; i++)
        {
            var main = Particles[i].main;

            Particles[i].Stop();
        }

        GetComponent<AudioSource>().enabled = false;
        Destroy(transform.gameObject, deleteTime);
        yield return null;
    }

    IEnumerator DeleteObject_AfterManyTime()
    {
        yield return new WaitForSeconds(10);
        Destroy(transform.gameObject);
    }
}
