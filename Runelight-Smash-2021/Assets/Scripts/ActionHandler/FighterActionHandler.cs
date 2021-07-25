using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class JumpStartEvent : UnityEvent { }
public class ShortHopEvent : UnityEvent { }
public class FullHopEvent : UnityEvent { }
public class MovementEvent : UnityEvent<Vector2> { }
public class ShieldEvent : UnityEvent<bool> { }
public class RollEvent : UnityEvent<Vector2> { }
public class GrabEvent : UnityEvent { }

public class FighterActionHandler : MonoBehaviour
{
    public JumpStartEvent jumpStartEvent = new JumpStartEvent();
    public ShortHopEvent shortHopEvent = new ShortHopEvent();
    public FullHopEvent fullHopEvent = new FullHopEvent();
    public MovementEvent movementEvent = new MovementEvent();
    public RollEvent rollEvent = new RollEvent();
    public ShieldEvent shieldEvent = new ShieldEvent();
    public GrabEvent grabEvent = new GrabEvent();

    // TODO: Decouple FighterActionHandler from FighterController
    private FighterController fighterController;

    // Start is called before the first frame update
    void Start()
    {
        fighterController = GetComponent<FighterController>();
    }

    public void HandleJump(InputAction.CallbackContext input)
    {
        if (!fighterController.isGrounded)
        {
            return;
        }
        if (input.started)
        {
            jumpStartEvent.Invoke();
        }
        if (input.performed)
        {
            shortHopEvent.Invoke();
        }
        if (input.canceled)
        {
            fullHopEvent.Invoke();
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

    public void HandleSnap(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Snap!!!!!!!!!!!!!!!!!!!");
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