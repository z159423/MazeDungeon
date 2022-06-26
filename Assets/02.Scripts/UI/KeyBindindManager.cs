using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;
using TMPro;

public class KeyBindindManager : MonoBehaviour
{
    public static KeyBindindManager instance;

    public delegate void OnUpdateKeyBinds();
    public OnUpdateKeyBinds OnUpdateKeyBindsCallBack;

    [SerializeField]
    private PlayerInput playerInput;

    public GamepadIcons xbox;
    public GamepadIcons ps4;

    [Serializable]
    public struct GamepadIcons
    {
        public Sprite buttonSouth;
        public Sprite buttonNorth;
        public Sprite buttonEast;
        public Sprite buttonWest;
        public Sprite startButton;
        public Sprite selectButton;
        public Sprite leftTrigger;
        public Sprite rightTrigger;
        public Sprite leftShoulder;
        public Sprite rightShoulder;
        public Sprite dpad;
        public Sprite dpadUp;
        public Sprite dpadDown;
        public Sprite dpadLeft;
        public Sprite dpadRight;
        public Sprite leftStick;
        public Sprite rightStick;
        public Sprite leftStickPress;
        public Sprite rightStickPress;

        public Sprite GetSprite(string controlPath)
        {
            // From the input system, we get the path of the control on device. So we can just
            // map from that to the sprites we have for gamepads.
            switch (controlPath)
            {
                case "Button South": return buttonSouth;
                case "Button North": return buttonNorth;
                case "Button East": return buttonEast;
                case "Button West": return buttonWest;
                case "Start": return startButton;
                case "Select": return selectButton;
                case "Left Trigger": return leftTrigger;
                case "Right Trigger": return rightTrigger;
                case "Left Shoulder": return leftShoulder;
                case "Right Shoulder": return rightShoulder;
                case "D-Pad": return dpad;
                case "D-Pad/Up": return dpadUp;
                case "D-Pad/Down": return dpadDown;
                case "D-Pad/Left": return dpadLeft;
                case "D-Pad/Right": return dpadRight;
                case "Left Stick": return leftStick;
                case "Right Stick": return rightStick;
                case "Left Stick Press": return leftStickPress;
                case "Right Stick Press": return rightStickPress;
                case "Left Stick/Left": return leftStick;
                case "Left Stick/Right": return leftStick;
                case "Left Stick/Down": return leftStick;
                case "Left Stick/Up": return leftStick;
                case "Right Stick/Left": return rightStick;
                case "Right Stick/Right": return rightStick;
                case "Right Stick/Down": return rightStick;
                case "Right Stick/Up": return rightStick;
            }
            return null;
        }
    }

    private PlayerInputAction playerInputActions;

    private void Awake()
    {
        instance = this;

        OnUpdateKeyBindsCallBack += UpdateKeyBinds;
    }

    private void Start()
    {
        playerInputActions = new PlayerInputAction();
    }

    public void UpdateKeyBinds()
    {
        Debug.Log("키 설정 업데이트됨");
    }

    public void UpdateHotKeyDisplay(TextMeshProUGUI text, Image image, string controlPath)
    {
        if (string.IsNullOrEmpty(controlPath) || text == null || image == null)
            return;

        var currentGamePad = Gamepad.current;

        var omitDeviceName = InputControlPath.ToHumanReadableString(controlPath, InputControlPath.HumanReadableStringOptions.OmitDevice);

        //Debug.LogError(omitDeviceName);

        var icon = default(Sprite);
        if (currentGamePad is UnityEngine.InputSystem.DualShock.DualShockGamepad || currentGamePad is UnityEngine.InputSystem.DualShock.DualShock4GamepadHID || currentGamePad is UnityEngine.InputSystem.DualShock.DualShock3GamepadHID)
            icon = ps4.GetSprite(omitDeviceName);
        else if (currentGamePad is UnityEngine.InputSystem.XInput.XInputController || currentGamePad is UnityEngine.InputSystem.XInput.XInputControllerWindows)
            icon = xbox.GetSprite(omitDeviceName);

        if (icon != null)
        {
            text.gameObject.SetActive(false);
            image.sprite = icon;
            image.gameObject.SetActive(true);
        }
        else
        {
            text.gameObject.SetActive(true);
            image.gameObject.SetActive(false);
            text.text = InputControlPath.ToHumanReadableString(controlPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
    }

    public void DisplayShortcutText(TextMeshProUGUI text, Image image, string effectivePath)
    {
        text.gameObject.SetActive(true);
        SetBindingKeyToShort(InputControlPath.ToHumanReadableString(effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice), text);
        image.gameObject.SetActive(false);
    }

    public void DisplayCurrentControllerShortcut(TextMeshProUGUI text, Image image, InputAction inputAction)
    {
        int bindingIndex = PadCursor.instance.GetCurrentBindingIndex();

        try
        {
            UpdateHotKeyDisplay(text, image, inputAction.bindings[bindingIndex].effectivePath);
        }
        catch
        {
            text.gameObject.SetActive(true);
            SetBindingKeyToShort(InputControlPath.ToHumanReadableString(inputAction.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice), text);
            image.gameObject.SetActive(false);
        }
    }

    public void SetBindingKeyToShort(string bindingKeyText, TextMeshProUGUI bindingKeyCode)
    {
        bindingKeyCode.text = "";

        if (bindingKeyText.Equals("Left Button"))
        {
            bindingKeyCode.text = "M1";
        }
        else if (bindingKeyText.Equals("Right Button"))
        {
            bindingKeyCode.text = "M2";
        }
        else if (bindingKeyText.Equals("Middle Button"))
        {
            bindingKeyCode.text = "M3";
        }
        else if (bindingKeyText.Equals("Control"))
        {
            bindingKeyCode.text = "Ctrl";
        }
        else
        {
            bindingKeyCode.text = bindingKeyText;
        }
    }

    public void ChangeGamepadImage(TextMeshProUGUI text,Image image, InputAction inputAction)
    {
        int bindingIndex = PadCursor.instance.GetCurrentBindingIndex();

        try
        {
            image.gameObject.SetActive(true);
            UpdateHotKeyDisplay(text, image, inputAction.bindings[bindingIndex].effectivePath);
        }
        catch
        {
            text.gameObject.SetActive(true);
            SetBindingKeyToShort(InputControlPath.ToHumanReadableString(inputAction.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice), text);
            image.gameObject.SetActive(false);
        }
    }
}
