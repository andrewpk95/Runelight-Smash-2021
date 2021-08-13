using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AirMovementComponent))]
[RequireComponent(typeof(SlopeComponent))]
[RequireComponent(typeof(JoystickComponent))]
[RequireComponent(typeof(VelocityComponent))]
[RequireComponent(typeof(GravityComponent))]

public class JumpComponent : MonoBehaviour
{
    // Required Variables
    public float shortHopHeight = 1.5f;
    public float fullHopHeight = 3.0f;
    public float doubleJumpHeight = 3.0f;
    public int maxDoubleJumpCount = 1;
    public bool canJumpChangeDirection = true;
    public float minSlopeJumpAngle = 30.0f;

    // Required Components
    protected AirMovementComponent airMovementComponent;
    protected SlopeComponent slopeComponent;
    protected JoystickComponent joystickComponent;
    protected VelocityComponent velocityComponent;
    protected GravityComponent gravityComponent;

    // Public Jump States
    public bool isJumpSquatting;
    public bool isJumping;

    // Jump Variables
    protected JumpEventType jumpEventType = JumpEventType.None;
    protected int doubleJumpLeft;

    // Events
    public UnityEvent onJumpEvent = new UnityEvent();

    void Start()
    {
        airMovementComponent = GetComponent<AirMovementComponent>();
        gravityComponent = GetComponent<GravityComponent>();
        slopeComponent = GetComponent<SlopeComponent>();
        joystickComponent = GetComponent<JoystickComponent>();
        velocityComponent = GetComponent<VelocityComponent>();

        slopeComponent.onLandEvent.AddListener(OnLand);
        doubleJumpLeft = maxDoubleJumpCount;
    }

    void FixedUpdate()
    {
        ApplyJumpMovement();
    }

    protected virtual void ApplyJumpMovement()
    {
        switch (jumpEventType)
        {
            case JumpEventType.Start:
                isJumpSquatting = true;
                break;
            case JumpEventType.ShortHop:
                if (!slopeComponent.isGrounded)
                {
                    break;
                }
                ShortHop();
                break;
            case JumpEventType.FullHop:
                if (!slopeComponent.isGrounded)
                {
                    break;
                }
                FullHop();
                break;
            case JumpEventType.DoubleJump:
                if (slopeComponent.isGrounded)
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
        float targetJumpSpeed = joystickComponent.joystick.x > 0 ? Mathf.Max(joystickComponent.joystick.x * airMovementComponent.maxAirSpeed, velocityComponent.velocity.x) : Mathf.Min(joystickComponent.joystick.x * airMovementComponent.maxAirSpeed, velocityComponent.velocity.x);
        float jumpSpeed = canJumpChangeDirection ? targetJumpSpeed : velocityComponent.velocity.x;
        Vector2 jumpVelocity = new Vector2(jumpSpeed, Mathf.Sqrt(2.0f * gravityComponent.gravity * jumpHeight));
        float jumpAngle = Vector2.Angle(Vector2.right, jumpVelocity);
        float slopeAngle = Mathf.Sign(velocityComponent.velocity.y) * slopeComponent.centerSlopeAngle;

        // Make sure jump velocityComponent.velocity is away from the grounds
        if (jumpVelocity.x < 0.0f && 180 - jumpAngle < slopeAngle + minSlopeJumpAngle)
        {
            float rotateAngle = Mathf.Deg2Rad * -((slopeAngle - (180 - jumpAngle)) + minSlopeJumpAngle);
            float cos = Mathf.Cos(rotateAngle);
            float sin = Mathf.Sin(rotateAngle);

            jumpVelocity.x = jumpVelocity.x * cos - jumpVelocity.y * sin;
        }
        else if (jumpVelocity.x > 0.0f && jumpAngle < slopeAngle + minSlopeJumpAngle)
        {
            float rotateAngle = Mathf.Deg2Rad * (slopeAngle - jumpAngle + minSlopeJumpAngle);
            float cos = Mathf.Cos(rotateAngle);
            float sin = Mathf.Sin(rotateAngle);

            jumpVelocity.x = jumpVelocity.x * cos - jumpVelocity.y * sin;
        }

        velocityComponent.velocity = jumpVelocity;
        isJumpSquatting = false;
        isJumping = true;
        slopeComponent.isGrounded = false;
        onJumpEvent.Invoke();
    }

    protected void OnLand()
    {
        doubleJumpLeft = maxDoubleJumpCount;
        isJumping = false;
    }

    public void SetJumpInput(JumpEventType input)
    {
        jumpEventType = input;
    }
}
