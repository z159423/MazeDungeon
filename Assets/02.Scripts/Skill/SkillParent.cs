using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillParent : MonoBehaviour
{
    public List<DragSkill> dragSkills = new List<DragSkill>();

    public delegate void Skilldelegate();
    public Skilldelegate OnSkillPointChange;

    public void GetDragSkills()
    {
        dragSkills.AddRange(GetComponentsInChildren<DragSkill>());
    }

    public void InitSkill()
    {
        GetDragSkills();

        foreach (DragSkill skill in dragSkills)
        {
            skill.InitThisSkill(this);
        }
    }
}
