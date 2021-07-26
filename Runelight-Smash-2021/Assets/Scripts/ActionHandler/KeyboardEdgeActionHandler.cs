using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardEdgeActionHandler : MonoBehaviour
{
    private FighterController fighterController;

    // Start is called before the first frame update
    void Start()
    {
        fighterController = GetComponent<FighterController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void HandleMovement(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Vector2 direction = input.ReadValue<Vector2>();

            // For now, assume always hanging at the left side
            if (direction.x > 0)
            {
                Debug.Log("Normal Getup");
                fighterController.ReleaseEdge();
            }
            else if (direction.x < 0)
            {
                Debug.Log("Let Go (Normal)");
                fighterController.ReleaseEdge();
            }

        }
    }

    public void HandleLetGo(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Let Go (Fall)");
            fighterController.ReleaseEdge();
        }
    }

    public void HandleAttackGetup(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Attack Getup");
            fighterController.ReleaseEdge();
        }
    }

    public void HandleJumpGetup(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Jump Getup");
        }
    }

    public void HandleRollGetup(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Roll Getup");
            fighterController.ReleaseEdge();
        }
    }
}
