using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;


public class ParticleGenerator : MonoBehaviour
{

    [System.Serializable]
    public struct particleStruct
    {
        public string S;
        public ParticleSystem PS;
    }

    [System.Serializable]
    public struct LootEffectStruct
    {
        public string S;
        public GameObject PS;
    }

    public static ParticleGenerator instance;

    [SerializeField]
    public particleStruct[] particleStructs;

    [SerializeField]
    public LootEffectStruct[] LootEffectStruects;

    [SerializeField]
    public Dictionary<string, GameObject> LootEffectDic = new Dictionary<string, GameObject>();

    [SerializeField]
    public Dictionary<string, ParticleSystem> particleObjectDic = new Dictionary<string, ParticleSystem>();

    private void Awake()
    {

        instance = this;

        for (int i = 0; i < particleStructs.Length; i++)
        {
            particleObjectDic.Add(particleStructs[i].S, particleStructs[i].PS);
        }

        for (int i = 0; i < LootEffectStruects.Length; i++)
        {
            LootEffectDic.Add(LootEffectStruects[i].S, LootEffectStruects[i].PS);
        }
    }

    public void GenerateLootEffect(Transform transform, ItemQuality itemQuality)
    {
        string S = "Common";
        if (itemQuality == ItemQuality.Common)
        {
            S = "Common";
        }
        /*else if (itemQuality == ItemQuality.Uncommon)
        {
            S = "Uncommon";
        }*/
        else if (itemQuality == ItemQuality.Rare)
        {
            S = "Rare";
        }
        else if (itemQuality == ItemQuality.Unique)
        {
            S = "Unique";
        }
        else if (itemQuality == ItemQuality.Epic)
        {
            S = "Epic";
        }

        LootEffectDic.TryGetValue(S, out GameObject value);

        var lootEffect = Instantiate(value, transform);
        lootEffect.GetComponentInChildren<LootEffectController>().TargetObject = transform;
    }

    public void GenerateConsumableEffect(Transform transform)
    {
        var lootEffect = Instantiate(PrefabCollect.instance.consumableEffect, transform);
        lootEffect.GetComponentInChildren<LootEffectController>().TargetObject = transform;
    }

    public void GenerateItemQualityParticle(Transform transform, ItemQuality itemQuality)
    {
        string S = "Common";
        if(itemQuality == ItemQuality.Common)
        {
            S = "Common";
        } /*else if(itemQuality == ItemQuality.Uncommon)
        {
            S = "Uncommon";
        }*/
        else if (itemQuality == ItemQuality.Rare)
        {
            S = "Rare";
        }
        else if (itemQuality == ItemQuality.Unique)
        {
            S = "Unique";
        }
        else if (itemQuality == ItemQuality.Epic)
        {
            S = "Epic";
        }

        particleObjectDic.TryGetValue(S, out ParticleSystem value);

        var particleObject = Instantiate(value.gameObject,transform);
        var particleComponent = particleObject.GetComponent<ParticleSystem>();
        var PS = particleComponent.shape;
        PS.mesh = particleObject.GetComponentInParent<ItemPickup>().item.skinedMesh.sharedMesh;
    }

    public void GenerateDeathParticle(Vector3 Vec, string S, Material[] mat, SkinnedMeshRenderer skinned, float particleSize)
    {
        particleObjectDic.TryGetValue(S, out ParticleSystem value);
        GameObject ob = Instantiate(value.gameObject, Vec + Vector3.up, new Quaternion(0, 0, 0, 0));

        var shape = ob.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        var particleSystem = ob.GetComponentInChildren<ParticleSystem>();
        //Debug.Log((skinned.bounds.extents.x + skinned.bounds.extents.y + skinned.bounds.extents.z));
        particleSystem.startSize = particleSystem.startSize / particleSize;
        particleSystem.startSpeed = particleSystem.startSpeed / particleSize;

        //var emission = particleSystem.emission;
        
        //emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(emission.GetBurst(0).time, (short)(emission.GetBurst(0).maxCount * particleSize)) });

        //Destroy(gameObject, value.startLifetime);

        //var em = ob.GetComponent<ParticleSystem>().emission;
        //em.rateOverTime = em.rateOverTime.constant / mat.Length;
        //ParticleSystem.Burst burst = em.GetBurst(0);
        //burst.count = burst.count.constant / mat.Length;
        //em.SetBurst(0, burst);


        //ParticleSystemRenderer pr = ob.GetComponent<ParticleSystemRenderer>();
        //pr.material = mat[i];

        Destroy(ob, value.main.startLifetime.constant);

    }

    public void GenerateDeathParticle_meshrenderer(Vector3 Vec, string S, MeshRenderer renderer, float particleSize)
    {
        particleObjectDic.TryGetValue(S, out ParticleSystem value);
        GameObject ob = Instantiate(value.gameObject, Vec + Vector3.up, new Quaternion(0, 0, 0, 0));

        var shape = ob.GetComponent<ParticleSystem>().shape;
        shape.meshRenderer = renderer;

        print(ob.GetComponent<ParticleSystem>().shape.meshRenderer);

        var particleSystem = ob.GetComponentInChildren<ParticleSystem>();
        particleSystem.startSize = particleSystem.startSize / particleSize;
        particleSystem.startSpeed = particleSystem.startSpeed / particleSize;

        //var emission = particleSystem.emission;

        //emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(emission.GetBurst(0).time, (short)(emission.GetBurst(0).maxCount * particleSize)) });


        //Destroy(gameObject, value.startLifetime);

        //var em = ob.GetComponent<ParticleSystem>().emission;
        //em.rateOverTime = em.rateOverTime.constant / mat.Length;
        //ParticleSystem.Burst burst = em.GetBurst(0);
        //burst.count = burst.count.constant / mat.Length;
        //em.SetBurst(0, burst);


        //ParticleSystemRenderer pr = ob.GetComponent<ParticleSystemRenderer>();
        //pr.material = mat[i];

        Destroy(ob, value.main.startLifetime.constant);

    }

