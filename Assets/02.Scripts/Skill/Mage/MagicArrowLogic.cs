using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicArrowLogic : MonoBehaviour
{
    public float ChargeMultiply;
    public GameObject target;
    public float arrowSpeed = 500;
    public float deleteTime = 0;

    public Light magicArrowLight;

    public GameObject player;
    private GameObject owner;
    private Rigidbody rigid;
    private Collider Collider;
    [SerializeField] private GameObject FireballObject;

    private int Damage;

    private int minDamage;
    private int maxDamage;

    void Start()
    {
        rigid = transform.GetComponent<Rigidbody>();
        target = GameObject.FindWithTag("playertarget");
        Collider = transform.GetComponent<Collider>();
        magicArrowLight = transform.GetComponent<Light>();
    }

    public void Instans()
    {
        rigid = transform.GetComponent<Rigidbody>();
        target = GameObject.FindWithTag("playertarget");
        Collider = transform.GetComponent<Collider>();
        magicArrowLight = transform.GetComponent<Light>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<NPC_AI>() != null && other.CompareTag("Enemy"))
        {
            if (other.GetComponent<NPC_AI>().npcType == NPC_Type.enemy || other.GetComponentInParent<NPC_AI>().npcType == NPC_Type.neutrality)
            {
                Damage = Mathf.RoundToInt(player.GetComponent<PlayerStats>().maxDamage.GetFinalStatValue() * (int)(ChargeMultiply)) / 10;
                Damage = Mathf.Clamp(Damage, Mathf.RoundToInt(player.GetComponent<PlayerStats>().maxDamage.GetFinalStatValue()), int.MaxValue);

                minDamage = Mathf.RoundToInt(player.GetComponent<PlayerStats>().minDamage.GetFinalStatValue());
                maxDamage = Mathf.RoundToInt(player.GetComponent<PlayerStats>().maxDamage.GetFinalStatValue());

                other.transform.GetComponentInParent<CharacterStats>().TakeDamage(minDamage, maxDamage, player, true, true, false);

                StartCoroutine(DeleteObject());
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("obstacle"))
        {
            StartCoroutine(DeleteObject());
        }
        else if (other.tag == "BreakableDoor")
        {
            other.gameObject.GetComponent<DestroyableObject>().GetDamage(5);
            StartCoroutine(DeleteObject());
        }

        if (other.GetComponent<DestroyableObject>() != null)
        {
            other.GetComponent<DestroyableObject>().GetDamage(1);
            other.GetComponent<DestroyableObject>().GenerateHitParticle(GetComponent<Collider>().bounds.center);
        }
    }
    
    /*private void OnCollisionEnter(Collision other)
    {
        if (other.collider.GetComponent<NPC_AI>() != null)
        {
            if (other.collider.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
            {
                Damage = (player.GetComponent<PlayerStats>().damage.GetValue() * (int)(ChargeMultiply)) / 10;
                Damage = Mathf.Clamp(Damage, player.GetComponent<PlayerStats>().damage.GetValue(), int.MaxValue);
                other.transform.GetComponentInParent<CharacterStats>().TakeDamage(Damage, player, true, true, false);

                StartCoroutine(DeleteObject());
                //print(other.gameObject);
            }
        }
        else if (other.collider.tag == "Ground" || other.collider.tag == "Environment" || other.collider.tag == "Untagged")
        {
            StartCoroutine(DeleteObject());
            //print(other.gameObject);
        }else if (other.collider.tag == "BreakableDoor")
        {
            other.gameObject.GetComponent<DestroyableObject>().GetDamage(5);
            StartCoroutine(DeleteObject());
        }
    }*/
    public void ColliderOn()
    {
        Collider.enabled = true;
    }   

    public void MagicArrowFire()
    {
        var dir = (target.transform.position - transform.position);
        transform.LookAt(target.transform);

        rigid.AddForce(dir.normalized * arrowSpeed);
        Collider.enabled = true;
        StartCoroutine(DeleteObject_AfterManyTime());
    }


    IEnumerator DeleteObject()
    {
        var Particles = GetComponentsInChildren<ParticleSystem>();

        Collider.enabled = false;
        Destroy(rigid);

        for (int i = 0; i < Particles.Length; i++)
        {
            Debug.Log(Particles[i]);
            var main = Particles[i].main;

            Particles[i].Stop();
        }
        Destroy(FireballObject);
        Destroy(transform.gameObject, deleteTime);
        GetComponent<AudioSource>().enabled = false;
        yield return null;
    }

    IEnumerator DeleteObject_AfterManyTime()
    {
        yield return new WaitForSeconds(10);
        Destroy(transform.gameObject);
    }
}
