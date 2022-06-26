using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenKing : MonoBehaviour
{

    public int phase = 0;

    public bool fall = false;

    public GameObject phase2StartEffect;
    public GameObject headFireEffect;

    public Transform headFirePos;

    public GameObject[] motionTrails;

    public bool stunImmunity = true;
    public bool slowdownImmunity = true;


    private NPCStats stat;
    private Animator animator;
    private NPC_AI ai;

    private bool sAttack = false;
    private Vector3 dirToEnemy;
    private Rigidbody rigid;
    private float dashSpeed = 50;

    public GameObject chargingParticle;
    [Space]

    public bool attackReady = false;
    public float targetDist;
    [Space]

    public bool standUpReady = true;
    public float standUpMinDist = 30f;

    [Space]
    public float changeMoveSpeedMultiy = 0.05f;
    public float runMindist = 8f;
    public float runSpeed = 10;
    public float walkSpeed = 5;

    [Space]

    public float meleeAttack01CoolTime = 3f;
    public float meleeAttack01MaxDist = 5f;
    public bool meleeAttack01Ready = true;

    [Space]

    public float rangeAttack01CoolTime = 3f;
    public float rangeAttack01MinDist = 8f;
    public float rangeAttack01MaxDist = 25f;
    public GameObject projectile;
    public Transform firePos;
    public int darkArrowFireAngle = 30;
    public int darkArrowAmount = 1;

    public bool rangeAttack01Ready = true;

    [Space]
    public GameObject[] Skeletons;
    public float maxSummonCount = 3;
    public float summonMinion01CoolTime = 25f;
    public List<GameObject> SummonedCreatures = new List<GameObject>();
    public bool summonMinion01Ready = true;

    [Space]
    public float sAttack01CoolTime = 8;
    public float sAttack01MinDist = 8f;
    public float sAttack01MaxDist = 20f;
    public float sAttack01StopDist = 3f;
    public bool sAttack01Ready = true;
    public bool sAttack01Charging = false;

    [Space]
    public float darkArrowSprayCoolTime = 20f;
    public float darkArrowSprayMinDist = 10f;
    public float darkArrowSprayMaxDist = 25f;
    public bool darkArrowSprayReady = true;
    public bool darkArrowSprayWhile = false;
    public GameObject darkArrowSprayChargingParticle;
    public float darkArrowSprayFireTickRate = 0.15f;

    [Space]

    public float darkSwordCoolTime = 15;
    public float darkSwordMaxDist = 8f;
    public Transform darkSwordExplosionPos;
    public GameObject darkSwordExplosionParticle;
    public float darkSwordRadius = 8f;
    public float darkSwordExplosionDamageMultifly = 2;
    public bool darkSwordReady = true;
    public ParticleSystem darkSwordParticle;
    public Light darkSwordLight;

    [Space]

    public float gravityBallCoolTime = 10;
    public float gravityBallMinDist = 9;
    public float gravityBallMaxDist = 25;
    public GameObject gravityBallPrefab;
    public int gravityBallAngle = 30;
    public int gravityBallAmount = 1;
    public bool gravityBallReady = true;

    [Space]

    public float shieldCoolTime = 30;
    public int shieldAmount = 100;
    public float shieldTime = 20;
    public bool shieldReady = true;

    private void Awake()
    {
        stat = GetComponent<NPCStats>();
        animator = GetComponent<Animator>();
        ai = GetComponent<NPC_AI>();
        rigid = GetComponent<Rigidbody>();

        if (stunImmunity)
            stat.StunImmunity.AddBoolModifier();
        if (slowdownImmunity)
            stat.SlowDownImmunity.AddBoolModifier();

    }

    // Update is called once per frame
    void Update()
    {
        if(ai.Target)
            targetDist = Vector3.Distance(ai.Target.transform.position, transform.position);

        if (ai.Target)
        {
            if (targetDist < sAttack01StopDist && sAttack01Charging)
            {
                Sattack01End();
            }
        }

        if(ai.monsterState == MonsterState.trace)
        {
            if (animator.GetBool("IsRun"))
            {
                if (ai.navMeshAgent.speed < walkSpeed)
                    ai.navMeshAgent.speed = walkSpeed;

                ai.navMeshAgent.speed = Mathf.Lerp(ai.navMeshAgent.speed, runSpeed, changeMoveSpeedMultiy * Time.deltaTime);
                animator.SetFloat("WalkSpeed", ai.navMeshAgent.speed);
            }
            else if (animator.GetBool("IsWalk"))
            {
                if (ai.navMeshAgent.speed < walkSpeed)
                    ai.navMeshAgent.speed = walkSpeed;

                ai.navMeshAgent.speed = Mathf.Lerp(ai.navMeshAgent.speed, walkSpeed, changeMoveSpeedMultiy * Time.deltaTime);
                animator.SetFloat("WalkSpeed", ai.navMeshAgent.speed);
            }
        }
        
    }

    public void FireDarkArrow()
    {
        //var angel = Quaternion.FromToRotation(transform.rotation.eulerAngles, Target.transform.position);

        int Angel = darkArrowFireAngle / (darkArrowAmount - 1);

        for (int i = 0; i < darkArrowAmount; i++)
        {
            var arrow = Instantiate(projectile, firePos.position, Quaternion.identity).GetComponent<ProjectileLogic>();
            arrow.Setting(gameObject, Mathf.RoundToInt(stat.minDamage.GetFinalStatValue() / 1.3f), Mathf.RoundToInt(stat.maxDamage.GetFinalStatValue() / 1.3f));
            arrow.Target = GetComponent<NPC_AI>().Target;
            arrow.GetComponent<Collider>().enabled = true;

            if (darkArrowSprayWhile)
                arrow.GetComponent<ProjectileLogic>().guidedMissile = false;

            if (phase == 2)
                arrow.GetComponent<ProjectileLogic>().arrowSpeed += arrow.GetComponent<ProjectileLogic>().arrowSpeed * .3f;

            arrow.Fire(GetComponent<NPC_AI>().Target.bounds.center, firePos.position, gameObject, (Angel * (i)) - (darkArrowFireAngle / 2));
        }

        /*
        GameObject fire = Instantiate(projectile, firePos.position, firePos.localRotation);

        fire.transform.LookAt(GetComponent<NPC_AI>().Target.bounds.center);
        fire.transform.Rotate(new Vector3(90, 0, 0));
        if (darkArrowSprayWhile)
            fire.GetComponentInChildren<Rigidbody>().AddForce(((GetComponent<NPC_AI>().Target.bounds.center + new Vector3(Random.Range(2f, -2f), Random.Range(2f, -2f), Random.Range(2f, -2f))) - firePos.position).normalized * 1500f);
        else
            fire.GetComponentInChildren<Rigidbody>().AddForce(((GetComponent<NPC_AI>().Target.bounds.center) - firePos.position).normalized * 1500f);
        //fire.GetComponent<NPC_ProjectileLogic>().moveForce = 1500f;
        fire.GetComponent<ProjectileLogic>().Setting(gameObject, Mathf.RoundToInt(stat.minDamage.GetFinalStatValue()), Mathf.RoundToInt(stat.maxDamage.GetFinalStatValue()));
        fire.GetComponent<ProjectileLogic>().Target = ai.Target;
        fire.GetComponent<ProjectileLogic>().dirToAim = (firePos.position - ai.Target.transform.position).normalized;
        fire.GetComponent<Collider>().enabled = true;

        if (phase == 2)
            fire.GetComponent<ProjectileLogic>().guidedMissile = true;

        Destroy(fire, 4);*/
    }

    public void FireDarkArrow2()
    {
        GameObject fire = Instantiate(projectile, firePos.position, firePos.localRotation);

        fire.transform.LookAt(GetComponent<NPC_AI>().Target.bounds.center);
        fire.transform.Rotate(new Vector3(90, 0, 0));
        if (darkArrowSprayWhile)
            fire.GetComponentInChildren<Rigidbody>().AddForce(((GetComponent<NPC_AI>().Target.bounds.center + new Vector3(Random.Range(2f, -2f), Random.Range(2f, -2f), Random.Range(2f, -2f))) - firePos.position).normalized * 1500f);
        else
            fire.GetComponentInChildren<Rigidbody>().AddForce(((GetComponent<NPC_AI>().Target.bounds.center) - firePos.position).normalized * 1500f);
        //fire.GetComponent<NPC_ProjectileLogic>().moveForce = 1500f;
        fire.GetComponent<ProjectileLogic>().Setting(gameObject, Mathf.RoundToInt(stat.minDamage.GetFinalStatValue() / 2f), Mathf.RoundToInt(stat.maxDamage.GetFinalStatValue() / 2f));
        fire.GetComponent<ProjectileLogic>().Target = ai.Target;
        fire.GetComponent<ProjectileLogic>().dirToAim = (firePos.position - ai.Target.transform.position).normalized;
        fire.GetComponent<Collider>().enabled = true;

        if (darkArrowSprayWhile)
            fire.GetComponent<ProjectileLogic>().guidedMissile = false;

        if (phase == 2)
            fire.GetComponent<ProjectileLogic>().arrowSpeed += fire.GetComponent<ProjectileLogic>().arrowSpeed * .3f;

        Destroy(fire, 4);
    }

    public void SpawnChargingParticle()
    {
        GameObject particle = Instantiate(chargingParticle, firePos.position, firePos.localRotation,firePos);

        Destroy(particle, 5);
    }

    public void SummonSkeleton()
    {
        /*if (SummonedCreatures.Count != 0)
        {
            foreach (GameObject Creature in SummonedCreatures)
            {
                if (Creature.activeSelf == false)
                {
                    SummonedCreatures.Remove(Creature);
                }
            }
        }*/

        if(phase == 1)
        {
            var Skeleton = Skeletons[Random.Range(0, Skeletons.Length)];

            var SkeletonMinion = Instantiate(Skeleton, transform.position + transform.forward * 5, Quaternion.identity);
            SkeletonMinion.GetComponentInChildren<NPC_AI>().Summoner = gameObject;
            SkeletonMinion.GetComponentInChildren<NPC_AI>().setTarget(ai.Target);

            SkeletonMinion.GetComponentInChildren<NPCStats>().setLevel(5);
            SkeletonMinion.GetComponentInChildren<NPCStats>().StatFitToLevel();
            SkeletonMinion.GetComponentInChildren<NPCStats>().ChangeIndividualDrops();
            GetComponent<Animator>().SetTrigger("Attack");

            ParticleGenerator.instance.GenerateGroundParticle(SkeletonMinion.transform.position, "SummonCircle", 10f);

            SkeletonMinion.GetComponentInChildren<Animator>().SetTrigger("Summon");

            SummonedCreatures.Add(SkeletonMinion);
        }else if(phase == 2)
        {
            var Skeleton1 = Skeletons[Random.Range(0, Skeletons.Length)];
            var Skeleton2 = Skeletons[Random.Range(0, Skeletons.Length)];
            var Skeleton3 = Skeletons[Random.Range(0, Skeletons.Length)];

            var SkeletonMinion = Instantiate(Skeleton1, transform.position + transform.forward * 5, Quaternion.identity);
            SkeletonMinion.GetComponentInChildren<NPC_AI>().Summoner = gameObject;
            SkeletonMinion.GetComponentInChildren<NPC_AI>().setTarget(ai.Target);

            SkeletonMinion.GetComponentInChildren<NPCStats>().setLevel(5);
            SkeletonMinion.GetComponentInChildren<NPCStats>().StatFitToLevel();
            SkeletonMinion.GetComponentInChildren<NPCStats>().ChangeIndividualDrops();
            GetComponent<Animator>().SetTrigger("Attack");

            ParticleGenerator.instance.GenerateGroundParticle(SkeletonMinion.transform.position, "SummonCircle", 10f);

            SkeletonMinion.GetComponentInChildren<Animator>().SetTrigger("Summon");

            SummonedCreatures.Add(SkeletonMinion);

            var SkeletonMinion2 = Instantiate(Skeleton2, transform.position + transform.forward * 5 + transform.right * 5, Quaternion.identity);
            SkeletonMinion2.GetComponentInChildren<NPC_AI>().Summoner = gameObject;
            SkeletonMinion2.GetComponentInChildren<NPC_AI>().setTarget(ai.Target);

            SkeletonMinion2.GetComponentInChildren<NPCStats>().setLevel(5);
            SkeletonMinion2.GetComponentInChildren<NPCStats>().StatFitToLevel();
            SkeletonMinion2.GetComponentInChildren<NPCStats>().ChangeIndividualDrops();
            ParticleGenerator.instance.GenerateGroundParticle(SkeletonMinion2.transform.position, "SummonCircle", 10f);

            SkeletonMinion2.GetComponentInChildren<Animator>().SetTrigger("Summon");

            SummonedCreatures.Add(SkeletonMinion2);

            var SkeletonMinion3 = Instantiate(Skeleton3, transform.position + transform.forward * 5 + transform.right * -5, Quaternion.identity);
            SkeletonMinion3.GetComponentInChildren<NPC_AI>().Summoner = gameObject;
            SkeletonMinion3.GetComponentInChildren<NPC_AI>().setTarget(ai.Target);

            SkeletonMinion3.GetComponentInChildren<NPCStats>().setLevel(5);
            SkeletonMinion3.GetComponentInChildren<NPCStats>().StatFitToLevel();
            SkeletonMinion3.GetComponentInChildren<NPCStats>().ChangeIndividualDrops();
            ParticleGenerator.instance.GenerateGroundParticle(SkeletonMinion3.transform.position, "SummonCircle", 10f);

            SkeletonMinion3.GetComponentInChildren<Animator>().SetTrigger("Summon");

            SummonedCreatures.Add(SkeletonMinion3);
        }
    }

    public void phaseOneEnd()
    {
        attackReady = false;
        phase = 2;
        animator.SetTrigger("Fall");
        animator.SetBool("IsRun", false);
        animator.SetBool("IsWalk", false);
    }

    public void  phaseTwoStart()
    {
        stat.isInvincibility = false;
        stat.maxHealth.AddIntModifier(500);
        stat.fullHealth();
        animator.SetFloat("Speed", 1.35f);
        ai.moveSpeed.AddPercentModifier(.35f);
        darkArrowAmount = 5;
    }

    public void phaseTwoStartEffect()
    {
        Instantiate(phase2StartEffect, transform.position + transform.up * 2, Quaternion.identity);
        var headfire = Instantiate(headFireEffect, new Vector3(0,0,0), Quaternion.Euler(0,-90,0), headFirePos);
        headfire.transform.localPosition = new Vector3(0, 0, 0);
        headfire.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void Sattack01()
    {
        //ai.traceSpeed = 25f;
        sAttack01Charging = true;
        ai.moveSpeed.AddPercentModifier(25);
        animator.SetFloat("Speed", 0.07f);
        foreach(GameObject trail in motionTrails)
        {
            trail.SetActive(true);
        }
        //sAttack = true;
        ai.meleeAttackMoveSpeed = 25f;
        ai.navMeshAgent.speed = ai.meleeAttackMoveSpeed;
    }

    public void Sattack01End()
    {
        //sAttack = false;
        //ai.navMeshAgent.speed = ai.meleeAttackMoveSpeed;
        //ai.traceSpeed = 7.5f;
        if(sAttack01Charging)
        {
            ai.moveSpeed.RemoveIntModifier(25);
            animator.SetFloat("Speed", 1.35f);
            ai.meleeAttackMoveSpeed = 1f;
            ai.navMeshAgent.speed = ai.meleeAttackMoveSpeed;
        }
        
        //ai.navMeshAgent.speed = 1;
    }

    public void motionTrailsOff()
    {
        foreach (GameObject trail in motionTrails)
        {
            trail.SetActive(false);
        }
    }


    public IEnumerator StandUp()
    {
        standUpReady = false;
        stat.isInvincibility = false;
        animator.SetTrigger("StandUp");
        //AudioManager.instance.PlayRandomBossFightBGM();
        AudioManager.instance.FindBGM("finalBossFight", false);

        yield return null;
    }

    public IEnumerator MeleeAttack01Start()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                animator.SetTrigger("meleeAttack01");
                break;
            case 1:
                animator.SetTrigger("meleeAttack02");
                break;
            case 2:
                animator.SetTrigger("meleeAttack03");
                break;
            case 3:
                animator.SetTrigger("meleeAttack04");

                break;
        }

        meleeAttack01Ready = false;
        yield return new WaitForSeconds(meleeAttack01CoolTime);
        meleeAttack01Ready = true;
    }

    public IEnumerator RangeAttack01Start()
    {
        animator.SetTrigger("RangeAttack01");

        rangeAttack01Ready = false;
        yield return new WaitForSeconds(rangeAttack01CoolTime);
        rangeAttack01Ready = true;
    }

    public IEnumerator SummonMinion01()
    {
        animator.SetTrigger("SummonSkeleton");

        summonMinion01Ready = false;
        yield return new WaitForSeconds(summonMinion01CoolTime);
        summonMinion01Ready = true;
    }

    public void ClearDeadMinion()
    {
        for (int i = 0; i < SummonedCreatures.Count; i++)
        {
            if (SummonedCreatures[i].GetComponentInChildren<NPCStats>() != null)
            {
                if(SummonedCreatures[i].GetComponentInChildren<NPCStats>().isDead)
                    SummonedCreatures.Remove(SummonedCreatures[i]);
            }
        }
    }


    public IEnumerator SattckStart()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                animator.SetTrigger("sMeleeAttack01");
                break;
            case 1:
                animator.SetTrigger("sMeleeAttack02");
                break;
            case 2:
                animator.SetTrigger("sMeleeAttack03");
                break;
            case 3:
                animator.SetTrigger("sMeleeAttack04");

                break;
        }

        sAttack01Ready = false;
        yield return new WaitForSeconds(sAttack01CoolTime);
        sAttack01Ready = true;
    }


    public IEnumerator DarkArrowSpray()
    {
        animator.SetTrigger("darkArrowSpray");
        animator.SetBool("darkArrowSprayWhile", true);

        darkArrowSprayReady = false;

        darkArrowSprayWhile = true;

        yield return new WaitForSeconds(5f);

        animator.SetBool("darkArrowSprayWhile", false);

        darkArrowSprayWhile = false;

        yield return new WaitForSeconds(darkArrowSprayCoolTime);
        darkArrowSprayReady = true;
    }

    public void DarkArrowSprayStart()
    {
        StartCoroutine(DrakArrowSprayWhile());
    }

    public IEnumerator DrakArrowSprayWhile()
    {
        while(darkArrowSprayWhile)
        {
            FireDarkArrow2();
            yield return new WaitForSeconds(darkArrowSprayFireTickRate);
        }
    }

    public void SpawndarkArrowSprayChargingParticle()
    {
        GameObject particle = Instantiate(darkArrowSprayChargingParticle, firePos.position, firePos.localRotation, firePos);

        Destroy(particle, 5);
    }


    public IEnumerator DarkSword()
    {
        darkSwordReady = false;
        animator.SetTrigger("darkSword");

        yield return new WaitForSeconds(darkSwordCoolTime);

        darkSwordReady = true;
    }

    public void DarkSwordParticlePlay()
    {
        darkSwordParticle.Play();
    }

    public void DarkSwordParticleStop()
    {
        darkSwordParticle.Stop();
    }



    public IEnumerator GravityBall()
    {
        animator.SetTrigger("GravityBall");

        gravityBallReady = false;
        yield return new WaitForSeconds(gravityBallCoolTime);
        gravityBallReady = true;
    }

    public void GenerateGravityBall()
    {
        int Angel = 0;

        if (gravityBallAmount > 1)
            Angel = gravityBallAngle / (gravityBallAmount - 1);


        for (int i = 0; i < gravityBallAmount; i++)
        {
            var gravityBall = Instantiate(gravityBallPrefab, firePos.position, Quaternion.identity);

            if (phase == 2)
                gravityBall.GetComponent<GravityArea>().moveSpeed += gravityBall.GetComponent<GravityArea>().moveSpeed * .25f;

            var gravityArea = gravityBall.GetComponentInChildren<GravityArea>();
            gravityArea.tickDamage = Mathf.RoundToInt(stat.minDamage.GetFinalStatValue() * .33f);
            gravityArea.owner = gameObject;
            gravityArea.explodeDamage = Mathf.RoundToInt(stat.minDamage.GetFinalStatValue() * 2);
            gravityArea.Target = GetComponent<NPC_AI>().Target;
        }
    }



    public void darkSwordExplosion()
    {
        AttackEffectFunctions.explode(darkSwordRadius, Mathf.RoundToInt(stat.minDamage.GetFinalStatValue() * darkSwordExplosionDamageMultifly)
            , darkSwordExplosionPos.position, darkSwordExplosionParticle, NPC_Type.enemy, gameObject);

        AudioManager.instance.GenerateAudioAndPlaySFX("explode1", darkSwordExplosionPos.position);
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
        var iceShieldParticle = Instantiate(PrefabCollect.instance.PurpleShield2, GetComponent<Collider>().bounds.center, new Quaternion(0, 0, 0, 0), transform);

        iceShieldParticle.GetComponent<IceShield>().ownerStat = stat;
        stat.shieldStat.DeleteShieldAsName("Shield");

        stat.shieldStat.AddShield("Shield", shieldAmount, shieldAmount, stat);

        stat.OnShieldChange();

        yield return new WaitForSeconds(shieldTime);

        stat.shieldStat.DeleteShieldAsName("Shield");

        stat.OnShieldChange();
    }
}