    public void GenerateHitParticle(Vector3 Vec, string S, Material[] mat, Vector3 hitDirection, SkinnedMeshRenderer skinned, float particleSize)
    {
        //print("HitEffect : " + hitDirection);
        particleObjectDic.TryGetValue(S, out ParticleSystem value);
        GameObject ob = Instantiate(value.gameObject, Vec + Vector3.up, Quaternion.FromToRotation(Vec, hitDirection));
        ParticleSystemRenderer pr = ob.GetComponent<ParticleSystemRenderer>();
        //pr.material = mat[Random.Range(0, mat.Length)];

        var shape = ob.GetComponent<ParticleSystem>().shape;
        shape.skinnedMeshRenderer = skinned;

        var particleSystem = ob.GetComponentInChildren<ParticleSystem>();
        particleSystem.startSize = particleSystem.startSize / particleSize;
        particleSystem.startSpeed = particleSystem.startSpeed / particleSize;

        var emission = particleSystem.emission;
        //emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(emission.GetBurst(0).time, (short)(emission.GetBurst(0).maxCount * particleSize)) });


        Destroy(ob, value.main.startLifetime.constant);
    }

    public void GenerateHitEffect(Vector3 Vec, string S)
    {
        particleObjectDic.TryGetValue(S, out ParticleSystem value);
        GameObject ob = Instantiate(value.gameObject, Vec, Quaternion.identity);

        Destroy(ob, 3f);
    }

    public GameObject GenerateGroundParticle(Vector3 Vec, string S, float deleteTime)
    {
        particleObjectDic.TryGetValue(S, out ParticleSystem value);
        if (value == null)
            print("value == null");
        GameObject ob = Instantiate(value.gameObject, Vec, Quaternion.identity);

        AudioManager.instance.GenerateAudioAndPlaySFX("teleport1", Vec);

        Destroy(ob, deleteTime);

        return ob;
    }

    public void ExplosionParticle(Vector3 Vec, string S)
    {
        particleObjectDic.TryGetValue(S, out ParticleSystem value);
        GameObject ob = Instantiate(value.gameObject, Vec, new Quaternion(0,0,0,0));

        Destroy(ob, 5f);
    }

    public void StaticParticleGenerate(Transform Start, Transform[] Ends)
    {

    }

    public void LifeDrainParticleGenerate(Vector3 otherPosition, Vector3 ownerPosition, GameObject other, GameObject owner)
    {
        GameObject effect = Instantiate(PrefabCollect.instance.LifeDrainParticle, otherPosition, new Quaternion(0, 0, 0, 0), other.transform);

        effect.GetComponent<LifeDrainParticlePath>().owner = other.transform;
        var disToOwner = Vector3.Distance(ownerPosition, otherPosition);

        if (disToOwner <= 5)
        {
            effect.GetComponent<LifeDrainParticlePath>().disToTarget = 1f;
        }
        else if (disToOwner <= 10)
        {
            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 5;
        }
        else if (disToOwner <= 25)
        {
            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 10;
        }
        else
        {
            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 15;
        }

        StartCoroutine(effect.GetComponent<LifeDrainParticlePath>().ChangePath());
    }

    public void ManaDrainParticleGenerate(Vector3 otherPosition, Vector3 ownerPosition, GameObject other)
    {
        GameObject effect = Instantiate(PrefabCollect.instance.ManaDrainParticle, otherPosition, new Quaternion(0, 0, 0, 0), other.transform);

        effect.GetComponent<LifeDrainParticlePath>().owner = other.transform;
        var disToOwner = Vector3.Distance(ownerPosition, otherPosition);

        if (disToOwner <= 5)
        {
            effect.GetComponent<LifeDrainParticlePath>().disToTarget = 1f;
        }
        else if (disToOwner <= 10)
        {
            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 5;
        }
        else if (disToOwner <= 25)
        {
            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 10;
        }
        else
        {
            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 15;
        }

        StartCoroutine(effect.GetComponent<LifeDrainParticlePath>().ChangePath());
    }

    public void SoulDrainParticleGenerate(Vector3 otherPosition, Vector3 ownerPosition, GameObject other, GameObject owner)
    {
        //Debug.LogError("otherPosition : " + otherPosition + " ownerPosition : " + ownerPosition);
        GameObject effect = Instantiate(PrefabCollect.instance.SoulDrainParticle, otherPosition, new Quaternion(0, 0, 0, 0), other.transform);
        effect.GetComponent<LifeDrainParticlePath>().owner = other.transform;
        var disToOwner = Vector3.Distance(ownerPosition, otherPosition);

        if (disToOwner <= 5)
        {
            effect.GetComponent<LifeDrainParticlePath>().disToTarget = 1f;
        }
        else if (disToOwner <= 10)
        {
            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 5;
        }
        else if (disToOwner <= 25)
        {
            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 10;
        }
        else
        {
            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 15;
        }

        StartCoroutine(effect.GetComponent<LifeDrainParticlePath>().ChangePath());
    }

}