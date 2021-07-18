using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeyboardAirborneActionHandler : MonoBehaviour
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

    // Mock Handler to toggle edge grab
    public void HandleEdgeGrab(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Edge Grab");
            fighterController.EdgeGrab();
        }
    }

    public void HandleAirMovement(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            float direction = input.ReadValue<float>();

            if (direction > 0)
            {
                Debug.Log("Move Right");
            }
            else if (direction < 0)
            {
                Debug.Log("Move Left");
            }
        }
        if (input.canceled)
        {
            {
                Debug.Log("Stop");
            }
        }
    }

    public void HandleFastFall(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Fast Fall");
            fighterController.Land();
        }
    }

    public void HandleDoubleJump(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Double Jump");
        }
    }

    public void HandleAirDodge(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Air Dodge");
        }
    }

    public void HandleZAir(InputAction.CallbackContext input)
    {
        if (input.performed)
        {
            Debug.Log("Zair");
        }
    }
}
