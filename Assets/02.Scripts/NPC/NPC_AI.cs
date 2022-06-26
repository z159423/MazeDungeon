using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using EZCameraShake;
using UnityEditor;


public class NPC_AI : MonoBehaviour
{
    public MonsterType monsterType;
    public MonsterState monsterState = MonsterState.idle;
    public NPC_Type npcType = NPC_Type.none;
    public BossType bossType = BossType.none;

    Collider[] colls;
    [SerializeField]
    public Collider Target;
    [SerializeField]
    public Collider LookingTarget;

    [HideInInspector]
    public List<Collider> collsList = new List<Collider>();
    Dictionary<GameObject, float> TakeDamage = new Dictionary<GameObject, float>();
    float[] collsFloatList;

    public List<GameObject> TakeDamageTargets = new List<GameObject>();

    GameObject player;
    NPCStats stat;
    Animator animator;
    HeadLook lookat;
    AudioSource audioSource;
    HealthUI healthUI;

    public LocalNavMeshBuilder LocalNav;
    public NavMeshAgent navMeshAgent;

    public GameObject heightDamageTarget;
    public float TargetDist = 0;
    private Vector3 vectorToTarget;
    private Quaternion q;
    Vector3 dirToTarget;
    float dstToTarget;
    bool grounded = true;

    public bool applyRootMotionSpeed = false;

    //public GameObject projectile;
    //public Transform firePos;
    private int mask;

    private Vector3 gizmosPosition;

    public LayerMask TargetMask;
    public LayerMask ObstacleMask;

    public float sensingDist = 100.0f;
    public float traceDist = 10.0f;
    public float meleeAttackDist = 2.0f;
    public float announceDist = 10f;
    //public float rangeAttackDist = 8f;

    public Stat moveSpeed;
    public float moveBaseSpeed = 5;
    public Stat attackAnimationSpeed;
    public Stat moveAnimationSpeed;

    public float wanderSpeed = 5.0f;
    public float followSpeed = 10.0f;
    public float meleeAttackMoveSpeed = 1.5f;
    //public float animationmoveSpeed = 1f;
    //public float rangeAttackSpeed = 1f;
    //public float meleeAttackSpeed = 1f;

    public bool FollowItToTheEnd = true;            //적을 끝까지 추격
    public bool highestDamageFirst = true;          //피해량이 가장 높은 적부터 공격
    public bool canRotationWhileAttack = true;      //공격중에 회전가능
    [Range(0,1.0f)]
    public float RotationSpeed = 0.05f;

    public float jumpAttackCoolTime = 0;            //점프공격 쿨타임
    public float wideAttackArea;                    //광역공격 범위
    public float wideAttackDamageMulti;             //광역공격 데미지 퍼센트  ※기본데미지 * 

    public List<AudioClips> AudioList = new List<AudioClips>();

    public Vector3 contactPoint;

    public GameObject Summoner;
    public float followSummonerDist = 1f;
    private float SummonerDist;

    public float ExplosionRadius = 5f;
    public int DestructDamage = 30;
    private bool Explosioned = false;
    private float SelfExplosionTime = 0f;

    public float specialAttackCoolTime = 10;

    private RangeAttackAi rangeAttackAi;
    private EnergyGolem energyGolem;
    private StatueKnight statueKnightScript;
    private Lich lichScript;
    private SpiderQueen spiderQueen;
    private FallenKing fallenKingScript;
    private WraithKing wraithKing;
    private GiantSkeletonAI giantSkeletonAI;
    private Guros gurosAI;
    private Skulder skulder;
    private GhostSkull ghostSkull;
    private SkeletonGuardian skeletonGuardian;
    private Golem golem;

    public bool miniBoss = false;

    public float repeatTime = 0.5f;

    public string currentState = "";

    [SerializeField] public float wanderLeastDst = 0.5f;


    public bool unSyncPosition = false;
    private Vector3 dir;
    public Vector3[] corners;
    public Vector3 UnSyncWanderDestination;
    public float unSyncTraceSpeed;
    public float unSyncRotationSpeed = 0.1f;
    public float unSyncWanderSpeed;
    public float unSyncStopDst = 0.5f;
    public float unSyncCurrentMoveSpeed;
    public float unSynceAcceleration;

    private Rigidbody rigidbody;
    private Spider spider;


    [System.Serializable]
    public class AudioClips
    {
        public string name;
        public AudioClip audioClip;
    }

    public List<Colliders> thisColliders = new List<Colliders>();

    [System.Serializable]
    public class Colliders
    {
        public string name;
        public Collider Collider;
    }

    #region Wander

    public bool canWander = false;
    private bool isWandering = false;
    public float wanderDistanse = 10f;
    public int WanderMinWaitTime = 2;
    public int WanderMaxWaitTime = 5;
    private Vector3 patrolVec;
    private float wanderPointDst;
    private GameObject wanderPoint;

    #endregion

    #region Patrol
    public bool canPatrol = false;
    private bool isPatrol = false;
    public int PatrolMinWaitTime = 15;
    public int PatrolMaxWaitTime = 30;

    #endregion

    #region Follow
    public bool canFollow = false;
    public GameObject followTarget = null;
    private bool isFollowing = false;
    public int FollowMinDist = 7;
    private float targetDist;

    #endregion

    //private bool isDie = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //wanderPoint = new GameObject("wanderPoint");
        lookat = GetComponent<HeadLook>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        stat = GetComponent<NPCStats>();
        LocalNav = GetComponent<LocalNavMeshBuilder>();
        audioSource = GetComponent<AudioSource>();
        //StartCoroutine(this.CheckMonsterState());
        //StartCoroutine(this.MonsterAction());
        healthUI = GetComponent<HealthUI>();
        rigidbody = GetComponent<Rigidbody>();

        if(monsterType == MonsterType.Range)
        {
            rangeAttackAi = GetComponentInChildren<RangeAttackAi>();
        }

        if (bossType == BossType.Golem)
        {
            golem = GetComponentInChildren<Golem>();
        }

        if (bossType == BossType.EnergyGolem)
        {
            energyGolem = GetComponentInChildren<EnergyGolem>();
        }

        if (bossType == BossType.StatueKnight)
        {
            statueKnightScript = GetComponentInChildren<StatueKnight>();
        }

        if (bossType == BossType.Lich)
        {
            lichScript = GetComponentInChildren<Lich>();
        }

        if (bossType == BossType.FallenKing)
        {
            fallenKingScript = GetComponentInChildren<FallenKing>();
        }

        if (bossType == BossType.WraithKing)
        {
            wraithKing = GetComponentInChildren<WraithKing>();
        }

        if (bossType == BossType.GiantSkeleton)
        {
            giantSkeletonAI = GetComponentInChildren<GiantSkeletonAI>();
        }

        if (monsterType == MonsterType.Spider || bossType == BossType.SpiderQueen)
        {
            spider = GetComponentInChildren<Spider>();
        }

        if (bossType == BossType.Guros)
        {
            gurosAI = GetComponentInChildren<Guros>();
        }

        if (bossType == BossType.SpiderQueen)
        {
            spiderQueen = GetComponentInChildren<SpiderQueen>();
        }

        if (bossType == BossType.Skulder)
        {
            skulder = GetComponentInChildren<Skulder>();
        }

        if (bossType == BossType.SkeletonGuardian)
        {
            skeletonGuardian = GetComponentInChildren<SkeletonGuardian>();
        }

        if (monsterType == MonsterType.GhostSkull)
        {
            ghostSkull = GetComponentInChildren<GhostSkull>();
        }

        mask = (1 << 9) + (1 << 10);
        mask = ~mask;

        //StartCoroutine(RepeatAIRoutine());

        if(unSyncPosition)
        {
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
            navMeshAgent.updateUpAxis = false;
            navMeshAgent.isStopped = false;

            navMeshAgent.stoppingDistance = unSyncStopDst;
        }

        moveSpeed.SetBaseValue(moveBaseSpeed);
        attackAnimationSpeed.SetBaseValue(1);
        moveAnimationSpeed.SetBaseValue(1);

        if(monsterType == MonsterType.Boss && miniBoss)
        {
            animator.SetTrigger("Summon");
        }

