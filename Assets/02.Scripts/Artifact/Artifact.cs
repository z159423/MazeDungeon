using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Artifact", menuName = "Artifact/Artifact")]
public class Artifact : ScriptableObject
{
    public ArtifactType artifactType;
    public ArtifactTier artifactTier;
    public string nameLocalizationKey;
    public string descriptionLocalizationKey;
    public GameObject ArtifactObject;
    [Space]
    public GameObject FollowerObject;
    [Space]
    public GameObject[] SummonMonsters;
    [Space]
    public List<BuffNDebuffObject> Debuff = new List<BuffNDebuffObject>();
    public List<BuffNDebuffObject> Buff = new List<BuffNDebuffObject>();
}

public enum ArtifactTier { Common, Rare, Unique, Epic }

public enum ArtifactType{SilverRing, SilverNecklace, RubyRing, SapphireRing, ToothNecklace, BurnedStick, GoldRing, GoldNecklace, LostFairy, BabySpider, TreeRoots, Stone,
    BlueCrystal, RedCrystal, YellowCrystal, BloodyGem, ParasiteSpiderEgg, ProtectionPendant, VampirePendant, FrozenHeart, Feather, ElfBread, SoulCollector, MagmaStone, StoneGauntlet
        ,SnakeFang, Icicle, ShieldCharm, DodgeCharm, ThirdEye, ThunderBeastFoot, MoneyBag, ChickenLeg, AncientSnailShell, BerserkerEyePatch, BerserkerHeadBand, BombBag, FighterGlove
        ,FlameNeedle, IceCube, NecromancerOldBook, OgreClub, SkeletonKey, ThunderBeastClaw, TrapBag, BrokenDirk, GriffinFeather
}
