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

    // Start is called before the first frame update
    void Start()
    {
        fighterController = GetComponent<FighterController>();
    }

    // TODO: Change to Custom Composite Interaction
    public void HandleDash(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            if (fighterController.isShielding)
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
}
