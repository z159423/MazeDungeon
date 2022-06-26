using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.Events;

public class PadCursor : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    [SerializeField]
    private RectTransform cursorTransform;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RectTransform canvasRectTransform;
    [SerializeField]
    private float cursorSpeed = 1000f;
    [SerializeField]
    private float padding = 35f;

    private bool previousMouseState;
    private bool previousMouseState2;
    private bool previousMouseState3;
    private bool previousMouseState4;
    private Mouse virtualMouse;
    private Mouse currentMouse;
    private Camera mainCamera;

    private string previouseControlScheme = "";
    public const string gamepadScheme = "Gamepad";
    public const string mouseScheme = "Keyboard&Mouse";
    private string currentScheme = "";

    public UnityEvent OnChangeControl;

    public static PadCursor instance;

    private void OnEnable()
    {
        instance = this;

        if(Gamepad.current != null)
        {
            currentScheme = gamepadScheme;
        }
        else
        {
            currentScheme = mouseScheme;
        }

        mainCamera = Camera.main;
        currentMouse = Mouse.current;

        if(virtualMouse == null)
        {
            virtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
        }
        else if(!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        if(cursorTransform != null)
        {
            Vector2 position = cursorTransform.anchoredPosition;
            InputState.Change(virtualMouse.position, position);
        }

        InputSystem.onAfterUpdate += UpdateMotion;
        //playerInput.onControlsChanged += OnContraolsChanged;
    }

    public void AddEvent()
    {
        mainCamera = Camera.main;
        currentMouse = Mouse.current;

        if (virtualMouse == null)
        {
            virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
        }
        else if (!virtualMouse.added)
        {
            InputSystem.AddDevice(virtualMouse);
        }

        InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

        InputSystem.onAfterUpdate += UpdateMotion;
        //playerInput.onControlsChanged += OnContraolsChanged;
    }

    /*private void OnDisable()
    {
        if(virtualMouse != null && virtualMouse.added)
            InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        playerInput.onControlsChanged -= OnContraolsChanged;
    }*/

    private void Update()
    {
        if (currentScheme != GetCurrentCursorScheme())
        {
            //Debug.Log("OnChangeControl");
            OnChangeControl.Invoke();
            currentScheme = GetCurrentCursorScheme();
        }

        /*if (playerInput.currentControlScheme == mouseScheme && previouseControlScheme != mouseScheme)
        {
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
            previouseControlScheme = mouseScheme;
        }
        else if (playerInput.currentControlScheme == gamepadScheme && previouseControlScheme != gamepadScheme)
        {
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
            AnchorCursor(currentMouse.position.ReadValue());
            previouseControlScheme = gamepadScheme;
        }*/
    }

    private void OnDestroy()
    {
        if (virtualMouse != null && virtualMouse.added)
            InputSystem.RemoveDevice(virtualMouse);
        InputSystem.onAfterUpdate -= UpdateMotion;
        //playerInput.onControlsChanged -= OnContraolsChanged;
        //Debug.LogError(2);
    }

    private void UpdateMotion()
    {
        if (virtualMouse == null || Gamepad.current == null)
        {
            return;
        }

        Vector2 deltaValue = Gamepad.current.leftStick.ReadValue();

        deltaValue *= cursorSpeed * Time.unscaledDeltaTime;


        Vector2 currentPosition = virtualMouse.position.ReadValue();
        Vector2 newPosition = currentPosition + deltaValue;

        newPosition.x = Mathf.Clamp(newPosition.x, padding, Screen.width - padding);
        newPosition.y = Mathf.Clamp(newPosition.y, padding, Screen.height - padding);

        if (cursorTransform.gameObject.activeSelf)
            InputState.Change(virtualMouse.position, newPosition);
        //InputState.Change(virtualMouse.delta, deltaValue);

        bool aButtonIsPressed = Gamepad.current.aButton.IsPressed();
        if (previousMouseState != Gamepad.current.aButton.isPressed && cursorTransform.gameObject.activeSelf)                 //가상 마우스의 왼쪽 클릭 - 패드의 A
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, aButtonIsPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState = aButtonIsPressed;
        }

        bool bButtonIsPressed = Gamepad.current.bButton.IsPressed();
        if (previousMouseState2 != Gamepad.current.bButton.isPressed && cursorTransform.gameObject.activeSelf)                 //가상 마우스의 오른쪽 클릭 - 패드의 A
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Right, bButtonIsPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState2 = bButtonIsPressed;
        }

        bool crossButtonIsPressed = Gamepad.current.crossButton.IsPressed();
        if (previousMouseState3 != Gamepad.current.crossButton.isPressed && cursorTransform.gameObject.activeSelf)                 //가상 마우스의 왼쪽 클릭 - 패드의 A
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Left, crossButtonIsPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState3 = crossButtonIsPressed;
        }

        bool circleButtonIsPressed = Gamepad.current.circleButton.IsPressed();
        if (previousMouseState4 != Gamepad.current.circleButton.isPressed && cursorTransform.gameObject.activeSelf)                 //가상 마우스의 오른쪽 클릭 - 패드의 A
        {
            virtualMouse.CopyState<MouseState>(out var mouseState);
            mouseState.WithButton(MouseButton.Right, circleButtonIsPressed);
            InputState.Change(virtualMouse, mouseState);
            previousMouseState4 = circleButtonIsPressed;
        }

        AnchorCursor(newPosition);
    }

    private void AnchorCursor(Vector2 position)
    {
        if (!cursorTransform.gameObject.activeSelf)
            return;

        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, position, canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out anchoredPosition);
        cursorTransform.anchoredPosition = anchoredPosition;
    }

    public void OnContraolsChanged()
    {
        //Debug.Log(playerInput.currentControlScheme);

        if (currentMouse == null)
            currentMouse = Mouse.current;

        if (playerInput.currentControlScheme == mouseScheme && previouseControlScheme != mouseScheme)
        {
            cursorTransform.gameObject.SetActive(false);
            Cursor.visible = true;
            currentMouse.
                WarpCursorPosition(
                virtualMouse.position.ReadValue());
            previouseControlScheme = mouseScheme;
        }
        else if(playerInput.currentControlScheme == gamepadScheme && previouseControlScheme != gamepadScheme)
        {
            cursorTransform.gameObject.SetActive(true);
            Cursor.visible = false;
            InputState.Change(
                virtualMouse.position, 
                currentMouse.position.ReadValue());
            AnchorCursor(currentMouse.position.ReadValue());
            previouseControlScheme = gamepadScheme;
        }
    }

    public void GamePadCursorActive()
    {
        cursorTransform.gameObject.SetActive(true);
        Cursor.visible = false;
        //InputState.Change(virtualMouse.position, currentMouse.position.ReadValue());
        //AnchorCursor(currentMouse.position.ReadValue());
        //previouseControlScheme = gamepadScheme;
    }

    public void MouseCursorActive()
    {
        cursorTransform.gameObject.SetActive(false);
        Cursor.visible = true;
        //currentMouse.WarpCursorPosition(virtualMouse.position.ReadValue());
        //previouseControlScheme = mouseScheme;
    }


    public RectTransform getCursorTransform()
    {
        return cursorTransform;
    }

    public Vector2 GetCurrentCursorPosition()
    {
        if(playerInput.currentControlScheme == gamepadScheme)
        {
            return virtualMouse.position.ReadValue();
        } 
        else if (playerInput.currentControlScheme == mouseScheme)
        {
            if(currentMouse == null)
                currentMouse = Mouse.current;

            return currentMouse.position.ReadValue();
        }

        return currentMouse.position.ReadValue();
    }

    public string GetCurrentCursorScheme()
    {
        //Debug.LogError(playerInput.currentControlScheme);

        if (playerInput.currentControlScheme == gamepadScheme)
        {
            return gamepadScheme;
        }
        else if (playerInput.currentControlScheme == mouseScheme)
        {
            return mouseScheme;
        }

        return mouseScheme;
    }

    public int GetCurrentBindingIndex()
    {
        var currentGamePad = Gamepad.current;

        if (currentGamePad is UnityEngine.InputSystem.XInput.XInputController || currentGamePad is UnityEngine.InputSystem.DualShock.DualShockGamepad)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

}
