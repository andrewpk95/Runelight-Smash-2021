using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterController : MonoBehaviour
{
    private PlayerInput playerInput;

    private bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    // Mock jump simulation
    public void Jump(float duration)
    {
        isGrounded = false;
        playerInput.SwitchCurrentActionMap("Airborne");
    }

    public void Land()
    {
        playerInput.SwitchCurrentActionMap("Grounded");
        isGrounded = true;
        Debug.Log("Landed");
    }
}
