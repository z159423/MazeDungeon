using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderEgg : MonoBehaviour
{
    public SpiderQueen Owner;

    [SerializeField]GameObject spawnObject;

    [SerializeField] float spawnTime = 7;

    [SerializeField] int eggLevel = 1;

    private void OnEnable()
    {
        StartCoroutine(SpawnEgg());
        StartCoroutine(Wiggle());
    }

    IEnumerator SpawnEgg()
    {
        yield return new WaitForSeconds(spawnTime);

        var spider = Instantiate(spawnObject, transform.position, Quaternion.identity);

        spider.GetComponentInChildren<NPCStats>().setLevel(eggLevel);
        spider.GetComponentInChildren<NPCStats>().StatFitToLevel();

        if (Owner != null)
            Owner.spawnedSpider.Add(spider);

        int name = Random.Range(1, 4);

        AudioManager.instance.GenerateAudioAndPlaySFX("spiderEggBroke" + name.ToString(), GetComponent<Collider>().bounds.center);

        Destroy(transform.root.gameObject);

        if (GetComponentInChildren<MeshRenderer>())
            ParticleGenerator.instance.GenerateDeathParticle_meshrenderer(transform.position + transform.up, "Death_Renderer", GetComponentInChildren<MeshRenderer>(), 1);

        
    }

    IEnumerator Wiggle()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(1.25f,1.57f));

            GetComponent<Rigidbody>().AddForce(Random.insideUnitCircle.normalized * 50);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetEggLevel(int level)
    {
        eggLevel = level;
    }
}
