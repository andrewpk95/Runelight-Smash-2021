using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    protected SlopeComponent slopeComponent;
    protected JoystickComponent joystickComponent;
    protected VelocityComponent velocityComponent;

    // Public Ground Movement State
    public bool isDashing = false;

    // Ground Movement Variables
    public bool isDashStarted = false;
    protected float groundSpeed { get { return isDashing ? maxDashSpeed : Mathf.Abs(joystickComponent.joystick.x) * maxWalkSpeed; } }
    protected float groundAccelerationRate { get { return isDashing ? dashAccelerationRate : walkAccelerationRate; } }

    void Start()
    {
        slopeComponent = GetComponent<SlopeComponent>();
        joystickComponent = GetComponent<JoystickComponent>();
        velocityComponent = GetComponent<VelocityComponent>();
    }

    void FixedUpdate()
    {
        if (slopeComponent.isGrounded && slopeComponent.canWalkOnSlope)
        {
            ApplyGroundMovement();
        }
    }

    protected virtual void ApplyGroundMovement()
    {
        if (isDashing && !isDashStarted)
        {
            Vector2 normalized = velocityComponent.velocity.normalized;

            velocityComponent.velocity.x = initialDashSpeed * (joystickComponent.joystick.x > 0 ? Mathf.Abs(normalized.x) : -Mathf.Abs(normalized.x));
            velocityComponent.velocity.y = initialDashSpeed * normalized.y;

            isDashStarted = true;
        }

        float speed = Mathf.Sign(joystickComponent.joystick.x) * groundSpeed;
        float acceleration = Mathf.Abs(joystickComponent.joystick.x) > 0 ? groundAccelerationRate : groundDecelerationRate;

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

    public void SetDashInput(float direction)
    {
        if (direction < 0)
        {
            if (!isDashing)
            {
                Debug.Log("Dash Left");
            }
            isDashing = true;
        }
        else if (direction > 0)
        {
            if (!isDashing)
            {
                Debug.Log("Dash Right");
            }
            isDashing = true;
        }
        else
        {
            if (isDashing)
            {
                Debug.Log("Stop");
                isDashing = false;
                isDashStarted = false;
            }
        }
    }
}
