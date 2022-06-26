using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;
using System.ComponentModel;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerController01 : MonoBehaviour {

    public BoxCollider rightSwordColider;
    public PlayerAttackLogic rightSwordAttackLogic;
    public MeleeWeaponTrail rightSwordTrail;
    public ParticleSystem rightSwordTrailParticle;
    [Space]
    public BoxCollider rightDaggerColider;
    public PlayerAttackLogic rightDaggerAttackLogic;
    public MeleeWeaponTrail rightDaggerTrail;
    public ParticleSystem rightDaggerTrailParticle;
    private BoxCollider leftKnifeColider;
    private PlayerAttackLogic leftDaggerAttackLogic;
    private MeleeWeaponTrail leftKnifeTrail;
    [Space]
    public Transform LeftHand;
    [Space]
    public BoxCollider frontAttackCollider;
    public PlayerAttackLogic frontAttackLogic;
    [Space]

    [SerializeField] Color SwordTrailGeryColor;
    [SerializeField] Color SwordTrailRedColor;
    public Transform Chest;
    public Transform head;
    public SkinnedMeshRenderer BodyMeshRenderer;
    private Vector3 targetPosition;

    private GameObject MagicBall;
    private MagicArrowLogic magicArrowLogic;
    private Slider dodgeCoolSlider;
    private GameObject dodgeObject;
    private Rigidbody rigdbody;
    private CapsuleCollider playerCollider;
    private Slider Chargebar;
    private PlayerStats playerstat;
    private PlayerSkill playerskill;
    private SkinnedMeshRenderer[] BodySkinnedMeshRenderers;
    private AudioSource AudioSource;
    private Material[] mat;

    public Animator animator;
    public Vector3 headOffset;
    public Transform target;
    public GameObject inventory;
    public GameObject equipment;
    public GameObject raycastTarget;
    public Vector3 Gap;
    private Vector2 padRotate;
    
    [Space]
    public float maxSpeed = 5f;
    //public float currentDashSpeed = 0f;   //달리기 속도
    public float DashSpeed = 25;
    public float moveSpeed = 500;               //이동속도
    public float moveSpeedPenalty = 1;
    public float jumpPower = 8;                  //점프파워
    public float rotateSpeed = 15f;                //캐릭터회전속도
    
    //public float RotationSpeed = 5f;              //카메라회전속도
    public float keepCombatTime = 1.25f;
    //public float dodgeCoolTime = 5;

    [Space]
    
    public float maxChargeCount = 30f;                   //마법 기본공격 최대 차지량
    public float minChargeCount = 5f;

    private Quaternion RotF, RotR, RotL, RotB, RotFR, RotFL, RotBR, RotBL;
    private Vector3 targetrotation;
    private Transform firePos;
    private Transform tr;
    private Vector3 movement;
    private RaycastHit hit;
    //private CharacterController characterController;

    public BoolStat isAttacking;

    public bool isBlocking;
    private bool grounded = false;                         //점프
    private bool staminaRunout;                  //스태미나 오링
    private bool isJump;
    private bool nextAttack;
    private bool inCombat;
    private bool isUsingSkill;
    private bool isArrowCharging;
    private bool isDodge;
    private bool isRunning = false;
    private bool handBowReady = true;
    private bool isSneaking = false;
    private float AttackAnimTime = 0;
    private float horizontalMove;
    private float verticalMove;
    public float minMovementDetectionValue = 0.5f;
    private bool isMoving = false;
    public float ChargeCount = 0;

    [SerializeField]
    private float CombatTime = 0;

    [SerializeField]
    private float currentRollCoolTime = 0;

    

    public delegate void onUseSkill();
    public onUseSkill onUseSkillCallback;

    //public event System.Action<SkillSlot> OnUseSkill;

    public Vector3 contactPoint;

    public LayerMask obstacleMask;
    public LayerMask groundCheckMask;
    public LayerMask npcMask;

    public Rig UpperBodyRig;

    public GameObject DefaultCloat;
    public GameObject Body;
    public GameObject Foot1;
    public GameObject Foot2;

    public Text playerAngle;

    public CharacterController controller;

    private Vector3 lastGroundedPosition;

    private PlayerInputAction playerInputActions;
    private bool PressedAttackButton = false, PressedShieldButton = false;
    [Space]

    #region 스킬 관련 변수
    private GameObject BombModel;
    private GameObject Bomb;
    private bool bombReady;
    private Skill bombThrowSkillInfo;
    private AudioSource magicArrowAudioSource;
    public ParticleSystem bowChargingParticle;
    public void BombisExpolde()
    {
        animator.SetBool("Bomb_Ready", false);
        bombReady = false;
    }

    public List<GameObject> SummonedCreature = new List<GameObject>();
    public Stack<MagicOrb> magicOrbs = new Stack<MagicOrb>();
    public Stack<GameObject> magicOrbBeacons = new Stack<GameObject>();

    private int BouncingBallCount = 0;

    private bool FirstShackleReady = false;
    private bool SecondShackleReady = false;
    private RopeBothObject ShackleScripts = null;
    public float SneakGague = 0;

    private bool SoulDraining = false;
    private float soulDrainCoolTime = 0;

    public List<Arrow> arrowList = new List<Arrow>();
    public List<GameObject> arrowCellList = new List<GameObject>();
    public Transform FlameFirePos;
    public VisualEffect FlameVFX;
    public Transform FlameTarget;

    #endregion

    [Space]

    [SerializeField] private ParticleSystem rollDustParticle;

    private void Awake()
    {
        Gap = Vector3.zero;

        AddPlayerInputAction();

        KeyBindindManager.instance.OnUpdateKeyBindsCallBack += UpdateBindingKey;
    }

    private void AddPlayerInputAction()
    {
        playerInputActions = new PlayerInputAction();

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            playerInputActions.LoadBindingOverridesFromJson(rebinds);

        playerInputActions.Player.Jump.performed += Jump;
        playerInputActions.Player.Move.performed += Move_performed;

        playerInputActions.Player.BasicAttack.started += Attack_started;
        playerInputActions.Player.BasicAttack.canceled += Attack_canceled;

        playerInputActions.Player.Shield.started += Shield_started;
        playerInputActions.Player.Shield.canceled += Shield_canceled;

        playerInputActions.Player.Sneak.started += Sneak_started;
        playerInputActions.Player.Sneak.canceled += Sneak_canceled;

        playerInputActions.Player.Running.performed += Running_performed;
        playerInputActions.Player.Running.canceled += Running_canceled;

        playerInputActions.Player.Roll.performed += Roll_started;

        playerInputActions.Player.RotateCamera.performed += ctx => padRotate = ctx.ReadValue<Vector2>();
        playerInputActions.Player.RotateCamera.canceled += ctx => padRotate = Vector2.zero;

        playerInputActions.Player.Enable();
    }

    private void UpdateBindingKey()
    {
        playerInputActions.Disable();
        AddPlayerInputAction();
    }

    void Start()
    {
        tr = GetComponent<Transform>();
        //AttackAnimTime = AnimationLength("Sword_Atk02");

        playerskill = GetComponent<PlayerSkill>();
        rigdbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerstat = GetComponent<PlayerStats>();
        AudioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<CapsuleCollider>();
        //head = animator.GetBoneTransform(HumanBodyBones.Head);
        //StartCoroutine("WhatHappen");      //Coroutione 테스트

        if (inventory == null)
            inventory = UIManager.instance.inventory;
        if (equipment == null)
            equipment = UIManager.instance.equipment;
        if (BombModel == null)
            BombModel = PrefabCollect.instance.Bomb;

        GameObject cameraMan = GameObject.Find("Camera Man");
        target = GameObject.Find("RayCastTarget").transform;
        Chargebar = UIManager.instance.ChargeBar;
        firePos = transform.Find("firePos");

        rightSwordTrail = rightSwordColider.transform.GetComponent<MeleeWeaponTrail>();
        rightDaggerTrail = rightDaggerColider.transform.GetComponent<MeleeWeaponTrail>();
        //leftKnifeTrail = leftKnifeColider.transform.GetComponent<MeleeWeaponTrail>();

        rightSwordAttackLogic = rightSwordColider.GetComponent<PlayerAttackLogic>();
        rightDaggerAttackLogic = rightDaggerColider.GetComponent<PlayerAttackLogic>();
        //leftDaggerAttackLogic = leftKnifeColider.GetComponent<PlayerAttackLogic>();
        frontAttackLogic = frontAttackCollider.GetComponent<PlayerAttackLogic>();

        dodgeCoolSlider = UIManager.instance.DodgeBar;
        dodgeObject = UIManager.instance.Dodge;

        SceneChanger.instance.OnStageEnd += ReplaceMinions;
        //ChangeLayer(transform, 9);

        var bodyMat = GetComponents<SkinnedMeshRenderer>();

        mat = GetComponentInChildren<SkinnedMeshRenderer>().materials;
        BodySkinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        controller = GetComponent<CharacterController>();

        FlameVFX.Stop();
    }

    void Update()
    {
        if (isAttacking.GetBoolValue() == true)          //근접공격 잔상 온 오프
        {
            //weapontrail.Emit = true;
        }
        else if (animator.GetBool("isAttack") == false && isAttacking.GetBoolValue() == false)
        {
            //weapontrail.Emit = false;
        }

        targetrotation = new Vector3(target.position.x, transform.position.y, target.position.z);

        //verticalVelocity += Physics.gravity.y * Time.deltaTime;

        CheckGround(); // 밑이 땅인지 확인

        animator.SetFloat("AttackSpeed", playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat());

        if (!playerstat.isStun.GetBoolValue() && !DungeonGenerator.instance.GetDungeonGenerating())
        {
            PlayerAttack(null);
            AnimationUpdate();
            Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

            horizontalMove = inputVector.x;
            verticalMove = inputVector.y;

            if (isJump)
            {
                rigdbody.AddForce(new Vector3(0f, jumpPower, 0f));
                isJump = false;
            }

            if ((horizontalMove > minMovementDetectionValue || verticalMove > minMovementDetectionValue
                || horizontalMove < -minMovementDetectionValue || verticalMove < -minMovementDetectionValue))
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }

        if (CombatTime > -1)
            CombatTime -= Time.deltaTime;

        if (CombatTime < 0)
        {
            inCombat = false;
        }
        else
        {
            inCombat = true;
        }

        //if (Input.GetKeyDown(KeyCode.G))
        //{
            //DropItem.dropItem(DataMannager.instance.itemDataBase, transform, GetComponent<PlayerStats>(), 100);
        //    DropItem.instance.CreateEquipmentDropItem(transform, GetComponent<PlayerStats>().playerClass);
        //}

        if (Chargebar.value <= minChargeCount)
        {
            Chargebar.gameObject.SetActive(false);
        }
        else
        {
            Chargebar.gameObject.SetActive(true);
        }

        if (isBlocking == true)
        {
            ResetCombatTime(0.1f);
        }

        if (isSneaking && isAttacking.GetBoolValue() == false && playerskill.CheckPlayerHaveSkill(SkillManager.instance.Sneak))
        {
            SneakGague += Time.deltaTime * playerstat.SneakGagueGainValue.GetFinalStatValueAsMultiflyFloat();
            //print("SneakGague : " + SneakGague);
            OnSneakGagueChange();
        }
        else
        {
            SneakGague -= Time.deltaTime * 15;
            OnSneakGagueChange();
        }

        SneakGague = Mathf.Clamp(SneakGague, 0, playerstat.playerMaxSneakGague);

        Debug.DrawLine(transform.position, transform.position + transform.forward, Color.cyan);

        if(Time.timeScale == 0)
        {
            UpperBodyRig.weight = 0;
        }
        else
        {
            UpperBodyRig.weight = 1;

        }

        //최대 속도보다 크면 최대속도를 조절
        if (Mathf.Abs(rigdbody.velocity.x) > maxSpeed)
        {
            //속도 조절
            //sign -> 값이 양이거나 0 일때 1반환 음의 값이면 -1 반환
            //rigdbody.velocity = new Vector3(Mathf.Sign(rigdbody.velocity.x) * maxSpeed, rigdbody.velocity.y, rigdbody.velocity.z);
        }
        //horizontalMove = Input.GetAxisRaw("Horizontal");   // Horizontal = 왼쪽, 오른쪽 방향키
        //verticalMove = Input.GetAxisRaw("Vertical");     // Vertical = 위, 아래 방향키


        

        /*var bindings = playerInputActions.Player.Attack.bindings;

        print(bindings[0].effectivePath);*/

        //if (playerInputActions.Player.Attack.triggered)
        //print("공격버튼 클릭됨");

        //PressedAttackButton = playerInputActions.Player.Attack.;
    }

    void LateUpdate()
    {
        PlayerMovement();
    }

    void AnimationUpdate()
    {
        if (horizontalMove > minMovementDetectionValue || verticalMove > minMovementDetectionValue
            || horizontalMove < -minMovementDetectionValue || verticalMove < -minMovementDetectionValue)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        if (isRunning && (isMoving) && playerstat.playerCurrentStamina > 0 && staminaRunout == false && inCombat == false)
        {
            //animator.SetBool("isDashing", true);
            animator.SetFloat("WalkSpeed", 1.3f);
        }
        else
        {
            //animator.SetBool("isDashing", false);
            animator.SetFloat("WalkSpeed", 1f);
        }

    }

    void PlayerMovement()
    {
        if (!LockRotate())
            Gap.y += padRotate.x * Time.deltaTime * 10 * UIManager.instance.settingMenu.mouseSensitiveSlider.value;
        //Gap.y += Input.GetAxis("Mouse X") * Time.deltaTime * 150 * UIManager.instance.settingMenu.mouseSensitiveSlider.value;

        //Debug.LogError(Gap.y);

        //if (Gap.y >= 360 || Gap.y <= -360) // 각도가 360또는 - 360을 초과하지 않게하는 코드
            //Gap.y = 0;

        playerCollider.height = animator.GetFloat("Collider");  //닷지 애니메이션에 따른 플레이어 캡슐콜라이더의 크기조절

        RotF = Quaternion.Euler(0, Gap.y, 0);               // 각 조작키에 따른 회전 각도
        RotR = Quaternion.Euler(0, Gap.y + 90, 0);
        RotL = Quaternion.Euler(0, Gap.y + -90, 0);
        RotB = Quaternion.Euler(0, Gap.y + -180, 0);

        RotFR = Quaternion.Euler(0, Gap.y + 45, 0);
        RotFL = Quaternion.Euler(0, Gap.y + -45, 0);
        RotBR = Quaternion.Euler(0, Gap.y + 135, 0);
        RotBL = Quaternion.Euler(0, Gap.y + -135, 0);

        if ((inCombat == true || nextAttack == true || isAttacking.GetBoolValue() == true) && isDodge == false)
        {
            AttackRotation();
        }
        else if ((animator.GetBool("isAttack") == false && isAttacking.GetBoolValue() == false) || isDodge == true)
        {
            NotAttackRotation();
        }

        if (isRunning && isMoving)      //달리기
        {
            //currentDashSpeed = DashSpeed;
            playerstat.DecreaseStamina(10f * Time.deltaTime);
        }
        else
        {
            //currentDashSpeed = 1;

            if (playerstat.playerCurrentStamina <= playerstat.MaxStamina.GetFinalStatValueAsMultiflyFloat())
                playerstat.RegenStamina(playerstat.SteminaRecoveryRate.GetFinalStatValueAsMultiflyFloat() * Time.deltaTime);
        }

        if(SneakGague > 15)
        {
            moveSpeedPenalty = .7f;
        }
        else if(inCombat)
        {
            moveSpeedPenalty = .5f;
        }
        else
        {
            moveSpeedPenalty = 1;
        }

        if (isMoving && !playerstat.isStun.GetBoolValue() && !DungeonGenerator.instance.GetDungeonGenerating())     // 어느 방향키를 누르던지 앞쪽으로 이동하게 함, 벽과의 충돌때문에 update가 아닌 fixedUpdate에 넣음
        {
            if ((isAttacking.GetBoolValue() == true || inCombat == true || nextAttack == true) && !isDodge)       //공격중일때 바라보는 방향 기준으로 키보드 방향 이동
            {
                if ((inCombat == true && isDodge == false) || animator.GetBool("Block"))
                {
                    Vector3 moveDir = new Vector3(horizontalMove, 0, verticalMove);
                    //tr.Translate(moveDir.normalized * Time.deltaTime * (moveSpeed + dashspeed - (moveSpeed / 1.6f)), Space.Self);

                    addforce(moveDir,0.5f);
                }
                else
                {
                    Vector3 moveDir = new Vector3(horizontalMove, 0, verticalMove);
                    //tr.Translate(moveDir.normalized * Time.deltaTime * (moveSpeed + dashspeed), Space.Self);

                    addforce(moveDir,1);
                }
            }
            else    // 공격중이 아닐때 앞으로만 이동하게
            {
                if ((inCombat == true && isDodge == false) || animator.GetBool("Block"))
                {
                    Vector3 moveDir = (Vector3.forward * 1);
                    //tr.Translate(moveDir.normalized * Time.deltaTime * (moveSpeed + dashspeed - (moveSpeed / 1.6f)), Space.Self);

                    addforce(moveDir,0.5f);
                }
                else
                {
                    Vector3 moveDir = (Vector3.forward * 1);
                    //tr.Translate(moveDir.normalized * Time.deltaTime * (moveSpeed + dashspeed), Space.Self);

                    addforce(moveDir,1);
                }
            }
        }

        if (animator.GetBool("Charge") == true)
        {

            Vector3 moveDir = new Vector3((target.transform.position - gameObject.transform.position).x, 0, (target.transform.position - gameObject.transform.position).z);

            tr.Translate(moveDir.normalized * Time.deltaTime * (30), Space.World);
        }

        if (playerstat.playerCurrentStamina < 0.5f)     //스테미나
        {
            staminaRunout = true;
        }
        if (playerstat.playerCurrentStamina > 25f && staminaRunout == true)
        {
            staminaRunout = false;
        }

    }

    void addforce(Vector3 moveDir,float animationSpeed)
    {
        //tr.Translate(moveDir.normalized * ((moveSpeed * moveSpeedPenalty) + currentDashSpeed) * Time.deltaTime, Space.Self);
        //rigdbody.velocity = (transform.TransformDirection(moveDir.normalized) * ((moveSpeed * moveSpeedPenalty) + dashspeed) * (Time.deltaTime * 45));

        float playerSpeed = playerstat.MoveSpeedStat.GetFinalStatValueAsMultiflyFloat();
        float runningSpeed = playerstat.RunningSpeedStat.GetFinalStatValueAsMultiflyFloat();
        float correctionSpeed = 0;

        if (playerSpeed != 0)
        {
            correctionSpeed = Mathf.Log(playerSpeed, 1.25f);
        }
        
        //Debug.LogError("additionalSpeed + " + correctionSpeed + " playerSpeed + " + playerSpeed);

        transform.position += (transform.TransformDirection(moveDir) * correctionSpeed * runningSpeed * moveSpeedPenalty) * (Time.deltaTime);
        //rigdbody.AddForce((transform.TransformDirection(moveDir.normalized) * ((moveSpeed * moveSpeedPenalty * playerSpeed) + currentDashSpeed) * (Time.deltaTime * 45)));
        animator.SetFloat("Animation_Speed", animationSpeed);
    }


    void PlayerAttack(Skill skill)
    {
        if (UIManager.instance.IsAnyUiOn())
        {
            if (!UIManager.instance.skillParent)
            {
                SkillUI.instance.Skill.SetActive(!SkillUI.instance.Skill.activeSelf);
                SkillUI.instance.Skill.SetActive(!SkillUI.instance.Skill.activeSelf);
            }

            if (PressedAttackButton && FirstShackleReady)
            {
                StartCoroutine(FirstShackleFire());
            }
            else if (PressedAttackButton && SecondShackleReady)
            {
                StartCoroutine(SecondShackleFire());
            }
            else if (animator.GetLayerWeight(animator.GetLayerIndex("Warrior")) == 1)    //Warrior가 활성화된 경우
            {
                if(EquipmentManager.instance.currentEquipment[3])
                {
                    if (EquipmentManager.instance.currentEquipment[3].weaponType == WeaponType.Sword && PressedAttackButton && !playerskill.usingSkill.GetBoolValue())
                    {
                        if (nextAttack == false && isAttacking.GetBoolValue() == false) //!animator.GetCurrentAnimatorStateInfo 이건 애니메이션이 실행중인지 확인하는 스크립트)
                        {
                            StartCoroutine(Attack01());
                            StartCoroutine(AttackSoundPlay());
                            Debug.Log("Sword 공격 1");

                        }
                        else if (nextAttack == true)
                        {
                            //Debug.Log("어택2 실행");
                            StartCoroutine(Attack01_2());
                            StartCoroutine(AttackSoundPlay());
                            Debug.Log("Sword 공격 2");
                        }
                    }
                }

                if (EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon])
                {
                    if (EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon].secondaryWeaponType == SecondaryWeaponType.Shield)
                    {
                        if (PressedShieldButton && isAttacking.GetBoolValue() == false && playerskill.CheckPlayerHaveSkill(PrefabCollect.instance.BlockSkill)
                                        && playerstat.PlayerCurrentShieldPower > (playerstat.playerMaxShieldPower.GetFinalStatValueAsMultiflyFloat() * .35f) && !isDodge)
                        {
                            StartCoroutine(Block());
                        }
                    }
                }
            }
            else if (animator.GetLayerWeight(animator.GetLayerIndex("Rogue")) == 1)    //Rogue 활성화된 경우
            {
                if(EquipmentManager.instance.currentEquipment[3])
                {
                    if (EquipmentManager.instance.currentEquipment[3].weaponType == WeaponType.Dagger && PressedAttackButton && !playerskill.usingSkill.GetBoolValue())
                    {
                        if (nextAttack == false && isAttacking.GetBoolValue() == false) //!animator.GetCurrentAnimatorStateInfo 이건 애니메이션이 실행중인지 확인하는 스크립트)
                        {
                            StartCoroutine(Dagger_Attack01());
                            StartCoroutine(AttackSoundPlay());
                        }
                        else if (nextAttack == true)
                        {
                            StartCoroutine(Dagger_Attack02());
                            StartCoroutine(AttackSoundPlay());
                        }
                    }
                }
                

                /*if (PressedShieldButton)
                {
                    if (EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon].secondaryWeaponType == SecondaryWeaponType.HandCrossBow)
                    {
                        if(PressedShieldButton && isAttacking.GetBoolValue() == false && handBowReady)
                        {
                            print("핸드 크로스보우 발사!!");
                            animator.SetTrigger("FireHandCorssBow");
                        }
                    }
                }*/
            }
            else if (animator.GetLayerWeight(animator.GetLayerIndex("Wizard")) == 1)    //Wizard 활성화된 경우
            {
                if(EquipmentManager.instance.currentEquipment[3])
                {
                    if (EquipmentManager.instance.currentEquipment[3].weaponType == WeaponType.Wand && !playerskill.usingSkill.GetBoolValue())
                    {
                        if (isAttacking.GetBoolValue() == false && PressedAttackButton)
                        {
                            StartCoroutine(Wand_Basic_Attack());
                        }

                        if (PressedShieldButton)
                        {
                            if (playerstat.playerCurrentMp > 5 && !animator.GetBool("isFlame") && !isDodge)
                            {
                                StartCoroutine(Flame());
                            }
                            else if ((playerstat.playerCurrentMp < 5 && animator.GetBool("isFlame")))
                            {
                                StartCoroutine(FlamesOff());
                            }
                        }
                    }
                }
                
            }
            else if (animator.GetLayerWeight(animator.GetLayerIndex("Archer")) == 1)    //Archer 활성화된 경우
            {
                if (EquipmentManager.instance.currentEquipment[3])
                {
                    if (EquipmentManager.instance.currentEquipment[3].weaponType == WeaponType.Bow && !playerskill.usingSkill.GetBoolValue())
                    {
                        if (isAttacking.GetBoolValue() == false && PressedAttackButton) //!animator.GetCurrentAnimatorStateInfo 이건 애니메이션이 실행중인지 확인하는 스크립트)
                        {
                            StartCoroutine(Bow_Attack01());
                            //StartCoroutine(BowAttackSoundPlay());
                        }
                        else if (PressedShieldButton && !isDodge)
                        {
                            StartCoroutine(Bow_Charging());
                        }
                    }
                }
                
            }
            /*else if (animator.GetLayerWeight(1) == 1)
            {
                if (PressedAttackButton && nextAttack == false && isArrowCharging == false && isAttacking.GetBoolValue() == false) //!animator.GetCurrentAnimatorStateInfo 이건 애니메이션이 실행중인지 확인하는 스크립트)
                {
                    if (EquipmentManager.instance.currentEquipment[3].weaponType == WeaponType.Bow
                        && isAttacking.GetBoolValue() == false)     //활로 공격
                    {
                        StartCoroutine(Bow_Attack01());
                        //StartCoroutine(BowAttackSoundPlay());
                    }
                    else if (EquipmentManager.instance.currentEquipment[3].weaponType == WeaponType.Sword)        //칼 근접 공격
                    {
                        StartCoroutine(Attack01());
                        StartCoroutine(AttackSoundPlay());
                        Debug.Log("공격 1");
                    }
                    else if (EquipmentManager.instance.currentEquipment[3].weaponType == WeaponType.Dagger)
                    {
                        StartCoroutine(Dagger_Attack01());
                        StartCoroutine(AttackSoundPlay());
                    }
                }
                else if (PressedAttackButton && nextAttack == true && animator.GetLayerWeight(1) == 1)
                {
                    //Debug.Log("어택2 실행");
                    StartCoroutine(Attack01_2());
                    StartCoroutine(AttackSoundPlay());
                    Debug.Log("공격 2");
                }
                else if (EquipmentManager.instance.currentEquipment[(int)ItemType.Weapon] != null)
                {
                    if (EquipmentManager.instance.currentEquipment[(int)ItemType.Weapon].weaponType == WeaponType.Sword)
                    {
                        if (EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon] != null)
                        {
                            if (EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon].weaponType == WeaponType.None)
                            {
                                if (PressedShieldButton && isAttacking.GetBoolValue() == false && playerskill.skillList.ContainsKey(SkillManager.instance.ShieldBlock)
                                    && playerstat.PlayerCurrentShieldPower > (playerstat.playerMaxShieldPower.GetFinalStatValueAsMultiflyFloat() * .35f))
                                {
                                    StartCoroutine(Block());
                                }
                            }
                            else if (PressedShieldButton && isAttacking.GetBoolValue() == false)    // 한손검 특수공격
                            {
                                if (playerstat.playerCurrentMp >= 15)
                                {
                                    StartCoroutine(SwordSpecialAttack());
                                    StartCoroutine(AttackSoundPlay());
                                }
                            }
                        }
                    }
                    else if (EquipmentManager.instance.currentEquipment[(int)ItemType.Weapon].weaponType == WeaponType.Bow)  // 활 특수공격
                    {
                        if (PressedShieldButton && playerskill.skillList.ContainsKey(SkillManager.instance.ChargingShot))
                        {
                            StartCoroutine(Bow_Charging());
                        }
                    }
                }
            }
            else if (animator.GetLayerWeight(2) == 1)
            {
                if (EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon].weaponType == WeaponType.None)
                {
                    if (PressedShieldButton && isAttacking.GetBoolValue() == false && playerstat.PlayerCurrentShieldPower > (playerstat.playerMaxShieldPower.GetFinalStatValueAsMultiflyFloat() * .35f))
                    {
                        StartCoroutine(Block());
                    }
                }
            }
            else if (animator.GetLayerWeight(3) == 1)
            {
                if (PressedAttackButton)
                {
                    if (animator.GetBool("isDaggerAttack") == false && nextAttack == true
                                    && !animator.GetCurrentAnimatorStateInfo(1).IsName("Dagger_Left_Attack01"))
                    {
                        StartCoroutine(Dagger_Attack02());
                        StartCoroutine(AttackSoundPlay());
                    }
                    else if (animator.GetBool("isDaggerAttack") == false
                  && !animator.GetCurrentAnimatorStateInfo(1).IsName("Dagger_Right_Attack01") && nextAttack == false && isAttacking.GetBoolValue() == false)
                    {
                        StartCoroutine(Dagger_Attack01());
                        StartCoroutine(AttackSoundPlay());
                    }
                    else if (bombReady == true && animator.GetBool("Bomb_Ready") == true)
                    {
                        StartCoroutine(BombThrow());
                    }
                    else if (PoisonbombReady == true && animator.GetBool("Bomb_Ready") == true)
                    {
                        StartCoroutine(PoisonBombThrow());
                    }
                }
                else if (PressedShieldButton && animator.GetBool("isAttack") == false)
                {
                    if (EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon].secondaryWeaponType == SecondaryWeaponType.HandCrossBow && handBowReady)
                    {
                        print("핸드 크로스보우 발사!!");
                        animator.SetTrigger("FireHandCorssBow");
                    }
                }
            }
            else if (animator.GetLayerWeight(4) == 1)
            {
                if (playerstat.playerClass == CharacterClass.Necromancer)
                {
                    if (PressedAttackButton && isAttacking.GetBoolValue() == false && isArrowCharging == false)
                    {
                        StartCoroutine(SoulFire());
                    }
                    else if (PressedShieldButton && playerskill.skillList.ContainsKey(SkillManager.instance.SoulRecovery))
                    {
                        StartCoroutine(SoulDrain());
                    }

                }
                else if (playerstat.playerClass == CharacterClass.Wizard)
                {
                    if (PressedAttackButton && isAttacking.GetBoolValue() == false && isArrowCharging == false)
                    {
                        if (BouncingBallCount > 0)
                        {
                            StartCoroutine(FireBouncingBall(skill));
                        }
                        else
                        {
                            StartCoroutine(Wand_Basic_Attack());
                        }

                    }
                    else if (PressedShieldButton)
                    {
                        if (playerstat.playerCurrentMp > 5 && !animator.GetBool("isFlame") && !isDodge)
                        {
                            StartCoroutine(Flame());
                        }
                        else if ((playerstat.playerCurrentMp < 5 && animator.GetBool("isFlame")) || isDodge)
                        {
                            StartCoroutine(FlamesOff());
                        }

                    }
                    *//*else if (PressedShieldButton && isAttacking.GetBoolValue() == false && magicOrbs.Count < 5 && playerstat.playerCurrentMp >= 25)
                    {
                        StartCoroutine(MagicOrb());
                        playerstat.UseMana(25);
                    }*/
                    /*else if (Input.GetButton("Fire2") && isAttacking == false)
                    {
                        StartCoroutine(Wand_Attack_Charge());
                    }
                    else if (Input.GetButtonUp("Fire2") && ChargeCount > 0)
                    {
                        StartCoroutine(Wand_Attack_Fire());
                    }*//*
                }
            }
            else if (PressedAttackButton && animator.GetLayerWeight(3) == 1 && animator.GetBool("isDaggerAttack") == false && nextAttack == true
                && !animator.GetCurrentAnimatorStateInfo(1).IsName("Dagger_Left_Attack01"))
            {
                StartCoroutine(Dagger_Attack02());
                StartCoroutine(AttackSoundPlay());
            }
            else if (PressedAttackButton && animator.GetLayerWeight(3) == 1 && animator.GetBool("isDaggerAttack") == false
                && !animator.GetCurrentAnimatorStateInfo(1).IsName("Dagger_Right_Attack01") && nextAttack == false && isAttacking.GetBoolValue() == false)
            {
                StartCoroutine(Dagger_Attack01());
                StartCoroutine(AttackSoundPlay());
            }*/
            else
            {
                if (PressedAttackButton && isAttacking.GetBoolValue() == false && isArrowCharging == false)
                {
                    StartCoroutine(Punch());
                }
            }
        }
    }
    private void Attack_started(InputAction.CallbackContext obj)
    {
        PressedAttackButton = true;
    }

    private void Attack_canceled(InputAction.CallbackContext obj)
    {
        PressedAttackButton = false;
    }

    private void Shield_started(InputAction.CallbackContext context)
    {
        PressedShieldButton = true;
    }

    private void Shield_canceled(InputAction.CallbackContext context)
    {
        PressedShieldButton = false;

        if (isBlocking)
        {
            StartCoroutine(StopBlock());
        }

        if (ChargeCount > 0)
        {
            StartCoroutine(Bow_ChargingShot());
        }

        if (SoulDraining)
        {
            StartCoroutine(SoulDrainOff());
        }

        if(animator.GetBool("isFlame"))
        {
            StartCoroutine(FlamesOff());
        }
    }

    private void Move_performed(InputAction.CallbackContext context)
    {

    }

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed && grounded && !playerstat.isStun.GetBoolValue() && !DungeonGenerator.instance.GetDungeonGenerating())
        {
            print("플레이어 점프" + context.phase);
            rigdbody.velocity = Vector3.up * jumpPower;
        }
    }

    private void Sneak_started(InputAction.CallbackContext obj)
    {
        isSneaking = true;

        mat = GetComponentInChildren<SkinnedMeshRenderer>().materials;
    }

    private void Sneak_canceled(InputAction.CallbackContext obj)
    {
        isSneaking = false;
    }

    private void Running_performed(InputAction.CallbackContext obj)
    {
        isRunning = true;
        playerstat.RunningSpeedStat.AddPercentModifier(.2f);
    }

    private void Running_canceled(InputAction.CallbackContext obj)
    {
        isRunning = false;
        playerstat.RunningSpeedStat.RemovePercentModifier(.2f);
    }

    private void Roll_started(InputAction.CallbackContext obj)
    {
        if (playerstat.playerCurrentStamina > playerstat.RollRequireStamina.GetFinalStatValueAsMultiflyFloat() 
            && grounded && !playerstat.isStun.GetBoolValue() && !isDodge && !DungeonGenerator.instance.GetDungeonGenerating())     // 닷지 관련 함수
        {

            //Debug.LogError("1");
            animator.SetTrigger("Dodge");
            StartCoroutine(Roll());

            AudioManager.instance.PlaySFXWithAudioSource("roll", playerstat.GetAudioSource());
        }   
    }

    #region 무착용 공격관련

    IEnumerator Punch()
    {
        animator.SetBool("Punch", true);
        isAttacking.AddBoolModifier();
        ResetCombatTime(keepCombatTime);
        AudioSource.PlayOneShot(SoundManager.instance.PunchSound);

        yield return new WaitForSeconds(AnimationLength("Punch_Right") / playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat() * .8f);
        animator.SetBool("Punch", false);
        isAttacking.RemoveBoolModifier();
    }

    #endregion

    #region 전사 공격관련
    IEnumerator Attack01()
    {
        //animator.SetBool("isAttack", true);
        animator.SetTrigger("IsSwordAttack");
        //isAttacking.AddBoolModifier();
        ResetCombatTime(keepCombatTime);
        //rightSwordColider.enabled = true;
        //frontAttackCollider.enabled = true;
        //swordTrail.Emit = true;

        yield return new WaitForSeconds(AnimationLength("Sword_Atk02") /playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat());
        //rightSwordColider.enabled = false;
        //frontAttackCollider.enabled = false;
        //animator.SetBool("isAttack", false);
        //isAttacking.RemoveBoolModifier();
        //nextAttack = true;
        //rightSwordColider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
        //frontAttackCollider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
        //swordTrail.Emit = false;
        yield return new WaitForSeconds(0.8f);
        //nextAttack = false;

    }

    IEnumerator Attack01_2()
    {
        //animator.SetBool("2rdAttack", true);
        animator.SetTrigger("IsSword2rdAttack");
        //isAttacking.AddBoolModifier();
        ResetCombatTime(keepCombatTime);
        nextAttack = false;
        //StopCoroutine(nextAttackReady());
        //rightSwordColider.enabled = true;
        //frontAttackCollider.enabled = true;
        //swordTrail.Emit = true;

        yield return new WaitForSeconds(AnimationLength("Sword_Atk02-2") / playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat() * 1.5f);

        //rightSwordColider.enabled = false;
        //frontAttackCollider.enabled = false;
        //animator.SetBool("2rdAttack", false);
        //isAttacking.RemoveBoolModifier();
        //rightSwordColider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
        //frontAttackCollider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
        //swordTrail.Emit = false;
    }

    IEnumerator SwordSpecialAttack()
    {
        animator.SetBool("SwordSpecialAttack01", true);
        isAttacking.AddBoolModifier();
        ResetCombatTime(keepCombatTime);
        rightSwordColider.enabled = true;
        rightSwordTrail.Emit = true;
        playerstat.playerCurrentMp -= 15;

        yield return new WaitForSeconds(AnimationLength("Sword_Special_Atk01") / playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat() * 1f);

        isAttacking.RemoveBoolModifier();
        rightSwordColider.enabled = false;
        rightSwordTrail.Emit = false;
        animator.SetBool("SwordSpecialAttack01", false);

        rightSwordColider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
    }

    IEnumerator Block()
    {
        //ResetCombatTime(keepCombatTime);
        if (isBlocking == false)
        {
            animator.SetBool("Block", true);
        }
        //isAttacking.AddBoolModifier();
        isBlocking = true;

        yield return null;
    }

    public IEnumerator StopBlock()
    {
        animator.SetBool("Block", false);
        //isAttacking.RemoveBoolModifier();
        isBlocking = false;

        yield return null;
    }

    IEnumerator ShieldBash(Skill skill)
    {
        //animator.SetBool("Shield_Bash", true);
        animator.SetTrigger("Shield_Bash");
        //isAttacking.AddBoolModifier();
        ResetCombatTime(keepCombatTime);
        //swordTrail.Emit = true;
        //frontAttackCollider.enabled = true;
        //frontAttackLogic.inUseSkill = skill;
        playerstat.UseRage(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage);
        StartCoroutine(PlayShieldBashSound());

        yield return new WaitForSeconds(AnimationLength("Shield_Bash") / playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat() * 1f);

        //frontAttackCollider.enabled = false;
        //isAttacking.RemoveBoolModifier();
        //swordTrail.Emit = false;
        //animator.SetBool("Shield_Bash", false);
        //frontAttackLogic.inUseSkill = null;
        //frontAttackCollider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
    }

    IEnumerator Charge(Skill skill)
    {
        animator.SetBool("Charge", true);
        isAttacking.AddBoolModifier();
        ResetCombatTime(keepCombatTime);
        rightSwordTrail.Emit = true;
        frontAttackCollider.enabled = true;
        //frontAttackLogic.inUseSkill = skill;
        playerstat.UseRage(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage);

        yield return new WaitForSeconds(AnimationLength("Charge") / playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat());

        frontAttackCollider.enabled = false;
        isAttacking.RemoveBoolModifier();
        rightSwordTrail.Emit = false;
        animator.SetBool("Charge", false);
        //frontAttackLogic.inUseSkill = null;
        frontAttackCollider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
    }

    IEnumerator Smite(Skill skill)
    {
        animator.SetTrigger("Smite");
        ResetCombatTime(keepCombatTime);
        //frontAttackLogic.inUseSkill = skill;

        playerstat.UseRage(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage);

        SoundManager.instance.PlaySound_SwordSmite(AudioSource);

        yield return null;
    }

    IEnumerator SkullSmash(Skill skill)
    {
        animator.SetTrigger("SkullSmash");
        ResetCombatTime(keepCombatTime);
        //isAttacking.AddBoolModifier();
        //frontAttackLogic.inUseSkill = skill;

        playerstat.UseRage(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage);

        SoundManager.instance.PlaySound_SwordSmite(AudioSource);

        yield return null;
    }

    IEnumerator Maimed(Skill skill)
    {
        animator.SetTrigger("Maimed");
        ResetCombatTime(keepCombatTime);
        //isAttacking.AddBoolModifier();
        //frontAttackLogic.inUseSkill = skill;
        //swordTrail.Emit = true;
        //swordTrail._colors[0] = SwordTrailRedColor;

        playerstat.UseRage(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage);

        yield return null;
    }

    IEnumerator WhirlWind(Skill skill)
    {
        animator.SetBool("WhirlWind", true);
        //isAttacking.AddBoolModifier();
        //swordTrail.Emit = true;

        //frontAttackCollider.enabled = true;
        //frontAttackLogic.inUseSkill = skill;
        playerstat.UseRage(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage);
        //playerstat.playerCurrentRage -= skill.needRage;
        //ResetCombatTime(keepCombatTime);

        yield return new WaitForSeconds(5f);

        animator.SetBool("WhirlWind", false);
        //isAttacking.RemoveBoolModifier();
        //swordTrail.Emit = false;
        //frontAttackCollider.enabled = false;
        //frontAttackLogic.inUseSkill = null;
    }

    IEnumerator CoupDeGrace(Skill skill)
    {
        ResetCombatTime(keepCombatTime);
        animator.SetTrigger("CoupDeGrace");
        //isAttacking.AddBoolModifier();
        //rightSwordTrail.Emit = true;
        //frontAttackLogic.inUseSkill = skill;

        yield return null;
    }

    public void CoupDeGraceEffect()
    {
        int damage = Random.Range(Mathf.RoundToInt(playerstat.minDamage.GetFinalStatValueAsMultiflyFloat()), Mathf.RoundToInt(playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat()));

        damage = Mathf.RoundToInt(damage * PrefabCollect.instance.CoupDeGrace.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.CoupDeGrace)].damageFactor 
            * (1 + (playerstat.playerCurrentRage / 100)));

        NPC_Type type = NPC_Type.friendly;

        AttackEffectFunctions.explode(3.5f, damage, rightSwordColider.bounds.center, PrefabCollect.instance.ExplodeParticle,type,gameObject);
    }

    IEnumerator SuperArmor(Skill skill)
    {
        animator.SetTrigger("SuperArmor");

        playerstat.UseRage(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage);

        int armor =  PrefabCollect.instance.superArmor.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.superArmor)].value1;

        playerstat.armor.AddIntModifier(armor);
        var Aura = ParticleGenerator.instance.GenerateGroundParticle(transform.position, "Aura01", 10f);
        Aura.transform.SetParent(transform);

        GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(PrefabCollect.instance.StunImmunity);
        GetComponent<CharacterBuffDeBuff>().AddBuffOrDebuff(PrefabCollect.instance.SlowdownImmunity);

        yield return new WaitForSeconds(PrefabCollect.instance.superArmor.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.superArmor)].value2);

        playerstat.armor.RemoveIntModifier(armor);
    }

    public void ConSumeAllRage()
    {
        playerstat.playerCurrentRage = 0;
    }

    IEnumerator ShieldShockWave(Skill skill)
    {
        ResetCombatTime(keepCombatTime);
        animator.SetTrigger("ShieldShockWave");

        playerstat.UseRage(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage);

        yield return null;
    }

    public void ShieldShockWaveParticle()
    {
        var particle = Instantiate(PrefabCollect.instance.ShieldShockWaveParticle, firePos.position, Quaternion.identity);

        AudioManager.instance.GenerateAudioAndPlaySFX("shieldBash", firePos.position);

        Destroy(particle, 4);

        particle.transform.LookAt(target);
        particle.GetComponent<ParticleSystem>().Play();

        var flameTick = Instantiate(PrefabCollect.instance.ShieldShockWaveTick, FlameFirePos.position, new Quaternion(0, 0, 0, 0));

        var skill = PrefabCollect.instance.ShieldShockWave.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.ShieldShockWave)];

        Destroy(flameTick, 0.5f);

        float damagemulfly = skill.damageFactor;

        flameTick.GetComponentInChildren<FlameTick>().Set(gameObject, Mathf.RoundToInt(playerstat.minDamage.GetFinalStatValueAsMultiflyFloat() * damagemulfly),
            Mathf.RoundToInt(playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat() * damagemulfly), FlameTarget);

        //flameTick.transform.LookAt(target);

        flameTick.GetComponentInChildren<Rigidbody>().velocity = AttackEffectFunctions.GetDirection(target.position, flameTick.transform.position) * 15;

        flameTick.GetComponentInChildren<FlameTick>().debuff.Add(skill.buffNDebuffObject);
        flameTick.GetComponentInChildren<FlameTick>().debuff.Add(skill.buffNDebuffObject2);
    }


    #endregion

    #region 궁수 공격관련
    IEnumerator Bow_Attack01()
    {
        ResetCombatTime(keepCombatTime);
        animator.SetTrigger("isBowAttack");

        //StartCoroutine(ArrowFire());

        //yield return new WaitForSeconds(playerstat.bow_BaseAttackSpeed / playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat());
        yield return null;
    }

    IEnumerator Bow_Charging()
    {
        ResetCombatTime(keepCombatTime);
        Debug.Log("화살 공격 차징");

        if (isArrowCharging == false)
        {
            animator.SetBool("isBowCharging", true);
            bowChargingParticle.Play();
            AudioManager.instance.PlaySFXWithAudioSource("bowDrawing1", AudioSource);
        }
        //isAttacking.AddBoolModifier();
        isArrowCharging = true;

        if (isArrowCharging)
        {
            ChargeCount += Time.deltaTime * 10;
            ChargeCount = Mathf.Clamp(ChargeCount, minChargeCount, maxChargeCount);
        }

        

        yield return null;

    }

    IEnumerator Bow_ChargingShot()
    {

        animator.SetBool("isBowCharging", false);
        isArrowCharging = false;

        bowChargingParticle.Stop();

        if (ChargeCount > 15)
        {
            GameObject Arrow = Instantiate(PrefabCollect.instance.Chargingarrow, firePos.position, transform.rotation);
            var scripts = Arrow.GetComponent<Arrow_Logic>();
            scripts.getOwner(gameObject);
            scripts.Fire(target.position);

            float damageDivid =  PrefabCollect.instance.ChargingShotSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.ChargingShotSkill)].value1;

            float DamageMultifly = (ChargeCount) / damageDivid;
            DamageMultifly = Mathf.Clamp(DamageMultifly, 1, 3);

            scripts.DamageMultifly = DamageMultifly;

            AudioManager.instance.PlaySFXWithAudioSource("bowFire1", AudioSource);

            //StartCoroutine(BowAttackSoundPlay());
        }
        else
        {
            if(AudioSource.clip.name == "DrawingBow1")
                AudioSource.Stop();
        }

        ChargeCount = 0;
        isAttacking.AddBoolModifier();

        yield return new WaitForSeconds(playerstat.bow_BaseAttackSpeed / playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat());

        isAttacking.RemoveBoolModifier();
    
    }

    public void arrowFire()
    {
        var arrow = ArrowFire(1, 0);
        StartCoroutine(GetComponent<PlayerEnchant>().ArrowFireEnchantEffect(arrow));
    }

    public void continuosArrowFire()
    {
        var arrow = ArrowFire(1, 0);
        StartCoroutine(GetComponent<PlayerEnchant>().ArrowFireEnchantEffect(arrow,applyDoubleShot:false));;
    }

    public GameObject ArrowFire(int arrowAmount, int angle)
    {
        int Angel = 0;
        if (arrowAmount > 1)
            Angel = angle / (arrowAmount - 1);

        GameObject arr = null;

        for (int i = 0; i < arrowAmount; i++)
        {
            arr = Instantiate(PrefabCollect.instance.arrow, firePos.position, transform.rotation);

            if (arrowList.Count > 0)
            {
                arr.GetComponent<Arrow_Logic>().arrowType = arrowList[0].arrowType;
                RemoveArrow();
            }

            arr.GetComponent<Arrow_Logic>().getOwner(gameObject);
            arr.GetComponent<Arrow_Logic>().Fire(target.position, (Angel * (i)) - (angle / 2));
            //GetComponent<PlayerEnchant>().FiredArrowEnchantEffect(arr.GetComponent<Arrow_Logic>());

            //StartCoroutine(BowAttackSoundPlay());
            AudioManager.instance.GenerateAudioAndPlaySFX("bowFire1", firePos.position);
        }

        return arr;
    }

    IEnumerator DoubleShot()
    {
        ResetCombatTime(keepCombatTime);

        animator.SetTrigger("DoubleShoot");

        /*animator.SetTrigger("isBowAttack");
        StartCoroutine(GetComponent<PlayerEnchant>().ArrowFireEnchantEffect());
        yield return new WaitForSeconds(.1f);
        animator.SetTrigger("isBowAttack");
        StartCoroutine(GetComponent<PlayerEnchant>().ArrowFireEnchantEffect());*/

        /*GameObject Arrow = Instantiate(arrow, firePos.position + firePos.right * .4f, transform.rotation);
        GameObject Arrow2 = Instantiate(arrow, firePos.position + firePos.right * -.4f, transform.rotation);
        Arrow.GetComponent<Arrow_Logic>().getOwner(gameObject);
        Arrow2.GetComponent<Arrow_Logic>().getOwner(gameObject);*/
        yield return null;

    }

    public void MultiShot()
    {
        int arrowAmount = PrefabCollect.instance.MultiShot.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.MultiShot)].value1;
        int angle = PrefabCollect.instance.MultiShot.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.MultiShot)].value2;
        ArrowFire(arrowAmount, angle);
    }

    IEnumerator ContinuousFiring()
    {
        ResetCombatTime(keepCombatTime);

        animator.SetTrigger("ContinuousFiring");

        yield return null;

        /*isAttacking.AddBoolModifier();

        for (int i = 0; i < 5; i++)
        {
            animator.SetTrigger("isBowAttack");
            StartCoroutine(GetComponent<PlayerEnchant>().ArrowFireEnchantEffect());
            *//*GameObject Arrow = Instantiate(arrow, firePos.position + firePos.right * .4f, transform.rotation);
            Arrow.GetComponent<Arrow_Logic>().getOwner(gameObject);*//*
            //StartCoroutine(BowAttackSoundPlay());
            yield return new WaitForSeconds(.1f);
        }

        isAttacking.RemoveBoolModifier();*/
    }

    public void StartContinuousFiring()
    {
        StartCoroutine(StartContinuousFiringCoroutine());
    }

    public IEnumerator StartContinuousFiringCoroutine()
    {
        var fireAmount = PrefabCollect.instance.continuousFiring.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.continuousFiring)].value1;

        for (int i = 0; i < fireAmount; i++)
        {
            continuosArrowFire();

            if (playerskill.usingSkillObject != PrefabCollect.instance.continuousFiring)
                break;

            yield return new WaitForSeconds(0.15f / playerstat.AttackSpeed.GetFinalStatValue());
        }
    }

    public void AddArrow(Arrow arrow)
    {
        arrowList.Add(arrow);

        switch(arrow.arrowType)
        {
            case ArrowType.Steel:
                var arrowCell = Instantiate(PrefabCollect.instance.SteelArrowCell, UIManager.instance.ArcherArrowLayout);
                arrowCellList.Add(arrowCell);
                break;

            case ArrowType.Poison:
                arrowCell =  Instantiate(PrefabCollect.instance.PoisonArrowCell, UIManager.instance.ArcherArrowLayout);
                arrowCellList.Add(arrowCell);
                break;

            case ArrowType.Explosion:
                arrowCell =  Instantiate(PrefabCollect.instance.ExplosionArrowCell, UIManager.instance.ArcherArrowLayout);
                arrowCellList.Add(arrowCell);
                break;
        }
    }

    public void RemoveArrow()
    {
        arrowList.RemoveAt(0);
        Destroy(arrowCellList[0]);
        arrowCellList.RemoveAt(0);
    }

    IEnumerator SteelArrow()
    {
        Arrow arrow = new Arrow();
        arrow.arrowType = ArrowType.Steel;

        int arrowAmount = PrefabCollect.instance.SteelArrowSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.SteelArrowSkill)].value1;

        int sameArrowinList = 0;
        foreach (Arrow arrow1 in arrowList)
        {
            if (arrow1.arrowType == ArrowType.Steel)
                sameArrowinList++;
        }

        arrowAmount -= sameArrowinList;

        for (int i = 0; i < arrowAmount; i++)
        {
            AddArrow(arrow);
        }

        AudioManager.instance.PlaySFXWithAudioSource("trap1", AudioSource);

        yield return null;
    }

    IEnumerator PoisonArrow()
    {
        Arrow arrow = new Arrow();
        arrow.arrowType = ArrowType.Poison;

        int arrowAmount = PrefabCollect.instance.PoisonArrowSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.PoisonArrowSkill)].value1;

        int sameArrowinList = 0;
        foreach (Arrow arrow1 in arrowList)
        {
            if (arrow1.arrowType == ArrowType.Poison)
                sameArrowinList++;

        }

        arrowAmount -= sameArrowinList;

        for (int i = 0; i < arrowAmount; i++)
        {
            AddArrow(arrow);
        }

        AudioManager.instance.PlaySFXWithAudioSource("trap1", AudioSource);

        yield return null;
    }

    IEnumerator ExplosionArrow()
    {
        Arrow arrow = new Arrow();
        arrow.arrowType = ArrowType.Explosion;

        int arrowAmount = PrefabCollect.instance.ExplosionArrowSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.ExplosionArrowSkill)].value1;

        int sameArrowinList = 0;
        foreach (Arrow arrow1 in arrowList)
        {
            if (arrow1.arrowType == ArrowType.Explosion)
                sameArrowinList++;

        }

        arrowAmount -= sameArrowinList;

        for (int i = 0; i < arrowAmount; i++)
        {
            AddArrow(arrow);
        }

        AudioManager.instance.PlaySFXWithAudioSource("trap1", AudioSource);

        yield return null;
    }

    IEnumerator GrapplingArrow()
    {
        GameObject grapplingArrow = Instantiate(PrefabCollect.instance.GrapplingArrow, firePos.position, transform.rotation);
        var script = grapplingArrow.GetComponent<GrapplingArrow>();
        animator.SetTrigger("isBowAttack");
        script.player = gameObject;
        script.First = firePos.gameObject;
        script.playerRig = rigdbody;
        yield return null;
    }

    IEnumerator Kick()
    {
        animator.SetTrigger("Kick");
        ResetCombatTime(keepCombatTime);
        //isAttacking.AddBoolModifier();
        //frontAttackLogic.inUseSkill = SkillManager.instance.Kick;

        AudioManager.instance.GenerateAudioAndPlaySFX("throw1", firePos.position);

        yield return null;
    }

    IEnumerator BackStepShoot()
    {
        animator.SetTrigger("BackStepShoot");
        ResetCombatTime(keepCombatTime);
        //isAttacking.RemoveBoolModifier();

        yield return null;
    }


    #endregion

    #region 도적 공격관련
    IEnumerator Dagger_Attack01()
    {
        //animator.SetBool("isDaggerAttack", true);
        animator.SetTrigger("Dagger1stAttack");
        //isAttacking.AddBoolModifier();
        ResetCombatTime(keepCombatTime);
        //rightKnifeColider.enabled = true;
        //frontAttackCollider.enabled = true;
        //rightDaggerTrail.Emit = true;

        yield return new WaitForSeconds(AnimationLength("Dagger_Right_Attack01") / playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat() * 0.9f);

        //rightKnifeColider.enabled = false;
        //frontAttackCollider.enabled = false;
        //frontAttackCollider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
        //animator.SetBool("isDaggerAttack", false);
        //isAttacking.RemoveBoolModifier();
        //nextAttack = true;
        //rightDaggerColider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
        //rightDaggerTrail.Emit = false;

        yield return new WaitForSeconds(0.8f);
        //nextAttack = false;
    }

    IEnumerator Dagger_Attack02()
    {
        //animator.SetBool("isDaggerLeftAttack", true);
        animator.SetTrigger("Dagger2stAttack");
        //isAttacking.AddBoolModifier();
        ResetCombatTime(keepCombatTime);
        nextAttack = false;
        //frontAttackCollider.enabled = true;
        //leftKnifeColider.enabled = true;
        //rightDaggerTrail.Emit = true;

        yield return new WaitForSeconds(AnimationLength("Dagger_Left_Attack01") / playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat() * 0.9f);

        //leftKnifeColider.enabled = false;
        //frontAttackCollider.enabled = false;
        //frontAttackCollider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
        //animator.SetBool("isDaggerLeftAttack", false);
        //isAttacking.RemoveBoolModifier();
        //leftKnifeColider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
        //inCombat = true;
        //rightDaggerTrail.Emit = false;

        yield return new WaitForSeconds(0.8f);
        //inCombat = false;
    }

    IEnumerator HandBow_Fire()
    {
        ResetCombatTime(keepCombatTime);

        GameObject arr = Instantiate(PrefabCollect.instance.arrow, firePos.position, transform.rotation);

        if (arrowList.Count > 0)
        {
            arr.GetComponent<Arrow_Logic>().arrowType = arrowList[0].arrowType;
            arrowList.RemoveAt(0);
        }

        arr.GetComponent<Arrow_Logic>().getOwner(gameObject);
        //StartCoroutine(BowAttackSoundPlay());

        handBowReady = false;

        yield return new WaitForSeconds(2.0f);

        handBowReady = true;
    }

    IEnumerator Dagger_Spin(Skill skill)
    {
        //animator.SetBool("Dagger_Spin", true);
        animator.SetTrigger("DaggerSpin");
        isAttacking.AddBoolModifier();
        ResetCombatTime(keepCombatTime);
        leftKnifeColider.enabled = true;
        rightDaggerColider.enabled = true;
        leftKnifeTrail.Emit = true;
        rightDaggerTrail.Emit = true;
        //rightDaggerAttackLogic.inUseSkill = skill;
        //leftDaggerAttackLogic.inUseSkill = skill;


        yield return new WaitForSeconds(AnimationLength("Dagger_Spin") / playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat() * 0.9f);

        isAttacking.RemoveBoolModifier();
        leftKnifeColider.enabled = false;
        rightDaggerColider.enabled = false;
        leftKnifeTrail.Emit = false;
        rightDaggerTrail.Emit = false;
        //rightDaggerAttackLogic.inUseSkill = null;
        //leftDaggerAttackLogic.inUseSkill = null;

        //animator.SetBool("Dagger_Spin", false);

        rightDaggerColider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
        leftKnifeColider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
    }

    IEnumerator PoisonJab(Skill skill)
    {
        //animator.SetBool("Poison_Jab", true);
        animator.SetTrigger("PoisionJab");
        //isAttacking.AddBoolModifier();
        ResetCombatTime(keepCombatTime);
        //leftKnifeColider.enabled = true;
        //rightKnifeColider.enabled = true;
        //frontAttackCollider.enabled = true;
        //leftKnifeTrail.Emit = true;
        //rightDaggerTrail.Emit = true;
        //frontAttackLogic.inUseSkill = skill;
        //rightDaggerAttackLogic.inUseSkill = skill;
        //leftDaggerAttackLogic.inUseSkill = skill;
        playerstat.UseMana(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp);
        //playerstat.playerCurrentMp -= skill.needMp;

        yield return new WaitForSeconds(AnimationLength("Poison_Jab") / playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat() * 0.9f);

        //isAttacking.RemoveBoolModifier();
        //leftKnifeColider.enabled = false;
        //rightKnifeColider.enabled = false;
        //leftKnifeTrail.Emit = false;
        //rightDaggerTrail.Emit = false;
        //frontAttackCollider.enabled = false;
        //frontAttackLogic.inUseSkill = null;
        //rightDaggerAttackLogic.inUseSkill = null;
        //leftDaggerAttackLogic.inUseSkill = null;

        //animator.SetBool("Poison_Jab", false);

        //rightKnifeColider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
        //leftKnifeColider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
        //frontAttackCollider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
    }

    public void ShockBombGenerate()
    {
        var bomb = Instantiate(PrefabCollect.instance.shockBomb, rightDaggerColider.transform.position, Quaternion.identity);

        bomb.GetComponentInChildren<ShockBomb>().Stun = PrefabCollect.instance.ShockBombSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.ShockBombSkill)].buffNDebuffObject;

        bomb.GetComponentInChildren<Rigidbody>().AddForce(AttackEffectFunctions.GetDirection(target.position, bomb.transform.position) * 1500);

        AudioManager.instance.PlaySFXWithAudioSource("throw1", AudioSource);
    }

    IEnumerator ShockBombThrow(Skill skill)
    {
        animator.SetTrigger("ShockBombThrow");
        playerstat.UseMana(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp);

        yield return null;
    }

    IEnumerator BombReady(Skill skill)
    {
        animator.SetBool("Bomb_Ready", true);
        isAttacking.AddBoolModifier();
        bombReady = true;
        ResetCombatTime(keepCombatTime);
        bombThrowSkillInfo = skill;

        playerstat.UseMana(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp);

        Bomb = Instantiate(BombModel, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), rightSwordColider.transform);
        Bomb.transform.Rotate(0, 0, 90);
        Bomb.transform.localPosition = new Vector3(-.25f, 0, 0);

        yield return null;
    }

    IEnumerator BombThrow()
    {
        animator.SetTrigger("Bomb_Throw");
        animator.SetBool("Bomb_Ready", false);
        bombReady = false;

        ResetCombatTime(keepCombatTime);

        yield return new WaitForSeconds(AnimationLength("Bomb_Throw") * 0.5f);

        Bomb.transform.parent = null;
        Bomb.GetComponent<Collider>().enabled = true;
        Bomb.GetComponent<Rigidbody>().useGravity = true;

        var target = GameObject.FindWithTag("playertarget");
        var dir = (target.transform.position + transform.up * 2) - transform.position;

        Bomb.GetComponent<Rigidbody>().AddForce(dir.normalized * 200);

        Bomb.GetComponent<BombThrow>().ExplodeCountDown(7f, playerstat, bombThrowSkillInfo, 4f);
        isAttacking.RemoveBoolModifier();
    }

    IEnumerator BearTrap(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        var BearTrap = Instantiate(PrefabCollect.instance.BearTrap, GetComponent<Collider>().bounds.center + transform.forward, new Quaternion(0, 0, 0, 0));

        RogueScripts rogueScripts = GetComponent<RogueScripts>();

        AudioManager.instance.PlaySFXWithAudioSource("trap1", AudioSource);

        var BearTrapScript = BearTrap.GetComponentInChildren<BearTrap>();

        rogueScripts.AddNewBearTrap(BearTrapScript);
        BearTrapScript.owner = gameObject;
        BearTrapScript.rougeScripts = rogueScripts;

        BearTrapScript.stun = skill.skillLeveling[playerskill.GetSkillLevel(skill)].buffNDebuffObject;

        BearTrapScript.damage = Mathf.RoundToInt(skill.skillLeveling[playerskill.GetSkillLevel(skill)].damageFactor * playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat());

        yield return null;
    }

    IEnumerator KnifeThrowing(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        animator.SetTrigger("KnifeThrowing");

        yield return null;
    }

    public void ThrowingKnife()
    {
        var Knife = Instantiate(PrefabCollect.instance.ThrowingKnife, GetComponent<Collider>().bounds.center, new Quaternion(0, 0, 0, 0));

        var KnifeScripts = Knife.GetComponentInChildren<ThrowingKnife>();

        Knife.GetComponentInChildren<Animator>().SetBool("Spin", true);

        KnifeScripts.owner = gameObject;

        KnifeScripts.minDamage = Mathf.RoundToInt(playerstat.minDamage.GetFinalStatValueAsMultiflyFloat() * PrefabCollect.instance.ThrowingKnifeSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.ThrowingKnifeSkill)].damageFactor);
        KnifeScripts.maxDamage = Mathf.RoundToInt(playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat() * PrefabCollect.instance.ThrowingKnifeSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.ThrowingKnifeSkill)].damageFactor);
        KnifeScripts.debuff = PrefabCollect.instance.ThrowingKnifeSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.ThrowingKnifeSkill)].buffNDebuffObject;
        Knife.GetComponentInChildren<Animator>().SetTrigger("Spin");

        AudioManager.instance.PlaySFXWithAudioSource("throw1", AudioSource);
    }

    IEnumerator DoubleShackle(Skill skill)
    {
        ResetCombatTime(keepCombatTime);
        isAttacking.AddBoolModifier();

        var DoubleShackle = Instantiate(PrefabCollect.instance.DoubleShackle, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), rightSwordColider.transform);

        DoubleShackle.transform.localPosition = new Vector3(0, 0, 0);

        ShackleScripts = DoubleShackle.GetComponent<RopeBothObject>();

        FirstShackleReady = true;

        yield return null;
    }

    IEnumerator FirstShackleFire()
    {
        ShackleScripts.FireFirstShackle();
        animator.SetTrigger("Bomb_Throw");
        ShackleScripts.First.GetComponent<Collider>().enabled = true;

        FirstShackleReady = false;
        SecondShackleReady = true;

        yield return null;
    }

    IEnumerator SecondShackleFire()
    {
        ShackleScripts.FireSecondShackle();
        animator.SetTrigger("Bomb_Throw");
        ShackleScripts.Second.GetComponent<Collider>().enabled = true;
        SecondShackleReady = false;

        ShackleScripts.DestroyShackles(10);
        ShackleScripts = null;


        yield return null;
        isAttacking.RemoveBoolModifier();
    }

    IEnumerator ExplosionTrap(Skill skill)
    {
        var explosionTrap = Instantiate(PrefabCollect.instance.ExplosionTrap, GetComponent<Collider>().bounds.center + transform.forward, new Quaternion(0, 0, 0, 0));

        var explosionTrapScript = explosionTrap.GetComponentInChildren<ExplosionTrap>();
        explosionTrapScript.owner = gameObject;
        explosionTrapScript.Burn = PrefabCollect.instance.ExplosionTrapSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.ExplosionTrapSkill)].buffNDebuffObject;

        RogueScripts rogueScripts = GetComponent<RogueScripts>();

        rogueScripts.AddNewExplosionTrap(explosionTrapScript);
        explosionTrapScript.rougeScripts = rogueScripts;

        explosionTrapScript.damage = Mathf.RoundToInt(skill.skillLeveling[playerskill.GetSkillLevel(skill)].damageFactor * playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat());

        AudioManager.instance.PlaySFXWithAudioSource("trap1", AudioSource);

        yield return null;
    }

    IEnumerator BladeDance()
    {
        animator.SetTrigger("BladeDance");
        isAttacking.AddBoolModifier();


        yield return null;
    }

    IEnumerator PoisonBomb(Skill skill)
    {
        animator.SetTrigger("PoisonBomb");

        ResetCombatTime(keepCombatTime);

        yield return null;
    }

    public void PoisonBombThrow()
    {
        GameObject poisonBomb = Instantiate(PrefabCollect.instance.PoisonBomb, rightSwordColider.transform.position, new Quaternion(0, 0, 0, 0));
        poisonBomb.GetComponent<PoisonBomb>().owner = gameObject;

        poisonBomb.GetComponent<PoisonBomb>().mindamage = Mathf.RoundToInt(PrefabCollect.instance.PoisonBombSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.PoisonBombSkill)].damageFactor * playerstat.minDamage.GetFinalStatValueAsMultiflyFloat());
        poisonBomb.GetComponent<PoisonBomb>().maxdamage = Mathf.RoundToInt(PrefabCollect.instance.PoisonBombSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.PoisonBombSkill)].damageFactor * playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat());

        poisonBomb.GetComponent<PoisonBomb>().poison = PrefabCollect.instance.PoisonBombSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.PoisonBombSkill)].buffNDebuffObject;
        poisonBomb.GetComponent<PoisonBomb>().DeleteTime = PrefabCollect.instance.PoisonBombSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.PoisonBombSkill)].value3;

        poisonBomb.GetComponent<Collider>().enabled = true;
        poisonBomb.GetComponent<Rigidbody>().useGravity = true;

        poisonBomb.GetComponent<Rigidbody>().AddForce(AttackEffectFunctions.GetDirection(target.position, poisonBomb.transform.position).normalized * 2000);

        AudioManager.instance.PlaySFXWithAudioSource("throw1", AudioSource);
    }

    #endregion

    #region 법사 공격관련

    IEnumerator Wand_Basic_Attack()
    {
        ResetCombatTime(keepCombatTime);

        //animator.SetBool("isBasicMageAttack", true);
        animator.SetTrigger("MagicArrowFire");
        //isAttacking.AddBoolModifier();

        //GameObject BasicMagicBall = Instantiate(magicBall, firePos.position, new Quaternion(0, 0, 0, 0), rightSwordColider.transform);
        //BasicMagicBall.transform.localPosition = new Vector3(-2.5f, 0, 0);
        //BasicMagicBall.transform.localScale = new Vector3(5, 5, 5);

        //MagicArrowLogic BasicMagicArrowLogic = BasicMagicBall.GetComponent<MagicArrowLogic>();
        //BasicMagicArrowLogic.Instans();
        //BasicMagicArrowLogic.ColliderOn();
        //BasicMagicArrowLogic.player = gameObject;

        yield return new WaitForSeconds(0.5f);
        //BasicMagicBall.transform.parent = firePos.transform;
        //BasicMagicBall.transform.localPosition = new Vector3(0, 0, 0);
        //BasicMagicBall.transform.parent = null;
        //BasicMagicArrowLogic.MagicArrowFire();
        //animator.SetBool("isBasicMageAttack", false);
        //BasicMagicArrowLogic.magicArrowLight.range = 3f;

        //yield return new WaitForSeconds(0.2f);
        //isAttacking.RemoveBoolModifier();
    }

    IEnumerator Wand_Attack_Charge()
    {
        /*ResetCombatTime(keepCombatTime);

        if (isArrowCharging == false)
        {
            MagicBall = Instantiate(magicBall, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), rightSwordColider.transform);
            MagicBall.transform.localPosition = new Vector3(-2.5f, 0, 0);
            magicArrowLogic = MagicBall.AddComponent<MagicArrowLogic>();
            magicArrowAudioSource = magicBall.GetComponent<AudioSource>();
            MagicBall.GetComponent<Collider>().enabled = false;
        }
        else
        {
            MagicBall.transform.localScale = new Vector3(ChargeCount / 5, ChargeCount / 5, ChargeCount / 5);
            magicArrowLogic.magicArrowLight.range = 3f * (ChargeCount / 10);
        }
        isArrowCharging = true;

        if (Input.GetButton("Fire2"))
        {
            animator.SetBool("isMagicCharging", true);
            ChargeCount += Time.deltaTime * 10;

            ChargeCount = Mathf.Clamp(ChargeCount, minChargeCount, maxChargeCount);
        }

        magicArrowAudioSource.volume = (ChargeCount / maxChargeCount) / 2;*/
        yield return null;
    }

    IEnumerator Wand_Attack_Fire()
    {
        //isAttacking = false;
        animator.SetBool("isMagicCharging", false);
        animator.SetBool("isMagicFire", true);

        var Scripts = MagicBall.GetComponent<MagicArrowLogic>();

        Scripts.player = gameObject;
        Scripts.ChargeMultiply = ChargeCount;

        MagicBall.transform.position = firePos.position;

        ChargeCount = 0;
        MagicBall.transform.parent = null;

        MagicBall = null;
        isAttacking.AddBoolModifier();
        isArrowCharging = false;

        Scripts.MagicArrowFire();
        Scripts.ColliderOn();

        yield return new WaitForSeconds(AnimationLength("Wand_Fire") / playerstat.AttackSpeed.GetFinalStatValueAsMultiflyFloat() * 3f);
        isAttacking.RemoveBoolModifier();
        animator.SetBool("isMagicFire", false);
    }

    public void GenerateMagicMissile()
    {
        GameObject BasicMagicArrow = Instantiate(PrefabCollect.instance.magicBall, firePos.position, new Quaternion(0, 0, 0, 0));

        var projectileLogic = BasicMagicArrow.GetComponent<ProjectileLogic>();

        projectileLogic.Setting(gameObject, Mathf.RoundToInt(playerstat.minDamage.GetFinalStatValue()), Mathf.RoundToInt(playerstat.maxDamage.GetFinalStatValue()));
        projectileLogic.Fire(target.position, firePos.position); 

        AudioManager.instance.GenerateAudioAndPlaySFX("fireMagicMissile1", rightSwordColider.transform.position);
        /*MagicArrowLogic BasicMagicArrowLogic = BasicMagicBall.GetComponent<MagicArrowLogic>();
        BasicMagicArrowLogic.Instans();
        BasicMagicArrowLogic.player = gameObject;
        BasicMagicBall.transform.parent = firePos.transform;
        BasicMagicBall.transform.localPosition = new Vector3(0, 0, 0);
        BasicMagicBall.transform.parent = null;
        BasicMagicArrowLogic.MagicArrowFire();*/
    }

    IEnumerator MagicOrb(Skill skill)
    {
        ResetCombatTime(keepCombatTime);
        animator.SetTrigger("SpellCasting");
        isAttacking.AddBoolModifier();

        var magicOrb = Instantiate(PrefabCollect.instance.MagicOrb, rightSwordColider.transform.position, new Quaternion(0, 0, 0, 0));

        var magicOrbScripts = magicOrb.GetComponent<MagicOrb>();

        var magicOrbBeacon = new GameObject("magicOrb");
        magicOrbBeacon.transform.SetParent(transform);
        magicOrbBeacon.transform.localPosition = new Vector3(0, 3f, 0);
        magicOrbs.Push(magicOrbScripts);
        magicOrbBeacons.Push(magicOrbBeacon);
        magicOrbScripts.owner = gameObject;
        magicOrbScripts.target = magicOrbBeacon.transform;

        /*MagicOrb[] magicOrbArray = magicOrbs.ToArray();
        GameObject[] magicOrbBeaconsArray = magicOrbBeacons.ToArray();

        float count = magicOrbArray.Length;

        if (count > 1)
        {
            for (int i = 0; i < count; i++)
            {
                magicOrbArray[i].positionFix = (count/2) + (i - count) + 0.5f;
                magicOrbArray[i].ChangePosition();

                magicOrbBeaconsArray[i].transform.position = new Vector3((count / 2) + (i - count) + 0.5f,2.5f,0);

               //print(i + " 번째 매직볼 : " + magicOrbArray[i].positionFix + "count : " + count + " " + count/2);
            }
        }*/

        StartCoroutine(ChangeMagicOrbsPosition());
        magicOrbScripts.damage = Mathf.RoundToInt(skill.skillLeveling[playerskill.GetSkillLevel(skill)].damageFactor * playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat());

        yield return new WaitForSeconds(1);

        isAttacking.RemoveBoolModifier();
    }

    IEnumerator Flame()
    {
        animator.SetBool("isFlame", true);
        isAttacking.AddBoolModifier();
        FlameVFX.Play();
        AudioManager.instance.PlaySFXWithAudioSource("fireGiggle1", AudioSource);

        yield return null;
    }

    IEnumerator FlamesOff()
    {
        isAttacking.RemoveBoolModifier();
        animator.SetBool("isFlame", false);
        FlameVFX.Stop();
        AudioSource.Stop();

        yield return null;
    }

    public void generateFlameTick()
    {
        playerstat.UseMana(PrefabCollect.instance.FlameSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.FlameSkill)].needMp);
        ResetCombatTime(keepCombatTime);
        var flameTick = Instantiate(PrefabCollect.instance.FlameTick, FlameFirePos.position, new Quaternion(0, 0, 0, 0));

        float damagemulfly = PrefabCollect.instance.FlameSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.FlameSkill)].damageFactor;

        flameTick.GetComponentInChildren<FlameTick>().Set(gameObject, Mathf.RoundToInt(playerstat.minDamage.GetFinalStatValueAsMultiflyFloat() * damagemulfly), 
            Mathf.RoundToInt(playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat() * damagemulfly), target);
    }

    public IEnumerator ChangeMagicOrbsPosition()
    {
        MagicOrb[] magicOrbArray = magicOrbs.ToArray();
        GameObject[] magicOrbBeaconsArray = magicOrbBeacons.ToArray();

        float count = magicOrbArray.Length;

        if (count > 1)
        {
            for (int i = 0; i < count; i++)
            {
                //magicOrbArray[i].positionFix = (count / 2) + (i - count) + 0.5f;
                //magicOrbArray[i].ChangePosition();
                magicOrbArray[i].target = magicOrbBeaconsArray[i].transform;
                magicOrbBeaconsArray[i].transform.localPosition = new Vector3(((count / 2) + (i - count + 0.5f)) * 0.7f, 3.5f + (Mathf.Abs((count / 2) + (i - count + 0.5f))) * -0.5f, 0);

                //print(i + " 번째 매직볼 : " + magicOrbArray[i].positionFix + "count : " + count + " " + count/2);
            }
        }
        else if (count == 1)
        {
            magicOrbArray[0].target = magicOrbBeaconsArray[0].transform;
            magicOrbBeaconsArray[0].transform.localPosition = new Vector3(0, 3f, 0);

        }

        yield return null;
    }

    IEnumerator FireBall_Fire(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        animator.SetTrigger("FireBall");

        playerstat.UseMana(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp);

        yield return null;
    }

    public void FireBall()
    {
        var fireBall = Instantiate(PrefabCollect.instance.FireBall, rightSwordColider.transform.position, Quaternion.identity);

        AudioManager.instance.GenerateAudioAndPlaySFX("fireBall1", rightSwordColider.transform.position);

        var projectileLogic = fireBall.GetComponent<ProjectileLogic>();

        projectileLogic.Setting(gameObject, Mathf.RoundToInt(playerstat.minDamage.GetFinalStatValueAsMultiflyFloat())
                , Mathf.RoundToInt(playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat()));
        projectileLogic.Fire(target.position, rightSwordColider.transform.position, gameObject);
        projectileLogic.ExplodeDamageMultiply = PrefabCollect.instance.FireBallSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.FireBallSkill)].damageFactor;
        projectileLogic.explodeEffects.Add(PrefabCollect.instance.FireBallSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.FireBallSkill)].buffNDebuffObject);
        /*var Scripts = fireBall.GetComponent<FireBall>();
        Scripts.SetInfo(PrefabCollect.instance.FireBallSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.FireBallSkill)].damageFactor, 10f, gameObject 
            , PrefabCollect.instance.FireBallSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.FireBallSkill)].buffNDebuffObject);

        Scripts.FireBallFire();*/
    }

    IEnumerator IceSpear(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        animator.SetTrigger("IceSpear");

        playerstat.UseMana(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp);

        yield return null;
    }

    public void GenerateIceSpear()
    {
        int amount = PrefabCollect.instance.IceSpearSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.IceSpearSkill)].value1;

        int Angel = 0;

        int fireAngle = PrefabCollect.instance.IceSpearSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.IceSpearSkill)].value2;

        if (amount > 1)
            Angel = fireAngle / (amount - 1);

        for (int i = 0; i < amount; i++)
        {
            var IceSpear = Instantiate(PrefabCollect.instance.IceSpear, rightSwordColider.transform.position, new Quaternion(0, 0, 0, 0));

            var iceSpearScript = IceSpear.GetComponent<ProjectileLogic>();

            iceSpearScript.projectileEffect = PrefabCollect.instance.IceSpearSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.IceSpearSkill)].buffNDebuffObject;
            iceSpearScript.GetComponent<ProjectileLogic>().Setting(gameObject, Mathf.RoundToInt(playerstat.minDamage.GetFinalStatValueAsMultiflyFloat())
                , Mathf.RoundToInt(playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat()));

            iceSpearScript.Fire(target.position, rightSwordColider.transform.position, gameObject, (Angel * (i)) - (fireAngle / 2),extraAngleX : -90);
        }


        AudioManager.instance.GenerateAudioAndPlaySFX("ice1", rightSwordColider.transform.position);
    }

    IEnumerator LightningOrb(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        animator.SetTrigger("LightningOrb");

        playerstat.UseMana(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp);

        yield return null;
    }

    public void GenerateLightningOrb()
    {
        var LightningOrb = Instantiate(PrefabCollect.instance.LightingOrb, rightSwordColider.transform.position, new Quaternion(0, 0, 0, 0));

        var LightningOrbScript = LightningOrb.GetComponent<LightningOrb>();

        LightningOrbScript.owner = gameObject;
        LightningOrbScript.minDamage = Mathf.RoundToInt(PrefabCollect.instance.LightingOrbSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.LightingOrbSkill)].damageFactor * playerstat.minDamage.GetFinalStatValueAsMultiflyFloat());
        LightningOrbScript.maxDamage = Mathf.RoundToInt(PrefabCollect.instance.LightingOrbSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.LightingOrbSkill)].damageFactor * playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat());
        LightningOrbScript.OrbSize = PrefabCollect.instance.LightingOrbSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.LightingOrbSkill)].value3;
        LightningOrbScript.maxTargetCount = PrefabCollect.instance.LightingOrbSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.LightingOrbSkill)].value2;

        AudioManager.instance.PlaySFXWithAudioSource("elec1", LightningOrb.GetComponentInChildren<AudioSource>());
    }

    IEnumerator ReroadBouncingBall(Skill skill)
    {
        animator.SetTrigger("SpellCasting");
        isAttacking.AddBoolModifier();

        BouncingBallCount = playerskill.GetSkillLevel(skill) * 2;

        yield return new WaitForSeconds(1);

        isAttacking.RemoveBoolModifier();
    }

    IEnumerator FireBouncingBall(Skill skill)
    {
        animator.SetTrigger("MagicArrowFire");
        isAttacking.AddBoolModifier();

        yield return new WaitForSeconds(0.5f);

        isAttacking.RemoveBoolModifier();

        var bouncingBall = Instantiate(PrefabCollect.instance.BouncingBall, firePos.position, new Quaternion(0, 0, 0, 0));
        var bouncingBallScript = bouncingBall.GetComponent<BouncingBall>();

        bouncingBallScript.damage = Mathf.RoundToInt(skill.skillLeveling[playerskill.GetSkillLevel(skill)].damageFactor * playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat()); ;
        bouncingBallScript.MaxbouncingCount = 3 + playerskill.GetSkillLevel(skill);
        bouncingBallScript.owner = gameObject;

        BouncingBallCount--;

        yield return null;
    }

    IEnumerator IceShield(Skill skill)
    {
        ResetCombatTime(keepCombatTime);
        animator.SetTrigger("SpellCasting");

        playerstat.UseMana(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp);

        var iceShieldParticle = Instantiate(PrefabCollect.instance.IceShield, GetComponent<Collider>().bounds.center, new Quaternion(0, 0, 0, 0), transform);

        AudioManager.instance.GenerateAudioAndPlaySFX("ice1", rightSwordColider.transform.position);

        iceShieldParticle.GetComponent<IceShield>().ownerStat = playerstat;

        playerstat.shieldStat.AddShield("IceShield", skill.skillLeveling[playerskill.GetSkillLevel(skill)].value1, skill.skillLeveling[playerskill.GetSkillLevel(skill)].value1, playerstat);

        yield return new WaitForSeconds(skill.skillLeveling[playerskill.GetSkillLevel(skill)].value2);

        playerstat.shieldStat.DeleteShieldAsName("IceShield");
        Destroy(iceShieldParticle);
    }

    IEnumerator OverCharge(Skill skill)
    {
        ResetCombatTime(keepCombatTime);
        animator.SetTrigger("SpellCasting");

        var particle = Instantiate(PrefabCollect.instance.OverChargeParticle, GetComponent<Collider>().bounds.center, new Quaternion(0, 0, 0, 0), transform);

        var particles = particle.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem particleSystem in particles)
        {
            var shape = particleSystem.shape;
            shape.skinnedMeshRenderer = BodyMeshRenderer;
        }

        float MpGenerateSpeed = skill.skillLeveling[playerskill.GetSkillLevel(skill)].value3;

        AudioManager.instance.GenerateAudioAndPlaySFX("lightning1", GetComponent<Collider>().bounds.center);

        for (int i = 0; i < 5; i++)
        {
            var playerVec = transform.position;
            AttackEffectFunctions.GenerateElectircLineRenderer(PrefabCollect.instance.LightingRenderer
                , new Vector3(playerVec.x + Random.Range(-5, 5), playerVec.y + 20, playerVec.z + Random.Range(-5, 5))
                , GetComponent<Collider>().bounds.center, 1);
            EZCameraShake.CameraShaker.Instance.ShakeOnce(2, 2, .1f, 1);

            yield return new WaitForSeconds(0.15f);
        }

        //playerstat.MPGenerationValue.AddPercentModifier(MpGenerateSpeed - 1f);
        playerstat.MPGenerationSpeedMulti.AddPercentModifier(MpGenerateSpeed);

        yield return new WaitForSeconds(skill.skillLeveling[playerskill.GetSkillLevel(skill)].value1);

        Destroy(particle);

        //playerstat.MPGenerationValue.RemovePercentModifier(MpGenerateSpeed - 1f);
        playerstat.MPGenerationSpeedMulti.RemovePercentModifier(MpGenerateSpeed);
    }

    IEnumerator MagicMissileSpray(Skill skill)
    {
        ResetCombatTime(keepCombatTime);
        animator.SetTrigger("MagicMissileSpray");

        playerstat.UseMana(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp);

        yield return null;
    }

    public void MagicMissileSprayStart()
    {
        StartCoroutine(GenerateMagicMissileSpray());
    }

    IEnumerator GenerateMagicMissileSpray()
    {
        int missileCount = PrefabCollect.instance.magicMissileSpraySkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.magicMissileSpraySkill)].value1;

        for(int i = 0; i < missileCount; i++)
        {
            var magicMissile = Instantiate(PrefabCollect.instance.magicMissileSprayProjectile, rightSwordColider.transform.position, Quaternion.identity);

            float damageMulti = PrefabCollect.instance.magicMissileSpraySkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.magicMissileSpraySkill)].damageFactor;

            magicMissile.GetComponent<ProjectileLogic>().Setting(gameObject, Mathf.RoundToInt(playerstat.minDamage.GetFinalStatValueAsMultiflyFloat() * damageMulti), Mathf.RoundToInt(playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat() * damageMulti));

            Vector3 randomPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            magicMissile.GetComponent<ProjectileLogic>().Fire(rightSwordColider.transform.position + (AttackEffectFunctions.GetDirection(target.position, magicMissile.transform.position) * 30) + randomPosition
                , magicMissile.transform.position, gameObject);

            AudioManager.instance.GenerateAudioAndPlaySFX("magicArrow1", rightSwordColider.transform.position);

            if (!playerskill.usingSkill.GetBoolValue())
                break;


            //Debug.LogError(PrefabCollect.instance.magicMissileSpraySkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.magicMissileSpraySkill)].value2 / 100f);
            yield return new WaitForSeconds(PrefabCollect.instance.magicMissileSpraySkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.magicMissileSpraySkill)].value2 / 100f);
        }
    }

    public IEnumerator MagicBomb(Skill skill)
    {
        ResetCombatTime(keepCombatTime);
        animator.SetTrigger("MagicBomb");

        playerstat.UseMana(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp);

        yield return null;
    }

    public void GenereateMagicBomb()
    {
        var bomb = Instantiate(PrefabCollect.instance.magicBomb, rightSwordColider.transform.position, Quaternion.identity);
        bomb.GetComponent<Rigidbody>().AddForce(AttackEffectFunctions.GetDirection(target.position, bomb.transform.position) * 120);

        float damage = Random.Range(playerstat.minDamage.GetFinalStatValueAsMultiflyFloat(), playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat());

        bomb.GetComponent<BombThrow>().explodeDamage = Mathf.RoundToInt(damage * PrefabCollect.instance.magicbombSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.magicbombSkill)].damageFactor);
        bomb.GetComponent<BombThrow>().Owner = gameObject;
        BuffNDebuffObject[] buffNDebuffObject = new BuffNDebuffObject[1];
        buffNDebuffObject[0] = PrefabCollect.instance.magicbombSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.magicbombSkill)].buffNDebuffObject;
        bomb.GetComponent<BombThrow>().debuffs = buffNDebuffObject;
    }

    public IEnumerator FlameExplosion(Skill skill)
    {
        ResetCombatTime(keepCombatTime);
        animator.SetTrigger("FlameExplosion");

        playerstat.UseMana(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp);

        yield return null;
    }

    public void GenerateFlameExplosionChargingParticle()
    {
        var particle = Instantiate(PrefabCollect.instance.flameExplosionChargingParticle, LeftHand.transform.position, Quaternion.identity, LeftHand);

        Destroy(particle, 5);

    }

    public void GenerateFlameExplosion()
    {
        var dir = AttackEffectFunctions.GetDirection(target.position, LeftHand.transform.position);

        var particle = Instantiate(PrefabCollect.instance.flameExplosion, LeftHand.transform.position, Quaternion.identity);

        particle.transform.LookAt(target);

        float damage = Random.Range(playerstat.minDamage.GetFinalStatValueAsMultiflyFloat(), playerstat.maxDamage.GetFinalStatValueAsMultiflyFloat());
        NPC_Type nPC_Type = NPC_Type.friendly;

        BuffNDebuffObject[] buffNDebuffObjects = new BuffNDebuffObject[2];

        buffNDebuffObjects[0] = 
            PrefabCollect.instance.flameExplosionSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.flameExplosionSkill)]
            .buffNDebuffObject;
        buffNDebuffObjects[1] = 
            PrefabCollect.instance.flameExplosionSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.flameExplosionSkill)]
            .buffNDebuffObject2;


        AttackEffectFunctions.explode(6, Mathf.RoundToInt(damage * PrefabCollect.instance.flameExplosionSkill.skillLeveling[playerskill.GetSkillLevel(PrefabCollect.instance.flameExplosionSkill)].damageFactor)
                                        , LeftHand.transform.position + (dir * 3f), PrefabCollect.instance.flameExplosion, nPC_Type, gameObject, debuffs: buffNDebuffObjects, explosionParticle: false);

        Destroy(particle, 5);
    }
    #endregion

    #region 사령술사 스킬 관련

        IEnumerator SoulFire()
    {
        ResetCombatTime(keepCombatTime);

        animator.SetTrigger("SoulFire");
        yield return null;
    }
    public void CreateSoulFire()
    {
        GameObject SoulFire = Instantiate(PrefabCollect.instance.SoulFire, leftKnifeColider.transform.position, new Quaternion(0, 0, 0, 0));
        MagicArrowLogic BasicMagicArrowLogic = SoulFire.GetComponent<MagicArrowLogic>();
        BasicMagicArrowLogic.Instans();
        BasicMagicArrowLogic.player = gameObject;
        BasicMagicArrowLogic.MagicArrowFire();
    }

    IEnumerator SoulSword()
    {
        ResetCombatTime(keepCombatTime);
        animator.SetTrigger("SoulSword");

        yield return null;
    }

    IEnumerator SoulDrain()
    {
        ResetCombatTime(keepCombatTime);
        animator.SetBool("SoulDrain", true);
        isAttacking.AddBoolModifier();
        SoulDraining = true;

        if (SoulDraining)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 50, Color.white);
            RaycastHit Hit;
            if (Physics.Raycast(ray.origin, ray.direction.normalized, out Hit, 50, npcMask))
            {
                if (Hit.transform.GetComponentInChildren<NPC_AI>() != null)
                {
                    if (soulDrainCoolTime <= 0 && Hit.transform.GetComponentInChildren<NPC_AI>().npcType == NPC_Type.enemy)
                    {
                        Hit.transform.GetComponentInChildren<NPCStats>().TakeDamage(2,3, gameObject, false, true, false);
                        GameObject effect = Instantiate(PrefabCollect.instance.SoulDrainParticle, Hit.transform.GetComponentInChildren<Collider>().bounds.center, new Quaternion(0, 0, 0, 0), Hit.transform);
                        effect.GetComponent<LifeDrainParticlePath>().owner = transform;
                        var disToOwner = Vector3.Distance(GetComponent<Collider>().bounds.center, Hit.transform.GetComponentInChildren<Collider>().bounds.center);

                        if (disToOwner <= 5)
                        {
                            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 4;
                        }
                        else if (disToOwner <= 10)
                        {
                            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 7;
                        }
                        else if (disToOwner <= 25)
                        {
                            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 12;
                        }
                        else
                        {
                            effect.GetComponent<LifeDrainParticlePath>().disToTarget = disToOwner / 15;
                        }

                        playerstat.GetSoul(3);
                        soulDrainCoolTime = .5f;
                    }
                    else
                    {
                        soulDrainCoolTime -= Time.deltaTime;
                    }
                }
            }
        }
        yield return null;
    }

    IEnumerator SoulDrainOff()
    {
        SoulDraining = false;
        isAttacking.RemoveBoolModifier();
        animator.SetBool("SoulDrain", false);
        yield return null;
    }

    IEnumerator FireGhostArrow(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        var GhostArrow = Instantiate(PrefabCollect.instance.GhostArrow, rightSwordColider.transform.position, new Quaternion(0, 0, 0, 0));
        //GhostArrow.transform.localPosition = new Vector3(-2.5f, 0, 0);
        var Arrow = GhostArrow.GetComponent<GhostArrow>();
        Arrow.player = gameObject;
        Arrow.GetArrowDamageValue(Mathf.RoundToInt(playerstat.maxDamage.GetFinalStatValue()), (int)skill.skillLeveling[playerskill.GetSkillLevel(skill)].damageFactor);

        yield return null;
    }

    IEnumerator Harvest(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        int soulSkullNum = Random.Range(2, 6);
        RaycastHit raycastHit;

        if (Physics.Raycast(transform.GetComponent<Collider>().bounds.center, -transform.up, out raycastHit, 10f, obstacleMask))
        {
            ParticleGenerator.instance.GenerateGroundParticle(raycastHit.point, "GroundCrack_Harvest", 10f);
        }
        for (int i = 0; i < soulSkullNum; i++)
        {
            var soulskull = Instantiate(PrefabCollect.instance.SoulSkull, new Vector3(transform.position.x + Random.Range(-10, 10), raycastHit.point.y - 1, transform.position.z + Random.Range(-10, 10)), Quaternion.Euler(0, Random.Range(0, 361), 0));
            var script = soulskull.GetComponentInChildren<SoulSkull>();
            script.soulValue = Random.Range(5, 21);
            script.TargetPlayer = transform;

        }

        animator.SetTrigger("SpellCasting");

        EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, .1f, 1);

        yield return null;
    }

    IEnumerator SummonSkeleton(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        var SkeletonMinion = Instantiate(PrefabCollect.instance.SkeletonMinion, transform.position + transform.forward * 3, Quaternion.identity);
        SkeletonMinion.GetComponentInChildren<NPC_AI>().Summoner = gameObject;
        animator.SetTrigger("SpellCasting");

        ParticleGenerator.instance.GenerateGroundParticle(SkeletonMinion.transform.position, "SummonCircle", 10f);

        SkeletonMinion.GetComponentInChildren<Animator>().SetTrigger("Summon");

        SummonedCreature.Add(SkeletonMinion);

        yield return null;
    }

    IEnumerator SummonSkeletonArcher(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        var SkeletonMinion = Instantiate(PrefabCollect.instance.SkeletonArcherMinion, transform.position + transform.forward * 3, Quaternion.identity);
        SkeletonMinion.GetComponentInChildren<NPC_AI>().Summoner = gameObject;
        animator.SetTrigger("SpellCasting");

        ParticleGenerator.instance.GenerateGroundParticle(SkeletonMinion.transform.position, "SummonCircle", 10f);

        SkeletonMinion.GetComponentInChildren<Animator>().SetTrigger("Summon");

        SummonedCreature.Add(SkeletonMinion);

        yield return null;
    }

    IEnumerator SummonSkeletonKnight(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        var SkeletonMinion = Instantiate(PrefabCollect.instance.SkeletonKnightMinion, transform.position + transform.forward * 3, Quaternion.identity);
        SkeletonMinion.GetComponentInChildren<NPC_AI>().Summoner = gameObject;
        animator.SetTrigger("SpellCasting");

        ParticleGenerator.instance.GenerateGroundParticle(SkeletonMinion.transform.position, "SummonCircle", 10f);

        SkeletonMinion.GetComponentInChildren<Animator>().SetTrigger("Summon");

        SummonedCreature.Add(SkeletonMinion);

        yield return null;
    }

    IEnumerator SummonLich(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        var SkeletonMinion = Instantiate(PrefabCollect.instance.LichMinion, transform.position + transform.forward * 3, Quaternion.identity);
        SkeletonMinion.GetComponentInChildren<NPC_AI>().Summoner = gameObject;
        animator.SetTrigger("SpellCasting");

        ParticleGenerator.instance.GenerateGroundParticle(SkeletonMinion.transform.position, "SummonCircle", 10f);

        SkeletonMinion.GetComponentInChildren<Animator>().SetTrigger("Summon");

        SummonedCreature.Add(SkeletonMinion);

        EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, .1f, 2);

        yield return null;
    }

    IEnumerator AuraOfSoulFire(Skill skill)
    {
        var Aura = ParticleGenerator.instance.GenerateGroundParticle(transform.position, "AuraOfSoulFire", 7f);
        Aura.transform.SetParent(transform);

        bool AuraOfSoulFire = true;

        StartCoroutine(AuraOfSoulFireFindEnemy(AuraOfSoulFire));
        yield return new WaitForSeconds(7);

        AuraOfSoulFire = false;
    }

    IEnumerator AuraOfSoulFireFindEnemy(bool auraOfSoulFire)
    {
        WaitForSeconds delay = new WaitForSeconds(0.5f);

        float time = 0;
        Collider[] colls;
        LayerMask targetMask = (1 << LayerMask.NameToLayer("NPC"));


        while (auraOfSoulFire)
        {
            if (time > 7)
                auraOfSoulFire = false;

            print(time);

            colls = Physics.OverlapSphere(transform.position, 8, targetMask);

            print(colls.Length);

            foreach (Collider NPCCollider in colls)
            {
                if (NPCCollider.GetComponentInChildren<NPC_AI>() != null)
                {
                    if (NPCCollider.GetComponentInChildren<NPC_AI>().npcType == NPC_Type.enemy)
                        NPCCollider.GetComponentInChildren<NPCStats>().TakeDamage(1,3, gameObject, false, true, false);
                }
            }
            time += 0.3f;
            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator SoulSacrifice(Skill skill)
    {
        var Particle = ParticleGenerator.instance.GenerateGroundParticle(transform.position + new Vector3(0, 1), "SoulSacrifice", 5f);
        Particle.transform.SetParent(transform);

        animator.SetTrigger("SpellCasting");

        yield return new WaitForSeconds(1);

        EZCameraShake.CameraShaker.Instance.ShakeOnce(4, 4, .1f, 1);
        AudioSource.PlayOneShot(SoundManager.instance.ExplosionSound);

        Particle.transform.SetParent(null);

        Collider[] colliders = Physics.OverlapSphere(transform.position, 5f);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<NPC_AI>() != null)
            {
                if (colliders[i].GetComponent<NPC_AI>().npcType == NPC_Type.enemy)
                {
                    if (colliders[i].gameObject == gameObject)
                    {
                        //사용자는 피해 없음
                    }
                    else
                    {
                        CharacterStats Cstats = colliders[i].GetComponent<CharacterStats>();

                        Cstats.TakeDamage((int)Mathf.Round((playerstat.minDamage.GetFinalStatValue() * 2) * ((playerstat.playerCurrentSoul + 100) / 100)), (int)Mathf.Round((playerstat.maxDamage.GetFinalStatValue() * 2) * ((playerstat.playerCurrentSoul + 100) / 100)), gameObject, false, true, false);
                    }
                }

            }
        }

        playerstat.playerCurrentSoul = 0;

    }

    IEnumerator ChainOfPurgatory(Skill skill)
    {
        var chainOfPurgatory = Instantiate(PrefabCollect.instance.Skill_ChainOfPurgatory, leftKnifeColider.transform.position, new Quaternion());

        chainOfPurgatory.GetComponentInChildren<EvilSpiritsHand>().skill = skill;
        chainOfPurgatory.GetComponentInChildren<EvilSpiritsHand>().owner = gameObject;
        yield return null;
    }

    #endregion

    #region 땜장이 스킬 관련

    IEnumerator BombRobot(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        var Robot = Instantiate(PrefabCollect.instance.BombRobot, transform.position + transform.forward * 3, transform.rotation);
        Robot.GetComponentInChildren<NPC_AI>().Summoner = gameObject;

        yield return null;
    }

    IEnumerator CreateCrossBowTurret(Skill skill)
    {
        ResetCombatTime(keepCombatTime);

        var Turret = Instantiate(PrefabCollect.instance.CrossbowTurret, transform.GetComponent<Collider>().bounds.center + transform.forward, transform.rotation);
        Turret.GetComponentInChildren<CrossBowTurret>().Summoner = gameObject;

        Turret.GetComponentInChildren<Rigidbody>().velocity = transform.forward * 4;
        yield return null;
    }

    IEnumerator CreateMissileDrone(Skill skill)
    {
        ResetCombatTime(keepCombatTime);
        var Turret = Instantiate(PrefabCollect.instance.MissileDrone, transform.GetComponent<Collider>().bounds.center + transform.forward, transform.rotation);
        Turret.GetComponentInChildren<DroneAi>().Summoner = gameObject;
        yield return null;
    }

    #endregion

    IEnumerator PlayerSkill(Skill skill, SkillSlot skillSlot)
    {
        if (UIManager.instance.IsAnyUiOn() && skillSlot.slider.value <= skillSlot.slider.minValue && !DungeonGenerator.instance.GetDungeonGenerating())
        {
            if (animator.GetLayerWeight(animator.GetLayerIndex("Warrior")) == 1 /*&& isAttacking.GetBoolValue() == false*/
                && playerstat.playerCurrentRage >= skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage && !playerskill.usingSkill.GetBoolValue())
            {
                if (EquipmentManager.instance.currentEquipment[3])
                {
                    if (skill.Name == "Charge")
                    {
                        StartCoroutine(Charge(skill));
                        //StartCoroutine(AttackSoundPlay());

                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }

                    if (skill.Name == "Smite")
                    {
                        StartCoroutine(Smite(skill));
                        EZCameraShake.CameraShaker.Instance.ShakeOnce(1, 1, .1f, 1);
                        StartCoroutine(AttackSoundPlay());
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }

                    if (skill.Name == "WhirlWind")
                    {
                        StartCoroutine(WhirlWind(skill));
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }

                    if (skill.Name == "Maimed")
                    {
                        StartCoroutine(Maimed(skill));
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }

                    if (skill.Name == "CoupDeGrace")
                    {
                        StartCoroutine(CoupDeGrace(skill));
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                }

                if (EquipmentManager.instance.currentEquipment[(int)ItemType.SecondaryWeapon])
                {
                    if (skill.Name == "ShieldBash")
                    {
                        StartCoroutine(ShieldBash(skill));
                        //StartCoroutine(AttackSoundPlay());

                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }

                    if (skill.Name == "SkullSmash")
                    {
                        StartCoroutine(SkullSmash(skill));

                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }

                    if (skill.Name == "ShieldShockWave")
                    {
                        StartCoroutine(ShieldShockWave(skill));

                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                }

                if (skill.Name == "SuperArmor")
                {
                    StartCoroutine(SuperArmor(skill));
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }


            }
            else if (animator.GetLayerWeight(animator.GetLayerIndex("Rogue")) == 1 && !playerskill.usingSkill.GetBoolValue()
                && playerstat.playerCurrentMp >= skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp)
            {
                if (EquipmentManager.instance.currentEquipment[3])
                {
                    if (skill.Name == "DaggerSpin")
                    {
                        StartCoroutine(Dagger_Spin(skill));
                        StartCoroutine(AttackSoundPlay());
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                    else if (skill.Name == "PoisonJab")
                    {
                        StartCoroutine(PoisonJab(skill));
                        StartCoroutine(AttackSoundPlay());
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                    else if (skill.Name == "BladeDance")
                    {
                        StartCoroutine(BladeDance());
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                }

                if (skill.Name == "ShockBomb")
                {
                    StartCoroutine(ShockBombThrow(skill));
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "BearTrap")
                {
                    StartCoroutine(BearTrap(skill));
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "KnifeThrowing")
                {
                    StartCoroutine(KnifeThrowing(skill));
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "DoubleShackle")
                {
                    StartCoroutine(DoubleShackle(skill));
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "ExplosionTrap")
                {
                    StartCoroutine(ExplosionTrap(skill));
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "PoisonBomb")
                {
                    StartCoroutine(PoisonBomb(skill));
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }


            }
            else if (animator.GetLayerWeight(animator.GetLayerIndex("Wizard")) == 1 && isAttacking.GetBoolValue() == false
                && playerstat.playerCurrentMp >= skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp)
            {

                if (EquipmentManager.instance.currentEquipment[3])
                {
                    if (skill.Name == "FireBall")
                    {
                        StartCoroutine(FireBall_Fire(skill));
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                    else if (skill.Name == "IceSpear")
                    {
                        StartCoroutine(IceSpear(skill));
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                    else if (skill.Name == "LightingOrb")
                    {
                        StartCoroutine(LightningOrb(skill));
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                    else if (skill.Name == "BouncingBall")
                    {
                        StartCoroutine(ReroadBouncingBall(skill));
                        skillSlot.slider.value = skillSlot.slider.maxValue;

                    }
                    else if (skill.Name == "IceShield")
                    {
                        StartCoroutine(IceShield(skill));
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                    else if (skill.Name == "OverCharge")
                    {
                        StartCoroutine(OverCharge(skill));
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                    else if (skill.Name == "MagicMissileSpray")
                    {
                        StartCoroutine(MagicMissileSpray(skill));
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                    else if (skill.Name == "MagicBomb")
                    {
                        StartCoroutine(MagicBomb(skill));
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                    else if (skill.Name == "FlameExplosion")
                    {
                        StartCoroutine(FlameExplosion(skill));
                        skillSlot.slider.value = skillSlot.slider.maxValue;
                    }
                    
                }

                if (playerstat.playerCurrentSoul >= skill.skillLeveling[playerskill.GetSkillLevel(skill)].needSoul)
                {
                    if (EquipmentManager.instance.currentEquipment[3])
                    {
                        if (skill.Name == "GhostArrow")
                        {
                            StartCoroutine(FireGhostArrow(skill));
                            playerstat.UseSoul(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needSoul);
                            skillSlot.slider.value = skillSlot.slider.maxValue;
                        }
                        else if (skill.Name == "Harvest")
                        {
                            StartCoroutine(Harvest(skill));
                            playerstat.UseSoul(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needSoul);
                            skillSlot.slider.value = skillSlot.slider.maxValue;
                        }
                        else if (skill.Name == "SummonSkeleton")
                        {
                            StartCoroutine(SummonSkeleton(skill));
                            playerstat.UseSoul(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needSoul);
                            skillSlot.slider.value = skillSlot.slider.maxValue;
                        }
                        else if (skill.Name == "SummonSkeletonArcher")
                        {
                            StartCoroutine(SummonSkeletonArcher(skill));
                            playerstat.UseSoul(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needSoul);
                            skillSlot.slider.value = skillSlot.slider.maxValue;
                        }
                        else if (skill.Name == "SummonSkeletonKnigth")
                        {
                            StartCoroutine(SummonSkeletonKnight(skill));
                            playerstat.UseSoul(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needSoul);
                            skillSlot.slider.value = skillSlot.slider.maxValue;
                        }
                        else if (skill.Name == "SummonLich")
                        {
                            StartCoroutine(SummonLich(skill));
                            playerstat.UseSoul(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needSoul);
                            skillSlot.slider.value = skillSlot.slider.maxValue;
                        }
                        else if (skill.Name == "AuraOfSoulFire")
                        {
                            StartCoroutine(AuraOfSoulFire(skill));
                            playerstat.UseSoul(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needSoul);
                            skillSlot.slider.value = skillSlot.slider.maxValue;
                        }
                        else if (skill.Name == "SoulSacrifice")
                        {
                            StartCoroutine(SoulSacrifice(skill));
                            playerstat.UseSoul(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needSoul);
                            skillSlot.slider.value = skillSlot.slider.maxValue;
                        }

                        else if (skill.Name == "ChainOfPurgatory")
                        {
                            StartCoroutine(ChainOfPurgatory(skill));
                            playerstat.UseSoul(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needSoul);
                            skillSlot.slider.value = skillSlot.slider.maxValue;
                        }
                    }
                }

            }
            else if (animator.GetLayerWeight(animator.GetLayerIndex("Archer")) == 1
                && playerstat.playerCurrentMp >= skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp)
            {
                if (skill.Name == "DoubleShot")
                {
                    StartCoroutine(DoubleShot());
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "ContinuousFiring")
                {
                    StartCoroutine(ContinuousFiring());
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "SteelArrow")
                {
                    StartCoroutine(SteelArrow());
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "PoisonArrow")
                {
                    StartCoroutine(PoisonArrow());
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "ExplosionArrow")
                {
                    StartCoroutine(ExplosionArrow());
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "GrapplingArrow")
                {
                    StartCoroutine(GrapplingArrow());
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "Kick")
                {
                    StartCoroutine(Kick());
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "BackStepShoot")
                {
                    StartCoroutine(BackStepShoot());
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }

            }
            else if (isAttacking.GetBoolValue() == false)
            {
                if (skill.Name == "BombRobot")
                {
                    StartCoroutine(BombRobot(skill));
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "CreateCrossBowTurret")
                {
                    StartCoroutine(CreateCrossBowTurret(skill));
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }
                else if (skill.Name == "MissileDrone")
                {
                    StartCoroutine(CreateMissileDrone(skill));
                    skillSlot.slider.value = skillSlot.slider.maxValue;
                }

            }
        }
        yield return null;
    }

    public void SkillUse(Skill skill, SkillSlot skillSlot)
    {
        if (skill == null)
        {
            print("스킬이 스킬단축창에 없습니다.");
        }
        else if (skillSlot == null)
        {
            print("스킬슬롯이 등록되어있지 않습니다.");
        }
        else
        {
            StartCoroutine(PlayerSkill(skill, skillSlot));
        }

    }

    IEnumerator AttackSoundPlay()
    {
        yield return new WaitForSeconds(0.15f);
        //SoundManager.instance.PlaySound_SwingSword(AudioSource);
        AudioManager.instance.PlaySFXWithAudioSource("swordSwing1", AudioSource);
        yield return new WaitForSeconds(AttackAnimTime);
    }

    IEnumerator BowAttackSoundPlay()
    {
        SoundManager.instance.PlaySound_BowLaunch(AudioSource);
        yield return new WaitForSeconds(AttackAnimTime);
    }

    IEnumerator PlayShieldBashSound()
    {
        yield return new WaitForSeconds(0.15f);
        SoundManager.instance.PlayShieldBashSound(AudioSource);
        yield return new WaitForSeconds(AttackAnimTime);
    }

    void NotAttackRotation()
    {
        if (verticalMove > minMovementDetectionValue && horizontalMove > minMovementDetectionValue)       //방향키를 누루는것을 하나하나 else if 로 구현
        {
            //tr.rotation = RotFR;
            tr.rotation = Quaternion.Slerp(tr.rotation, RotFR, rotateSpeed * Time.deltaTime);
        }
        else if (verticalMove > minMovementDetectionValue && horizontalMove < -minMovementDetectionValue)
        {
            tr.rotation = Quaternion.Slerp(tr.rotation, RotFL, rotateSpeed * Time.deltaTime);
        }
        else if (verticalMove < -minMovementDetectionValue && horizontalMove > minMovementDetectionValue)
        {
            tr.rotation = Quaternion.Slerp(tr.rotation, RotBR, rotateSpeed * Time.deltaTime);
        }
        else if (verticalMove < -minMovementDetectionValue && horizontalMove < -minMovementDetectionValue)
        {
            tr.rotation = Quaternion.Slerp(tr.rotation, RotBL, rotateSpeed * Time.deltaTime);
        }
        else if (verticalMove > minMovementDetectionValue)
        {
            tr.rotation = Quaternion.Slerp(tr.rotation, RotF, rotateSpeed * Time.deltaTime);
        }
        else if (verticalMove < -minMovementDetectionValue)
        {
            tr.rotation = Quaternion.Slerp(tr.rotation, RotB, rotateSpeed * Time.deltaTime);
        }
        else if (horizontalMove > minMovementDetectionValue)
        {
            tr.rotation = Quaternion.Slerp(tr.rotation, RotR, rotateSpeed * Time.deltaTime);
        }
        else if (horizontalMove < -minMovementDetectionValue)
        {
            tr.rotation = Quaternion.Slerp(tr.rotation, RotL, rotateSpeed * Time.deltaTime);
        }
    }

    void AttackRotation()
    {
        //tr.rotation = Quaternion.EulerAngles(0,Gap.y,0);

        transform.LookAt(targetrotation);   //공격시 target의 y좌표로만 회전하도록 함

        //Vector3 moveDir = (Vector3.forward * verticalMove) + (Vector3.right * horizontalMove);

        //tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed, Space.Self);
    }

    IEnumerator Roll()
    {
        //currentRollCoolTime = playerstat.RollRequireStamina.GetFinalStatValueAsMultiflyFloat();
        //dodgeCoolSlider.maxValue = playerstat.RollRequireStamina.GetFinalStatValueAsMultiflyFloat();
        /*while (currentRollCoolTime > 1.0f)
        {
            dodgeObject.SetActive(true);
            currentRollCoolTime -= Time.deltaTime;
            dodgeCoolSlider.value = currentRollCoolTime;
            yield return new WaitForFixedUpdate();
        }*/

        playerstat.DecreaseStamina(playerstat.RollRequireStamina.GetFinalStatValueAsMultiflyFloat());
        playerstat.OnStatChange();
        //dodgeObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        GetComponent<PlayerArtifact>().ArtifactRollEffect();
        rollDustParticle.Play();

        //FrontColliderReset();

    }

    void MovementWhileAttack()
    {
        Vector3 moveDir = new Vector3(horizontalMove, 0, verticalMove);
        movement.Set(horizontalMove, 0, verticalMove);
        movement = movement.normalized * moveSpeed * Time.deltaTime;        //(speed = 이동속도) * (Time.deltaTime = 프레임 보정시간)  두방향키를 동시에 눌러 대각선 이동이 가능해짐

        //rigdbody.MovePosition(transform.position + movement);
        tr.Translate(moveDir.normalized * Time.deltaTime * (moveSpeed), Space.Self);
    }

    void WeaponColliderOnOff()
    {
        if (animator.GetBool("isDaggerAttack") == true)
        {
            rightDaggerColider.enabled = true;
        }
        else
        {
            rightDaggerColider.enabled = false;
        }

        if (animator.GetBool("isDaggerAttack") == true)
        {
            leftKnifeColider.enabled = true;
        }
        else
        {
            leftKnifeColider.enabled = false;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Atk02") || animator.GetCurrentAnimatorStateInfo(0).IsName("Sword_Atk02-2"))
        {
            rightSwordColider.enabled = true;
        }
        else
        {
            rightSwordColider.enabled = false;
        }
    }

    bool AnimatorIsPlaying(string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    float AnimationLength(string name)          //"Punch" 애니메이션클립의 길이를 알아내는 함수
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        for (int i = 0; i < ac.animationClips.Length; i++)
            if (ac.animationClips[i].name == name)
                AttackAnimTime = ac.animationClips[i].length;

        return AttackAnimTime;
    }

    public bool LockRotate()
    {
        if (!UIManager.instance.IsAnyUiOn())
            return true;
        else
            return false;
    }

    void CheckGround()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position + new Vector3(0, 0.7f), Vector3.down * 0.9f, Color.red);

        if (Physics.Raycast(transform.position + new Vector3(0, 0.7f), Vector3.down, out hit, 1.3f, groundCheckMask))
        {
            //print(hit.transform.tag + " " + hit.transform.root.name);
            if (hit.transform.CompareTag("Ground") || hit.transform.CompareTag("Environment"))
            {
                grounded = true;
                // animator.SetBool("isJumping", false);
                /*if (inCombat == false)
                    animator.SetFloat("Animation_Speed", 1);*/

                lastGroundedPosition = hit.point;
                return;
            }
        }
        grounded = false;

        animator.SetFloat("Animation_Speed", 0.5f);
        //animator.SetBool("isJumping", true);
    }

    void OnSneakGagueChange()
    {
        if (SneakGague > 1)
        {
            moveSpeedPenalty = 0.5f;

            /*for (int i = 0; i < mat.Length; i++)
            {
                BlendMode.SetBlendMode(mat[i], BlendMode.Mode.Fade);
                //mat[i].SetFloat("_Mode", 2f);
                mat[i].color = new Color(mat[i].color.r, mat[i].color.g, mat[i].color.b, 0.5f);
            }*/

            float transparency = (100f + (SneakGague * -0.6f)) / 100;

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in BodySkinnedMeshRenderers)
            {
                for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
                {
                    skinnedMeshRenderer.materials[i].SetFloat("_Opacity", transparency);
                    //BlendMode.SetBlendMode(skinnedMeshRenderer.materials[i], BlendMode.Mode.Fade);
                    //skinnedMeshRenderer.materials[i].color = new Color(skinnedMeshRenderer.materials[i].color.r, skinnedMeshRenderer.materials[i].color.g, skinnedMeshRenderer.materials[i].color.b, transparency);
                }
            }

            List<SkinnedMeshRenderer> tempMesh = new List<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in EquipmentManager.instance.currentSkinnedMeshes)
            {
                if (skinnedMeshRenderer)
                    tempMesh.Add(skinnedMeshRenderer);
            }

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in tempMesh)
            {
                for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
                {
                    skinnedMeshRenderer.materials[i].SetFloat("_Opacity", transparency);
                    //BlendMode.SetBlendMode(skinnedMeshRenderer.materials[i], BlendMode.Mode.Fade);
                    //skinnedMeshRenderer.materials[i].color = new Color(skinnedMeshRenderer.materials[i].color.r, skinnedMeshRenderer.materials[i].color.g, skinnedMeshRenderer.materials[i].color.b, transparency);
                }
            }

        }
        else
        {

            moveSpeedPenalty = 1f;
            /*for (int i = 0; i < mat.Length; i++)
            {
                mat[i].color = new Color(mat[i].color.r, mat[i].color.g, mat[i].color.b, 1f);
                //mat[i].SetFloat("_Mode", 0f);
                BlendMode.SetBlendMode(mat[i], BlendMode.Mode.Opaque);
            }*/

            float transparency = (100f + (SneakGague * -1)) / 100f;

            List<SkinnedMeshRenderer> tempMesh = new List<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in EquipmentManager.instance.currentSkinnedMeshes)
            {
                if (skinnedMeshRenderer)
                    tempMesh.Add(skinnedMeshRenderer);
            }

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in tempMesh)
            {
                for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
                {
                    skinnedMeshRenderer.materials[i].SetFloat("_Opacity", transparency);
                    //BlendMode.SetBlendMode(skinnedMeshRenderer.materials[i], BlendMode.Mode.Fade);
                    //skinnedMeshRenderer.materials[i].color = new Color(skinnedMeshRenderer.materials[i].color.r, skinnedMeshRenderer.materials[i].color.g, skinnedMeshRenderer.materials[i].color.b, transparency);
                }
            }

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in BodySkinnedMeshRenderers)
            {
                for (int i = 0; i < skinnedMeshRenderer.materials.Length; i++)
                {
                    skinnedMeshRenderer.materials[i].SetFloat("_Opacity", transparency);
                    //skinnedMeshRenderer.materials[i].color = new Color(skinnedMeshRenderer.materials[i].color.r, skinnedMeshRenderer.materials[i].color.g, skinnedMeshRenderer.materials[i].color.b, transparency);
                    //BlendMode.SetBlendMode(skinnedMeshRenderer.materials[i], BlendMode.Mode.Opaque);
                }
            }
        }
    }

    public void onfrontCollider()
    {
        StartCoroutine(frontCollider());
    }

    public void dodgeOn()
    {
        isDodge = true;
    }

    public void dodgeOff()
    {
        isDodge = false;
    }

    public bool getDodgeBool()
    {
        return isDodge;
    }

    public void ResetSneakGague()
    {
        SneakGague = 0;
    }

    public float GetSneakGague()
    {
        return SneakGague;
    }

    IEnumerator frontCollider()
    {
        frontAttackCollider.enabled = true;
        isAttacking.AddBoolModifier();
        yield return new WaitForSeconds(0.1f);
        isAttacking.RemoveBoolModifier();
        frontAttackCollider.enabled = false;
        frontAttackCollider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
    }

    public void makeOneArrow()
    {
        GameObject arr = Instantiate(PrefabCollect.instance.arrow, firePos.position, transform.rotation);

        arr.GetComponent<Arrow_Logic>().getOwner(gameObject);
    }

    public void isAttackingOff()
    {
        isAttacking.RemoveBoolModifier();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(contactPoint, 0.2f);
    }

    void ResetCombatTime(float keepCombatTime)
    {
        CombatTime = keepCombatTime;
    }

    public void ResetSkillInfo()
    {
        bombReady = false;
        animator.SetBool("isMagicCharging", false);
    }

    void ChangeLayer(Transform trans, int layer)
    {
        trans.gameObject.layer = layer;

        for (int i = 0; i < trans.childCount; i++)
        {
            Transform child = trans.GetChild(i);

            child.gameObject.layer = layer;

            ChangeLayer(child, layer);
        }
    }

    public void DeleteMinion(GameObject Minion)
    {

        SummonedCreature.Remove(Minion);
    }

    public void ReplaceMinions()
    {
        print("소환수들 플레이어 옆으로 위치 변경됨");
        foreach (GameObject minion in SummonedCreature)
        {
            minion.GetComponentInChildren<NPC_AI>().transform.position = transform.position + -(transform.forward * 2);
        }
    }

    public void SwordTrailOn()
    {
        //rightSwordTrail.Emit = true;
        
        rightSwordTrailParticle.Play();
    }

    public void SwordTrailOff()
    {
        //rightSwordTrail.Emit = false;
        //rightSwordTrail._colors[0] = SwordTrailGeryColor;
        rightSwordTrailParticle.Stop();
        //rightSwordTrailParticle.Clear();
    }


    public void FCON()
    {
        rightSwordColider.enabled = true;
        //frontAttackCollider.enabled = true;
    }

    public void FCOFF()
    {
        rightSwordColider.enabled = false;
        //frontAttackCollider.enabled = false;
    }

    public void SwordColliderOn()
    {
        frontAttackCollider.enabled = true;
    }

    public void SwordColliderOff()
    {
        frontAttackCollider.enabled = false;
        isAttacking.RemoveBoolModifier();
        //frontAttackLogic.inUseSkill = null;
        frontAttackCollider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
    }

    public void FrontColliderReset()
    {
        isAttacking.RemoveBoolModifier();
        //frontAttackLogic.inUseSkill = null;
        frontAttackCollider.enabled = false;
        rightSwordTrail.Emit = false;
        frontAttackCollider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
        rightSwordTrail._colors[0] = SwordTrailGeryColor;
    }

    public void attackedByClear()
    {
        frontAttackCollider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
    }

    public void SwordColliderAttackedByClear()
    {

        rightSwordColider.GetComponent<PlayerAttackLogic>().attackedByPlayerEnemyClear();
    }

    public void RDTON()
    {
        //rightDaggerTrail.Emit = true;
        rightDaggerTrailParticle.Play();
    }

    public void RDTOFF()
    {
        //rightDaggerTrail.Emit = false;
        //rightDaggerTrail._colors[0] = SwordTrailGeryColor;
        rightDaggerTrailParticle.Stop();
    }

    public void RDCON()
    {
        rightDaggerColider.enabled = true;
    }

    public void RDCOFF()
    {
        rightDaggerColider.enabled = false;
        
    }

    public void NextAttackReady()
    {
        StartCoroutine(nextAttackReady());
    }

    private IEnumerator nextAttackReady()
    {
        nextAttack = true;
        yield return new WaitForSeconds(1f);
        nextAttack = false;
    }

    public void InvincibleStart()
    {
        StartCoroutine(dodgeInvincible());
    }

    public IEnumerator dodgeInvincible()
    {
        playerstat.dodgeInvincible = true;

        yield return new WaitForSeconds(playerstat.rollInvincibleTime.GetFinalStatValueAsMultiflyFloat());

        playerstat.dodgeInvincible = false;
    }

    public void CameraShake()
    {
        EZCameraShake.CameraShaker.Instance.ShakeOnce(3, 3, 0, 1);
    }

    public void GenerateSFX(string soundName)
    {
        AudioManager.instance.GenerateAudioAndPlaySFX(soundName, GetComponent<Collider>().bounds.center);
    }

    private void footStep01()
    {
        if(grounded)
            AudioSource.PlayOneShot(SoundManager.instance.getRandomFootStepSound(), 0.6f);

    }

    public void UseSneakGague()
    {
        SneakGague = 1;
    }
    #region OldCode definition

    void Turn()
    {

        Quaternion newRotation = Quaternion.LookRotation(movement);

        rigdbody.rotation = Quaternion.Slerp(rigdbody.rotation, newRotation, rotateSpeed * Time.deltaTime);
    }

    public class Arrow
    {
        public ArrowType arrowType;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Exit"))
        {
            DungeonGenerator.instance.EndStage();
        }
    }

    public void RePositionToLastGroundedPosition()
    {
        rigdbody.isKinematic = true;

        transform.position = lastGroundedPosition;

        rigdbody.isKinematic = false;
    }

    /*private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Statue") && Input.GetButtonDown("ItemPickup"))
        {
            animator.SetTrigger("Pray");
            other.GetComponent<PrayToStatue>().ActiveStatue();
        }

    }*/

    #endregion
}

public enum ArrowType {none, Steel, Poison, Explosion }
