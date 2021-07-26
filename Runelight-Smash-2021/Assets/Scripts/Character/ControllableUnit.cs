using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControllableUnit : PhysicsUnit
{
    // Input Variables
    protected bool isInputEnabled = true;
    protected Vector2 joystick;
    protected JumpEventType jumpEventType = JumpEventType.None;

    // Ground Movement Variables
    public float maxWalkSpeed = 5.0f;
    public float walkAccelerationRate = 15.0f;
    public float groundDecelerationRate = 20.0f;

    // Air Movement Variables
    public float maxAirSpeed = 3.0f;
    public float airAccelerationRate = 8.0f;
    public float airDecelerationRate = 10.0f;

    // Jump Variables
    protected bool isJumpSquatting;
    public float shortHopHeight = 1.5f;
    public float fullHopHeight = 3.0f;
    public float doubleJumpHeight = 3.0f;
    public int maxDoubleJumpCount = 1;
    protected int doubleJumpLeft;
    public bool canJumpChangeDirection = true;

    // Events
    public UnityEvent onJumpEvent = new UnityEvent();

    protected override void Start()
    {
        doubleJumpLeft = maxDoubleJumpCount;
        base.Start();
    }

    protected override void FixedUpdate()
    {
        ApplyControls();
        base.FixedUpdate();
    }

    private void ApplyControls()
    {
        ApplyMovement();
        ApplyJumpMovement();
    }

    private void ApplyMovement()
    {
        if (isGrounded)
        {
            ApplyGroundMovement();
        }
        else
        {
            ApplyAirMovement();
        }
    }

    protected virtual void ApplyGroundMovement()
    {
        if (Mathf.Abs(joystick.x) > 0)
        {
            velocity.x = GetNewVelocity(velocity.x, joystick.x * maxWalkSpeed, walkAccelerationRate);
        }
        else
        {
            velocity.x = GetNewVelocity(velocity.x, 0.0f, groundDecelerationRate);
        }
    }

    protected virtual void ApplyAirMovement()
    {
        if (Mathf.Abs(joystick.x) > 0)
        {
            velocity.x = GetNewVelocity(velocity.x, joystick.x * maxAirSpeed, airAccelerationRate);
        }
        else
        {
            velocity.x = GetNewVelocity(velocity.x, 0.0f, airDecelerationRate);
        }
    }

    protected virtual void ApplyJumpMovement()
    {
        switch (jumpEventType)
        {
            case JumpEventType.Start:
                isJumpSquatting = true;
                break;
            case JumpEventType.ShortHop:
                if (!isGrounded)
                {
                    break;
                }
                ShortHop();
                break;
            case JumpEventType.FullHop:
                if (!isGrounded)
                {
                    break;
                }
                FullHop();
                break;
            case JumpEventType.DoubleJump:
                if (isGrounded)
                {
                    break;
                }
                DoubleJump();
                break;
        }
        jumpEventType = JumpEventType.None;
    }

    private void ShortHop()
    {
        Jump(shortHopHeight);
    }

    private void FullHop()
    {
        Jump(fullHopHeight);
    }

    private void DoubleJump()
    {
        if (doubleJumpLeft <= 0)
        {
            return;
        }
        doubleJumpLeft -= 1;
        Jump(doubleJumpHeight);
    }

    private void Jump(float jumpHeight)
    {
        isJumpSquatting = false;
        if (canJumpChangeDirection)
        {
            velocity.x = joystick.x * maxAirSpeed;
        }
        velocity.y = Mathf.Sqrt(2.0f * gravity * jumpHeight);
        onJumpEvent.Invoke();
    }

    protected override void OnLand()
    {
        doubleJumpLeft = maxDoubleJumpCount;
        base.OnLand();
    }

    public void SetJoystickInput(Vector2 input)
    {
        if (!isInputEnabled)
        {
            return;
        }
        joystick = input;
    }

    public void SetJumpInput(JumpEventType input)
    {
        if (!isInputEnabled)
        {
            return;
        }
        jumpEventType = input;
    }

    public void EnableInput()
    {
        isInputEnabled = true;
    }

    public void DisableInput()
    {
        isInputEnabled = false;
        joystick = Vector2.zero;
    }


}
