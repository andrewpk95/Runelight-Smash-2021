using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterController : MonoBehaviour
{
    private PlayerInput playerInput;
    private FighterActionHandler fighterActionHandler;

    public bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        fighterActionHandler = GetComponent<FighterActionHandler>();

        BindActionEvents();
    }

    private void BindActionEvents()
    {
        fighterActionHandler.jumpStartEvent.AddListener(HandleJumpStart);
        fighterActionHandler.shortHopEvent.AddListener(HandleShortHop);
        fighterActionHandler.fullHopEvent.AddListener(HandleFullHop);
    }

    private void HandleJumpStart()
    {
        Debug.Log("Jump Start");
    }

    private void HandleShortHop()
    {
        Debug.Log("Short Hop");
        Jump(0.5f);
    }

    private void HandleFullHop()
    {
        Debug.Log("Full Hop");
        Jump(1.0f);
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

    // Mock edge grab simulation
    public void EdgeGrab()
    {
        playerInput.SwitchCurrentActionMap("EdgeGrab");
    }

    public void ReleaseEdge()
    {
        playerInput.SwitchCurrentActionMap("Grounded");
        isGrounded = true;
    }
}
