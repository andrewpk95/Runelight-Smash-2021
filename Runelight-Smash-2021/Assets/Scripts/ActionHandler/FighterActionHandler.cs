using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class JumpStartEvent : UnityEvent { }
public class ShortHopEvent : UnityEvent { }
public class FullHopEvent : UnityEvent { }

public class FighterActionHandler : MonoBehaviour
{
    public JumpStartEvent jumpStartEvent = new JumpStartEvent();
    public ShortHopEvent shortHopEvent = new ShortHopEvent();
    public FullHopEvent fullHopEvent = new FullHopEvent();

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
}