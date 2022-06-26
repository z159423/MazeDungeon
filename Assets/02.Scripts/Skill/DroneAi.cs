using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAi : MonoBehaviour
{
    enum OwnerType {Player, Enemy}

    public GameObject Summoner = null;

    private Vector3 FlyingPosition;
    [SerializeField] OwnerType ownerType;
    [SerializeField] GameObject Owner;
    [Space]
    [SerializeField] float MoveSpeed;
    [SerializeField] float RotationSpeed;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 rotationOffset;
    [SerializeField] float FlyingRange;
    [SerializeField] GameObject Target;
    [SerializeField] float SearchRange;
    [SerializeField] LayerMask TargetLayer;
    [SerializeField] LayerMask m_ObstacleMask = 0;
    [SerializeField] Transform FirePos;
    [SerializeField] GameObject DroneBody;
    [SerializeField] float FireLate;
    [SerializeField] int missileDamage = 15;
    [SerializeField] float DestroyTime = 60;
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileSpeed = 1500f;
    float currentFireLate = 0;

    Vector3 currentoffset;

    void Start()
    {
        InvokeRepeating("ChangeOffset", 0 ,2f);
        InvokeRepeating("SearchEnemy", 0, 0.1f);
        currentoffset = offset;
        if(DestroyTime != 0)
        {
            Destroy(gameObject, DestroyTime);
        }
    }

    void LateUpdate()
    {
        if (Summoner != null)
        {
            FlyingPosition = Summoner.transform.position + currentoffset;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, currentoffset, MoveSpeed);

        if(Target != null)
        {
            Quaternion t_lookRotation = Quaternion.LookRotation((Target.GetComponentInChildren<Collider>().bounds.center - DroneBody.transform.position));
            Vector3 t_euler = Quaternion.RotateTowards(DroneBody.transform.rotation,
                                                       t_lookRotation * Quaternion.Euler(rotationOffset),
                                                       RotationSpeed * Time.deltaTime).eulerAngles;

            DroneBody.transform.rotation = Quaternion.Euler(t_euler.x, t_euler.y, t_euler.z);

            Quaternion t_fireRotation = Quaternion.Euler(0, t_lookRotation.eulerAngles.y, 0);
            if (Quaternion.Angle(DroneBody.transform.rotation, t_lookRotation * Quaternion.Euler(rotationOffset)) < 5f)
            {
                currentFireLate -= Time.deltaTime;
                if (currentFireLate <= 0)
                {
                    currentFireLate = FireLate;

                    GameObject Projectile = Instantiate(projectile, FirePos.position, FirePos.localRotation);
                    var TargetCollider = Target.GetComponentInChildren<Collider>();

                    Projectile.transform.LookAt(TargetCollider.bounds.center);
                    Projectile.transform.Rotate(new Vector3(90, 0, 0));
                    Projectile.GetComponentInChildren<Rigidbody>().AddForce(((TargetCollider.bounds.center) - FirePos.position).normalized * projectileSpeed);
                    if(Projectile.GetComponentInChildren<Missile>() != null)
                    {
                        Projectile.GetComponentInChildren<Missile>().Summoner = Summoner;
                    }
                    else if(Projectile.GetComponentInChildren<ProjectileLogic>() != null)
                    {
                        int minDamage = 0, maxDamage = 0;

                        if (ownerType == OwnerType.Player)
                        {
                            minDamage = Mathf.RoundToInt(Owner.GetComponentInChildren<PlayerStats>().minDamage.GetFinalStatValueAsMultiflyFloat());
                            maxDamage = Mathf.RoundToInt(Owner.GetComponentInChildren<PlayerStats>().maxDamage.GetFinalStatValueAsMultiflyFloat());
                        }
                        else if (ownerType == OwnerType.Enemy)
                        {
                            minDamage = Mathf.RoundToInt(Owner.GetComponentInChildren<NPCStats>().minDamage.GetFinalStatValueAsMultiflyFloat());
                            maxDamage = Mathf.RoundToInt(Owner.GetComponentInChildren<NPCStats>().maxDamage.GetFinalStatValueAsMultiflyFloat());
                        }

                        Projectile.GetComponentInChildren<ProjectileLogic>().Setting(Owner, minDamage, maxDamage);
                    }
                    
                    Destroy(Projectile, 4);

                }
            }
        }
        else
        {
            Vector3 t_euler = Quaternion.RotateTowards(DroneBody.transform.rotation,
                                                       Summoner.transform.rotation * Quaternion.Euler(rotationOffset),
                                                       RotationSpeed * Time.deltaTime).eulerAngles;

            DroneBody.transform.rotation = Quaternion.Euler(t_euler.x, t_euler.y, t_euler.z);
        }
               
    }

    private void ChangeOffset()
    {
        currentoffset = offset;
        Vector3 randomVec = new Vector3(Random.Range(-FlyingRange, FlyingRange), 0, Random.Range(-FlyingRange, FlyingRange));
        randomVec += offset;
        currentoffset += randomVec;
    }

    void SearchEnemy()
    {
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

            if(ownerType == OwnerType.Player)
            {
                foreach (Collider t_colTarget in t_cols)
                {
                    if (t_colTarget.GetComponentInChildren<NPC_AI>() != null)
                    {
                        var NPCAI = t_colTarget.GetComponentInChildren<NPC_AI>();
                        float t_distance = Vector3.Distance(FirePos.position, t_colTarget.GetComponentInChildren<Collider>().bounds.center);
                        Vector3 dirToTarget = (t_colTarget.bounds.center - FirePos.position).normalized;
                        if (!Physics.Raycast(FirePos.position, dirToTarget, t_distance, m_ObstacleMask) && NPCAI.npcType == NPC_Type.enemy)
                        {
                            if (t_shortestDistance > t_distance)
                            {
                                t_shortestDistance = t_distance;
                                t_shortestTarget = t_colTarget.gameObject;
                            }

                        }
                    }
                    Target = t_shortestTarget;
                }
            }
            else if(ownerType == OwnerType.Enemy)
            {
                foreach (Collider t_colTarget in t_cols)
                {
                    if (t_colTarget.GetComponentInChildren<NPC_AI>() != null || t_colTarget.GetComponentInChildren<PlayerController01>() != null)
                    {
                        if(t_colTarget.GetComponentInChildren<NPC_AI>() != null)
                        {
                            var NPCAI = t_colTarget.GetComponentInChildren<NPC_AI>();

                            if (NPCAI.npcType == NPC_Type.friendly || NPCAI.npcType == NPC_Type.Minion)
                            {
                                float t_distance = Vector3.Distance(FirePos.position, t_colTarget.GetComponentInChildren<Collider>().bounds.center);
                                Vector3 dirToTarget = (t_colTarget.bounds.center - FirePos.position).normalized;
                                if (!Physics.Raycast(FirePos.position, dirToTarget, t_distance, m_ObstacleMask))
                                {
                                    if (t_shortestDistance > t_distance)
                                    {
                                        t_shortestDistance = t_distance;
                                        t_shortestTarget = t_colTarget.gameObject;
                                    }
                                }
                            }

                        } else if(t_colTarget.GetComponentInChildren<PlayerController01>() != null)
                        {
                            if(t_colTarget.GetComponentInChildren<PlayerController01>().SneakGague < 70)
                            {
                                var NPCAI = t_colTarget.GetComponentInChildren<NPC_AI>();
                                float t_distance = Vector3.Distance(FirePos.position, t_colTarget.GetComponentInChildren<Collider>().bounds.center);
                                Vector3 dirToTarget = (t_colTarget.bounds.center - FirePos.position).normalized;
                                if (!Physics.Raycast(FirePos.position, dirToTarget, t_distance, m_ObstacleMask))
                                {
                                    if (t_shortestDistance > t_distance)
                                    {
                                        t_shortestDistance = t_distance;
                                        t_shortestTarget = t_colTarget.gameObject;
                                    }
                                }
                            }
                        }
                        
                    }
                    Target = t_shortestTarget;
                }
            }
            
        }


    }
}
