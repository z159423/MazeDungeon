using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBowTurret : MonoBehaviour
{
    [SerializeField] public Transform m_tfGunBody = null;
    [SerializeField] float m_range = 0f;
    [SerializeField] LayerMask m_layerMask = 0;
    [SerializeField] LayerMask m_ObstacleMask = 0;
    [SerializeField] float m_spinSpeed = 0;
    [SerializeField] Vector3 offset;
    [SerializeField] float m_fireRate = 1f;
    [SerializeField] public Transform FirePos;
    [SerializeField] int PlusDamage = 5;
    [SerializeField] float DestroyTime = 20;
    float m_currentFireRate;
    Animator animator;
    bool IsSetUp = false;

    public Transform m_tfTarget = null;

    public GameObject Summoner;


    

    void Start()
    {
        animator = GetComponent<Animator>();
        InvokeRepeating("SearchEnemy", 0f, 0.5f);

        DestoryTurret();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(m_tfTarget == null)
        {
            if(IsSetUp == true)
                m_tfGunBody.Rotate(new Vector3(40, 0, 0) * Time.deltaTime);
        }
        else
        {
            Quaternion t_lookRotation = Quaternion.LookRotation((m_tfTarget.GetComponentInChildren<Collider>().bounds.center - m_tfGunBody.position));
            Vector3 t_euler = Quaternion.RotateTowards(m_tfGunBody.rotation,
                                                       t_lookRotation * Quaternion.Euler(offset),
                                                       m_spinSpeed * Time.deltaTime).eulerAngles;

            m_tfGunBody.rotation = Quaternion.Euler(t_euler.x, t_euler.y, t_euler.z);

            //m_tfGunBody.LookAt(t_euler);
            //m_tfGunBody.rotation = m_tfGunBody.rotation * Quaternion.Euler(offset);

            Quaternion t_fireRotation = Quaternion.Euler(0, t_lookRotation.eulerAngles.y, 0);
            if(Quaternion.Angle(m_tfGunBody.rotation, t_lookRotation * Quaternion.Euler(offset)) < 5f)
            {
                m_currentFireRate -= Time.deltaTime;
                if(m_currentFireRate <= 0)
                {
                    m_currentFireRate = m_fireRate;
                    animator.SetTrigger("Fire");
                    GameObject fire = Instantiate(PrefabCollect.instance.arrow2, FirePos.position, FirePos.localRotation);

                    var TargetCollider = m_tfTarget.GetComponentInChildren<Collider>();

                    fire.transform.LookAt(TargetCollider.bounds.center);
                    fire.transform.Rotate(new Vector3(90, 0, 0));
                    fire.GetComponentInChildren<Rigidbody>().AddForce(((TargetCollider.bounds.center) - FirePos.position).normalized * 2000f);
                    fire.GetComponent<ProjectileLogic>().Setting(Summoner, PlusDamage, PlusDamage);
                    Destroy(fire, 4);
                }
            }
        }
    }
    void SearchEnemy()
    {
        Collider[] t_cols = Physics.OverlapSphere(transform.position, m_range, m_layerMask);
        Transform t_shortestTarget = null;
        if (m_tfTarget != null)
        {
            if (!m_tfTarget.parent.gameObject.activeSelf)
            {
                m_tfTarget = null;
            }
        }

        if (t_cols.Length > 0 && IsSetUp == true)
        {
            float t_shortestDistance = Mathf.Infinity;
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
                            t_shortestTarget = t_colTarget.transform;
                        }

                    }
                }
                m_tfTarget = t_shortestTarget;
            }
        }
    }
    private void DestoryTurret()
    {
       Destroy(transform.root.gameObject, DestroyTime);
    }

    public void CompleteSetUp()
    {
        IsSetUp = true;
    }
}
