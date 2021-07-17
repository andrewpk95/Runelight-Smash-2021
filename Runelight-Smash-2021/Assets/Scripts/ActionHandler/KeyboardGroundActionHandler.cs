using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardGroundActionHandler : MonoBehaviour
{
    public float DASH_BUFFER_DURATION = 0.2f;

    private FighterController fighterController;

    private bool isGrounded = true;

    private float dashBufferedDirection;
    private Coroutine dashCoroutine;

    private bool isShielding = false;

    // Start is called before the first frame update
    void Start()
    {
        fighterController = GetComponent<FighterController>();
    }

    public void HandleGroundMovement(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
            }

            float direction = input.ReadValue<float>();

            if (direction > 0)
            {
                if (isShielding)
                {
                    Debug.Log("Roll Right");
                }
                else if (dashBufferedDirection > 0)
                {
                    Debug.Log("Dash Right");
                    dashBufferedDirection = 0.0f;
                }
                else
                {
                    Debug.Log("Move Right");
                    dashBufferedDirection = direction;
                    dashCoroutine = StartCoroutine(DashBuffer());
                }
            }
            else if (direction < 0)
            {
                if (isShielding)
                {
                    Debug.Log("Roll Left");
                }
                else if (dashBufferedDirection < 0)
                {
                    Debug.Log("Dash Left");
                    dashBufferedDirection = 0.0f;
                }
                else
                {
                    Debug.Log("Move Left");
                    dashBufferedDirection = direction;
                    dashCoroutine = StartCoroutine(DashBuffer());
                }
            }
        }
        if (input.canceled)
        {
            {
                Debug.Log("Stop");
            }
        }
    }

    IEnumerator DashBuffer()
    {
        yield return new WaitForSeconds(DASH_BUFFER_DURATION);
        dashBufferedDirection = 0.0f;
    }

    public void HandleCrouch(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (isShielding)
            {
                Debug.Log("Spot Dodge");
            }
            else
            {
                Debug.Log("Crouch");
            }
        }
    }

    public void HandleJump(InputAction.CallbackContext input)
    {
        if (!isGrounded)
        {
            return;
        }
        if (input.started)
        {
            Debug.Log("Jump Started");
        }
        if (input.performed)
        {
            Debug.Log("Short Hop");
            // TODO: Use Event System to trigger character action
            fighterController.Jump(0.5f);
        }
        if (input.canceled)
        {
            Debug.Log("Full Hop");
            fighterController.Jump(1.0f);
        }
    }

    public void HandleShield(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Shield");
            isShielding = true;
        }
        if (input.canceled)
        {
            Debug.Log("Shield Release");
            isShielding = false;
        }
    }

    public void HandleGrab(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Grab");
        }
    }
}
