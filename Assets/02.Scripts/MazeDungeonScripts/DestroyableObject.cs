using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    public bool useAnimator = false;

    public int HP;
    public GameObject destroyableObject;
    public GameObject AudioSource = null;
    [Space]
    public bool useDestroySound = false;
    public string destroySound;

    [Space]
    public bool useHitSound = false;
    public string hitSound;

    [Space]
    public bool useHitParticle = false;

    [Space]
    public bool useDeathParticle = false;

    private Animator Animator;

    public float particleSize = 1;
    [Space]
    [Range(0, 100)]
    public int coinDropPercent = 30;
    public int coinDropMinValue = 0;
    public int coinDropMaxValue = 10;
    [Space]
    [Range(0, 100)]
    public int consumableItemDropPercent = 15;
    [Space]
    [Range(0, 100)]
    public int keyDropPercent = 5;

    private void Start()
    {
        if(useAnimator)
            Animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.GetComponent<PlayerController01>() != null)
        {
            if(collision.transform.GetComponent<PlayerController01>().getDodgeBool())
            {
                DESTRUCTION();
            }
        }
    }

    public void GetDamage(int damage)
    {
        HP -= damage;
        //FloatingTextController.CreateFloatingText(damage.ToString(), transform, transform.gameObject.tag, 15, gameObject);
        if (useHitSound)
            AudioManager.instance.GenerateAudioAndPlaySFX(hitSound, GetComponent<Collider>().bounds.center);
        //GetComponent<AudioSource>().PlayOneShot(HitSound);

        if (useAnimator)
            Animator.SetTrigger("Hit");

        if (HP <= 0)
            DESTRUCTION();
    }

    public void DESTRUCTION()
    {
        if(useDestroySound)
        {
            //var audioSource = Instantiate(AudioSource,null,true);
            //audioSource.transform.position = transform.position;
            //audioSource.GetComponent<AudioSource>().PlayOneShot(DestroySound);
            //Destroy(audioSource, 5);

            AudioManager.instance.GenerateAudioAndPlaySFX(destroySound, GetComponent<Collider>().bounds.center);
        }

        if (useDeathParticle)
            ParticleGenerator.instance.GenerateDeathParticle_meshrenderer(transform.position + transform.up, "Death_Renderer", GetComponentInChildren<MeshRenderer>(), particleSize);

        destroyableObject.SetActive(false);

        int coinPercent = Random.Range(0, 100);

        if(coinDropPercent > coinPercent)
        {
            int coinValue = Random.Range(coinDropMinValue, coinDropMaxValue);

            DropItem.instance.DropCoin(coinValue, transform.position, new Vector3(0, 0, 0), 0.1f);
        }

        int consumablePercent = Random.Range(0, 100);

        if (consumableItemDropPercent > consumablePercent)
        {
            DropItem.instance.CreateConsumeableItem(transform);
        }

        int keyPercent = Random.Range(0, 100);

        if (keyDropPercent > keyPercent)
        {
            DropItem.instance.KeyGenerate(transform.position);
        }

        Destroy(destroyableObject, 5);
    }

    public void GenerateHitParticle(Vector3 colliderCenterPosition)
    {
        if(useHitParticle)
            ParticleGenerator.instance.GenerateHitEffect(GetComponent<Collider>().ClosestPointOnBounds(colliderCenterPosition), "Hit2");
    }
}
