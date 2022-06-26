using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillLevelUpButton : MonoBehaviour,IPointerExitHandler, IPointerEnterHandler
{
    public DragSkill dragSkill;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if(dragSkill.playerSkill.CheckPlayerHaveSkill(dragSkill.skill) && dragSkill.skill.maxLevel >= dragSkill.playerSkill.GetSkillLevel(dragSkill.skill)+1)
        {
            SkillToolTip.instance.GetSkillInfo(dragSkill.skill, dragSkill.playerSkill.GetSkillLevel(dragSkill.skill)+1);
            SkillToolTip.instance.ShowToolTip(true);
        }
        else if(!dragSkill.playerSkill.CheckPlayerHaveSkill(dragSkill.skill))
        {
            SkillToolTip.instance.GetSkillInfo(dragSkill.skill, dragSkill.playerSkill.GetSkillLevel(dragSkill.skill));
            SkillToolTip.instance.ShowToolTip(true);
        }
        
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        SkillToolTip.instance.HideToolTip();

    }
}
