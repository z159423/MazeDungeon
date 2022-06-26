using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(Skill))]
public class SkillEditor : Editor
{
    private Skill skill;
    private bool _initialized;

    public override void OnInspectorGUI()
    {

        _Initialize();

        skill.Name = EditorGUILayout.TextField("Skill 이름", skill.Name);
        EditorGUILayout.Space();
        skill.Sprite = (Sprite)EditorGUILayout.ObjectField("Icon", skill.Sprite, typeof(Sprite), false);

        /*skill.needMp = EditorGUILayout.IntField("Need Mana", skill.needMp);
        skill.needSoul = EditorGUILayout.IntField("Need Soul", skill.needSoul);
        skill.needRage = EditorGUILayout.IntField("Need Rage", skill.needRage);
        skill.damageFactor = EditorGUILayout.FloatField("Damage Factor", skill.damageFactor);
        skill.coolTime = EditorGUILayout.FloatField("Cool Time", skill.coolTime);
        skill.PassiveSkill = EditorGUILayout.Toggle("Passive", skill.PassiveSkill);

        EditorGUILayout.Space();
        skill.BufforDebuff.debuff = (buffType)EditorGUILayout.EnumPopup("Debuff : ", skill.BufforDebuff.debuff);
        if (skill.BufforDebuff.debuff != buffType.none)
        {
            skill.BufforDebuff.Duration = EditorGUILayout.FloatField("Debuff Duration : ", skill.BufforDebuff.Duration);
        }
        if(skill.BufforDebuff.debuff == buffType.poison)
        {
            skill.BufforDebuff.poisonDamage = EditorGUILayout.IntField("Poision Damage : ", skill.BufforDebuff.poisonDamage);
        }

        skill.buffNDebuffObject = (BuffNDebuffObject)EditorGUILayout.ObjectField("buffNDebuffObject", skill.buffNDebuffObject, typeof(BuffNDebuffObject), false);*/

        EditorGUILayout.Space();

        skill.skillDescription = EditorGUILayout.TextField("Skill 설명", skill.skillDescription);

        EditorUtility.SetDirty(skill);
        
    }

    private void _Initialize()
    {
        if (!_initialized)
        {
            skill = (Skill)target;
            _initialized = true;
        }

    }
}
