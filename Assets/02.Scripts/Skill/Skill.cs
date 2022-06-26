using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill/new Skill")]
public class Skill : ScriptableObject
{
    public string Name = "New Skill";
    public Sprite Sprite;

    public int maxLevel = 2;

    public SkillInfo[] skillLeveling;

    /*public int needMp;
    public int needSoul;
    public int needRage;
    public float damageFactor = 1;
    public bufforDebuff BufforDebuff = new bufforDebuff();
    public BuffNDebuffObject buffNDebuffObject = null;
    public float coolTime;*/

    public bool PassiveSkill;
    public string skillDescription;

    [System.Serializable]
    public class SkillInfo
    {
        public int needMp;
        public int needSoul;
        public int needRage;
        public float damageFactor = 1;
        public int value1;
        public int value2;
        public float value3;
        //public bufforDebuff BufforDebuff = new bufforDebuff();
        public BuffNDebuffObject buffNDebuffObject = null;
        public BuffNDebuffObject buffNDebuffObject2 = null;
        public float coolTime;
    }
}