        StartCoroutine(CheckNearObject());
    }

    private void OnDisable()
    {
        isWandering = false;
    }


    void Update()
    {
        SelfExplosionTime += Time.deltaTime;

        CheckGround();

        /*if(!grounded || stat.isKnockBack)
        {
            navMeshAgent.enabled = false;
        }
        else
        {
            navMeshAgent.enabled = true;
        }*/

        /*if(animator.applyRootMotion == true)
            navMeshAgent.speed = (animator.deltaPosition / Time.deltaTime).magnitude * 1.5f;*/

        if (Target != null)
        {
            if(vectorToTarget != Vector3.zero)
                q = Quaternion.LookRotation(vectorToTarget);

            if (Target.gameObject.activeInHierarchy == false)
            {
                Target = null;
            }
        }

        if ((monsterState == MonsterState.meleeAttack || monsterState == MonsterState.closeAttack || monsterState == MonsterState.rangeAttack) && canRotationWhileAttack)
        {
            //공격중에 부드럽게 회전
            SmoothLookatTarget();
        }

        /*else if (applyRootMotionSpeed && (monsterState == MonsterState.trace || monsterState == MonsterState.wander))
        {
            q = Quaternion.LookRotation(vec);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, RotationSpeed);     //Trace 중에 npc가 타겟을 바라보게 하기
        }*/
    }

    private void FixedUpdate()
    {
        if (unSyncPosition)
        {
            if (Target && (monsterState != MonsterState.wander))
                navMeshAgent.SetDestination(Target.transform.position);

            navMeshAgent.velocity = Vector3.zero;

            corners = navMeshAgent.path.corners;

            if (corners.Length > 1 && monsterState == MonsterState.trace)
            {
                dir = corners[1] - transform.position;

                dir.y = 0;

                unSyncCurrentMoveSpeed = Mathf.Lerp(unSyncCurrentMoveSpeed, unSyncTraceSpeed, Time.deltaTime * unSynceAcceleration);

                //transform.position = transform.position + (dir.normalized * unSynceCurrentMoveSpeed * Time.deltaTime);

                q = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, unSyncRotationSpeed);

            }
            else if (monsterState == MonsterState.wander)
            {
                if (Vector3.Distance(transform.position, UnSyncWanderDestination) > wanderLeastDst)
                {
                    dir = UnSyncWanderDestination - transform.position;

                    dir.y = 0;

                    unSyncCurrentMoveSpeed = Mathf.Lerp(unSyncCurrentMoveSpeed, unSyncWanderSpeed, Time.deltaTime * unSynceAcceleration);

                    //transform.position = transform.position + (dir.normalized * unSynceCurrentMoveSpeed * Time.deltaTime);

                    q = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, q, unSyncRotationSpeed);
                }
                else
                {
                    unSyncCurrentMoveSpeed = Mathf.Lerp(unSyncCurrentMoveSpeed, 0, Time.deltaTime * unSynceAcceleration);
                    unSyncCurrentMoveSpeed = Mathf.Clamp(unSyncCurrentMoveSpeed, 0, float.PositiveInfinity);
                }
            }
            else
            {
                unSyncCurrentMoveSpeed = Mathf.Lerp(unSyncCurrentMoveSpeed, 0, Time.deltaTime * unSynceAcceleration);
                unSyncCurrentMoveSpeed = Mathf.Clamp(unSyncCurrentMoveSpeed, 0, float.PositiveInfinity);
            }

            if (unSyncCurrentMoveSpeed > 0.05f)
            {
                //rigidbody.velocity = (dir.normalized * unSyncCurrentMoveSpeed) / 2.5f;
                transform.position = transform.position + (dir.normalized * unSyncCurrentMoveSpeed * Time.deltaTime);
            }
                

            Vector3 agentDir = transform.position - navMeshAgent.nextPosition;

            float agentDst = Vector3.Distance(navMeshAgent.nextPosition, transform.position);

            navMeshAgent.Move(agentDir.normalized * Time.deltaTime * agentDst * 20);
        }
    }

    IEnumerator RepeatAIRoutine()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.cyan);
        //print("나의 각도 : " + Math.GetAngle(transform.position + transform.forward, transform.position));


            //if (stat.isStun.GetBoolValue())
            //{
            /*monsterState = MonsterState.idle;
            MonsterAction(null);

            StopCoroutine(RepeatAIRoutine());
            float num2 = Random.Range(-(repeatTime / 5), repeatTime / 5);
            yield return new WaitForSeconds(repeatTime + num2);
            StartCoroutine(RepeatAIRoutine());*/

            //}

        if (this.npcType == NPC_Type.enemy)
        {
            CheckMonsterState_Enemy();
            if (Target != null)
            {
                MonsterAction(Target.gameObject);
            }
        }
        else if (this.npcType == NPC_Type.friendly)
        {
            CheckMonsterState_Friendly();
            if (Target != null)
            {
                MonsterAction(Target.gameObject);
            }
        }
        else if(this.npcType == NPC_Type.neutrality)
        {
            CheckMonsterState_Neutrality();

            if (Target != null)
            {
                MonsterAction(Target.gameObject);
            }
        }
        else if (this.npcType == NPC_Type.Minion)
        {
            CheckMonsterState_Minion();
            if (Target != null)
            {
                MonsterAction(Target.gameObject);
            }
        }

        
        if (bossType == BossType.FallenKing)
        {
            if (Target != null)
            {
                FallenKingAction(Target.gameObject);
            }
        }
        else if(Target == null)
        {
            MonsterAction(null);
        }

        if (highestDamageFirst == true)
        {
            if (heightDamageTarget != null)
            {
                /*if (heightDamageTarget.activeInHierarchy == false)
                {
                    print(heightDamageTarget + " 딕셔너리에서 삭제");
                    TakeDamage.Remove(heightDamageTarget);
                }*/

                Target = heightDamageTarget.GetComponentInChildren<Collider>();
            }
        }

        if (heightDamageTarget != null)
        {
            GameObject DeleteKey = null;
            foreach (var key in TakeDamage.Keys.ToList())
            {
                if (key.activeInHierarchy == false)
                {
                    //TakeDamage.Clear();

                    DeleteKey = key;
                }
            }

            if (DeleteKey != null)
                TakeDamage.Remove(DeleteKey);
        }

        TakeDamageTargets.Clear();

        foreach (GameObject key in TakeDamage.Keys)
        {
            TakeDamageTargets.Add(key);
        }

        SortDic();

        if (LocalNav != null)
        {
            if (monsterState == MonsterState.wander)
            {
                LocalNav.m_Size = new Vector3(wanderDistanse * 2, 50f, wanderDistanse * 2);
            }
            else
            {
                LocalNav.m_Size = new Vector3(TargetDist * 2, 50f, TargetDist * 2);
            }
        }

        //animator.SetFloat("MoveSpeed", animationmoveSpeed);

        if (stat.isRestraint.GetBoolValue())
        {
            navMeshAgent.speed = 0;
        }

        if (monsterState == MonsterState.wander && Vector3.Distance(transform.position, patrolVec) < 0.5f)
        {
            animator.SetBool("IsTrace", false);
        }

        if(unSyncPosition)
        {
            //navMeshAgent.Warp(transform.position);
        }
            
        float num = Random.Range(-(repeatTime / 2), repeatTime / 2);

        yield return null;
        //yield return new WaitForSeconds(repeatTime + num);
        //StartCoroutine(RepeatAIRoutine());
    }

    IEnumerator CheckNearObject()
    {
        WaitForSeconds delay = new WaitForSeconds(0.3f + Random.Range(-0.05f,0.05f));

        if ((monsterState == MonsterState.idle || monsterState == MonsterState.wander))
        {
            while (gameObject.activeSelf)
            {
                colls = Physics.OverlapSphere(this.transform.position, sensingDist, TargetMask);

                collsList.Clear();

                for (int i = 0; i < colls.Length; i++)
                {
                    Transform TargetTransform = colls[i].transform;
                    dirToTarget = ((TargetTransform.position + Vector3.up) - (GetComponent<Collider>().bounds.center)).normalized;
                    dstToTarget = Vector3.Distance(GetComponent<Collider>().bounds.center, TargetTransform.position + Vector3.up);
                    if (!Physics.Raycast(GetComponent<Collider>().bounds.center, dirToTarget, dstToTarget, ObstacleMask))
                    {
                        collsList.Add(colls[i]);
                    }
                }

                if (Target == null)
                {
                    foreach (Collider collider in collsList)
                    {
                        if (npcType == NPC_Type.enemy)                                              //만약 이 NPC_TYPE가 ENEMY인 경우
                        {
                            if (collider.gameObject.GetComponent<NPC_AI>() != null || collider.transform.tag == "Player")
                            {
                                if (collider.transform.tag == "Player" || collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.friendly || collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.Minion)
                                {
                                    if (collider.transform.tag == "Player")
                                    {
                                        if (collider.GetComponentInChildren<PlayerController01>().SneakGague < 70)
                                        {
                                            Target = collider;
                                            TargetDist = Vector3.Distance(Target.transform.position, this.transform.position);
                                            //healthUI.enableHP_UI();

                                            if (GetComponent<ShopGuardLeader>())
                                            {
                                                GetComponent<ShopGuardLeader>().GenerateFindThiefTextBubble();
                                            }

                                            break;
                                        }
                                    }
                                    else
                                    {
                                        Target = collider;
                                        TargetDist = Vector3.Distance(Target.transform.position, this.transform.position);
                                        break;
                                    }
                                }
                            }
                        }
                        else if (npcType == NPC_Type.friendly || npcType == NPC_Type.Minion)       //만약 이 NPC_TYPE가 FRIENDLY 또는 MINION인 경우
                        {
                            if (collider.gameObject.GetComponent<NPC_AI>() != null)
                            {
                                if (collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
                                {
                                    Target = collider;
                                    TargetDist = Vector3.Distance(Target.transform.position, this.transform.position);
                                    break;
                                }
                            }
                        }
                        else if (npcType == NPC_Type.neutrality)
                        {
                            if (collider.gameObject.GetComponent<NPC_AI>() != null || collider.transform.tag == "Player")
                            {
                                LookingTarget = collider;
                            }
                        }
                    }
                }
                else
                {
                    TargetDist = Vector3.Distance(Target.transform.position, this.transform.position);

                    //if (Target.transform.tag == "Player")
                        //healthUI.enableHP_UI();
                }
                //collsList = colls.ToList();

                collsFloatList = new float[collsList.Count];

                for (int i = 0; i < collsList.Count; i++)
                {
                    collsFloatList[i] = Vector3.Distance(collsList[i].transform.position, this.transform.position);
                }

                //print(Vector3.Distance(transform.position, wanderVec));
                if (Vector3.Distance(transform.position, patrolVec) < wanderLeastDst)
                {
                    animator.SetBool("IsTrace", false);
                }

                StartCoroutine(RepeatAIRoutine());
                yield return delay;
            }
        }

    }

    

    void CheckMonsterState_Enemy()
    {
        if (Target != null)
        {
            vectorToTarget = Target.transform.position - transform.position;
        }

        if(canFollow && Target == null && followTarget != null)
        {
            FollowTarget();
        }
        else
        {
            NPC_Behavior();
        }
        
    }


    void CheckMonsterState_Friendly()
    {

        if (Target == null)
        {
            foreach (Collider collider in collsList)
            {
                if (collider.gameObject.GetComponent<NPC_AI>() != null)
                {
                    if (collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
                    {
                        Target = collider;
                        TargetDist = Vector3.Distance(Target.transform.position, this.transform.position);
                        //Debug.Log(dist);
                        break;
                    }
                }
                else
                {

                }
            }
        }
        else
        {
            TargetDist = Vector3.Distance(Target.transform.position, this.transform.position);
        }

        if (Target != null)
        {
            vectorToTarget = Target.transform.position - transform.position;
        }

        if (canFollow && Target == null && followTarget != null)
        {
            FollowTarget();
        }
        else
        {
            NPC_Behavior();
        }

    }

    void CheckMonsterState_Neutrality()
    {
        if (Target != null)
        {
            vectorToTarget = Target.transform.position - transform.position;
        }

        if (canFollow && Target == null && followTarget != null)
        {
            FollowTarget();
        }
        else
        {
            NPC_Behavior();
        }
    }

    void CheckMonsterState_Minion()
    {
        if (Target == null)
        {
            foreach (Collider collider in collsList)
            {
                if (collider == null)
                {
                    collsList.Remove(collider);
                    break;
                }
                else if (collider.gameObject.GetComponent<NPC_AI>() != null)
                {
                    if (collider.gameObject.GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
                    {
                        Target = collider;
                        //TargetDist = Vector3.Distance(Target.transform.position, this.transform.position);
                        //Debug.Log(dist);
                        break;
                    }
                }
            }
        }
        else
        {
            //TargetDist = Vector3.Distance(Target.transform.position, this.transform.position);
        }

        if (Target != null)
        {
            vectorToTarget = Target.transform.position - transform.position;
            NPC_Behavior();
        }
        else
        {
            FollowSummoner();
        }


    }

    private void NPC_Behavior()
    {
        if(stat.isStun.GetBoolValue())
        {
            monsterState = MonsterState.idle;

            return;
        }

        if (monsterType == MonsterType.Melee)
        {
            MonsterType_Melee();
        }
        else if (monsterType == MonsterType.Range)
        {
            MonsterType_Range();
        }
        else if (monsterType == MonsterType.Boss)
        {
            MonsterType_Boss();
        }
        else if (monsterType == MonsterType.SelfDestruct)
        {
            MonsterType_SelfDestruct();
        }
        else if(monsterType == MonsterType.Spider)
        {
            MonsterType_Spider();
        }
        else if (monsterType == MonsterType.GhostSkull)
        {
            MonsterType_GhostSkull();
        }


        if (monsterType == MonsterType.NonAttack)
        {
            MonsterType_NonAttack();
        }
        
    }

    void MonsterAction(GameObject Object)
    {
        switch (monsterState)
        {
            case MonsterState.idle:

                //nav.Stop();

                animator.SetBool("IsAttack", false);
                animator.SetBool("IsTrace", false);
                if (monsterType == MonsterType.Range)
                    animator.SetBool("IsRangeAttack", false);
                if (lookat != null)
                    lookat.target = null;

                //if(!applyRootMotionSpeed)
                //    navMeshAgent.speed = 0;
                break;

            case MonsterState.trace:
                if (navMeshAgent.isOnNavMesh == true && Object != null)
                {
                    navMeshAgent.destination = Object.transform.position;
                }
                else if(navMeshAgent.isOnNavMesh == false)
                {
                    Debug.Log(transform.gameObject + "가 NavMesh위에 있지 않습니다.!");
                }
                animator.SetBool("IsAttack", false);
                if (monsterType == MonsterType.Range || bossType == BossType.Lich)
                    animator.SetBool("IsRangeAttack", false);
                animator.SetBool("IsTrace", true);
                if (lookat != null && Target != null)
                    lookat.target = Target.transform;
                //LocalNav.m_Size = new Vector3(TargetDist * 1.5f, 50f, TargetDist * 1.5f);
                //if (!applyRootMotionSpeed)
                //    navMeshAgent.speed = traceSpeed;
                break;

            case MonsterState.meleeAttack:

                // nav.Stop();
                if (navMeshAgent.isOnNavMesh == true && Object != null)
                {
                    navMeshAgent.destination = Object.transform.position;
                }

                animator.SetBool("IsAttack", true);
                animator.SetBool("IsTrace", true);
                if (lookat != null)
                    lookat.target = Target.transform;
                //LocalNav.m_Size = new Vector3(TargetDist * 1.5f, 50f, TargetDist * 1.5f);
                //if (!applyRootMotionSpeed)
                //    navMeshAgent.speed = meleeAttackSpeed;
                break;

            case MonsterState.closeAttack:
                animator.SetBool("IsCloseAttack", true);
                animator.SetBool("IsAttack", false);
                animator.SetBool("IsTrace", false);
                if (lookat != null)
                    lookat.target = Target.transform;
                //if (!applyRootMotionSpeed)
               //     navMeshAgent.speed = 0;
                break;

            case MonsterState.rangeAttack:
                //if (monsterType == MonsterType.Range)
                animator.SetBool("IsRangeAttack", true);
                animator.SetBool("IsTrace", false);

                if (lookat != null)
                    lookat.target = Target.transform;
                //if (!applyRootMotionSpeed)
                //    navMeshAgent.speed = 0;
                break;

            case MonsterState.wander:
                if (isWandering == false)
                {
                    StartCoroutine(Wander());
                }
                if (Target == null)
                {
                    //animator.SetBool("IsTrace", true);

                    animator.SetBool("IsAttack", false);
                    if (monsterType == MonsterType.Range || bossType == BossType.Lich)
                        animator.SetBool("IsRangeAttack", false);
                    //if (!applyRootMotionSpeed)
                    //    navMeshAgent.speed = wanderSpeed;
                }
                break;

            case MonsterState.patrol:
                if (isPatrol == false)
                {
                    StartCoroutine(Patrol());
                }

                if (Target == null)
                {
                    //animator.SetBool("IsTrace", true);

                    animator.SetBool("IsAttack", false);
                    if (monsterType == MonsterType.Range || bossType == BossType.Lich)
                        animator.SetBool("IsRangeAttack", false);
                    //if (!applyRootMotionSpeed)
                    //    navMeshAgent.speed = wanderSpeed;
                }
                break;

            case MonsterState.Follow:
                animator.SetBool("IsTrace", true);
                animator.SetBool("IsAttack", false);
                if (monsterType == MonsterType.Range || bossType == BossType.Lich)
                    animator.SetBool("IsRangeAttack", false);
                TargetDist = 0;
                //if (!applyRootMotionSpeed)
                //    navMeshAgent.speed = followSpeed;
                break;

            case MonsterState.die:

                animator.SetBool("IsDie", true);
                navMeshAgent.isStopped = true;
                lookat.target = null;
                //if (!applyRootMotionSpeed)
                //    navMeshAgent.speed = 0;
                break;

            default:
                //if (!applyRootMotionSpeed)
                //    navMeshAgent.speed = 0;
                break;


                //yield return null;
        }
    }

    void FallenKingAction(GameObject Object)
    {
        switch (monsterState)
        {
            case MonsterState.idle:
                animator.SetBool("IsTrace", false);


                if (navMeshAgent.isOnNavMesh == true && Object != null)
                {
                    navMeshAgent.destination = Object.transform.position;
                }

                if (lookat != null && Target != null)
                    lookat.target = Target.transform;

                animator.SetBool("IsRun", false);
                animator.SetBool("IsWalk", false);
                break;

            case MonsterState.trace:
                if (navMeshAgent.isOnNavMesh == true && Object != null)
                {
                    navMeshAgent.destination = Object.transform.position;
                }
                else if (navMeshAgent.isOnNavMesh == false)
                {
                    Debug.Log(transform.gameObject + "가 NavMesh위에 있지 않습니다.!");
                }

                if (fallenKingScript.runMindist < fallenKingScript.targetDist)
                {
                    animator.SetBool("IsWalk", false);
                    animator.SetBool("IsRun", true);
                }
                else
                {
                    animator.SetBool("IsRun", false);
                    animator.SetBool("IsWalk", true);
                }

                if (lookat != null && Target != null)
                    lookat.target = Target.transform;

                animator.SetBool("IsTrace", true);

                break;

            case MonsterState.meleeAttack:

                if (navMeshAgent.isOnNavMesh == true && Object != null)
                {
                    navMeshAgent.destination = Object.transform.position;
                }

                if (lookat != null && Target != null)
                    lookat.target = Target.transform;

                animator.SetBool("IsTrace", false);
                animator.SetBool("IsRun", false);
                animator.SetBool("IsWalk", false);
                break;

            case MonsterState.rangeAttack:
                animator.SetBool("IsTrace", false);
                animator.SetBool("IsRun", false);
                animator.SetBool("IsWalk", false);
                break;

            case MonsterState.wander:
                if (isWandering == false)
                {
                    StartCoroutine(Wander());
                }
                if (Target == null)
                {
                    //animator.SetBool("IsTrace", true);

                    animator.SetBool("IsAttack", false);
                    if (monsterType == MonsterType.Range || bossType == BossType.Lich)
                        animator.SetBool("IsRangeAttack", false);
                    //if (!applyRootMotionSpeed)
                    //    navMeshAgent.speed = wanderSpeed;
                }
                break;

            case MonsterState.Follow:
                animator.SetBool("IsTrace", true);
                animator.SetBool("IsAttack", false);
                if (monsterType == MonsterType.Range || bossType == BossType.Lich)
                    animator.SetBool("IsRangeAttack", false);

                break;

            case MonsterState.die:

                animator.SetBool("IsDie", true);
                navMeshAgent.isStopped = true;
                lookat.target = null;
                break;

            default:

                break;
        }
    }

    private void MonsterType_Melee()            //근접 타입
    {
        //animator.SetFloat("AttackSpeed", meleeAttackSpeed);
        if (TargetDist <= meleeAttackDist && Target != null)
        {
            monsterState = MonsterState.meleeAttack;
            navMeshAgent.speed = meleeAttackMoveSpeed;
        }
        else if (TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
            navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (heightDamageTarget != null && monsterState == MonsterState.idle)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
            navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.trace && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
            navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (heightDamageTarget != null && highestDamageFirst)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = BaseSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.idle && canFollow == true)
        {
            monsterState = MonsterState.Follow;
            navMeshAgent.speed = followSpeed;
        }
        else if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
            navMeshAgent.speed = wanderSpeed;
        }
        else if (monsterState == MonsterState.idle && canPatrol == true)
        {
            monsterState = MonsterState.patrol;
            navMeshAgent.speed = wanderSpeed;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false && isPatrol == false)
        {
            monsterState = MonsterState.idle;
            navMeshAgent.speed = 0f;
        }
    }

    private void MonsterType_Range()            //원거리 타입
    {
        //animator.SetFloat("RangeAttackSpeed", rangeAttackSpeed);

        if (TargetDist < rangeAttackAi.maxRangeAttack && TargetDist > rangeAttackAi.minRangeAttack && Target != null)
        {
            RaycastHit hit;
            if (Target != null)
            {
                var distToTarget = Vector3.Distance(rangeAttackAi.rangeAttackFirePos.position, Target.bounds.center);

                if (!Physics.Raycast(rangeAttackAi.rangeAttackFirePos.position, (Target.bounds.center - rangeAttackAi.rangeAttackFirePos.position).normalized, out hit, distToTarget, ObstacleMask))
                {
                    monsterState = MonsterState.rangeAttack;
                    navMeshAgent.speed = 0f;

                    Debug.DrawRay(rangeAttackAi.rangeAttackFirePos.position, (Target.bounds.center - rangeAttackAi.rangeAttackFirePos.position));
                    
                    //print("Target의 position : " + Target.transform.position + " 레이케스트 : " + hit.collider);
                }
                else
                {
                    monsterState = MonsterState.trace;
                    navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
                }
                gizmosPosition = hit.point;
            }
            //Debug.DrawRay(firePos.position, Target.transform.position - firePos.position + Vector3.up,Color.red, 3f);
            /*
            if (hit.collider == Target)
            {
                monsterState = MonsterState.rangeAttack;
                nav.speed = 0f;
            } else
            {
                monsterState = MonsterState.trace;
                nav.speed = traceSpeed;
            }*/


        }/*
        else if (TargetDist <= meleeAttackDist && Target != null)
        {
            monsterState = MonsterState.meleeAttack;
            nav.speed = meleeAttackMoveSpeed;
            //this.transform.rotation = Quaternion.LookRotation(look, Vector3.up);
        }*/
        else if(TargetDist < rangeAttackAi.minRangeAttack && Target != null)
        {
            SmoothLookatTarget();
            monsterState = MonsterState.idle;
        }
        else if (TargetDist < traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
            navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if ((monsterState == MonsterState.trace || monsterState == MonsterState.rangeAttack) && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
            navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (heightDamageTarget != null && highestDamageFirst)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = BaseSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.idle && canFollow == true)
        {
            monsterState = MonsterState.Follow;
            navMeshAgent.speed = followSpeed;
        }
        else if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
            navMeshAgent.speed = wanderSpeed;
        }
        else if (monsterState == MonsterState.idle && canPatrol == true)
        {
            monsterState = MonsterState.patrol;
            navMeshAgent.speed = wanderSpeed;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false && isPatrol == false)
        {
            monsterState = MonsterState.idle;
            //navMeshAgent.speed = 0f;
        }
    }

    private void MonsterType_SelfDestruct()
    {
        if (Summoner == null)
        {
            Summoner = GameObject.FindGameObjectWithTag("Player");
        }

        if (6 < SelfExplosionTime && Explosioned == false)
        {
            animator.SetTrigger("Boom");
            Explosioned = true;
            navMeshAgent.speed = 0;
        }

        if (Target == null)
        {
            navMeshAgent.enabled = false;
        }
        else
        {
            navMeshAgent.enabled = true;
        }

        if (Target == null)
        {
            transform.Translate(Vector3.forward * 5 * Time.deltaTime);
            monsterState = MonsterState.trace;
            navMeshAgent.speed = 0;
        }
        else if (TargetDist <= 2 && Target != null && Explosioned == false)
        {
            animator.SetTrigger("Boom");
            Explosioned = true;
            navMeshAgent.speed = 0;
        }
        else if (TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
            navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else
        {
            monsterState = MonsterState.idle;
            navMeshAgent.speed = 0;
        }
    }

    private void MonsterType_NonAttack()            //근접 타입
    {
        if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
            navMeshAgent.speed = wanderSpeed;
        }
    }

    private void MonsterType_Spider()
    {
        if(!spider)
            spider = GetComponent<Spider>();

        //animator.SetFloat("AttackSpeed", meleeAttackSpeed);
        if (TargetDist <= meleeAttackDist && Target != null)
        {
            monsterState = MonsterState.meleeAttack;
            //navMeshAgent.speed = meleeAttackMoveSpeed;
        }
        else if(spider.webBallFireMinDist <= TargetDist && spider.webBallFireMaxDist >= TargetDist && spider.enableWebBallFire && spider.webBallReady && Target)
        {
            RaycastHit hit;

            var distToTarget = Vector3.Distance(spider.webBallFirePos.position, Target.bounds.center);

            if (!Physics.Raycast(spider.webBallFirePos.position, (Target.bounds.center - spider.webBallFirePos.position).normalized, out hit, distToTarget, ObstacleMask))
            {
                StartCoroutine(spider.FireWebBall());

                Debug.DrawRay(spider.webBallFirePos.position, (Target.bounds.center - spider.webBallFirePos.position));
            }
            gizmosPosition = hit.point;
            
        }
        else if (TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = BaseSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (heightDamageTarget != null && highestDamageFirst)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = BaseSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.trace && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = BaseSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
            //navMeshAgent.speed = wanderSpeed;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
        {
            monsterState = MonsterState.idle;
            //navMeshAgent.speed = 0f;
        }
    }

    private void MonsterType_GhostSkull()
    {
        if (Target)
        {
            if (jumpAttackCoolTime > 0)
                jumpAttackCoolTime -= Time.deltaTime;
            dirToTarget = ((Target.transform.position + Vector3.up) - (transform.position + Vector3.up)).normalized;
            dstToTarget = Vector3.Distance(transform.position + Vector3.up, Target.transform.position + Vector3.up);
        }
        //navMeshAgent.speed = animator.GetFloat("MoveSpeed");

        if(TargetDist < ghostSkull.biteMaxDist && ghostSkull.biteReady && Target != null && !ghostSkull.isAttacking)
        {
            StartCoroutine(ghostSkull.Bite());
        }
        else if (TargetDist > ghostSkull.fireBallMinDist && TargetDist < ghostSkull.fireBallMaxDist && ghostSkull.fireBallReady && Target != null && !ghostSkull.isAttacking)
        {
            StartCoroutine(ghostSkull.FireBall());
        }
        else if (TargetDist < ghostSkull.FireBreathMinDist && ghostSkull.FireBreathReady && Target != null && !ghostSkull.isAttacking)
        {
            StartCoroutine(ghostSkull.FireBreath());
        }
        else if (TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = BaseSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (heightDamageTarget != null && monsterState == MonsterState.idle)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = BaseSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.trace && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = BaseSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
            //navMeshAgent.speed = wanderSpeed;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
        {
            monsterState = MonsterState.idle;
            //navMeshAgent.speed = 0f;
        }
    }

    private void MonsterType_Boss()         //보스타입
    {
        switch(bossType)
        {
            case BossType.GiantSkeleton:
                GiantSkeleton();
                break;

            case BossType.Lich:
                Lich();
                break;

            case BossType.Golem:
                Golem_Behavior();
                break;

            case BossType.EnergyGolem:
                EnergyGolem_Behavior();
                break;

            case BossType.SpiderQueen:
                SpiderQueen_Behavior();
                break;

            case BossType.StatueKnight:
                StatueKnight_Behavior();
                break;

            case BossType.FallenKing:
                FallenKing_Behavior();
                break;

            case BossType.WraithKing:
                WraithKing_Behavior();
                break;

            case BossType.Guros:
                Guros();
                break;

            case BossType.Skulder:
                Skulder_Behavior();
                break;

            case BossType.SkeletonGuardian:
                SkeletonGuardian_Behavior();
                break;
        }
    }

    private void GiantSkeleton()
    {
        if (Target != null)
        {
            if (jumpAttackCoolTime > 0)
                jumpAttackCoolTime -= Time.deltaTime;
            dirToTarget = ((Target.transform.position + Vector3.up) - (transform.position + Vector3.up)).normalized;
            dstToTarget = Vector3.Distance(transform.position + Vector3.up, Target.transform.position + Vector3.up);
        }

        //navMeshAgent.speed = animator.GetFloat("MoveSpeed");

        if (TargetDist <= giantSkeletonAI.closeAttackmaxDist && giantSkeletonAI.closeAttackReady && Target != null && !giantSkeletonAI.isAttacking)
        {
            StartCoroutine(giantSkeletonAI.CloseAttack01());
        }
        else if (TargetDist <= giantSkeletonAI.meleeAttackmaxDist && giantSkeletonAI.meleeAttackReady && Target != null && !giantSkeletonAI.isAttacking)
        {
            StartCoroutine(giantSkeletonAI.MeleeAttack());
            //monsterState = MonsterState.meleeAttack;
            //nav.speed = meleeAttackMoveSpeed;
        }
        else if (TargetDist < giantSkeletonAI.JumpAttackMaxDistance && TargetDist > giantSkeletonAI.JumpAttackMinDistance && giantSkeletonAI.JumpAttackReady && Target != null && !giantSkeletonAI.isAttacking)
        {
            StartCoroutine(giantSkeletonAI.JumpAttack());
        }
        else if (TargetDist < giantSkeletonAI.Sattack1Distance && giantSkeletonAI.Sattack1Ready
            && !giantSkeletonAI.isAttacking && Target != null)
        {
            StartCoroutine(giantSkeletonAI.Sattack1());
        }
        else if (TargetDist <= giantSkeletonAI.RangeAttackMaxDistance && TargetDist > giantSkeletonAI.RangeAttackMinDistance && Target != null && giantSkeletonAI.RangeAttackReady && !giantSkeletonAI.isAttacking)
        {
            float distToTarget = 0;
            if (Target != null)
            {
                distToTarget = Vector3.Distance(giantSkeletonAI.RangeAttackFirePos.position, Target.bounds.center);
                TargetDist = distToTarget;
            }

            RaycastHit hit;
            if (Target != null)
            {
                if (!Physics.Raycast(giantSkeletonAI.RangeAttackFirePos.position, (Target.bounds.center - giantSkeletonAI.RangeAttackFirePos.position).normalized, out hit, distToTarget, ObstacleMask))
                {
                    StartCoroutine(giantSkeletonAI.RangeAttack());
                    monsterState = MonsterState.rangeAttack;

                    Debug.DrawRay(giantSkeletonAI.RangeAttackFirePos.position, (Target.bounds.center - giantSkeletonAI.RangeAttackFirePos.position));
                }
                else
                {
                    print("raycast가 걸려서 원거리 공격 불가!");
                    monsterState = MonsterState.rangeAttack;
                }
                gizmosPosition = hit.point;
            }
        }
        else if (TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
            //nav.speed = traceSpeed;
        }
        else if (heightDamageTarget != null)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
            //nav.speed = traceSpeed;
        }
        else if (monsterState == MonsterState.trace && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
            //nav.speed = traceSpeed;
        }
        else if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
            //nav.speed = wanderSpeed;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
        {
            monsterState = MonsterState.idle;
            //nav.speed = 0f;
        }
    }
    private void Lich()
    {
        //animator.SetFloat("RangeAttackSpeed", rangeAttackSpeed);

        float distToTarget = 0;
        if (Target != null)
        {
            distToTarget = Vector3.Distance(lichScript.rangeAttackFirePos.position, Target.bounds.center);
            TargetDist = distToTarget;
            navMeshAgent.destination = Target.transform.position;
        }

        if(Target != null && lichScript.summonReady && !lichScript.isAttacking)
        {
            StartCoroutine(lichScript.summonSkeleton());
        }
        else if(Target != null && lichScript.TeleportReady && !lichScript.isAttacking)
        {
            StartCoroutine(lichScript.Teleport());
        }
        else if(Target != null && lichScript.shieldReady && !lichScript.isAttacking)
        {
            StartCoroutine(lichScript.Shield());
        }
        else if (TargetDist < lichScript.explosionSkullMaxDist && TargetDist > lichScript.explosionSkullMinDist
            && Target != null && lichScript.explosionSkullReady && !lichScript.isAttacking)
        {
            RaycastHit hit;
            if (Target != null)
            {
                if (!Physics.Raycast(lichScript.rangeAttackFirePos.position, (Target.bounds.center - lichScript.rangeAttackFirePos.position).normalized, out hit, distToTarget, ObstacleMask))
                {
                    StartCoroutine(lichScript.ExplosionSkull());
                }
                else
                {
                    monsterState = MonsterState.trace;
                }
                gizmosPosition = hit.point;
            }
        }
        else if (TargetDist < lichScript.rangeAttackMaxDist && Target != null && lichScript.rangeAttackReady && !lichScript.isAttacking)
        {
            RaycastHit hit;
            if (Target != null)
            {
                if (!Physics.Raycast(lichScript.rangeAttackFirePos.position, (Target.bounds.center - lichScript.rangeAttackFirePos.position).normalized, out hit, distToTarget, ObstacleMask))
                {

                    //monsterState = MonsterState.rangeAttack;
                    //navMeshAgent.speed = 0f;
                    //print("currentState : rangeAttack");

                    /*print("RangeAttackDist : " + rangeAttackDist);
                    print("TargetDist : " + TargetDist);*/
                    StartCoroutine(lichScript.RangeAttack());

                    Debug.DrawRay(lichScript.rangeAttackFirePos.position, (Target.bounds.center - lichScript.rangeAttackFirePos.position));                
                }
                else
                {
                    //print("raycast가 걸려서 원거리 공격 불가!");
                    monsterState = MonsterState.trace;
                    //navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
                }
                gizmosPosition = hit.point;
            }
        }
        else if(Target != null && !lichScript.rangeAttackReady && TargetDist < lichScript.rangeAttackMaxDist && TargetDist < traceDist)
        {
            monsterState = MonsterState.idle;
        }
        else if(Target != null && TargetDist < traceDist)
        {
            monsterState = MonsterState.trace;
        }
        else if ((monsterState == MonsterState.trace || monsterState == MonsterState.rangeAttack) && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
            //navMeshAgent.speed = wanderSpeed;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
        {
            monsterState = MonsterState.idle;
            //navMeshAgent.speed = 0f;
        }
    }

    private void Golem_Behavior()
    {
        if (Target)
        {
            if (jumpAttackCoolTime > 0)
                jumpAttackCoolTime -= Time.deltaTime;
            dirToTarget = ((Target.transform.position + Vector3.up) - (transform.position + Vector3.up)).normalized;
            dstToTarget = Vector3.Distance(transform.position + Vector3.up, Target.transform.position + Vector3.up);
        }

        if (TargetDist < golem.sAttack01MaxDist && golem.sAttack01Ready && Target != null && !golem.isAttacking)
        {
            StartCoroutine(golem.sAttack01());
            //navMeshAgent.speed = 0;
        }
        else if (TargetDist < golem.meleeAttackMaxDist && Target != null && !golem.isAttacking && golem.meleeAttackReady)
        {
            //monsterState = MonsterState.meleeAttack;
            //navMeshAgent.speed = meleeAttackMoveSpeed;

            StartCoroutine(golem.meleeAttack());
        }
        else if (TargetDist < golem.rangeAttack01maxDist && TargetDist > golem.rangeAttack01minDist && golem.rangeAttack01ready && Target != null && !golem.isAttacking)
        {
            RaycastHit hit;
            if (Target != null)
            {
                var distToTarget = Vector3.Distance(golem.rangeAttackFirePos.position, Target.bounds.center);

                if (!Physics.Raycast(golem.rangeAttackFirePos.position, (Target.bounds.center - golem.rangeAttackFirePos.position).normalized, out hit, distToTarget, ObstacleMask))
                {
                    StartCoroutine(golem.throwRock());
                }
                else
                {
                    monsterState = MonsterState.trace;
                }
            }
        }
        else if (TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (heightDamageTarget != null)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.trace && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
            //navMeshAgent.speed = wanderSpeed;

        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
        {
            monsterState = MonsterState.idle;
            //navMeshAgent.speed = 0;
        }
    }

    private void EnergyGolem_Behavior()
    {
        if (Target)
        {
            if (jumpAttackCoolTime > 0)
                jumpAttackCoolTime -= Time.deltaTime;
            dirToTarget = ((Target.transform.position + Vector3.up) - (transform.position + Vector3.up)).normalized;
            dstToTarget = Vector3.Distance(transform.position + Vector3.up, Target.transform.position + Vector3.up);
        }
        //navMeshAgent.speed = animator.GetFloat("MoveSpeed");

        if (TargetDist < energyGolem.sAttack01MinDist && energyGolem.sAttack01Ready2 && Target != null && !energyGolem.isAttacking)
        {
            StartCoroutine(energyGolem.sAttack01Start());
        }
        else if(TargetDist > energyGolem.sAttack01MinDist && TargetDist < energyGolem.sAttack02MaxDist && energyGolem.sAttack02Ready && Target != null
            && (monsterState == MonsterState.trace || monsterState == MonsterState.idle) && !energyGolem.isAttacking)
        {
            StartCoroutine(energyGolem.sAttack02Start());
        }
        else if (TargetDist <= energyGolem.meleeAttackMaxDist && Target != null && energyGolem.meleeAttackReady && !energyGolem.isAttacking)
        {
            StartCoroutine(energyGolem.meleeAttack());
            //monsterState = MonsterState.meleeAttack;
            //navMeshAgent.speed = meleeAttackMoveSpeed;
        }
        else if (energyGolem.energyBoltReady && animator.GetBool("IsAttack") == false && Target != null
                 && !Physics.Raycast(transform.position + Vector3.up, dirToTarget, dstToTarget, ObstacleMask))
        {
            //animator.SetTrigger("ChargingEnergyBolt");
            energyGolem.EnergyBolt();
            energyGolem.energyBoltReady = false;
            //navMeshAgent.speed = 0;
        }
        else if (TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (heightDamageTarget != null)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.trace && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
            //navMeshAgent.speed = wanderSpeed;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
        {
            monsterState = MonsterState.idle;
            //navMeshAgent.speed = 0f;
        }
    }

    private void SpiderQueen_Behavior()
    {
        if (Target)
        {
            if (specialAttackCoolTime > 0)
                specialAttackCoolTime -= Time.deltaTime;
            dirToTarget = ((Target.transform.position + Vector3.up) - (transform.position + Vector3.up)).normalized;
            dstToTarget = Vector3.Distance(transform.position + Vector3.up, Target.transform.position + Vector3.up);
        }

        spiderQueen.CheckBabySpiderDead();


        if (TargetDist <= spiderQueen.MeleeAttackMaxDist && Target != null && !animator.GetBool("IsAttacking") && spiderQueen.MeleeAttackReady)
        {
            StartCoroutine(spiderQueen.MeleeAttack());

            //monsterState = MonsterState.meleeAttack;
        }
        else if (spider.webBallFireMinDist <= TargetDist && spider.webBallFireMaxDist >= TargetDist && spider.enableWebBallFire && spider.webBallReady && Target && !animator.GetBool("IsAttacking"))
        {
            RaycastHit hit;

            var distToTarget = Vector3.Distance(spider.webBallFirePos.position, Target.bounds.center);

            if (!Physics.Raycast(spider.webBallFirePos.position, (Target.bounds.center - spider.webBallFirePos.position).normalized, out hit, distToTarget, ObstacleMask))
            {
                StartCoroutine(spider.FireWebBall());

                Debug.DrawRay(spider.webBallFirePos.position, (Target.bounds.center - spider.webBallFirePos.position));
            }
            gizmosPosition = hit.point;

        }
        else if (spiderQueen.layEggReady && spiderQueen.maxSpiderSpawnCount > spiderQueen.spawnedSpider.Count && !animator.GetBool("IsAttacking") && Target)
        {
            animator.SetTrigger("layEggs");
            spiderQueen.layEgg();
            //specialAttackCoolTime = 10f;
        }
        else if (TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
        }
        else if (heightDamageTarget != null)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
        }
        else if (monsterState == MonsterState.trace && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
        }
        else if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
        {
            monsterState = MonsterState.idle;
        }
    }

    private void StatueKnight_Behavior()
    {
        if (Target != null)
        {
            dirToTarget = ((Target.transform.position + Vector3.up) - (transform.position + Vector3.up)).normalized;
            dstToTarget = Vector3.Distance(transform.position + Vector3.up, Target.transform.position + Vector3.up);
        }
        //navMeshAgent.speed = animator.GetFloat("MoveSpeed");

        if (statueKnightScript.statueForm && TargetDist < 15f && Target != null)
        {
            animator.SetTrigger("FormChange");
            return;
        }
        else if (TargetDist < 2f && Target != null)
        {
            animator.SetTrigger("VerticalSwing");
        }
        else if (TargetDist <= 4f && Target != null)
        {
            animator.SetTrigger("HorizontalSwing");
            //nav.speed = meleeAttackMoveSpeed;
        }
        else if (TargetDist < 8 && TargetDist > 5 && animator.GetBool("IsAttack") == false && Target != null)       //찌르기 공격
        {
            animator.SetTrigger("Sting");
            //nav.speed = 0;
        }
        else if (TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
            //nav.speed = traceSpeed;
        }
        else if (heightDamageTarget != null)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
            //nav.speed = traceSpeed;
        }
        else if (monsterState == MonsterState.trace && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
            //nav.speed = traceSpeed;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
        {
            monsterState = MonsterState.idle;
            //nav.speed = 0f;
        }
    }

    private void FallenKing_Behavior()
    {
        if (fallenKingScript.fall)
        {
            monsterState = MonsterState.idle;


            return;
        }

        if (Target != null)
        {
            dirToTarget = ((Target.transform.position + Vector3.up) - (transform.position + Vector3.up)).normalized;
            dstToTarget = Vector3.Distance(transform.position + Vector3.up, Target.transform.position + Vector3.up);
        }
        //navMeshAgent.speed = animator.GetFloat("MoveSpeed");

        /*if (TargetDist < 15f && Target != null)
        {
            animator.SetTrigger("FormChange");
            return;
        }
        else */

        fallenKingScript.ClearDeadMinion();

        if (fallenKingScript.standUpReady && TargetDist < fallenKingScript.standUpMinDist && Target)
        {
            StartCoroutine(fallenKingScript.StandUp());
        }
        if (fallenKingScript.phase == 1 && !fallenKingScript.standUpReady)
        {
            if (fallenKingScript.fall)
            {
                monsterState = MonsterState.idle;
                return;
            }
            else if (fallenKingScript.attackReady && Target)
            {
                if (fallenKingScript.darkSwordReady && TargetDist < fallenKingScript.darkSwordMaxDist)
                {
                    StartCoroutine(fallenKingScript.DarkSword());
                }
                else if (fallenKingScript.shieldReady)
                {
                    monsterState = MonsterState.meleeAttack;
                    StartCoroutine(fallenKingScript.Shield());
                }
                else if (fallenKingScript.meleeAttack01Ready && TargetDist < fallenKingScript.meleeAttack01MaxDist)
                {
                    monsterState = MonsterState.meleeAttack;
                    StartCoroutine(fallenKingScript.MeleeAttack01Start());
                }
                else if (fallenKingScript.rangeAttack01Ready && TargetDist < fallenKingScript.rangeAttack01MaxDist && TargetDist > fallenKingScript.rangeAttack01MinDist)
                {
                    monsterState = MonsterState.meleeAttack;
                    StartCoroutine(fallenKingScript.RangeAttack01Start());
                }
                else if (fallenKingScript.summonMinion01Ready && fallenKingScript.maxSummonCount > fallenKingScript.SummonedCreatures.Count)
                {
                    monsterState = MonsterState.meleeAttack;
                    StartCoroutine(fallenKingScript.SummonMinion01());
                }
                else if (!animator.GetBool("IsAttacking") && Target != null)
                {
                    monsterState = MonsterState.trace;
                }
                else if (!animator.GetBool("IsAttacking") && TargetDist <= traceDist && Target != null)
                {
                    monsterState = MonsterState.trace;
                }
                else if (!animator.GetBool("IsAttacking") && heightDamageTarget != null && monsterState == MonsterState.idle)
                {
                    Target = heightDamageTarget.GetComponent<Collider>();
                    monsterState = MonsterState.trace;
                }
                else if (!animator.GetBool("IsAttacking") && monsterState == MonsterState.trace && FollowItToTheEnd == true)
                {
                    monsterState = MonsterState.trace;
                }
            }
            else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
            {
                monsterState = MonsterState.idle;
            }
            else
            {
                monsterState = MonsterState.idle;
            }
        }
        else if (fallenKingScript.phase == 2 && !fallenKingScript.standUpReady)
        {
            if (fallenKingScript.attackReady && Target)
            {
                if (fallenKingScript.darkSwordReady && TargetDist < fallenKingScript.darkSwordMaxDist)
                {
                    StartCoroutine(fallenKingScript.DarkSword());
                }
                else if (fallenKingScript.gravityBallReady && TargetDist < fallenKingScript.gravityBallMaxDist && TargetDist > fallenKingScript.gravityBallMinDist)
                {
                    monsterState = MonsterState.meleeAttack;
                    StartCoroutine(fallenKingScript.GravityBall());
                }
                else if (fallenKingScript.sAttack01Ready && TargetDist < fallenKingScript.sAttack01MaxDist && TargetDist > fallenKingScript.sAttack01MinDist)     //특수공격
                {
                    monsterState = MonsterState.meleeAttack;
                    StartCoroutine(fallenKingScript.SattckStart());
                }
                else if (fallenKingScript.shieldReady)
                {
                    monsterState = MonsterState.meleeAttack;
                    StartCoroutine(fallenKingScript.Shield());
                }
                else if (fallenKingScript.meleeAttack01Ready && TargetDist < fallenKingScript.meleeAttack01MaxDist)
                {
                    monsterState = MonsterState.meleeAttack;
                    StartCoroutine(fallenKingScript.MeleeAttack01Start());
                }
                else if (fallenKingScript.darkArrowSprayReady && TargetDist < fallenKingScript.darkArrowSprayMaxDist && TargetDist > fallenKingScript.darkArrowSprayMinDist)
                {
                    monsterState = MonsterState.meleeAttack;
                    StartCoroutine(fallenKingScript.DarkArrowSpray());
                }
                else if (fallenKingScript.rangeAttack01Ready && TargetDist < fallenKingScript.rangeAttack01MaxDist && TargetDist > fallenKingScript.rangeAttack01MinDist)
                {
                    monsterState = MonsterState.meleeAttack;
                    StartCoroutine(fallenKingScript.RangeAttack01Start());
                }
                else if (fallenKingScript.summonMinion01Ready && fallenKingScript.maxSummonCount > fallenKingScript.SummonedCreatures.Count)
                {
                    monsterState = MonsterState.meleeAttack;
                    StartCoroutine(fallenKingScript.SummonMinion01());
                }
                else if (!animator.GetBool("IsAttacking") && Target != null)
                {
                    monsterState = MonsterState.trace;
                }
                else if (!animator.GetBool("IsAttacking") && TargetDist <= traceDist && Target != null)
                {
                    monsterState = MonsterState.trace;
                }
                else if (!animator.GetBool("IsAttacking") && heightDamageTarget != null && monsterState == MonsterState.idle)
                {
                    Target = heightDamageTarget.GetComponent<Collider>();
                    monsterState = MonsterState.trace;
                }
                else if (!animator.GetBool("IsAttacking") && monsterState == MonsterState.trace && FollowItToTheEnd == true)
                {
                    monsterState = MonsterState.trace;
                }
            }
            
            else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
            {
                monsterState = MonsterState.idle;
            }
            else
            {
                monsterState = MonsterState.idle;
            }
        }

    }

    private void WraithKing_Behavior()
    {

        if (!animator.GetBool("IsAttacking") && Target != null && TargetDist < wraithKing.meleeAttackMaxDist && wraithKing.meleeAttackReady)
        {
            int num = Random.Range(0, 1);

            switch (num)
            {
                case 0:
                    StartCoroutine(wraithKing.MeleeAttack());
                    break;
            }

        }
        else if (!animator.GetBool("IsAttacking") && Target != null && wraithKing.SummonReady && wraithKing.SummonedCreatures.Count <= wraithKing.maxSummonCount)
        {
            foreach (GameObject Creature in wraithKing.SummonedCreatures)
            {
                if (Creature.activeSelf == false)
                {
                    wraithKing.SummonedCreatures.Remove(Creature);
                    break;
                }
            }

            StartCoroutine(wraithKing.Summon());
        }
        else if (!animator.GetBool("IsAttacking") && Target != null && wraithKing.Sattack01Ready && TargetDist <= wraithKing.Sattack01Distance)
        {
            StartCoroutine(wraithKing.Sattack01());
        }
        else if (!animator.GetBool("IsAttacking") && Target != null && wraithKing.Sattack02Ready && TargetDist <= wraithKing.Sattack02Distance)
        {
            StartCoroutine(wraithKing.Sattack02());
        }
        else if (!animator.GetBool("IsAttacking") && Target != null && TargetDist < wraithKing.RangeAttackMaxDist && TargetDist > wraithKing.RangeAttackMinDist && wraithKing.RangeAttackReady)
        {
            float distToTarget = 0;
            if (Target != null)
            {
                distToTarget = Vector3.Distance(wraithKing.RangeAttackFirePos.position, Target.bounds.center);
                TargetDist = distToTarget;
            }

            RaycastHit hit;
            if (Target != null)
            {
                if (!Physics.Raycast(wraithKing.RangeAttackFirePos.position, (Target.bounds.center - wraithKing.RangeAttackFirePos.position).normalized, out hit, distToTarget, ObstacleMask))
                {
                    StartCoroutine(wraithKing.RangeAttack());
                    monsterState = MonsterState.trace;

                    Debug.DrawRay(wraithKing.RangeAttackFirePos.position, (Target.bounds.center - wraithKing.RangeAttackFirePos.position));
                }
                else
                {
                    print("raycast가 걸려서 원거리 공격 불가!");
                    monsterState = MonsterState.trace;
                }
                gizmosPosition = hit.point;
            }
        }
        else if (!animator.GetBool("IsAttacking") && Target != null)
        {
            monsterState = MonsterState.trace;
        }
        else if (!animator.GetBool("IsAttacking") && TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
        }
        else if (!animator.GetBool("IsAttacking") && heightDamageTarget != null)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
        }
        else if (!animator.GetBool("IsAttacking") && monsterState == MonsterState.trace && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
        {
            monsterState = MonsterState.idle;
        }
    }

    private void Skulder_Behavior()
    {
        if (Target)
        {
            if (jumpAttackCoolTime > 0)
                jumpAttackCoolTime -= Time.deltaTime;
            dirToTarget = ((Target.transform.position + Vector3.up) - (transform.position + Vector3.up)).normalized;
            dstToTarget = Vector3.Distance(transform.position + Vector3.up, Target.transform.position + Vector3.up);
        }
        //navMeshAgent.speed = animator.GetFloat("MoveSpeed");

        if (TargetDist < skulder.meleeAttackMinDist && skulder.meleeAttackReady && Target != null && !skulder.isAttacking)
        {
            StartCoroutine(skulder.MeleeAttack());
        }
        else if (TargetDist < skulder.BlastFireBallMinDist && skulder.BlastFireBallReady && Target != null && !skulder.isAttacking)
        {
            StartCoroutine(skulder.BlastFireBall());
        }
        else if (TargetDist > skulder.FireExplosionFireBallMinDist && TargetDist < skulder.FireExplosionFireBallMaxDist 
            && skulder.FireExplosionFireBallReady && Target != null && !skulder.isAttacking)
        {
            StartCoroutine(skulder.FireExplosionFireBall());
        }
        else if (TargetDist < skulder.FireBreathMinDist && skulder.FireBreathReady && Target != null && !skulder.isAttacking)
        {
            StartCoroutine(skulder.FireBreath());
        }
        /*else if (TargetDist > energyGolem.sAttack01MinDist && TargetDist < energyGolem.sAttack02MaxDist && energyGolem.sAttack02Ready && Target != null
            && (monsterState == MonsterState.trace || monsterState == MonsterState.idle))
        {
            StartCoroutine(energyGolem.sAttack02Start());
        }*/
        else if (skulder.FireMagicArrowReady && TargetDist < skulder.FireMagicArrowMaxDist && TargetDist > skulder.FireMagicArrowMinDist && Target != null && !skulder.isAttacking
                 && !Physics.Raycast(transform.position + Vector3.up, dirToTarget, dstToTarget, ObstacleMask))
        {
            StartCoroutine(skulder.FireMagicArrow());
        }
        else if (TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = BaseSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (heightDamageTarget != null)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = BaseSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.trace && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = BaseSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
            //navMeshAgent.speed = wanderSpeed;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
        {
            monsterState = MonsterState.idle;
            //navMeshAgent.speed = 0f;
        }
    }

    private void Guros()
    {
        if (Target != null)
        {
            if (jumpAttackCoolTime > 0)
                jumpAttackCoolTime -= Time.deltaTime;
            dirToTarget = ((Target.transform.position + Vector3.up) - (transform.position + Vector3.up)).normalized;
            dstToTarget = Vector3.Distance(transform.position + Vector3.up, Target.transform.position + Vector3.up);
        }

        //navMeshAgent.speed = animator.GetFloat("MoveSpeed");

        /*if (TargetDist < 2f && Target != null)
        {
            monsterState = MonsterState.closeAttack;
        }*/
        if(gurosAI.gurosSpear.isReadyPull && !gurosAI.gurosSpear.isPulling && animator.GetBool("IsAttacking") == false)
        {
            StartCoroutine(gurosAI.PullSpear());
        }
        else if (TargetDist < gurosAI.JumpAttackMaxDistance && TargetDist > gurosAI.JumpAttackMinDistance && gurosAI.JumpAttackReady
            && animator.GetBool("IsAttacking") == false && Target != null)
        {
            StartCoroutine(gurosAI.JumpAttack());
        }
        else if (TargetDist < gurosAI.Sattack1Distance && gurosAI.Sattack1Ready
            && animator.GetBool("IsAttacking") == false && Target != null)
        {
            StartCoroutine(gurosAI.Sattack1());
        }
        else if (TargetDist <= gurosAI.RangeAttackMaxDistance && TargetDist > gurosAI.RangeAttackMinDistance && Target != null && gurosAI.RangeAttackReady
            && !gurosAI.gurosSpear.isReadyPull && !gurosAI.gurosSpear.isPulling && gurosAI.SpearReady && animator.GetBool("IsAttacking") == false)
        {
            float distToTarget = 0;
            if (Target != null)
            {
                distToTarget = Vector3.Distance(gurosAI.Spear.transform.position, Target.bounds.center);
                TargetDist = distToTarget;
            }

            RaycastHit hit;
            if (Target != null)
            {
                if (!Physics.Raycast(gurosAI.Spear.transform.position, (Target.bounds.center - gurosAI.Spear.transform.position).normalized, out hit, distToTarget, ObstacleMask))
                {
                    StartCoroutine(gurosAI.RangeAttack());
                    monsterState = MonsterState.rangeAttack;

                    Debug.DrawRay(gurosAI.Spear.transform.position, (Target.bounds.center - gurosAI.Spear.transform.position));
                }
                else
                {
                    print("raycast가 걸려서 원거리 공격 불가!");
                    monsterState = MonsterState.rangeAttack;
                }
                gizmosPosition = hit.point;
            }
        }
        else if (TargetDist <= meleeAttackDist && Target != null)
        {
            monsterState = MonsterState.meleeAttack;
            //nav.speed = meleeAttackMoveSpeed;
        }
        else if (TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
            //nav.speed = traceSpeed;
        }
        else if (heightDamageTarget != null)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
            //nav.speed = traceSpeed;
        }
        else if (monsterState == MonsterState.trace && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
            //nav.speed = traceSpeed;
        }
        else if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
            //nav.speed = wanderSpeed;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
        {
            monsterState = MonsterState.idle;
            //nav.speed = 0f;
        }
    }

    private void SkeletonGuardian_Behavior()
    {
        if (Target)
        {
            if (jumpAttackCoolTime > 0)
                jumpAttackCoolTime -= Time.deltaTime;
            dirToTarget = ((Target.transform.position + Vector3.up) - (transform.position + Vector3.up)).normalized;
            dstToTarget = Vector3.Distance(transform.position + Vector3.up, Target.transform.position + Vector3.up);
        }
        //navMeshAgent.speed = animator.GetFloat("MoveSpeed");

        if (TargetDist < skeletonGuardian.WhirlWindForwardMaxDist && skeletonGuardian.WhirlWindForwardReady && !skeletonGuardian.isAttacking && Target != null)
        {
            StartCoroutine(skeletonGuardian.WhirlWindForwardStart());
        }
        else if (TargetDist > skeletonGuardian.jumpAttackMinDist && TargetDist < skeletonGuardian.jumpAttackMaxDist && skeletonGuardian.jumpAttackReady && !skeletonGuardian.isAttacking && Target != null)
        {
            StartCoroutine(skeletonGuardian.JumpAttackStart());
        }
        else if (TargetDist < skeletonGuardian.WhirlWindMaxDist && skeletonGuardian.WhirlWindReady && !skeletonGuardian.isAttacking && Target != null)
        {
            StartCoroutine(skeletonGuardian.WhirlWindStart());
        }
        else if (TargetDist < skeletonGuardian.attack01MaxDist && skeletonGuardian.attack01Ready && !skeletonGuardian.isAttacking && Target != null)
        {
            StartCoroutine(skeletonGuardian.Attack01Start());
        }
        else if (TargetDist < skeletonGuardian.attack02MaxDist && skeletonGuardian.attack02Ready && !skeletonGuardian.isAttacking && Target != null)
        {
            StartCoroutine(skeletonGuardian.Attack02Start());
        }
        else if (TargetDist > skeletonGuardian.attack03MinDist && TargetDist < skeletonGuardian.attack03MaxDist && skeletonGuardian.attack03Ready && !skeletonGuardian.isAttacking && Target != null)
        {
            StartCoroutine(skeletonGuardian.Attack03Start());
        }
        else if (TargetDist <= traceDist && Target != null)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (heightDamageTarget != null)
        {
            Target = heightDamageTarget.GetComponent<Collider>();
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.trace && FollowItToTheEnd == true)
        {
            monsterState = MonsterState.trace;
            //navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
        }
        else if (monsterState == MonsterState.idle && canWander == true)
        {
            monsterState = MonsterState.wander;
            //navMeshAgent.speed = wanderSpeed;
        }
        else if ((TargetDist > traceDist || Target == null || (TargetDist >= traceDist && FollowItToTheEnd == false)) && isWandering == false)
        {
            monsterState = MonsterState.idle;
            //navMeshAgent.speed = 0f;
        }
    }

    private void FollowSummoner()
    {
        if(Summoner != null)
        {
            SummonerDist = Vector3.Distance(Summoner.transform.position, transform.position);
            if (SummonerDist > followSummonerDist)
            {
                monsterState = MonsterState.Follow;
                navMeshAgent.destination = Summoner.transform.position;
                navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();
            }
            else if (SummonerDist < followSummonerDist && canWander == true)
            {
                monsterState = MonsterState.wander;
                navMeshAgent.speed = wanderSpeed;
            }
            else if (SummonerDist < followSummonerDist)
            {
                monsterState = MonsterState.idle;
                navMeshAgent.speed = 0f;
                
                /*if(canWander == true)
                {
                    monsterState = MonsterState.wander;
                    nav.speed = wanderSpeed;
                }*/
            }
            
        }
    }

    private void FollowTarget()
    {
        if (followTarget != null)
        {
            targetDist = Vector3.Distance(followTarget.transform.position, transform.position);

            if (targetDist > FollowMinDist)
            {
                monsterState = MonsterState.Follow;
                navMeshAgent.destination = followTarget.transform.position;
                navMeshAgent.speed = moveSpeed.GetFinalStatValueAsMultiflyFloat();

                Debug.DrawLine(transform.position, followTarget.transform.position, Color.yellow, 2);
            }
            else if (targetDist < FollowMinDist && canWander == true)
            {
                monsterState = MonsterState.wander;
                navMeshAgent.speed = wanderSpeed;
            }
            else if (targetDist < FollowMinDist)
            {
                monsterState = MonsterState.idle;
                navMeshAgent.speed = 0f;
            }

        }
    }

    public void WideAttack()                //범위공격
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, wideAttackArea, TargetMask);
        
        foreach(Collider collider in colliders)
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(transform.GetComponent<Collider>().bounds.center, -transform.up, out raycastHit, 10f, ObstacleMask))
            {
                ParticleGenerator.instance.GenerateGroundParticle(raycastHit.point, "GroundCrack01", 10f);
            }

            if (collider.CompareTag("Player"))
            {
                collider.GetComponent<PlayerStats>().TakeDamage((int)Mathf.Round(stat.minDamage.GetFinalStatValue() * wideAttackDamageMulti), (int)Mathf.Round(stat.maxDamage.GetFinalStatValue() * wideAttackDamageMulti), gameObject,true,true,false);
                EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, .1f, 1);

            }
            else if(collider.CompareTag("Enemy") && collider.gameObject != gameObject)
            {
                collider.GetComponent<NPCStats>().TakeDamage((int)Mathf.Round(stat.minDamage.GetFinalStatValue() * wideAttackDamageMulti)
                    , (int)Mathf.Round(stat.maxDamage.GetFinalStatValue() * wideAttackDamageMulti), gameObject, true, true, false);
            }
        }
    }

    public void IncreaseCoolTime()
    {
        jumpAttackCoolTime += 2;
    }

    public void PlayheavySwingSound()
    {
        audioSource.PlayOneShot(GetAudioClipInDic("HeavySwingSound"));
    }

    public void PlayFootStep1()
    {
        audioSource.PlayOneShot(GetAudioClipInDic("FootStep1"));
    }

    public void PlayFootStep2()
    {
        audioSource.PlayOneShot(GetAudioClipInDic("FootStep2"));
    }

    public AudioClip GetAudioClipInDic(string str)
    {
        for(int i = 0; i < AudioList.Count; i++)
        {
            if (str == AudioList[i].name)
                return AudioList[i].audioClip;
        }
        return null;
    }

    private void SmoothLookatTarget()
    {
        //transform.rotation = Quaternion.Slerp(transform.rotation, q, RotationSpeed);     //canRotationWhileAttack가 true면 npc가 타겟을 바라보게 하기

        vectorToTarget.y = 0;
        vectorToTarget.Normalize();

        Vector3 direction = navMeshAgent.desiredVelocity;
        Quaternion targetAngle = Quaternion.LookRotation(vectorToTarget);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetAngle, Time.deltaTime * 8);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawCube(gizmosPosition, new Vector3(0.5f,0.5f,0.5f));

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(contactPoint, 0.2f);
    }


    public void GetDamage(GameObject ob, float damage)
    {
        if (TakeDamage.ContainsKey(ob))
        {
            float outValue;
            TakeDamage.TryGetValue(ob, out outValue);

            TakeDamage[ob] += damage;

            //Debug.Log(ob + " " + outValue);
        }
        else
        {
            TakeDamage.Add(ob, damage);
        }

        /*foreach(GameObject key in TakeDamage.Keys)
        {
            if (key.activeInHierarchy == false)
                TakeDamage.Remove(key);
        }*/

    }

    public void SortDic()
    {
        var keys = from pair in TakeDamage
                   orderby pair.Value 
                   ascending select pair;

        heightDamageTarget = null;

        foreach (KeyValuePair<GameObject, float> pair in keys)
        {
            heightDamageTarget = pair.Key;
            //Debug.Log("공격 우선순위 : " + pair.Key);
        }

        
    }

    private void SetAnimatorLayer()
    {
        if(animator.GetBool("IsAttack"))
        {
            //animator.SetLayerWeight(1, 1);
        }
        else
        {
            //animator.SetLayerWeight(1, 0);
        }
        
    }

    IEnumerator resetNavMesh()
    {
        LocalNav.m_Size = new Vector3(TargetDist * 1.5f, 50f, TargetDist * 1.5f);

        yield return new WaitForSeconds(0.2f);
        StartCoroutine(resetNavMesh());
    }

    private void self_Destrution()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, ExplosionRadius);

        transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < colliders.Length; i++)
        {
            if(npcType == NPC_Type.friendly || npcType == NPC_Type.Minion)
            {
                if (colliders[i].GetComponentInChildren<NPC_AI>() != null)
                {
                    if(colliders[i].GetComponentInChildren<NPC_AI>().npcType == NPC_Type.enemy)
                    {
                        CharacterStats Cstats = colliders[i].GetComponentInChildren<CharacterStats>();

                        Cstats.TakeDamage(DestructDamage,DestructDamage, Summoner.gameObject, false, true, false);
                    }
                }
            } else if(npcType == NPC_Type.enemy)
            {
                if (colliders[i].CompareTag("player"))
                {
                    colliders[i].GetComponentInChildren<PlayerStats>().TakeDamage(DestructDamage,DestructDamage, Summoner.gameObject, false, true, false);
                }
                else if(npcType == NPC_Type.friendly || npcType == NPC_Type.Minion)
                {
                    CharacterStats Cstats = colliders[i].GetComponent<CharacterStats>();

                    Cstats.TakeDamage(DestructDamage,DestructDamage, Summoner.gameObject, false, true, false);
                }
            }
            
        }
        ParticleGenerator.instance.ExplosionParticle(transform.position, "Explosion");
    }

    IEnumerator Wander()
    {
        isWandering = true;
        int waitTime = Random.Range(WanderMinWaitTime, WanderMaxWaitTime);
        NavMeshHit navHit;

        /*RaycastHit Hit;
        Vector3 randomVector = new Vector3(Random.Range(-wanderDistanse, wanderDistanse), 100f, Random.Range(-wanderDistanse, wanderDistanse));
        if (Physics.Raycast(transform.position + randomVector, Vector3.down, out Hit, 1000f, mask))
        {
            if (navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.SetDestination(Hit.point);

                animator.SetBool("IsTrace", true);

                print("ISWANDERING");

                if (monsterType == MonsterType.Range)
                    animator.SetBool("IsRangeAttack", false);
            }
            else
            {
                print("객체가 NavMesh위에 있지 않습니다.");
            }
            wanderVec = Hit.point;
            TargetDist = Vector3.Distance(transform.position, wanderVec);
            //LocalNav.m_Size = new Vector3(wanderPointDst, 50f, wanderPointDst);

        }*/
        int infinityloopCount = 0;

        while(true)
        {
            if (infinityloopCount > 10)
            {
                //Debug.LogWarning("무한 루프 탈출");
                break;
            }
                

            Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderDistanse;

            bool followitem = false;

            if (GetComponent<Mimic>())
            {
                followitem = GetComponent<Mimic>().TraceItem;
            }

            if (NavMesh.SamplePosition(randomPoint, out navHit, 1f, NavMesh.AllAreas) && !followitem)
            {
                if(!navMeshAgent.isOnNavMesh)
                {
                    //print("navMeshAgent가 NavMesh위에 있지 않음 navMeshAgent를 재설정함");
                    navMeshAgent.enabled = false;
                    navMeshAgent.enabled = true;
                }

                patrolVec = navHit.position;

                navMeshAgent.SetDestination(navHit.position);

                if (unSyncPosition)
                    UnSyncWanderDestination = navHit.position;

                animator.SetBool("IsTrace", true);

                //print("ISWANDERING");

                if (monsterType == MonsterType.Range)
                    animator.SetBool("IsRangeAttack", false);

                TargetDist = Vector3.Distance(transform.position, patrolVec);

                if (applyRootMotionSpeed && monsterState == MonsterState.wander)
                {
                    vectorToTarget = navHit.position - transform.position;
                }

                Debug.DrawLine(transform.position, navHit.position, Color.green, 2);
                break;
            }
            else
            {
                //print("객체가 NavMesh위에 있지 않습니다. 다음 Wander 목적지 찾기 실패");
            }

            infinityloopCount++;
        }
        yield return new WaitForSeconds(waitTime);
        isWandering = false;
    }

    IEnumerator Patrol()
    {
        isPatrol = true;
        int waitTime = Random.Range(PatrolMinWaitTime, PatrolMaxWaitTime);
        NavMeshHit navHit;
        Vector3 nextPatrolPoint;

        while (true)
        {
            nextPatrolPoint = MazeDungeonNpcSpawner.instance.GetRandomSpawnPoint();

            NavMeshAgent navMeshAgent = transform.GetComponent<NavMeshAgent>();
            NavMeshPath navMeshPath = new NavMeshPath();

            if (navMeshAgent.CalculatePath(nextPatrolPoint, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                //다음 순찰지점 이동가능 테스트
                break;
            }
        }

        if (!navMeshAgent.isOnNavMesh)
        {
            //print("navMeshAgent가 NavMesh위에 있지 않음 navMeshAgent를 재설정함");
            navMeshAgent.enabled = false;
            navMeshAgent.enabled = true;
        }

        patrolVec = nextPatrolPoint;

        navMeshAgent.SetDestination(nextPatrolPoint);

        animator.SetBool("IsTrace", true);

        if (monsterType == MonsterType.Range)
            animator.SetBool("IsRangeAttack", false);

        TargetDist = Vector3.Distance(transform.position, patrolVec);

        if (applyRootMotionSpeed && monsterState == MonsterState.wander)
        {
            vectorToTarget = patrolVec - transform.position;
        }

        Debug.DrawLine(transform.position, patrolVec, Color.green, 2);

        yield return new WaitForSeconds(waitTime);
        isPatrol = false;
    }

    public void ResetNPCAi()
    {
        collsList.Clear();
        TakeDamage.Clear();
        Target = null;
        heightDamageTarget = null;
        monsterState = MonsterState.idle;
        isWandering = false;
    }

    public void OnChangeNPCSpeed(float movespeed, float meleeattackspeed, float rangeattackspeed, float animationmovespeed)
    {
        moveSpeed.AddIntModifier((int)movespeed);
        //animationmoveSpeed += animationmovespeed;
        //meleeAttackSpeed += meleeattackspeed;
        //rangeAttackSpeed += rangeattackspeed;

        /*if (animator.GetFloat("MoveSpeed") != 0)
            animator.SetFloat("MoveSpeed", animationmoveSpeed);
        if (animator.GetFloat("AttackSpeed") != 0)
            animator.SetFloat("AttackSpeed", meleeAttackSpeed);
        if (animator.GetFloat("RangeAttackSpeed") != 0)
            animator.SetFloat("RangeAttackSpeed", rangeAttackSpeed);*/
    }

    public void AddMoveAnimationSpeed(float speed)
    {
        moveAnimationSpeed.AddPercentModifier(speed);
        animator.SetFloat("MoveSpeed", moveAnimationSpeed.GetFinalStatValue());
    }

    public void RemoveMoveAnimationSpeed(float speed)
    {
        moveAnimationSpeed.RemovePercentModifier(speed);
        animator.SetFloat("MoveSpeed", moveAnimationSpeed.GetFinalStatValue());
    }

    public void AddAttackAnimationSpeed(float speed)
    {
        attackAnimationSpeed.AddPercentModifier(speed);
        animator.SetFloat("AttackSpeed", attackAnimationSpeed.GetFinalStatValue());
    }

    public void RemoveAttackAnimationSpeed(float speed)
    {
        attackAnimationSpeed.RemovePercentModifier(speed);
        animator.SetFloat("AttackSpeed", attackAnimationSpeed.GetFinalStatValue());
    }

    public void eachColliderOn(string str)
    {
        for (int i = 0; i < thisColliders.Count; i++)
        {
            if (thisColliders[i].name == str)
            {
                thisColliders[i].Collider.enabled = true;
            }
        }
    }

    public void eachColliderOff(string str)
    {
        for (int i = 0; i < thisColliders.Count; i++)
        {
            if (thisColliders[i].name == str)
            {
                thisColliders[i].Collider.enabled = false;
            }
        }
    }

    void CheckGround()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position + new Vector3(0, .2f, 0), Vector3.down * 1f, Color.green);

        if (Physics.Raycast(transform.position + new Vector3(0, .2f, 0), Vector3.down, out hit, 1f))
        {
            //if (hit.transform.CompareTag("Ground") || hit.transform.CompareTag("Environment"))
            //{
            grounded = true;

            return;
            //}
        }
        grounded = false;
    }

    public void setTarget(Collider target)
    {
        highestDamageFirst = target;
    }

    public void GetStun()
    {
        var logic = GetComponentInChildren<NPC_AttackLogic>();
        logic.ColliderOff();
        logic.WeaponTrailOff();
        logic.FunchEnd();

        rigidbody.isKinematic = true;

        navMeshAgent.SetDestination(transform.position);

        rigidbody.isKinematic = false;
    }

    public void DiggingDustParticlePlay()
    {
        //Debug.LogError(transform.position);
        Destroy(Instantiate(PrefabCollect.instance.DiggingDustParticle, new Vector3(transform.position.x, 0.3f, transform.position.z), Quaternion.identity), 6f);
    }

    public void AnnounceNearNpc(GameObject origin,GameObject Target)
    {
        var thisPosition = GetComponent<Collider>().bounds.center;

        Collider[] nearNpc = Physics.OverlapSphere(thisPosition, announceDist + 10, PrefabCollect.instance.npcMask);

        foreach (Collider npc in nearNpc)
        {
            if(npc.GetComponentInChildren<NPC_AI>() != null && npc.gameObject != gameObject)
            {
                if(npc.GetComponentInChildren<NPC_AI>().npcType == npcType && npc.GetComponentInChildren<NPC_AI>().Target == null)
                {
                    RaycastHit hit;
                    if (Target != null && origin != null)
                    {
                        var npcPosiition = npc.GetComponent<Collider>().bounds.center;

                        var distToTarget = Vector3.Distance(thisPosition, npcPosiition);

                        if(!AttackEffectFunctions.CheckRayCast(thisPosition,npcPosiition,PrefabCollect.instance.obstacleMask,drawDebugLine:true))
                        {
                            npc.GetComponentInChildren<NPC_AI>().heightDamageTarget = Target;
                        }

                        /*if (!Physics.Raycast(thisPosition, (npcPosiition - thisPosition).normalized, out hit, distToTarget, ObstacleMask))
                        {
                            Debug.LogError(hit);

                            npc.GetComponentInChildren<NPC_AI>().heightDamageTarget = Target;

                            Debug.DrawRay(thisPosition, (npcPosiition - thisPosition).normalized * distToTarget,Color.blue,3f);
                        }*/

                    }   
                }
            }
        }
    }

    public void GenerateSFX(string name)
    {
        AudioManager.instance.GenerateAudioAndPlaySFX(name, GetComponent<Collider>().bounds.center);
    }

}

public enum MonsterType { Melee, Range, Boss, SelfDestruct, NonAttack, Spider, GhostSkull, MiniBoss }
public enum MonsterState { idle, trace, meleeAttack, closeAttack, die, wander, rangeAttack, Follow, patrol };
public enum NPC_Type
{
    none, enemy, neutrality, friendly, Minion
};

public enum BossType {none, GiantSkeleton, Lich, Golem, SpiderQueen, StatueKnight, EnergyGolem, FallenKing, WraithKing, Guros, Skulder, SkeletonGuardian}


