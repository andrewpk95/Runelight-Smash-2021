using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterController : MonoBehaviour
{
    public float DASH_BUFFER_DURATION = 0.2f;

    private float dashBufferedDirection;
    private Coroutine dashCoroutine;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleMovement(InputAction.CallbackContext input)
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
                if (dashBufferedDirection > 0)
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
                if (dashBufferedDirection < 0)
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
        Debug.Log("Buffer Over");
    }

    public void HandleCrouch(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Crouch");
        }
    }

    public void HandleJump(InputAction.CallbackContext input)
    {
        if (input.started)
        {
            Debug.Log("Jump Started");
        }
        if (input.performed)
        {
            Debug.Log("Short Hop");
        }
        if (input.canceled)
        {
            Debug.Log("Full Hop");
        }
    }
}
