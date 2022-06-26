using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class FloatingTextController : MonoBehaviour
{
    public static FloatingText popupText;
    private static GameObject canvas;
    private static Camera UI_camera;
    private static RectTransform canvasRectTransform;
    private static Canvas Canvas2;


    public static void Initialize()
    {
        Canvas2 = GameObject.Find("UI").GetComponent<Canvas>();
        canvasRectTransform = Canvas2.transform as RectTransform;
        canvas = GameObject.Find("UI");
        UI_camera = GameObject.FindWithTag("UI_Camera").GetComponent<Camera>();

        if (!popupText)
            popupText = Resources.Load<FloatingText>("03.Prefabs/PopupTextParent");
        if(popupText == null)
            Debug.Log("불러오기 실패");
        //Debug.Log("FloatingText Initialize!");
    }


    public static void CreateFloatingText(string text, Transform location, string tag, int size, GameObject other, bool effectString = false, GameObject owner = null)
    {
        FloatingText instance = Instantiate(popupText, Canvas2.transform);
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(location.position + new Vector3(0,1));

        Vector2 localPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, UI_camera, out localPointerPosition);

        //print("screenPosition " + screenPosition);
        //print("location.position " + location.position);
        //print("localPointerPosition " + localPointerPosition);

        instance.transform.SetParent(canvas.transform, false);
        instance.transform.SetSiblingIndex(0);
        //instance.transform.localPosition = localPointerPosition;
        //instance.transform.position += new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));
        instance.targetPosition = location.position;
        instance.SetText(text);
        instance.SetColor(tag, other);
        instance.setTextSize(size);
        if (owner != null)
            instance.setOwner(owner);

        if (effectString)
        {
            instance.GetComponentInChildren<LocalizeStringEvent>().StringReference.SetReference("UI", text);
        }
    }

    public static void GenerateDamageFloatingText(string text, Transform location, int size, NPC_Type npcType, GameObject owner = null, bool isCritical = false)
    {
        if (!UIManager.instance.settingMenu.DamagePopUpToggle.isOn)
            return;

        FloatingText instance = Instantiate(popupText, Canvas2.transform);

        instance.transform.SetParent(canvas.transform, false);
        instance.transform.SetSiblingIndex(0);
        instance.targetPosition = location.position;
        instance.SetText(text);
        instance.setTextSize(size);
        instance.SetColorByNpcType(npcType, isCritical);

        if (owner != null)
            instance.setOwner(owner);
    }

    public static void GenerateHealFloatingText(string text, Transform location, int size, NPC_Type npcType, GameObject owner = null)
    {
        if (!UIManager.instance.settingMenu.DamagePopUpToggle.isOn)
            return;

        FloatingText instance = Instantiate(popupText, Canvas2.transform);

        instance.transform.SetParent(canvas.transform, false);
        instance.transform.SetSiblingIndex(0);
        instance.targetPosition = location.position;
        instance.SetText(text);
        instance.setTextSize(size);
        instance.SetBuffColor(npcType);

        if (owner != null)
            instance.setOwner(owner);
    }

    public static void GenerateShieldFloatingText(string text, Transform location, int size, NPC_Type npcType, GameObject owner = null)
    {
        if (!UIManager.instance.settingMenu.DamagePopUpToggle.isOn)
            return;

        FloatingText instance = Instantiate(popupText, Canvas2.transform);

        instance.transform.SetParent(canvas.transform, false);
        instance.transform.SetSiblingIndex(0);
        instance.targetPosition = location.position;
        instance.SetText(text);
        instance.setTextSize(size);
        instance.SetShieldColor();

        if (owner != null)
            instance.setOwner(owner);
    }

    public static void GenerateDebuffFloatingText(string key, Transform location, int size, NPC_Type npcType, GameObject owner = null)
    {
        if (!UIManager.instance.settingMenu.EffectPopUpToggle.isOn)
            return;

        FloatingText instance = Instantiate(popupText, Canvas2.transform);

        instance.transform.SetParent(canvas.transform, false);
        instance.transform.SetSiblingIndex(0);
        instance.targetPosition = location.position;
        instance.GetComponentInChildren<LocalizeStringEvent>().StringReference.SetReference("UI", key);
        instance.setTextSize(size);
        instance.SetColorByNpcType(npcType);

        if (owner != null)
            instance.setOwner(owner);
    }

    public static void GenerateBuffFloatingText(string key, Transform location, int size, NPC_Type npcType, GameObject owner = null)
    {
        if (!UIManager.instance.settingMenu.EffectPopUpToggle.isOn)
            return;

        FloatingText instance = Instantiate(popupText, Canvas2.transform);

        instance.transform.SetParent(canvas.transform, false);
        instance.transform.SetSiblingIndex(0);
        instance.targetPosition = location.position;
        instance.GetComponentInChildren<LocalizeStringEvent>().StringReference.SetReference("UI", key);
        instance.setTextSize(size);
        instance.SetBuffColor(npcType);

        if (owner != null)
            instance.setOwner(owner);
    }
}
