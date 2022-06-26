using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillUI : MonoBehaviour
{
    public static SkillUI instance;
    public GameObject skillSlot;

    public TextMeshProUGUI SkillLvlPoint;

    public GameObject SkillParent;
    public GameObject Skill;

    private void Start()
    {
        instance = this;
    }

    public void TurnOffUi()
    {
        Skill.SetActive(!Skill.activeSelf);
    }

    public void SkillInit()
    {

    }
}
