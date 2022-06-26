using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabCollect : MonoBehaviour
{
    [Header("투사체 프리팹")]
    public GameObject arrow;
    public GameObject arrow2;
    public GameObject Chargingarrow;
    public GameObject magicBall;
    public GameObject Bomb;
    public GameObject FireBall;
    public GameObject SoulFire;
    public GameObject SoulSkull;
    public GameObject GhostArrow;
    public GameObject Skill_ChainOfPurgatory;
    public GameObject IceSpear;
    public GameObject LightingOrb;
    public GameObject MagicOrb;
    public GameObject BouncingBall;
    public GameObject ThrowingKnife;
    public GameObject DoubleShackle;
    public GameObject PoisonBomb;
    public GameObject GrapplingArrow;
    public GameObject ThrowRock01;
    public GameObject FlameTick;
    public GameObject shockBomb;
    public GameObject magicMissileSprayProjectile;
    public GameObject magicBomb;
    public GameObject flameExplosionChargingParticle;
    public GameObject flameExplosion;
    public GameObject FireFlameSummon;
    public GameObject PurpleFlameSummon;
    public GameObject ShieldShockWaveTick;

    [Header("기타 프리팹")]
    public GameObject SkeletonMinion;
    public GameObject SkeletonArcherMinion;
    public GameObject SkeletonKnightMinion;
    public GameObject LichMinion;
    public GameObject Room_Minimap;
    public GameObject AudioSource;
    public GameObject GoldCoin;
    public GameObject SilverCoin;
    public GameObject CopperCoin;
    public GameObject FloatingText;
    public GameObject ChainOfPurgatoryEffect;
    public GameObject OverChargeParticle;
    public GameObject BearTrap;
    public GameObject ExplosionTrap;
    public GameObject PoisonGas;
    public GameObject SoulDrainParticle;
    public GameObject LifeDrainParticle;
    public Material[] DefaultVertexMat;
    public Material[] DefaultVertexMatWithStencil;
    public GameObject PoisonParticle;
    public GameObject BurnParticle;
    public GameObject SlowDownParticle;
    public GameObject LightingRenderer;
    public GameObject ExplodeParticle;
    public GameObject nothingBomb;
    public GameObject StunParticle;
    public GameObject MaimedParticle;
    public GameObject SteelArrowCell;
    public GameObject PoisonArrowCell;
    public GameObject ExplosionArrowCell;
    public GameObject HealthGenerationParticle;
    public GameObject DamageIncreaseParticle;
    public GameObject PurpleShield;
    public GameObject DiggingDustParticle;
    public GameObject miniBomb;
    public GameObject miniTrap;
    public GameObject keyPickUp;
    public GameObject ShopIcon;
    public GameObject BossIcon;
    public GameObject TreasureIcon;
    public GameObject DecreseDefenseParticle;
    public Color eliteMonsterNameColor;
    public GameObject levelUpParticle;
    public Sprite mouseLeftButtonIcon;
    public Sprite mouseRightButtonIcon;
    public Sprite mouseMiddleButtonIcon;
    public GameObject ShieldShockWaveParticle;
    public GameObject BuffDebuffIconPrefab;
    public GameObject defenseIncreseParticle;
    public GameObject MoveSpeedIncreaseParticle;
    public GameObject defenseDecreaseParticle;
    public GameObject moveSpeedDecreaseParticle;
    public GameObject damageDecreaseParticle;
    public GameObject ManaDrainParticle;
    public GameObject dropHeart;
    public GameObject PurpleShield2;
    public GameObject consumableEffect;
    public Color commonNameColor;
    public Color rareNameColor;
    public Color uniqueNameColor;
    public Color epicNameColor;


    [Header("직업스킬창 프리팹")]
    public GameObject BombRobot;
    public GameObject CrossbowTurret;
    public GameObject MissileDrone;
    public GameObject missile;
    public GameObject IceShield;

    public GameObject AdventureSkill;
    public GameObject WarriorSkill;
    public GameObject ArcherSkill;
    public GameObject RogueSkill;
    public GameObject MageSkill;
    public GameObject AlchemistSkill;
    public GameObject NecromencerSkill;
    public GameObject TinkerSkill;

    [Space]

    [Header("스킬 프리팹")]
    public Skill FlameSkill;
    public Skill ShockBombSkill;
    public Skill BlockSkill;
    public Skill BackAttack;
    public Skill ThrowingKnifeSkill;
    public Skill SteelArrowSkill;
    public Skill PoisonArrowSkill;
    public Skill ExplosionArrowSkill;
    public Skill FireBallSkill;
    public Skill BearTrapSkill;
    public Skill ExplosionTrapSkill;
    public Skill superArmor;
    public Skill CoupDeGrace;
    public Skill ShieldReflect;
    public Skill QuickShot;
    public Skill PoisonBombSkill;
    public Skill magicMissileSpraySkill;
    public Skill magicbombSkill;
    public Skill IceShieldSkill;
    public Skill IceSpearSkill;
    public Skill LightingOrbSkill;
    public Skill ChargingShotSkill;
    public Skill flameExplosionSkill;
    public Skill overChargeSkill;
    public Skill Flame;
    public Skill ShieldBlock;
    public Skill Sneak;
    public Skill ChargingShot;
    public Skill continuousFiring;
    public Skill MultiShot;
    public Skill ShieldShockWave;

    [Space]

    public BuffNDebuffObject StunImmunity;
    public BuffNDebuffObject SlowdownImmunity;


    [Space]

    public LayerMask npcMask;
    public LayerMask playerMask;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public static PrefabCollect instance;

    private void Awake()
    {
        instance = this;
    }
}
