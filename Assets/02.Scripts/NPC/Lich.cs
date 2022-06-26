using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Lich : MonoBehaviour
{

    public float rangeAttackCoolTime = 5;
    public float rangeAttackMaxDist = 20;
    public Transform rangeAttackFirePos;
    public GameObject rangeAttackProjectile;
    public bool rangeAttackReady = true;

    [Space]

    public float SummonCoolTime = 15f;
    public bool summonReady = true;
    public int maxSummonCount = 3;
    public GameObject[] Skeletons;
    public List<GameObject> SummonedCreatures = new List<GameObject>();

    [Space]

    public float TeleportCoolTime = 20;
    public float TeleportMaxDist = 15f;
    public bool TeleportReady = true;

    [Space]

    public float shieldCoolTime = 25;
    public int shieldAmount = 50;
    public bool shieldReady = true;

    [Space]

    public float explosionSkullCoolTime = 15f;
    public float explosionSkullMaxDist = 25f;
    public float explosionSkullMinDist = 7f;
    public bool explosionSkullReady = true;
    public GameObject explosionSkullPrefab;
    public int explosionSkullAmount = 1;
    public int explosionSkullAngle = 15;

    [Space]

    public bool isAttacking = false;
    private Animator animator;
    private NPC_AI Ai;
    private CharacterStats stat;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        Ai = GetComponent<NPC_AI>();
        stat = GetComponent<CharacterStats>();
    }


    public IEnumerator RangeAttack()
    {
        animator.SetTrigger("RangeAttack");

        rangeAttackReady = false;
        yield return new WaitForSeconds(rangeAttackCoolTime);
        rangeAttackReady = true;
    }

    public void GenerateRangeAttack()
    {
        //var angel = Quaternion.FromToRotation(transform.rotation.eulerAngles, Target.transform.position);

        GameObject fire = Instantiate(rangeAttackProjectile, rangeAttackFirePos.position, rangeAttackFirePos.localRotation);

        fire.transform.LookAt(Ai.Target.bounds.center);
        fire.transform.Rotate(new Vector3(90, 0, 0));
        fire.GetComponentInChildren<Rigidbody>().AddForce(((Ai.Target.bounds.center) - rangeAttackFirePos.position).normalized * 1500f);
        fire.GetComponent<ProjectileLogic>().moveForce = 1500f;
        fire.GetComponent<ProjectileLogic>().Setting(gameObject, Mathf.RoundToInt(GetComponent<NPCStats>().minDamage.GetFinalStatValue()), Mathf.RoundToInt(GetComponent<NPCStats>().maxDamage.GetFinalStatValue()));
        Destroy(fire, 4);
    }

    public IEnumerator summonSkeleton()
    {
        animator.SetTrigger("SummonSkeleton");

        summonReady = false;
        yield return new WaitForSeconds(SummonCoolTime);
        summonReady = true;
    }

    public void SummonSkeleton()
    {
        for(int i = 0; i < SummonedCreatures.Count ; i++)
        {
            if (SummonedCreatures[i].activeSelf == false)
            {
                SummonedCreatures.Remove(SummonedCreatures[i]);
            }
        }

        /*foreach (GameObject Creature in SummonedCreatures)
        {
            if (Creature.activeSelf == false)
            {
                SummonedCreatures.Remove(Creature);
            }
        }*/

        var Skeleton = Skeletons[Random.Range(0, Skeletons.Length)];

        var SkeletonMinion = Instantiate(Skeleton, transform.position + transform.forward * 3, Quaternion.identity);
        SkeletonMinion.GetComponentInChildren<NPC_AI>().Summoner = gameObject;

        SkeletonMinion.GetComponentInChildren<NPCStats>().setLevel(GetComponent<NPCStats>().npcLevel);
        SkeletonMinion.GetComponentInChildren<NPCStats>().StatFitToLevel();
        SkeletonMinion.GetComponentInChildren<NPCStats>().ChangeIndividualDrops();

        GetComponent<Animator>().SetTrigger("Attack");

        ParticleGenerator.instance.GenerateGroundParticle(SkeletonMinion.transform.position, "SummonCircle", 10f);

        SkeletonMinion.GetComponentInChildren<Animator>().SetTrigger("Summon");

        SummonedCreatures.Add(SkeletonMinion);
    }

    public IEnumerator Teleport()
    {
        TeleportReady = false;
        animator.SetTrigger("Teleport");
        ParticleGenerator.instance.GenerateGroundParticle(transform.position, "SummonCircle", 10f);

        AudioManager.instance.GenerateAudioAndPlaySFX("teleport1", GetComponent<Collider>().bounds.center);

        yield return new WaitForSeconds(TeleportCoolTime);
        TeleportReady = true;

    }

    public void Teleport_ChangePosition()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * TeleportMaxDist;

        NavMeshHit navHit;

        if (NavMesh.SamplePosition(randomPoint, out navHit, 20, NavMesh.AllAreas))
        {
            transform.position = navHit.position;
            ParticleGenerator.instance.GenerateGroundParticle(transform.position, "SummonCircle", 10f);
        }

        AudioManager.instance.GenerateAudioAndPlaySFX("teleport1", GetComponent<Collider>().bounds.center);
    }

    public IEnumerator Shield()
    {
        animator.SetTrigger("Shield");

        StartCoroutine(GetShield());

        shieldReady = false;
        yield return new WaitForSeconds(shieldCoolTime);
        shieldReady = true;
    }

    public IEnumerator GetShield()
    {
        var iceShieldParticle = Instantiate(PrefabCollect.instance.PurpleShield, GetComponent<Collider>().bounds.center, new Quaternion(0, 0, 0, 0), transform);

        iceShieldParticle.GetComponent<IceShield>().ownerStat = stat;
        stat.shieldStat.DeleteShieldAsName("Shield");

        stat.shieldStat.AddShield("Shield", shieldAmount, shieldAmount, stat);

        stat.OnShieldChange();

        yield return new WaitForSeconds(15f);

        stat.shieldStat.DeleteShieldAsName("Shield");

        stat.OnShieldChange();
    }

    public IEnumerator ExplosionSkull()
    {
        animator.SetTrigger("ExplosionSkull");

        explosionSkullReady = false;
        yield return new WaitForSeconds(shieldCoolTime);
        explosionSkullReady = true;
    }

    public void GenerateExplosionSkull()
    {
        int Angel = 0;

        if (explosionSkullAmount > 1)
            Angel = explosionSkullAngle / (explosionSkullAmount - 1);

        for (int i = 0; i < explosionSkullAmount; i++)
        {
            GameObject projectile = Instantiate(explosionSkullPrefab, rangeAttackFirePos.position, rangeAttackFirePos.localRotation);
            projectile.GetComponent<ProjectileLogic>().Setting(gameObject, Mathf.RoundToInt(stat.minDamage.GetFinalStatValue() * 2),
                Mathf.RoundToInt(stat.maxDamage.GetFinalStatValue() * 2));

            projectile.GetComponent<ProjectileLogic>().Fire(Ai.Target.bounds.center, rangeAttackFirePos.position, gameObject, (Angel * (i)) - (explosionSkullAngle / 2));
            projectile.GetComponent<ProjectileLogic>().Target = Ai.Target;
        }
    }
}
