using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [Header("전사 스킬")]
    public Skill ShieldBlock;
    public Skill ShieldReflect;

    [Header("사령술사 스킬")]
    public Skill SoulRecovery;

    [Header("도적 스킬")]
    public Skill Sneak;

    [Header("궁수 스킬")]
    public Skill ChargingShot;
    public Skill SteelArrow;
    public Skill PoisonArrow;
    public Skill ExplosionArrow;
    public Skill Kick;

    public static SkillManager instance;

    private void Awake()
    {
        instance = this;
    }
}
