using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FighterController : MonoBehaviour
{
    private PlayerInput playerInput;
    private FighterActionHandler fighterActionHandler;
    private FighterUnit fighterUnit;

    public bool isShielding = false;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        fighterActionHandler = GetComponent<FighterActionHandler>();
        fighterUnit = GetComponent<FighterUnit>();

        BindActionEvents();
    }

    private void BindActionEvents()
    {
        fighterActionHandler.jumpEvent.AddListener(HandleJump);
        fighterActionHandler.movementEvent.AddListener(HandleMovement);
        fighterActionHandler.rollEvent.AddListener(HandleRoll);
        fighterActionHandler.shieldEvent.AddListener(HandleShield);
        fighterActionHandler.grabEvent.AddListener(HandleGrab);
        fighterActionHandler.dashEvent.AddListener(HandleDash);

        fighterUnit.onJumpEvent.AddListener(Jump);
        fighterUnit.onLandEvent.AddListener(Land);
    }

    private void HandleJump(JumpEventType jumpEventType)
    {
        fighterUnit.SetJumpInput(jumpEventType);
    }

    private void HandleMovement(Vector2 direction)
    {
        fighterUnit.SetJoystickInput(direction);
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

    private void HandleDash(float direction)
    {
        fighterUnit.SetDashInput(direction);
    }

    // Mock jump simulation
    public void Jump()
    {
        // playerInput.SwitchCurrentActionMap("Airborne");
    }

    public void Land()
    {
        // playerInput.SwitchCurrentActionMap("Grounded");
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
    }
}
