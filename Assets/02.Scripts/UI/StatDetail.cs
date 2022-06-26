using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using TMPro;


public class StatDetail : MonoBehaviour
{
    public LocalizeStringEvent localize;

    public GameObject UI;
    public Camera Camera;
    public Vector3 offset;

    [SerializeField]
    private PadCursor padCursor;
    private RectTransform canvasRectTransform;
    private Vector3 UIposiiton;
    private Vector2 outVector2;
    private Vector3 DefaultPosition;

    public static StatDetail instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        canvasRectTransform = UI.GetComponent<RectTransform>();

        DefaultPosition = transform.localPosition;

        HideDefaultStatUI();
    }

    // Update is called once per frame
    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, PadCursor.instance.GetCurrentCursorPosition(), Camera, out outVector2);
        UIposiiton = outVector2;
        transform.localPosition = UIposiiton + new Vector3(0, 0, -50) + offset;
    }

    public void ChangeText(StatType statType)
    {
        switch(statType)
        {
            case StatType.StartHp:
                localize.StringReference.SetReference("UI", "StartHp");
                break;

            case StatType.HpGrowth:
                localize.StringReference.SetReference("UI", "HpGrowth");
                break;

            case StatType.MoveSpeed:
                localize.StringReference.SetReference("UI", "StartMoveSpeed");
                break;

            case StatType.CriticalChance:
                localize.StringReference.SetReference("UI", "StartCriticalChance");
                break;

            case StatType.DodgeChance:
                localize.StringReference.SetReference("UI", "DodgeChance");
                break;

            case StatType.Damage:
                localize.StringReference.SetReference("UI", "attackPower");
                break;

            case StatType.Defence:
                localize.StringReference.SetReference("UI", "defencePower");
                break;

            case StatType.MaxHp:
                localize.StringReference.SetReference("UI", "StartHp");
                break;

            case StatType.MaxSteamina:
                localize.StringReference.SetReference("UI", "MaxSteamina");
                break;

            case StatType.AttackSpeed:
                localize.StringReference.SetReference("UI", "AttackSpeed");
                break;
        }
    }

    public void ShowDefaultStatUI()
    {
        gameObject.SetActive(true);
    }

    public void HideDefaultStatUI()
    {
        gameObject.SetActive(false);
        transform.localPosition = DefaultPosition;
    }
}
