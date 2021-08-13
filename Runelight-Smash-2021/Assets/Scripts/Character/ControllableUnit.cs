using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableUnit : PhysicsUnit
{
    // Required Components
    protected CapsuleCollider2D capsule;
    protected JoystickComponent joystickComponent;
    protected AirMovementComponent airMovementComponent;
    protected JumpComponent jumpComponent;

    // Input Variables
    protected bool isInputEnabled = true;

    protected override void Start()
    {
        base.Start();
        capsule = GetComponent<CapsuleCollider2D>();
    }

    protected override void Tick()
    {
        base.Tick();
    }

    public void EnableInput()
    {
        isInputEnabled = true;
    }

    public void DisableInput()
    {
        isInputEnabled = false;
        joystickComponent.joystick = Vector2.zero;
    }
}
