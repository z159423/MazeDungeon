using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyGolem : Golem
{
    public GameObject BoltPrefab;
    public Transform BoltFirePos;
    public float ChargingTime = 2.5f;
    public bool isCharging = false;
    public float currentChargingTime = 0;
    public float energyBoltCoolTime = 8;
    public bool energyBoltReady = true;

    [Space]

    public float sAttack01CoolTime2 = 10;
    public float sAttack01MinDist = 3f;
    public float sAttack01Radius = 5f;
    public Transform sAttack01ExplodePos2;
    public bool sAttack01Ready2 = true;

    [Space]

    public float sAttack02CoolTime = 20;
    public float sAttack02MinDist = 5f;
    public float sAttack02MaxDist = 20f;
    public GameObject straightBoltPrefab;
    public Transform sAttack02FirePos;
    public Transform sAttack02Aim;
    public float aimSpeed = 1f;
    public LineRenderer lineRenderer;
    public Animation lineAnimation;
    public bool isAiming = false;
    public bool sAttack02Ready = true;

    [Space]

    public LayerMask ObstacleMask;
    public NPC_AI AI;
    private NPCStats npcStat;
    private GameObject energyBolt;
    private Animator animator2;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator2 = GetComponent<Animator>();
        AI = GetComponent<NPC_AI>();
        npcStat = GetComponent<NPCStats>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isCharging)
        {
            currentChargingTime += Time.deltaTime;
            energyBolt.transform.localScale = new Vector3(currentChargingTime / 1.5f, currentChargingTime / 1.5f, currentChargingTime / 1.5f);
        }

        if(isAiming)
        {
            Vector3 dir = AttackEffectFunctions.GetDirection(AI.Target.bounds.center, sAttack02Aim.position);
            sAttack02Aim.position += (dir * aimSpeed * Time.deltaTime);

            lineRenderer.SetPosition(0, sAttack02FirePos.position);
            lineRenderer.SetPosition(1, sAttack02Aim.position);
        }
    }

    public void EnergyBolt()
    {
        StartCoroutine(energyBolts());

    }

    public void EnergyBoltFire()
    {
        isCharging = false;
        energyBolt.GetComponent<GolemEnergyBolt>().Fire(AI.Target, BoltFirePos, gameObject);
        currentChargingTime = 0;

    }

    IEnumerator energyBolts()
    {
        isCharging = true;

        energyBolt = Instantiate(BoltPrefab, BoltFirePos.position, Quaternion.identity, BoltFirePos);
        energyBolt.GetComponent<GolemEnergyBolt>().Setting(gameObject, Mathf.RoundToInt(GetComponent<NPCStats>().minDamage.GetFinalStatValue()), Mathf.RoundToInt(GetComponent<NPCStats>().maxDamage.GetFinalStatValue()));

        yield return new WaitForSeconds(ChargingTime);

        EnergyBoltFire();

        yield return new WaitForSeconds(energyBoltCoolTime);
        energyBoltReady = true;
    }

    public IEnumerator sAttack01Start()
    {
        animator2.SetTrigger("sAttack01");

        sAttack01Ready2 = false;
        yield return new WaitForSeconds(sAttack01CoolTime2);
        sAttack01Ready2 = true;
    }

    public void sAttack01()
    {
        RaycastHit raycastHit;
        Physics.Raycast(sAttack01ExplodePos2.position, -transform.up, out raycastHit, 10f, ObstacleMask);

        NPC_Type type = NPC_Type.enemy;

        int damage = Random.Range(Mathf.RoundToInt(stats.minDamage.GetFinalStatValueAsMultiflyFloat()), Mathf.RoundToInt(stats.maxDamage.GetFinalStatValueAsMultiflyFloat()));

        AttackEffectFunctions.explode(sAttack01Radius, damage, raycastHit.point + new Vector3(0, 0.2f, 0), PrefabCollect.instance.ExplodeParticle, type, gameObject);
    }

    public IEnumerator sAttack02Start()
    {
        sAttack02Ready = false;
        animator2.SetTrigger("sAttack02");
        yield return new WaitForSeconds(sAttack02CoolTime);
        sAttack02Ready = true;
    }

    public void StartAiming()
    {
        sAttack02Aim.position = AI.Target.bounds.center;
        lineRenderer.enabled = true;
        isAiming = true;
        lineRenderer.widthMultiplier = 0;
    }

    public void EndAiming()
    {
        lineRenderer.enabled = false;
        isAiming = false;
    }

    public void BoltCharging()
    {
        lineAnimation.Play("EnergyGolemBoltAnimation");
    }

    public void FireStraightBolt()
    {

        lineAnimation.Play("EnergyGolemStopCharging");
        sAttack02Aim.position = AI.Target.bounds.center;

        var bolt = Instantiate(straightBoltPrefab, sAttack02FirePos.position, Quaternion.identity).GetComponent<ProjectileLogic>();
        bolt.Setting(gameObject, Mathf.RoundToInt(npcStat.minDamage.GetFinalStatValueAsMultiflyFloat()), Mathf.RoundToInt(npcStat.maxDamage.GetFinalStatValueAsMultiflyFloat()));

        bolt.Fire(sAttack02Aim.position, sAttack02FirePos.position, gameObject);
    }
}
