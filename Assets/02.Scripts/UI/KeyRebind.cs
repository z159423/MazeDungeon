using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeyRebind : MonoBehaviour
{
    [SerializeField] private InputActionReference Action = null;
    [SerializeField] private TextMeshProUGUI BindingKeyText = null;
    [SerializeField] private TextMeshProUGUI WaitForInputText = null;
    [SerializeField] private Image BindingKeyImage = null;

    [SerializeField] private Button rebindBtn;

    private void Start()
    {
        rebindBtn.onClick.AddListener(delegate ()
        {
            StartRebinding();
        });
    }

    private void OnEnable()
    {
        CheckingBindingKey();

        PadCursor.instance.OnChangeControl.AddListener(OnChangeControl);
    }

    private void OnDisable()
    {
        PadCursor.instance.OnChangeControl.RemoveListener(OnChangeControl);
    }

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public void StartRebinding()
    {
        Action.action.Disable();

        BindingKeyText.gameObject.SetActive(false);
        WaitForInputText.gameObject.SetActive(true);

        int bindingIndex = 0;

        if(PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
        {
            bindingIndex = 0;
        }
        else if(PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
        {
            bindingIndex = 1;
        }

        rebindingOperation = Action.action.PerformInteractiveRebinding(bindingIndex)
            //.WithControlsExcluding("<Keyboard>/escape")
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operate => RebindComplete())
            .Start();
    }

    private void RebindComplete()
    {
        int bindingIndex = 0;

        if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
        {
            bindingIndex = 0;
        }
        else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
        {
            bindingIndex = 1;
        }

        BindingKeyText.text = InputControlPath.ToHumanReadableString(
            Action.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);

        rebindingOperation.Dispose();

        BindingKeyText.gameObject.SetActive(true);
        WaitForInputText.gameObject.SetActive(false);

        Action.action.Enable();
    }

    public void CheckingBindingKey()
    {
        if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
        {
            KeyBindindManager.instance.DisplayShortcutText(BindingKeyText, BindingKeyImage, Action.action.bindings[0].effectivePath);
        }
        else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
        {
            KeyBindindManager.instance.DisplayCurrentControllerShortcut(BindingKeyText, BindingKeyImage, Action.action);
        }

        //KeyBindindManager.instance.DisplayCurrentControllerShortcut(BindingKeyText, BindingKeyImage, Action.action);

        /*try
        {
            int bindingIndex = Action.action.GetBindingIndexForControl(Action.action.controls[0]);

            Debug.LogError(bindingIndex);

            BindingKeyText.text = InputControlPath.ToHumanReadableString(
                Action.action.bindings[bindingIndex].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);
        }
        catch
        {
            BindingKeyText.text = "";
        }*/
    }

    public void OnChangeControl()
    {
        if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.mouseScheme)
        {
            KeyBindindManager.instance.DisplayShortcutText(BindingKeyText, BindingKeyImage, Action.action.bindings[0].effectivePath);
        }
        else if (PadCursor.instance.GetCurrentCursorScheme() == PadCursor.gamepadScheme)
        {
            KeyBindindManager.instance.DisplayCurrentControllerShortcut(BindingKeyText, BindingKeyImage, Action.action);
        }
    }

}
