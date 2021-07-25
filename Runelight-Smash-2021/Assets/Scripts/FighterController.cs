using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterController : MonoBehaviour
{
    private PlayerInput playerInput;
    private FighterActionHandler fighterActionHandler;

    public bool isGrounded = true;
    public bool isShielding = false;

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
        fighterActionHandler.movementEvent.AddListener(HandleMovement);
        fighterActionHandler.rollEvent.AddListener(HandleRoll);
        fighterActionHandler.shieldEvent.AddListener(HandleShield);
        fighterActionHandler.grabEvent.AddListener(HandleGrab);
        fighterActionHandler.dashEvent.AddListener(HandleDash);
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

    private void HandleMovement(Vector2 direction)
    {
        // Debug.Log($"Move {direction}");
    }

    private void HandleRoll(Vector2 direction)
    {
        if (Vector2.Angle(direction, Vector2.left) <= 45.0f)
        {
            Debug.Log("Roll Left");
        }
        else if (Vector2.Angle(direction, Vector2.right) <= 45.0f)
        {
            Debug.Log("Roll Right");
        }
        else if (Vector2.Angle(direction, Vector2.down) <= 45.0f)
        {
            Debug.Log("Spot Dodge");
        }
    }

    private void HandleShield(bool shield)
    {
        Debug.Log($"Shield {shield}");
        isShielding = shield;
    }

    private void HandleGrab()
    {
        Debug.Log("Grab");
    }

    private void HandleDash(bool isLeft)
    {
        string direction = isLeft ? "Left" : "Right";

        Debug.Log($"Dash {direction}");
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
