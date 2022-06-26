using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;


public class EnchantPanel : MonoBehaviour
{
    public Text EnchantNameText;
    public Text EnchantExplanationText;

    public string Value, Percent, Time, RepeatTime, EnchantLevel, value1, value2, value3, value4, value5;

    public LocalizeStringEvent EnchantNameLocalize;
    public LocalizeStringEvent EnchantExplanationLocalize;

    public void SetEnchantName(Enchant enchant)
    {
        PlayerEnchant EnchantSetting = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEnchant>();

        var InitialValue = EnchantSetting.GetEnchantInitialValue(enchant.enchants.enchantType);
        InitialValue *= enchant.enchants.EnchantCurrentLevel;

        if (InitialValue < 1)
            InitialValue *= 100;

        Value = InitialValue.ToString();

        var InitialPercent = EnchantSetting.GetEnchantInitialPercent(enchant.enchants.enchantType);
        InitialPercent *= enchant.enchants.EnchantCurrentLevel;

        if (InitialPercent < 1)
            InitialPercent *= 100;

        Percent = InitialPercent.ToString();

        var InitialTime = EnchantSetting.GetEnchantInitialTime(enchant.enchants.enchantType);
        InitialTime *= enchant.enchants.EnchantCurrentLevel;

        Time = InitialTime.ToString();

        var InitialRepeatTime = EnchantSetting.GetEnchantInitialRepeatTime(enchant.enchants.enchantType);
        //InitialRepeatTime *= enchant.enchants.EnchantLevel;

        RepeatTime = InitialRepeatTime.ToString();

        EnchantLevel = enchant.enchants.EnchantCurrentLevel.ToString();

        EnchantNameLocalize.StringReference.SetReference("Enchant", "Name." + System.Enum.GetName(typeof(EnchantType), enchant.enchants.enchantType));
        EnchantExplanationLocalize.StringReference.SetReference("Enchant", "Explanation." + System.Enum.GetName(typeof(EnchantType), enchant.enchants.enchantType));

        value1 = enchant.value1.element[enchant.enchants.EnchantCurrentLevel - 1].value.ToString();
        value2 = enchant.value2.element[enchant.enchants.EnchantCurrentLevel - 1].value.ToString();
        value3 = enchant.value3.element[enchant.enchants.EnchantCurrentLevel - 1].value.ToString();
        value4 = enchant.value4.element[enchant.enchants.EnchantCurrentLevel - 1].value.ToString();
        value5 = enchant.value5.element[enchant.enchants.EnchantCurrentLevel - 1].value.ToString();

        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
