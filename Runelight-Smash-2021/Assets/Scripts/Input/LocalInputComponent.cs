using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(InputComponent))]

public class LocalInputComponent : MonoBehaviour
{
    // Required Components
    private InputComponent inputComponent;

    // Input Variables
    private Vector2 joystick;
    private Queue<FighterInputAction> actionQueue = new Queue<FighterInputAction>();

    void Start()
    {
        inputComponent = GetComponent<InputComponent>();
    }

    void FixedUpdate()
    {
        inputComponent.SetJoystickInput(joystick);

        if (actionQueue.Count <= 0)
        {
            inputComponent.SetFighterActionInput(FighterInputAction.none);
            return;
        }

        FighterInputAction action = actionQueue.Dequeue();

        inputComponent.SetFighterActionInput(action);
    }

    public void HandleJump(InputAction.CallbackContext input)
    {
        if (input.started)
        {
            actionQueue.Enqueue(new FighterInputAction(ActionType.Jump, ActionStrength.None));
        }
        if (input.performed)
        {
            actionQueue.Enqueue(new FighterInputAction(ActionType.Jump, ActionStrength.Weak));
        }
        if (input.canceled)
        {
            actionQueue.Enqueue(new FighterInputAction(ActionType.Jump, ActionStrength.Strong));
        }
    }

    public void HandleJumpHold(InputAction.CallbackContext input)
    {
        bool isJumpKeyDown = input.control.IsActuated();

        if (input.performed || input.canceled)
        {
            inputComponent.isJumpKeyDown = isJumpKeyDown;
        }
    }

    public void HandleGroundMovement(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            joystick = input.ReadValue<Vector2>();
        }
        if (input.canceled)
        {
            joystick = Vector2.zero;
        }
    }

    public void HandleDashXBox(InputAction.CallbackContext input)
    {
        Vector2 direction = input.ReadValue<Vector2>();

        if (input.canceled)
        {
            actionQueue.Enqueue(new FighterInputAction(ActionType.Dash, ActionStrength.None));
        }
        if (input.performed)
        {
            actionQueue.Enqueue(new FighterInputAction(ActionType.Dash, ActionStrength.Strong, direction));
        }
    }

    public void HandleDashKeyboard(InputAction.CallbackContext input)
    {
        float direction = input.ReadValue<float>();

        if (input.canceled)
        {
            actionQueue.Enqueue(new FighterInputAction(ActionType.Dash, ActionStrength.None));
        }
        if (input.performed)
        {
            actionQueue.Enqueue(new FighterInputAction(ActionType.Dash, ActionStrength.Strong, direction));
        }
    }

    public void HandleFallThrough(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            actionQueue.Enqueue(new FighterInputAction(ActionType.FallThrough, ActionStrength.Strong));
        }
    }

    public void HandleShield(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            inputComponent.isShieldKeyDown = true;
        }
        if (input.canceled)
        {
            inputComponent.isShieldKeyDown = false;
        }
    }

    public void HandleAttack(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            AttackInputStruct value = input.ReadValue<AttackInputStruct>();
            Debug.Log($"Attack Performed {value.joystick} {value.button}");
        }
    }

    public void HandleSmash(InputAction.CallbackContext input)
    {
        AttackInputStruct value = input.ReadValue<AttackInputStruct>();

        if (input.started)
        {
            Debug.Log($"Charging {value.joystick} {value.button}");
        }
        if (input.performed)
        {
            Debug.Log($"Release Smash Attack {value.joystick} {value.button}");
        }
    }

    public void HandleSpecial(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            AttackInputStruct value = input.ReadValue<AttackInputStruct>();
            Debug.Log($"Special Performed {value.joystick} {value.button}");
        }
    }

    public void HandleSpecialSmash(InputAction.CallbackContext input)
    {
        AttackInputStruct value = input.ReadValue<AttackInputStruct>();

        if (input.started)
        {
            Debug.Log($"Charging {value.joystick} {value.button}");
        }
        if (input.performed)
        {
            Debug.Log($"Release Special {value.joystick} {value.button}");
        }
    }

    public void HandleRightJoystick(InputAction.CallbackContext input)
    {
        Vector2 direction = input.ReadValue<Vector2>();

        if (input.performed)
        {
            Debug.Log($"Right Joystick performed: {direction}");
        }
        if (input.canceled)
        {
            Debug.Log($"Right Joystick canceled");
        }
    }

    public void HandleGrab(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            actionQueue.Enqueue(new FighterInputAction(ActionType.Grab, ActionStrength.Weak));
        }
    }
}
