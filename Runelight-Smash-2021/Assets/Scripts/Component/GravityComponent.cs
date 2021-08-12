using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityComponent : MonoBehaviour
{
    // Required Variables
    public float gravity = 20.0f;
    public float maxFallSpeed = 5.0f;

    // Required Components
    protected SlopeComponent slopeComponent;
    protected VelocityComponent velocityComponent;

    void Start()
    {
        slopeComponent = GetComponent<SlopeComponent>();
        velocityComponent = GetComponent<VelocityComponent>();
    }

    void FixedUpdate()
    {
        ApplyGravity();
    }

    protected void ApplyGravity()
    {
        if (!slopeComponent.isGrounded || !slopeComponent.canWalkOnSlope)
        {
            velocityComponent.velocity.y = Velocity.GetNewVelocity(velocityComponent.velocity.y, -maxFallSpeed, gravity);
        }
        else if (slopeComponent.isOnSlope)
        {
            ApplySlopeGravity();
        }
        else
        {
            velocityComponent.velocity.y = 0.0f;
        }
    }

    protected void ApplySlopeGravity()
    {
        if (slopeComponent.isOnSlope && slopeComponent.canWalkOnSlope)
        {
            return;
        }
        Vector2 slopeDirection = Mathf.Sign(slopeComponent.centerSlopeDirection.x) * slopeComponent.centerSlopeDirection;
        Vector2 projectedVelocity = Vector3.Project(velocityComponent.velocity, slopeDirection);

        velocityComponent.velocity.x = Velocity.GetNewVelocity(projectedVelocity.x, -maxFallSpeed * slopeDirection.x, gravity * slopeDirection.x);
        velocityComponent.velocity.y = Velocity.GetNewVelocity(projectedVelocity.y, -maxFallSpeed * slopeDirection.y, gravity * slopeDirection.y);
    }
}
