using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions Input;
    public static InputManager Instance;

    public event Action OnInteraction = delegate { };
    public event Action OnDrop = delegate { };
    public event Action OnClick = delegate { };
    public event Action OnClickCanceled = delegate { };
    public event Action OnSlot1 = delegate { };
    public event Action OnSlot2 = delegate { };
    void Awake()
    {
        Instance = this;
        Input = new InputSystem_Actions();
    }
    
    void OnEnable() 
    {
        Input.Enable();
        Input.Player.Interact.performed += context => InteractionPressed();
        Input.Player.Drop.performed += context => DropPressed();
        Input.Player.Click.performed += context => ClickPressed();
        Input.Player.Click.canceled += context => ClickCanceled();

        Input.Player.Slot1.performed += context => Slot1Pressed();
        Input.Player.Slot2.performed += context => Slot2Pressed();

        Input.Player.Crouch.performed += context => CrouchPressed();
    }

    private void CrouchPressed()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    void OnDisable()
    {
        Input.Disable();
    }
    
    public Vector2 GetPlayerMovement() => Input.Player.Move.ReadValue<Vector2>();
    public Vector2 GetMouseDelta() => Input.Player.Look.ReadValue<Vector2>();

    public bool GetJumped() => Input.Player.Jump.triggered;
    private void InteractionPressed() => OnInteraction();
    private void DropPressed() => OnDrop();
    private void Slot1Pressed() => OnSlot1();
    private void Slot2Pressed() => OnSlot2();
    
    private void ClickPressed() => OnClick();
    private void ClickCanceled() => OnClickCanceled();
    public bool ClickHolding() => Input.Player.Click.triggered;

    public bool WatchHolding() => Input.Player.Watch.IsPressed();
}
