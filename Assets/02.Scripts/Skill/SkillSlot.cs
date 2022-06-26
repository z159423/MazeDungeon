using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlot : MonoBehaviour
{

    public Image image;
    [SerializeField]
    public Skill skill;
    public Slider slider;
    public Sprite backgroundImage;
    public Image padHotKeyImage;
    public float SkillCoolTimeMulti = 1;
    

    [Space]
    public TextMeshProUGUI useResourceValue;
    public TextMeshProUGUI bindingKeyCode;
    public Color manaColor;
    public Color rageColor;
    public Color dryingUpColor;

    [Space]
    public PlayerSkill playerskill;
    private PlayerStats playerStat;

    void Start()
    {
        playerskill = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerSkill>();
        playerStat = playerskill.GetComponent<PlayerStats>();
        slider = GetComponentInChildren<Slider>();

        playerStat.OnStatChangeCallBack += OnStatChange;
    }

    void Update()
    {
        //if(skill != null)
        //    slider.maxValue = skill.coolTime;

        if(slider.value > slider.minValue)
        {
            slider.value -= Time.deltaTime * SkillCoolTimeMulti;
        }
    }

    public void MoveToThisSkillQuickSlot(Skill skill)
    {
        this.skill = skill;
        image.sprite = skill.Sprite;
        slider.maxValue = skill.skillLeveling[playerskill.GetSkillLevel(skill)].coolTime;
        slider.value = slider.maxValue;
    }

    public void initializationSlot()
    {
        image.sprite = backgroundImage;
        skill = null;
        slider.maxValue = 0;
        useResourceValue.text = "";
    }

    public void ApplySkillThisSlot(Skill skill, bool startCoolTimeMax)
    {
        var slots = GetComponentInParent<SkillQuickSlot>().skillSlot;
        foreach (SkillSlot slot1 in slots)       //다른 슬롯에 같은 스킬이 있는지 확인
        {
            if (slot1.skill == skill)
            {
                slot1.initializationSlot();
            }
        }

        image.sprite = skill.Sprite;
        this.skill = skill;
        this.slider.maxValue = skill.skillLeveling[playerskill.GetSkillLevel(skill)].coolTime;
        if(startCoolTimeMax)
            this.slider.value = this.slider.maxValue;

        useResourceValue.text = "";

        if (skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp > 0)
        {
            //useResourceValue.color = manaColor;

            useResourceValue.text = skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp.ToString();

            if (playerStat.playerCurrentMp >= skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp)
            {
                useResourceValue.color = manaColor;

                useResourceValue.text = skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp.ToString();
            }
            else
            {
                useResourceValue.color = dryingUpColor;

                useResourceValue.text = skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp.ToString();
            }
        }
        else if(skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage > 0)
        {
            //useResourceValue.color = rageColor;
            useResourceValue.text = skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage.ToString();

            if (playerStat.playerCurrentRage >= skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp)
            {
                useResourceValue.color = rageColor;

                useResourceValue.text = skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage.ToString();
            }
            else
            {
                useResourceValue.color = dryingUpColor;

                useResourceValue.text = skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage.ToString();
            }
        }
    }

    public void OnStatChange()
    {
        if(skill)
        {
            if (skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp > 0)
            {
                if (playerStat.playerCurrentMp >= skill.skillLeveling[playerskill.GetSkillLevel(skill)].needMp)
                {
                    useResourceValue.color = manaColor;
                }
                else
                {
                    useResourceValue.color = dryingUpColor;
                }
            }
            else if (skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage > 0)
            {
                if (playerStat.playerCurrentRage >= skill.skillLeveling[playerskill.GetSkillLevel(skill)].needRage)
                {
                    useResourceValue.color = rageColor;
                }
                else
                {
                    useResourceValue.color = dryingUpColor;
                }
            }
        }
        
    }

    public void ChangeSkillMaxCoolTime()
    {
        this.slider.maxValue = skill.skillLeveling[playerskill.GetSkillLevel(skill)].coolTime;
    }
}
