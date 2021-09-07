using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SlopeComponent))]
[RequireComponent(typeof(InputComponent))]
[RequireComponent(typeof(VelocityComponent))]

public class GroundMovementComponent : MonoBehaviour
{
    // Required Variables
    public float maxWalkSpeed = 5.0f;
    public float walkAccelerationRate = 15.0f;
    public float groundDecelerationRate = 40.0f;
    public float initialDashSpeed = 6.0f;
    public float dashAccelerationRate = 40.0f;
    public float maxDashSpeed = 12.0f;

    // Required Components
    private SlopeComponent slopeComponent;
    private InputComponent inputComponent;
    private VelocityComponent velocityComponent;

    // Public Ground Movement State
    public bool isDashing = false;

    // Ground Movement Variables
    public bool isDashStarted = false;
    private float groundSpeed { get { return isDashing ? maxDashSpeed : Mathf.Abs(inputComponent.joystick.x) * maxWalkSpeed; } }
    private float groundAccelerationRate { get { return isDashing ? dashAccelerationRate : walkAccelerationRate; } }

    void Start()
    {
        slopeComponent = GetComponent<SlopeComponent>();
        inputComponent = GetComponent<InputComponent>();
        velocityComponent = GetComponent<VelocityComponent>();
    }

    void FixedUpdate()
    {
        ProcessInput();
        if (slopeComponent.isGrounded && slopeComponent.canWalkOnSlope)
        {
            ApplyGroundMovement();
        }
    }

    private void ProcessInput()
    {
        if (inputComponent.isDashStarted())
        {
            // Debug.Log("Dash!");
            isDashing = true;
        }
        else if (inputComponent.isDashCanceled())
        {
            // Debug.Log("Dash Stop");
            isDashing = false;
            isDashStarted = false;
        }
    }

    private void ApplyGroundMovement()
    {
        if (isDashing && !isDashStarted)
        {
            Vector2 normalized = inputComponent.joystick.normalized;

            velocityComponent.velocity.x = initialDashSpeed * (inputComponent.joystick.x > 0 ? Mathf.Abs(normalized.x) : -Mathf.Abs(normalized.x));
            velocityComponent.velocity.y = initialDashSpeed * normalized.y;

            isDashStarted = true;
        }

        float speed = Mathf.Sign(inputComponent.joystick.x) * groundSpeed;
        float acceleration = Mathf.Abs(inputComponent.joystick.x) > 0 ? groundAccelerationRate : groundDecelerationRate;

        if (slopeComponent.isOnSlope)
        {
            Vector2 slopeDirection = Mathf.Sign(slopeComponent.centerSlopeDirection.x) * slopeComponent.centerSlopeDirection;
            Vector2 projectedVelocity = Vector3.Project(velocityComponent.velocity, slopeDirection);

            velocityComponent.velocity.x = Velocity.GetNewVelocity(projectedVelocity.x, speed * slopeDirection.x, acceleration * slopeDirection.x);
            velocityComponent.velocity.y = Velocity.GetNewVelocity(projectedVelocity.y, speed * slopeDirection.y, acceleration * slopeDirection.y);
        }
        else
        {
            velocityComponent.velocity.x = Velocity.GetNewVelocity(velocityComponent.velocity.x, speed, acceleration);
        }

        if (velocityComponent.velocity.x < 0.0f && slopeComponent.leftMostSlopeAngle > slopeComponent.maxSlopeAngle)
        {
            velocityComponent.velocity = Vector2.zero;
            return;
        }
        else if (velocityComponent.velocity.x > 0.0f && slopeComponent.rightMostSlopeAngle > slopeComponent.maxSlopeAngle)
        {
            velocityComponent.velocity = Vector2.zero;
            return;
        }
    }
}
