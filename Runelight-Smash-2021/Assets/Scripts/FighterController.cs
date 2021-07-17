using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
