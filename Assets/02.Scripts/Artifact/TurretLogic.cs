using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretLogic : MonoBehaviour
{
    public Transform firePos;
    public GameObject projectile;
    public bool fireReady = false;
    public float fireCoolTime = 5;
    public float fireMinDist = 0;
    public float fireMaxDist = 15;
    [Space]
    public int fireAmount = 1;
    public int fireAngle = 0;
    public float damageMultiy = .3f;

    [Space]

    public bool fireAnimation = false;
    public Animator animator;

    [Space]

    public float SearchRange = 15;
    public LayerMask TargetLayer;
    public GameObject Target;
    public OwnerType ownerType = 0;
    public GameObject owner;
    public LayerMask m_ObstacleMask = 0;

    private FollowLogic followLogic;
    private PlayerStats playerStat;

    public enum OwnerType { Player, Enemy }

    private void Start()
    {
        if (fireAnimation)
            animator = GetComponent<Animator>();

        followLogic = GetComponent<FollowLogic>();

        if (owner.GetComponent<PlayerStats>() != null)
            playerStat = owner.GetComponent<PlayerStats>();

            InvokeRepeating("SearchEnemy", fireCoolTime, fireCoolTime);
    }

    public IEnumerator FireWebBall()
    {
        if (!fireAnimation)
        {
            fireProjectile();
        }
        else
        {
            GetComponent<Animator>().SetTrigger("FireProjectile");
        }

        fireReady = false;

        yield return new WaitForSeconds(fireCoolTime);

        fireReady = true;
    }

    public void fireProjectile()
    {
        if (Target == null)
            return;

        if(playerStat != null)
        {
            if (playerStat.isDead)
                return;
        }    

        int Angel = 0;

        if (fireAmount > 1)
            Angel = fireAngle / (fireAmount - 1);

        for (int i = 0; i < fireAmount; i++)
        {
            var webBall = Instantiate(projectile, firePos.position, Quaternion.identity).GetComponent<ProjectileLogic>();

            int minDamage = 0, maxDamage = 0;

            if(ownerType == OwnerType.Player)
            {
                minDamage = Mathf.RoundToInt(followLogic.owner.GetComponentInChildren<PlayerStats>().minDamage.GetFinalStatValueAsMultiflyFloat() * damageMultiy);
                maxDamage = Mathf.RoundToInt(followLogic.owner.GetComponentInChildren<PlayerStats>().maxDamage.GetFinalStatValueAsMultiflyFloat() * damageMultiy);
            }
            else if(ownerType == OwnerType.Enemy)
            {
                minDamage = Mathf.RoundToInt(followLogic.owner.GetComponentInChildren<NPCStats>().minDamage.GetFinalStatValueAsMultiflyFloat() * damageMultiy);
                maxDamage = Mathf.RoundToInt(followLogic.owner.GetComponentInChildren<NPCStats>().maxDamage.GetFinalStatValueAsMultiflyFloat() * damageMultiy);
            }

            webBall.Setting(GetComponent<FollowLogic>().owner, minDamage, maxDamage);

            webBall.Fire(Target.GetComponent<Collider>().bounds.center, firePos.position, GetComponent<FollowLogic>().owner, (Angel * (i)) - (fireAngle / 2));
        }
    }

    void SearchEnemy()
    {
        if (owner.GetComponentInChildren<PlayerController01>().SneakGague > 0)
            return;

        Collider[] t_cols = Physics.OverlapSphere(transform.position, SearchRange, TargetLayer);
        GameObject t_shortestTarget = null;
        if (Target != null)
        {
            if (!Target.transform.parent.gameObject.activeSelf)
            {
                Target = null;
            }
        }

        if (t_cols.Length > 0)
        {
            float t_shortestDistance = Mathf.Infinity;

            if (ownerType == OwnerType.Player)
            {
                foreach (Collider t_colTarget in t_cols)
                {
                    if (t_colTarget.GetComponentInChildren<NPC_AI>() != null)
                    {
                        if(t_colTarget.GetComponentInChildren<NPC_AI>().npcType == NPC_Type.enemy)
                        {
                            var NPCAI = t_colTarget.GetComponentInChildren<NPC_AI>();
                            float t_distance = Vector3.Distance(firePos.position, t_colTarget.GetComponentInChildren<Collider>().bounds.center);
                            Vector3 dirToTarget = (t_colTarget.bounds.center - firePos.position).normalized;
                            if (!Physics.Raycast(firePos.position, dirToTarget, t_distance, m_ObstacleMask) && NPCAI.npcType == NPC_Type.enemy)
                            {
                                if (t_shortestDistance > t_distance)
                                {
                                    t_shortestDistance = t_distance;
                                    t_shortestTarget = t_colTarget.gameObject;
                                }

                            }
                        }
                    }
                    if(t_shortestTarget != null)
                        Target = t_shortestTarget;
                }
                if (followLogic.owner.GetComponent<PlayerController01>().GetSneakGague() < 1)
                    fireProjectile();
            }
            else if (ownerType == OwnerType.Enemy)
            {
                foreach (Collider t_colTarget in t_cols)
                {
                    if (t_colTarget.GetComponentInChildren<NPC_AI>() != null || t_colTarget.GetComponentInChildren<PlayerController01>() != null)
                    {
                        if (t_colTarget.GetComponentInChildren<NPC_AI>() != null)
                        {
                            var NPCAI = t_colTarget.GetComponentInChildren<NPC_AI>();

                            if (NPCAI.npcType == NPC_Type.friendly || NPCAI.npcType == NPC_Type.Minion)
                            {
                                float t_distance = Vector3.Distance(firePos.position, t_colTarget.GetComponentInChildren<Collider>().bounds.center);
                                Vector3 dirToTarget = (t_colTarget.bounds.center - firePos.position).normalized;
                                if (!Physics.Raycast(firePos.position, dirToTarget, t_distance, m_ObstacleMask))
                                {
                                    if (t_shortestDistance > t_distance)
                                    {
                                        t_shortestDistance = t_distance;
                                        t_shortestTarget = t_colTarget.gameObject;
                                    }
                                }
                            }

                        }
                        else if (t_colTarget.GetComponentInChildren<PlayerController01>() != null)
                        {
                            var NPCAI = t_colTarget.GetComponentInChildren<NPC_AI>();
                            float t_distance = Vector3.Distance(firePos.position, t_colTarget.GetComponentInChildren<Collider>().bounds.center);
                            Vector3 dirToTarget = (t_colTarget.bounds.center - firePos.position).normalized;
                            if (!Physics.Raycast(firePos.position, dirToTarget, t_distance, m_ObstacleMask))
                            {
                                if (t_shortestDistance > t_distance)
                                {
                                    t_shortestDistance = t_distance;
                                    t_shortestTarget = t_colTarget.gameObject;
                                }
                            }
                        }
                    }
                    if (t_shortestTarget != null)
                        Target = t_shortestTarget;
                }
                if (followLogic.owner.GetComponent<PlayerController01>().GetSneakGague() < 1)
                    fireProjectile();
            }
        }
    }
}
