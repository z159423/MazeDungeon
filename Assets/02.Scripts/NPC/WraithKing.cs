using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class WraithKing : MonoBehaviour
{

    public float meleeAttackCoolTime = 5f;
    public float meleeAttackMaxDist = 5f;
    public bool meleeAttackReady = true;

    [Space]
    public float RangeAttackMaxDist = 25;
    public float RangeAttackMinDist = 10;
    public float RangeAttackCoolTime = 5f;
    public bool RangeAttackReady = true;
    public Transform RangeAttackFirePos;
    public GameObject Arrow;
    public int ArrowAmount = 3;
    public int ArrowFireAngle = 60;

    [Space]
    public float Sattack01CoolTime = 10f;
    public float Sattack01Distance = 5;
    public bool Sattack01Ready = true;
    public GameObject SkullBomb;
    public float FireForce = 1000;
    public Transform SkullBombSpawnPosition;
    
    [Space]
    public float Sattack02CoolTime = 10f;
    public float Sattack02Distance = 10;
    public bool Sattack02Ready = true;
    public float explodeRange = 5;
    public LayerMask targetMask;
    public GameObject ExplodeParticle;
    public Transform ExplodePosition;
    public int ExplodeDamage = 10;
    public ParticleSystem ChargingFire1;
    public ParticleSystem ChargingFire2;
    [Space]
    public float SummonCoolTime = 10f;
    public GameObject Wraith;
    public bool SummonReady = true;
    [Space]
    public int maxSummonCount = 3;
    public List<GameObject> SummonedCreatures = new List<GameObject>();

    Animator animator;
    NPC_AI ai;
    NavMeshAgent agnet;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ai = GetComponent<NPC_AI>();
        agnet = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(animator.GetBool("IsAttacking") && ai.Target != null)
        {
            agnet.SetDestination(ai.Target.transform.position);
        }
    }

    public IEnumerator MeleeAttack()
    {
        meleeAttackReady = false;
        animator.SetTrigger("meleeAttack");

        yield return new WaitForSeconds(meleeAttackCoolTime);

        meleeAttackReady = true;
    }


    public IEnumerator Summon()
    {
        SummonReady = false;
        animator.SetTrigger("SummonMob");

        yield return new WaitForSeconds(SummonCoolTime);

        SummonReady = true;
    }

    public void SummonWraith()
    {
        ChargingFire2.Stop();

        foreach (GameObject Creature in SummonedCreatures)
        {
            if (Creature.activeSelf == false)
            {
                SummonedCreatures.Remove(Creature);
                break;
            }
        }

        var SkeletonMinion = Instantiate(Wraith, transform.position + transform.forward * 3, Quaternion.identity);
        SkeletonMinion.GetComponentInChildren<NPC_AI>().Summoner = gameObject;

        SkeletonMinion.GetComponentInChildren<NPCStats>().setLevel(GetComponent<NPCStats>().npcLevel);
        SkeletonMinion.GetComponentInChildren<NPCStats>().StatFitToLevel();


        //ParticleGenerator.instance.GenerateGroundParticle(SkeletonMinion.transform.position, "SummonCircle", 10f);

        SummonedCreatures.Add(SkeletonMinion);
    }

    public IEnumerator RangeAttack()
    {
        RangeAttackReady = false;
        animator.SetTrigger("RangeAttack");

        yield return new WaitForSeconds(RangeAttackCoolTime);

        RangeAttackReady = true;
    }

    public void ChargingLeftStart()
    {
        ChargingFire1.Play();
    }

    public void ChargingRightStart()
    {
        ChargingFire2.Play();
    }

    public void RangeAttackFire()
    {
        ChargingFire1.Stop();
        ChargingFire2.Stop();

        int Angel = ArrowFireAngle / (ArrowAmount - 1);

        for (int i = 0; i < ArrowAmount; i++)
        {
            var arrow = Instantiate(Arrow, RangeAttackFirePos.position, Quaternion.identity).GetComponent<ProjectileLogic>();
            arrow.Setting(gameObject, Mathf.RoundToInt(GetComponent<NPCStats>().minDamage.GetFinalStatValue() * 0.7f), Mathf.RoundToInt(GetComponent<NPCStats>().maxDamage.GetFinalStatValue() * 0.7f));
            arrow.Target = GetComponent<NPC_AI>().Target;

            arrow.Fire(GetComponent<NPC_AI>().Target.bounds.center, RangeAttackFirePos.position, gameObject, (Angel * (i)) - (ArrowFireAngle / 2));
        }
    }

    public IEnumerator Sattack01()
    {
        Sattack01Ready = false;
        animator.SetTrigger("Sattack01");

        yield return new WaitForSeconds(Sattack01CoolTime);

        Sattack01Ready = true;
    }

    public void ChargingFireStart()
    {
        ChargingFire1.Play();
        ChargingFire2.Play();
        //ChargingFire.gameObject.SetActive(true);
    }

    public void ChargingFireStop()
    {
        ChargingFire1.Stop();
        ChargingFire2.Stop();
        //ChargingFire.gameObject.SetActive(true);
    }

    public void Explode()                //범위공격
    {
        ChargingFire1.Stop();
        ChargingFire2.Stop();

        Collider[] colliders = Physics.OverlapSphere(ExplodePosition.position, explodeRange, targetMask);

        var effect = Instantiate(ExplodeParticle, ExplodePosition.position, ExplodeParticle.transform.rotation);

        Destroy(effect, 3.5f);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerStats>().TakeDamage((int)Mathf.Round(ExplodeDamage)
                    , ExplodeDamage, gameObject, true, true, false);
                EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, .1f, 1);

                var dir = AttackEffectFunctions.GetDirection(collider.bounds.center, ExplodePosition.position);

                collider.GetComponentInChildren<Rigidbody>().AddForce(dir * 700, ForceMode.Acceleration);
            }

            if(collider.GetComponent<NPC_AI>())
            {
                if (collider.GetComponent<NPC_AI>().npcType == NPC_Type.Minion)
                {
                    collider.GetComponent<NPCStats>().TakeDamage((int)Mathf.Round(ExplodeDamage)
                        , ExplodeDamage, gameObject, true, true, false);
                }
            }
            
        }
    }

    public IEnumerator Sattack02()
    {
        Sattack02Ready = false;
        animator.SetTrigger("Sattack02");

        yield return new WaitForSeconds(Sattack02CoolTime);

        Sattack02Ready = true;
    }

    public void SkullBombFire()
    {
        Vector3 randomVector3 = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f));
        float randomForce = Random.Range(1f, .5f);

        var skullBomb = Instantiate(SkullBomb, SkullBombSpawnPosition.position, SkullBombSpawnPosition.rotation);

        skullBomb.GetComponent<PurpleBomb>().Owner = gameObject;
        skullBomb.GetComponent<Rigidbody>().AddForce(randomVector3 * FireForce * randomForce);
    }
}
