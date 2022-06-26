using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerModel : MonoBehaviour
{
    public static PlayerModel instance;

    [SerializeField] GameObject PlayerPrefab;

    public GameObject playerCube;
    public GameObject MinimapPlayerIcon;
    public RuntimeAnimatorController animatorController;
    public MeleeWeaponTrail weaponTrailPrefab;

    [System.Serializable]
    public struct ColliderInfo
    {
        public Vector3 PlayerColliderCenter;
        public Vector3 PlayerColliderSize;

        public float PlayerColliderRadius;
        public float PlayerColliderHeight;

        public Vector3 PlayerFireFosVec;

        public Vector3 SwordColliderCenter;
        public Vector3 SwordColliderSize;

        public Vector3 RightKnifeColliderCenter;
        public Vector3 RightKnifeColliderSize;

        public Vector3 LeftKnifeColliderCenter;
        public Vector3 LeftKnifeColliderSize;

        public Vector3 AttackColliderCenter;
        public Vector3 AttackColliderSize;

        public Vector3 weaponTrailstart;
        public Vector3 weaponTrailend;

    }

    [System.Serializable]
    public struct PlayerClassPreset
    {
        public List<Item> AdventurePreset;
        public List<Item> WarriorPreset;
        public List<Item> RoguePreset;
        public List<Item> MagePreset;
        public List<Item> ArcherPreset;
        public List<Item> AlchemistPreset;
        public List<Item> NecromancerPreset;
        public List<Item> TinkerPreset;
    }

    [System.Serializable]
    public struct PlayerClassStatPreset
    {
        public CharacterClass characterClass;
        public int StartMaxHp;
        public int StartMaxMp;
        public int StartMaxSteamina;
        public int StartHpGrowthRate;
        public int StartSpeed;
        public float StartCriticalChance;
        public float StartDodgeChance;
    }

    public ColliderInfo colliderInfo;
    public PlayerClassPreset playerClassPreset;
    public PlayerClassStatPreset[] playerClassStatPreset;
    public bool SpawnPlayerLight;
    public GameObject PlayerLight;
    public LayerMask PlayerLayer;
    public LayerMask obstacleMask;

    private Transform armature;
    private Transform body;

    private Transform Hips;
    private Transform RightSwordHold;
    private Transform RightKnifeHold;
    private Transform LeftKnifeHold;
    private Transform Shield;

    private GameObject thisPlayerObject;

    public PlayerComponent playerComponent;
    public AudioMixer audioMixer;


    private void Awake()
    {
        if(!instance)
        {
            instance = this;
        }

        //SetUpPlayerDefaultModel();
        thisPlayerObject = Instantiate(PlayerPrefab, transform.position, transform.rotation);
    }

    private void Start()
    {
        Invoke("ApplyCharacterPreset",0.1f);
    }
    void SetUpPlayerDefaultModel()
    {
        GameObject playerObject =  Instantiate(playerCube, transform.position, transform.rotation);

        thisPlayerObject = playerObject;
        
        playerObject.tag = "Player";
        playerObject.layer = 13;

        armature = playerObject.transform.Find("Armature");
        body = playerObject.transform.Find("Body");

        for (int i = 0; i < playerObject.transform.childCount; i++)
        {
            if (playerObject.transform.GetChild(i).gameObject == armature.gameObject || playerObject.transform.GetChild(i).gameObject == body.gameObject)
            {
                
            } else
            {
                Destroy(playerObject.transform.GetChild(i).gameObject);
            }
            
        }

        GameObject playerIcon = Instantiate(MinimapPlayerIcon, playerObject.transform);
        playerIcon.layer = LayerMask.NameToLayer("Minimap");

        var rigidbody = playerObject.AddComponent<Rigidbody>();
        rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        var capsulecollider = playerObject.AddComponent<CapsuleCollider>();
        capsulecollider.center = colliderInfo.PlayerColliderCenter;
        capsulecollider.radius = colliderInfo.PlayerColliderRadius;
        capsulecollider.height = colliderInfo.PlayerColliderHeight;

        var playerController = playerObject.AddComponent<PlayerController01>();
        playerObject.AddComponent<PlayerInteractionSystem>();
        playerObject.AddComponent<PlayerStats>();
        playerObject.AddComponent<PlayerAnimator>();
        playerObject.AddComponent<AudioListener>();
        playerObject.AddComponent<PlayerSkill>();
        var audiosource = playerObject.AddComponent<AudioSource>();
        //playerObject.AddComponent<CheckPlayerIn>();

        audiosource.outputAudioMixerGroup = audioMixer.outputAudioMixerGroup;

        #region headLook
        var headLook = playerObject.AddComponent<HeadLook>();
        headLook.viewAngle = 130;
        headLook.viewRadius = 30;
        headLook.offset = new Vector3(0, 0, -90);
        headLook.head = armature.Find("Hips").Find("Butt").Find("Spine").Find("Chest").Find("Neck");
        headLook.target = GameObject.FindGameObjectWithTag("playertarget").transform;
        #endregion

        var Firepos = new GameObject("firePos");
        Firepos.transform.SetParent(playerObject.transform);
        Firepos.transform.localPosition = colliderInfo.PlayerFireFosVec;
        Firepos.tag = "FirePos";


        Animator animator = playerObject.GetComponentInChildren<Animator>();
        animator.runtimeAnimatorController = animatorController;

        RightSwordHold = armature.Find("Hips").Find("Butt").Find("Spine").Find("Chest").Find("Shoulder_R").Find("Upper_Arm_R").Find("Lower_Arm_R").Find("Hand_R").Find("Right_Hand_Sword_Hold");
        RightKnifeHold = armature.Find("Hips").Find("Butt").Find("Spine").Find("Chest").Find("Shoulder_R").Find("Upper_Arm_R").Find("Lower_Arm_R").Find("Hand_R").Find("Right_Hand_Knife_Hold");
        LeftKnifeHold = armature.Find("Hips").Find("Butt").Find("Spine").Find("Chest").Find("Shoulder_L").Find("Upper_Arm_L").Find("Lower_Arm_L").Find("Hand_L").Find("Left_Hand_Knife_Hold");
        Shield = armature.Find("Hips").Find("Butt").Find("Spine").Find("Chest").Find("Shoulder_L").Find("Upper_Arm_L").Find("Lower_Arm_L").Find("Hand_L");
        Hips = armature.Find("Hips");

        //=====================================Sword 생성==========================================

        var swordBoxCollider = RightSwordHold.gameObject.AddComponent<BoxCollider>();
        swordBoxCollider.isTrigger = true;
        swordBoxCollider.enabled = false;
        swordBoxCollider.center = colliderInfo.SwordColliderCenter;
        swordBoxCollider.size = colliderInfo.SwordColliderSize;
        var swordrigid = RightSwordHold.gameObject.AddComponent<Rigidbody>();       //rigidbody를 넣어줘야 아이템픽업 트리거에 무기콜라이더가 섞이지 않음
        swordrigid.useGravity = false;
        var swordAttackLogic = swordBoxCollider.gameObject.AddComponent<PlayerAttackLogic>();
        swordAttackLogic.itemType = ItemType.Weapon;
        playerController.rightSwordColider = swordBoxCollider;
        var swordTrailStart = new GameObject("swordTrailStart");
        var swordTrailEnd = new GameObject("swordTrailEnd");
        swordTrailStart.transform.SetParent(RightSwordHold);
        swordTrailEnd.transform.SetParent(RightSwordHold);
        swordTrailStart.transform.localPosition = colliderInfo.weaponTrailstart;
        swordTrailEnd.transform.localPosition = colliderInfo.weaponTrailend;
        var swordTrail = RightSwordHold.gameObject.AddComponent<MeleeWeaponTrail>();
        swordTrail._start = swordTrailStart.transform;
        swordTrail._end = swordTrailEnd.transform;

        //=====================================RightKnife 생성==========================================

        var RightKnifeBoxCollider = RightKnifeHold.gameObject.AddComponent<BoxCollider>();
        RightKnifeBoxCollider.isTrigger = true;
        RightKnifeBoxCollider.enabled = false;
        RightKnifeBoxCollider.center = colliderInfo.RightKnifeColliderCenter;
        RightKnifeBoxCollider.size = colliderInfo.RightKnifeColliderSize;
        var rightKniferigid = RightKnifeHold.gameObject.AddComponent<Rigidbody>();
        rightKniferigid.useGravity = false;
        var rightKnifeAttackLogic = RightKnifeHold.gameObject.AddComponent<PlayerAttackLogic>();
        rightKnifeAttackLogic.itemType = ItemType.Weapon;
        playerController.rightDaggerColider = RightKnifeBoxCollider;
        var rightKnifeTrailStart = new GameObject("rightKnifeTrailStart");
        var rightKnifeTrailEnd = new GameObject("rightKnifeTrailEnd");
        rightKnifeTrailStart.transform.SetParent(RightKnifeHold);
        rightKnifeTrailEnd.transform.SetParent(RightKnifeHold);
        rightKnifeTrailStart.transform.localPosition = new Vector3(-0.25f, 0, 0);
        rightKnifeTrailEnd.transform.localPosition = new Vector3(-1f, 0, 0);
        var rightKnifeTrail = RightKnifeHold.gameObject.AddComponent<MeleeWeaponTrail>();
        rightKnifeTrail._start = rightKnifeTrailStart.transform;
        rightKnifeTrail._end = rightKnifeTrailEnd.transform;

        //=====================================LeftKnife 생성==========================================

        var LeftKnifeBoxCollider = LeftKnifeHold.gameObject.AddComponent<BoxCollider>();
        LeftKnifeBoxCollider.isTrigger = true;
        LeftKnifeBoxCollider.enabled = false;
        LeftKnifeBoxCollider.center = colliderInfo.LeftKnifeColliderCenter;
        LeftKnifeBoxCollider.size = colliderInfo.LeftKnifeColliderSize;
        var LeftKniferigid = LeftKnifeHold.gameObject.AddComponent<Rigidbody>();
        LeftKniferigid.useGravity = false;
        var LeftKnifeAttackLogic = LeftKnifeHold.gameObject.AddComponent<PlayerAttackLogic>();
        LeftKnifeAttackLogic.itemType = ItemType.SecondaryWeapon;

        var leftKnifeTrailStart = new GameObject("leftKnifeTrailStart");
        var leftKnifeTrailEnd = new GameObject("leftKnifeTrailEnd");
        leftKnifeTrailStart.transform.SetParent(LeftKnifeHold);
        leftKnifeTrailEnd.transform.SetParent(LeftKnifeHold);
        leftKnifeTrailStart.transform.localPosition = new Vector3(-0.25f, 0, 0);
        leftKnifeTrailEnd.transform.localPosition = new Vector3(-1f, 0, 0);
        var leftKnifeTrail = LeftKnifeHold.gameObject.AddComponent<MeleeWeaponTrail>();
        leftKnifeTrail._start = leftKnifeTrailStart.transform;
        leftKnifeTrail._end = leftKnifeTrailEnd.transform;

        //==================================Hips 생성==================================================

        var FrontAttackBoxCollider = Hips.gameObject.AddComponent<BoxCollider>();
        FrontAttackBoxCollider.isTrigger = true;
        FrontAttackBoxCollider.enabled = false;
        FrontAttackBoxCollider.center = colliderInfo.AttackColliderCenter;
        FrontAttackBoxCollider.size = colliderInfo.AttackColliderSize;
        playerController.frontAttackCollider = FrontAttackBoxCollider;
        var frontrigid = FrontAttackBoxCollider.gameObject.AddComponent<Rigidbody>();
        frontrigid.useGravity = false;
        var FrontAttackLogic = FrontAttackBoxCollider.gameObject.AddComponent<PlayerAttackLogic>();
        FrontAttackLogic.itemType = ItemType.Weapon;
        Hips.gameObject.AddComponent<contactPoint_Player>();
        Hips.gameObject.layer = 9;

        //=====================================Shield 생성==============================================

        var ShieldCollider = Instantiate(playerComponent.ShieldPrefab, Shield);
        ShieldCollider.AddComponent<Block>();

       // ===============================================================================================



        if (SpawnPlayerLight == true)
        {
            var light = Instantiate(PlayerLight, PlayerLight.transform.position, PlayerLight.transform.rotation,playerObject.transform);
            light.transform.localPosition = new Vector3(0, 2, 1);
        }

        playerController.obstacleMask = obstacleMask;
    }

    private void ApplyCharacterPreset()
    {
        SceneChanger sceneChanger = GameObject.Find("SceneChanger").GetComponent<SceneChanger>();
        GameObject skillui = null;
        switch (sceneChanger.characterclass)
        {
            case CharacterClass.Adventurer:
                foreach (Item item in playerClassPreset.AdventurePreset)
                {
                    EquipmentManager.instance.Equip(item);
                }
                skillui = Instantiate(PrefabCollect.instance.AdventureSkill, SkillUI.instance.SkillParent.transform);
                break;

            case CharacterClass.Warrior:
                foreach (Item item in playerClassPreset.WarriorPreset)
                {
                    EquipmentManager.instance.Equip(item);
                    
                }
                thisPlayerObject.GetComponentInChildren<PlayerStats>().gameObject.AddComponent<WarriorScripts>();
                skillui = Instantiate(PrefabCollect.instance.WarriorSkill, SkillUI.instance.SkillParent.transform);
                break;

            case CharacterClass.Rogue:
                foreach (Item item in playerClassPreset.RoguePreset)
                {
                    EquipmentManager.instance.Equip(item);
                    
                }
                thisPlayerObject.GetComponentInChildren<PlayerStats>().gameObject.AddComponent<RogueScripts>();
                skillui = Instantiate(PrefabCollect.instance.RogueSkill, SkillUI.instance.SkillParent.transform);
                break;

            case CharacterClass.Wizard:
                foreach (Item item in playerClassPreset.MagePreset)
                {
                    EquipmentManager.instance.Equip(item);
                    
                }
                thisPlayerObject.GetComponentInChildren<PlayerStats>().gameObject.AddComponent<WizardScripts>();
                skillui = Instantiate(PrefabCollect.instance.MageSkill, SkillUI.instance.SkillParent.transform);
                break;

            case CharacterClass.Archer:
                foreach (Item item in playerClassPreset.ArcherPreset)
                {
                    EquipmentManager.instance.Equip(item);
                    
                }
                thisPlayerObject.GetComponentInChildren<PlayerStats>().gameObject.AddComponent<ArcherScripts>();
                skillui = Instantiate(PrefabCollect.instance.ArcherSkill, SkillUI.instance.SkillParent.transform);
                break;

            case CharacterClass.Alchemist:
                foreach (Item item in playerClassPreset.AlchemistPreset)
                {
                    EquipmentManager.instance.Equip(item);
                }
                skillui = Instantiate(PrefabCollect.instance.AlchemistSkill, SkillUI.instance.SkillParent.transform);
                break;

            case CharacterClass.Necromancer:
                foreach (Item item in playerClassPreset.NecromancerPreset)
                {
                    EquipmentManager.instance.Equip(item);
                }
                skillui = Instantiate(PrefabCollect.instance.NecromencerSkill, SkillUI.instance.SkillParent.transform);
                break;

            case CharacterClass.Tinker:
                foreach (Item item in playerClassPreset.TinkerPreset)
                {
                    EquipmentManager.instance.Equip(item);
                }

                skillui = Instantiate(PrefabCollect.instance.TinkerSkill, SkillUI.instance.SkillParent.transform);
                break;
        }

        UIManager.instance.skillParent = skillui.GetComponentInChildren<SkillParent>();

        //SkillUI.instance.Skill.SetActive(!SkillUI.instance.Skill.activeSelf);

        switch (sceneChanger.characterclass)
        {
            case CharacterClass.Necromancer:
                UIManager.instance.Soulbar.gameObject.SetActive(true);
                //Debug.Log("플레이어의 직업이 " + CharacterClass.Necromancer + " 기 때문에 soulBar가 활성화됨");
                break;

            case CharacterClass.Warrior:
                UIManager.instance.Ragebar.gameObject.SetActive(true);
                //Debug.Log("플레이어의 직업이 " + CharacterClass.Warrior + " 기 때문에 rageBar가 활성화됨");
                break;

            case CharacterClass.Archer:
                UIManager.instance.ArcherArrowStack.gameObject.SetActive(true);
                break;

            case CharacterClass.Wizard:
                UIManager.instance.Manabar.gameObject.SetActive(true);
                break;

            default:
                //UIManager.instance.Manabar.gameObject.SetActive(true);
                //Debug.Log("플레이어가 마나를 사용하는 직업이기 때문에 manaBar가 활성화됨");
                break;
        }

        PlayerManager.PlayerInfo thisplayer = new PlayerManager.PlayerInfo();
        thisplayer.playerobject = thisPlayerObject;
        thisplayer.Skillobject = SkillUI.instance.SkillParent;
        PlayerManager.instance.Players.Add(thisplayer);

        thisPlayerObject.GetComponentInChildren<PlayerStats>().playerClass = sceneChanger.characterclass;
        //thisPlayerObject.GetComponentInChildren<PlayerStats>().SettingPlayerSpeedStat();

        var playerStat = thisPlayerObject.GetComponentInChildren<PlayerStats>();

        thisPlayerObject.GetComponentInChildren<PlayerAnimator>().SetPlayerAniamtorWeight();

        foreach (PlayerClassStatPreset statPreset in playerClassStatPreset)
        {
            if (statPreset.characterClass == sceneChanger.characterclass)
            {
                playerStat.MoveSpeedStat.AddIntModifier(statPreset.StartSpeed);
                playerStat.ChangeMaxHp(statPreset.StartMaxHp);
                playerStat.ChangeMaxMP(statPreset.StartMaxMp);
                playerStat.dodge.AddIntModifier((int)statPreset.StartDodgeChance);
                playerStat.CritialChange.AddPercentModifier(statPreset.StartCriticalChance);
                playerStat.MaxStamina.AddIntModifier(statPreset.StartMaxSteamina);
                playerStat.playerCurrentStamina = statPreset.StartMaxSteamina;
                break;
            }
        }
        skillui.GetComponentInChildren<SkillParent>().InitSkill();

        SkillQuickSlot.instance.ActiveDefaultSkillSlot(sceneChanger.characterclass);
    }

}

[System.Serializable]
public class PlayerComponent
{
    [SerializeField]
    private Rigidbody RigidBody;
    [SerializeField]
    private AudioSource AudioSource;
    [SerializeField]
    private Component BoxdCollider;
    [SerializeField]
    private Component ItemPickUpText;
    [SerializeField]
    private Component PlayerContoller01;
    [SerializeField]
    private Component PlayerStats;
    [SerializeField]
    private Component PlayerAnimator;
    [SerializeField]
    public GameObject ShieldPrefab;
}
