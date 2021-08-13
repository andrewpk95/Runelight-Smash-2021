using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class JumpEvent : UnityEvent<JumpEventType> { }
public class MovementEvent : UnityEvent<Vector2> { }
public class ShieldEvent : UnityEvent<bool> { }
public class RollEvent : UnityEvent<Vector2> { }
public class GrabEvent : UnityEvent { }
public class DashEvent : UnityEvent<float> { }

public class FighterActionHandler : MonoBehaviour
{
    public JumpEvent jumpEvent = new JumpEvent();
    public MovementEvent movementEvent = new MovementEvent();
    public RollEvent rollEvent = new RollEvent();
    public ShieldEvent shieldEvent = new ShieldEvent();
    public GrabEvent grabEvent = new GrabEvent();
    public DashEvent dashEvent = new DashEvent();

    // TODO: Decouple FighterActionHandler from FighterController
    private FighterController fighterController;
    private SlopeComponent slopeComponent;

    // Start is called before the first frame update
    void Start()
    {
        fighterController = GetComponent<FighterController>();
        slopeComponent = GetComponent<SlopeComponent>();
    }

    public void HandleJump(InputAction.CallbackContext input)
    {
        if (!slopeComponent.isGrounded)
        {
            if (input.started)
            {
                jumpEvent.Invoke(JumpEventType.DoubleJump);
            }
            return;
        }
        if (input.started)
        {
            jumpEvent.Invoke(JumpEventType.Start);
        }
        if (input.performed)
        {
            jumpEvent.Invoke(JumpEventType.ShortHop);
        }
        if (input.canceled)
        {
            jumpEvent.Invoke(JumpEventType.FullHop);
        }
    }

    public void HandleGroundMovement(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Vector2 direction = input.ReadValue<Vector2>();

            if (fighterController.isShielding)
            {
                rollEvent.Invoke(direction);
            }
            else
            {
                movementEvent.Invoke(direction);
            }
        }
        if (input.canceled)
        {
            {
                movementEvent.Invoke(Vector2.zero);
            }
        }
    }

    public void HandleDashXBox(InputAction.CallbackContext input)
    {
        Vector2 direction = input.ReadValue<Vector2>();

        if (input.canceled)
        {
            dashEvent.Invoke(0.0f);
        }
        if (input.performed)
        {
            if (fighterController.isShielding)
            {
                return;
            }
            if (Vector2.Angle(direction, Vector2.left) <= 45.0f)
            {
                dashEvent.Invoke(-1.0f);
            }
            else if (Vector2.Angle(direction, Vector2.right) <= 45.0f)
            {
                dashEvent.Invoke(1.0f);
            }
        }
    }

    public void HandleDashKeyboard(InputAction.CallbackContext input)
    {
        float direction = input.ReadValue<float>();

        if (input.canceled)
        {
            dashEvent.Invoke(direction);
        }
        if (input.performed)
        {
            if (fighterController.isShielding)
            {
                return;
            }
            dashEvent.Invoke(direction);
        }
    }

    public void HandleShield(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            shieldEvent.Invoke(true);
        }
        if (input.canceled)
        {
            shieldEvent.Invoke(false);
        }
    }

    public void HandleGrab(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            grabEvent.Invoke();
        }
    }
}