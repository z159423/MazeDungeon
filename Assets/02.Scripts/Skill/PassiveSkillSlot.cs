using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PassiveSkillSlot : MonoBehaviour
{
    public Skill skill;
    public Image cover;
    public Image ShortcutKeyImage;

    [Space]
    public TextMeshProUGUI useResourceValue;
    public TextMeshProUGUI bindingKeyCode;

    private PlayerStats playerStat;
    PlayerInputAction playerInputActions;

    public InputAction inputAction;

    void Start()
    {
        playerStat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        playerInputActions = new PlayerInputAction();
        KeyBindindManager.instance.OnUpdateKeyBindsCallBack += UpdateKeyBind;

        UpdateKeyBind();

        OnChangeControl();
    }

    public void UpdateKeyBind()
    {
        playerInputActions.Disable();

        playerInputActions = new PlayerInputAction();

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            playerInputActions.LoadBindingOverridesFromJson(rebinds);

        if (skill == PrefabCollect.instance.ShieldBlock || skill == PrefabCollect.instance.ChargingShot || skill == PrefabCollect.instance.Flame)
        {
            inputAction = playerInputActions.Player.Shield;
        }
        else if(skill == PrefabCollect.instance.Sneak)
        {
            inputAction = playerInputActions.Player.Sneak;
        }

        //KeyBindindManager.instance.DisplayCurrentControllerShortcut(bindingKeyCode, ShortcutKeyImage, inputAction);
    }

    public void OnChangeControl()
    {
        if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
        {

            KeyBindindManager.instance.DisplayShortcutText(bindingKeyCode, ShortcutKeyImage, inputAction.bindings[0].effectivePath);
        }
        else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
        {
            KeyBindindManager.instance.DisplayCurrentControllerShortcut(bindingKeyCode, ShortcutKeyImage, inputAction);
        }
    }
}
