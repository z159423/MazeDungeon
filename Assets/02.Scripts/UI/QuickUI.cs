using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QuickUI : MonoBehaviour
{
    private PlayerInputAction playerInputActions;

    public TextMeshProUGUI InventoryText;
    public Image InventoryHotKeyImage;

    public TextMeshProUGUI MapText;
    public Image MapHotKeyImage;

    public TextMeshProUGUI ArtifactText;
    public Image ArtifactHotKeyImage;

    public TextMeshProUGUI SkillText;
    public Image SkillHotKeyImage;

    public TextMeshProUGUI SkillPointText;

    public static QuickUI instance;

    private void Awake()
    {
        instance = this;
        

        KeyBindindManager.instance.OnUpdateKeyBindsCallBack += UpdateKeyBinds;
    }

    private void Start()
    {
        UpdateKeyBinds();
    }

    private void OnEnable()
    {
        //KeyBindindManager.instance.OnUpdateKeyBindsCallBack += UpdateKeyBinds;
    }

    private void OnDisable()
    {
        //KeyBindindManager.instance.OnUpdateKeyBindsCallBack -= UpdateKeyBinds;
    }

    private void UpdateKeyBinds()
    {
        playerInputActions = new PlayerInputAction();

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            playerInputActions.LoadBindingOverridesFromJson(rebinds);

        playerInputActions.UI.Enable();

        //InventoryText.text = InputControlPath.ToHumanReadableString(playerInputActions.UI.Inventory.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        //MapText.text = InputControlPath.ToHumanReadableString(playerInputActions.UI.Map.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        //ArtifactText.text = InputControlPath.ToHumanReadableString(playerInputActions.UI.Artifact.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        //SkillText.text = InputControlPath.ToHumanReadableString(playerInputActions.UI.Skill.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        //InitShortCutDisplay();

        OnChangeControl();
    }

    public void OnChangeSkillPoint(int skillPoint)
    {
        if(skillPoint > 0)
        {
            SkillPointText.gameObject.SetActive(true);
            SkillPointText.text = "+" + skillPoint;
        }
        else
        {
            SkillPointText.gameObject.SetActive(false);
        }
    }

    public void SetBindingKeyTest(string bindingKeyText, TextMeshProUGUI bindingKeyCode, Image image)
    {
        bindingKeyCode.text = "";

        //Debug.LogError(bindingKeyText);

        if (bindingKeyText.Equals("Left Button"))
        {
            //ShortcutKeyImage.enabled = true;
            //ShortcutKeyImage.sprite = PrefabCollect.instance.mouseLeftButtonIcon;

            bindingKeyCode.text = "M1";
        }
        else if (bindingKeyText.Equals("Right Button"))
        {
            //ShortcutKeyImage.enabled = true;
            //ShortcutKeyImage.sprite = PrefabCollect.instance.mouseRightButtonIcon;

            bindingKeyCode.text = "M2";
        }
        else if (bindingKeyText.Equals("Middle Button"))
        {
            //ShortcutKeyImage.enabled = true;
            //ShortcutKeyImage.sprite = PrefabCollect.instance.mouseMiddleButtonIcon;

            bindingKeyCode.text = "M3";
        }
        else if (bindingKeyText.Equals("Control"))
        {
            image.gameObject.SetActive(false);
            bindingKeyCode.text = "Ctrl";
        }
        else
        {
            image.gameObject.SetActive(false);
            bindingKeyCode.text = bindingKeyText;
        }
    }

    private void InitShortCutDisplay()
    {
        KeyBindindManager.instance.DisplayCurrentControllerShortcut(InventoryText, InventoryHotKeyImage, playerInputActions.UI.Inventory);
        KeyBindindManager.instance.DisplayCurrentControllerShortcut(MapText, MapHotKeyImage, playerInputActions.UI.Map);
        KeyBindindManager.instance.DisplayCurrentControllerShortcut(ArtifactText, ArtifactHotKeyImage, playerInputActions.UI.Artifact);
        KeyBindindManager.instance.DisplayCurrentControllerShortcut(SkillText, SkillHotKeyImage, playerInputActions.UI.Skill);
    }

    public void OnChangeControl()
    {
        if(PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
        {
            KeyBindindManager.instance.DisplayShortcutText(InventoryText, InventoryHotKeyImage, playerInputActions.UI.Inventory.bindings[0].effectivePath);
            KeyBindindManager.instance.DisplayShortcutText(MapText, MapHotKeyImage, playerInputActions.UI.Map.bindings[0].effectivePath);
            KeyBindindManager.instance.DisplayShortcutText(ArtifactText, ArtifactHotKeyImage, playerInputActions.UI.Artifact.bindings[0].effectivePath);
            KeyBindindManager.instance.DisplayShortcutText(SkillText, SkillHotKeyImage, playerInputActions.UI.Skill.bindings[0].effectivePath);
        }
        else if(PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
        {
            KeyBindindManager.instance.DisplayCurrentControllerShortcut(InventoryText, InventoryHotKeyImage, playerInputActions.UI.Inventory);
            KeyBindindManager.instance.DisplayCurrentControllerShortcut(MapText, MapHotKeyImage, playerInputActions.UI.Map);
            KeyBindindManager.instance.DisplayCurrentControllerShortcut(ArtifactText, ArtifactHotKeyImage, playerInputActions.UI.Artifact);
            KeyBindindManager.instance.DisplayCurrentControllerShortcut(SkillText, SkillHotKeyImage, playerInputActions.UI.Skill);
        }
    }
}
