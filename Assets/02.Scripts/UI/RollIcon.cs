using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class RollIcon : MonoBehaviour
{
    public float requireStamina;
    public Image cover;
    public Image ShortcutKeyImage; 

    [Space]
    public TextMeshProUGUI useResourceValue;
    public Color UseableColor;
    public Color UnUseableColor;

    public TextMeshProUGUI bindingKeyCode;

    public InputAction inputAction;

    private PlayerStats playerStat;
    private PlayerInputAction playerInputActions;

    // Start is called before the first frame update
    void Start()
    {
        playerStat = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        playerInputActions = new PlayerInputAction();
        KeyBindindManager.instance.OnUpdateKeyBindsCallBack += UpdateKeyBind;
        playerStat.OnStatChangeCallBack += OnChangeStat;

        UpdateKeyBind();
    }

    public void OnChangeStat()
    {
        requireStamina = playerStat.RollRequireStamina.GetFinalStatValue();
        useResourceValue.text = requireStamina.ToString();

        if (requireStamina < playerStat.playerCurrentStamina)
        {
            cover.enabled = false;
            useResourceValue.color = UseableColor;
        }
        else
        {
            cover.enabled = true;
            useResourceValue.color = UnUseableColor;
        }
    }

    public void UpdateKeyBind()
    {
        playerInputActions.Disable();

        playerInputActions = new PlayerInputAction();

        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            playerInputActions.LoadBindingOverridesFromJson(rebinds);

        inputAction = playerInputActions.Player.Roll;

        //KeyBindindManager.instance.DisplayCurrentControllerShortcut(bindingKeyCode, ShortcutKeyImage, playerInputActions.Player.Roll);

        OnChangeControl();

    }

    public void OnChangeControl()
    {
        if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
        {
            KeyBindindManager.instance.DisplayShortcutText(bindingKeyCode, ShortcutKeyImage, playerInputActions.Player.Roll.bindings[0].effectivePath);
        }
        else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
        {
            KeyBindindManager.instance.DisplayCurrentControllerShortcut(bindingKeyCode, ShortcutKeyImage, playerInputActions.Player.Roll);
        }
    }
}
