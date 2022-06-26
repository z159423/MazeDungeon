using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DefaultStatUi : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public StatType statType;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        StatDetail.instance.ChangeText(statType);
        StatDetail.instance.ShowDefaultStatUI();

    }

    void IPointerExitHandler.OnPointerExit(PointerEventData data)
    {
        StatDetail.instance.HideDefaultStatUI();
    }
}

public enum StatType { StartHp, HpGrowth, MoveSpeed, CriticalChance, DodgeChance, Damage, Defence, MaxHp, MaxSteamina, AttackSpeed }
