using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SkillToolTip : MonoBehaviour
{
    public LocalizeStringEvent SkillNameLocalize;
    public LocalizeStringEvent SkillLevelLocalize;
    public LocalizeStringEvent SkillCoolTimeLocalize;
    public LocalizeStringEvent SkillResourceNameLocalize;
    public LocalizeStringEvent SkillDetailLocalize;

    public Image skillImage;

    [Space]

    public string skillLevel = "";
    public string coolTime = "";
    public string BT1 = "";
    public string BD1 = "";
    public string BRT1 = "";
    public string DM1 = "";
    public string B1value1 = "";
    public string B1value2 = "";
    public string B1value3 = "";

    public string Mana = "";
    public string Rage = "";
    public string value1 = "";
    public string value2 = "";
    public string value3 = "";

    public string BT2 = "";
    public string BD2 = "";
    public string BRT2 = "";
    public string B2value1 = "";
    public string B2value2 = "";
    public string B2value3 = "";

    public bool isSkillTab = false;

    public static SkillToolTip instance;

    private RectTransform canvasRectTransform;
    private Vector2 outVector2;
    private Vector3 toolTipPotition;

    private void Awake()
    {
        instance = this;

        var UI = GameObject.FindGameObjectWithTag("UI");
        canvasRectTransform = UI.GetComponent<RectTransform>();

        HideToolTip();
    }

    private void Update()
    {
        SetPositionOnSkill();

        if (!gameObject.activeSelf)
            HideToolTip();
    }


    public void GetSkillInfo(Skill skill, int skillLevel)
    {
        this.skillLevel = (skillLevel + 1).ToString();
        this.coolTime = skill.skillLeveling[skillLevel].coolTime.ToString();

        if(skill.skillLeveling[skillLevel].buffNDebuffObject)
        {
            this.BT1 = skill.skillLeveling[skillLevel].buffNDebuffObject.buffOrDebuff.EndTime.ToString();
            this.BD1 = skill.skillLeveling[skillLevel].buffNDebuffObject.buffOrDebuff.Damage.ToString();
            this.BRT1 = skill.skillLeveling[skillLevel].buffNDebuffObject.buffOrDebuff.RepeatTime.ToString();
            this.B1value1 = skill.skillLeveling[skillLevel].buffNDebuffObject.buffOrDebuff.value1.ToString();
            this.B1value2 = skill.skillLeveling[skillLevel].buffNDebuffObject.buffOrDebuff.value2.ToString();
            this.B1value3 = skill.skillLeveling[skillLevel].buffNDebuffObject.buffOrDebuff.value3.ToString();
        }

        if(skill.skillLeveling[skillLevel].buffNDebuffObject2)
        {
            this.BT2 = skill.skillLeveling[skillLevel].buffNDebuffObject2.buffOrDebuff.EndTime.ToString();
            this.BD2 = skill.skillLeveling[skillLevel].buffNDebuffObject2.buffOrDebuff.Damage.ToString();
            this.BRT2 = skill.skillLeveling[skillLevel].buffNDebuffObject2.buffOrDebuff.RepeatTime.ToString();
            this.B2value1 = skill.skillLeveling[skillLevel].buffNDebuffObject2.buffOrDebuff.value1.ToString();
            this.B2value2 = skill.skillLeveling[skillLevel].buffNDebuffObject2.buffOrDebuff.value2.ToString();
            this.B2value3 = skill.skillLeveling[skillLevel].buffNDebuffObject2.buffOrDebuff.value3.ToString();
        }

        this.DM1 = (skill.skillLeveling[skillLevel].damageFactor * 100).ToString();
        this.Mana = skill.skillLeveling[skillLevel].needMp.ToString();
        this.Rage = skill.skillLeveling[skillLevel].needRage.ToString();
        this.value1 = skill.skillLeveling[skillLevel].value1.ToString();
        this.value2 = skill.skillLeveling[skillLevel].value2.ToString();
        this.value3 = skill.skillLeveling[skillLevel].value3.ToString();

        SkillNameLocalize.StringReference.SetReference("Skill", skill.Name);
        SkillLevelLocalize.StringReference.SetReference("Skill", "SkillLevel");


        if (skill.PassiveSkill)
        {
            SkillCoolTimeLocalize.gameObject.SetActive(false);
        }
        else
        {
            SkillCoolTimeLocalize.StringReference.SetReference("Skill", "SkillCoolTime");
            SkillCoolTimeLocalize.gameObject.SetActive(true);
        }

        if(skill.skillLeveling[skillLevel].needMp > 0)
        {
            SkillResourceNameLocalize.StringReference.SetReference("Skill", "ManaNeed");
            SkillResourceNameLocalize.gameObject.SetActive(true);
        }
        else if(skill.skillLeveling[skillLevel].needRage > 0)
        {
            SkillResourceNameLocalize.StringReference.SetReference("Skill", "RageNeed");
            SkillResourceNameLocalize.gameObject.SetActive(true);
        }
        else
        {
            SkillResourceNameLocalize.gameObject.SetActive(false);
        }
        
        SkillDetailLocalize.StringReference.SetReference("Skill", skill.Name + "-Detail");

        skillImage.sprite = skill.Sprite;
    }

    public void ShowToolTip(bool isSkillTab)
    {
        SetPositionOnSkill();
        gameObject.SetActive(true);
        this.isSkillTab = isSkillTab;
    }

    public void HideToolTip()
    {
        gameObject.SetActive(false);
        this.isSkillTab = false;
    }

    private void SetPositionOnSkill()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, PadCursor.instance.GetCurrentCursorPosition(), GameObject.FindGameObjectWithTag("UI_Camera").GetComponent<Camera>(), out outVector2);
        toolTipPotition = outVector2;
        transform.localPosition = toolTipPotition + new Vector3(0, 0, -50);
    }
}
