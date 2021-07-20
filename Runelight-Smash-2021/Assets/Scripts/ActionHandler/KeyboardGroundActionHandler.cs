using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardGroundActionHandler : MonoBehaviour
{
    public float DASH_BUFFER_DURATION = 0.2f;

    private FighterController fighterController;

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
            Vector2 direction = input.ReadValue<Vector2>();

            if (direction.x > 0)
            {
                if (isShielding)
                {
                    Debug.Log("Roll Right");
                }
                else
                {
                    Debug.Log("Move Right");
                }
            }
            else if (direction.x < 0)
            {
                if (isShielding)
                {
                    Debug.Log("Roll Left");
                }
                else
                {
                    Debug.Log("Move Left");
                }
            }
            else
            {
                Debug.Log("Stop");
            }
        }
        if (input.canceled)
        {
            {
                Debug.Log("Stop");
            }
        }
    }

    // TODO: Change to Custom Composite Interaction
    public void HandleDash(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (isShielding)
            {
                return;
            }
            if (dashCoroutine != null)
            {
                StopCoroutine(dashCoroutine);
            }

            float direction = input.ReadValue<float>();

            if (direction > 0)
            {
                if (dashBufferedDirection > 0)
                {
                    Debug.Log("Dash Right");
                    dashBufferedDirection = 0.0f;
                }
                else
                {
                    dashBufferedDirection = direction;
                    dashCoroutine = StartCoroutine(DashBuffer());
                }
            }
            else if (direction < 0)
            {
                if (dashBufferedDirection < 0)
                {
                    Debug.Log("Dash Left");
                    dashBufferedDirection = 0.0f;
                }
                else
                {
                    dashBufferedDirection = direction;
                    dashCoroutine = StartCoroutine(DashBuffer());
                }
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
