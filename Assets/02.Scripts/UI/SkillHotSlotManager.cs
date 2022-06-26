using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHotSlotManager : MonoBehaviour
{
    public List<SkillSlot> skillSlotList = new List<SkillSlot>();

    public static SkillHotSlotManager instance;

    private void Start()
    {
        instance = this;
    }

    public void MoveToSkillQuickSlotAuto(Skill skill)
    {
        foreach(SkillSlot skillSlot in skillSlotList)
        {
            if (skillSlot.skill == null)
            {
                skillSlot.MoveToThisSkillQuickSlot(skill);
                return;
            }
        }
    }
}
