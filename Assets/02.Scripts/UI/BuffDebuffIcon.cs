using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffDebuffIcon : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public BuffNDebuffObject buffNDebuffObject;

    [SerializeField] private Color buffCoverColor;
    [SerializeField] private Color buffBackgroundColor;

    [SerializeField] private Color debuffCoverColor;
    [SerializeField] private Color debuffBackgroundColor;

    [SerializeField] private Image cover;
    [SerializeField] private Image background;
    [SerializeField] private Image iconImage;
    [SerializeField] private Slider slider;

    private void Update()
    {
        slider.value = slider.maxValue - buffNDebuffObject.buffOrDebuff.GetCurrentRunningTime();
    }

    public static void GenerateNewIcon(Transform parent, BuffNDebuffObject Object)
    {
        var icon = Instantiate(PrefabCollect.instance.BuffDebuffIconPrefab, parent);
        icon.GetComponentInChildren<BuffDebuffIcon>().GetInfo(Object);
    }

    public static void DeleteIcon(BuffNDebuffObject Object)
    {
        if(Object.buffOrDebuff.iconObject != null)
            Destroy(Object.buffOrDebuff.iconObject);
    }

    public void GetInfo(BuffNDebuffObject Object)
    {
        iconImage.sprite = Object.buffOrDebuff.iconImage;

        Object.buffOrDebuff.iconObject = gameObject;

        buffNDebuffObject = Object;

        if (Object.buffOrDebuff.buffORDebuff == buffORDebuff.Buff)
        {
            cover.color = buffCoverColor;
            background.color = buffBackgroundColor;
        }
        else if (Object.buffOrDebuff.buffORDebuff == buffORDebuff.Debuff)
        {
            cover.color = debuffCoverColor;
            background.color = debuffBackgroundColor;
        }

        slider.maxValue = Object.buffOrDebuff.EndTime;
    }

    public void UpdateInfo(BuffOrDebuff Object)
    {
        slider.maxValue = Object.EndTime;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        BuffDebuffDetail.instance.ChangeText(buffNDebuffObject);
        BuffDebuffDetail.instance.ShowBuffDebuffDetailStatUI();

    }

    void IPointerExitHandler.OnPointerExit(PointerEventData data)
    {
        BuffDebuffDetail.instance.HideBuffDebuffDetailUI();
    }
}
