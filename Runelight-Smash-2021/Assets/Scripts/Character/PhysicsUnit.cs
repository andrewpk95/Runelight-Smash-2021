using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsUnit : BaseUnit
{
    // Required Components
    protected SlopeComponent slopeComponent;

    public float gravity = 20.0f;
    public float maxFallSpeed = 5.0f;

    protected override void Start()
    {
        base.Start();
        slopeComponent = GetComponent<SlopeComponent>();
    }

    protected override void Tick()
    {
        UpdatePhysics();
        base.Tick();
    }

    protected void UpdatePhysics()
    {
        ApplyGravity();
    }

    protected void ApplyGravity()
    {
        if (!slopeComponent.isGrounded || !slopeComponent.canWalkOnSlope)
        {
            velocityComponent.velocity.y = GetNewVelocity(velocityComponent.velocity.y, -maxFallSpeed, gravity);
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

    protected virtual void ApplySlopeGravity()
    {
        Vector2 slopeDirection = Mathf.Sign(slopeComponent.centerSlopeDirection.x) * slopeComponent.centerSlopeDirection;
        Vector2 projectedVelocity = Vector3.Project(velocityComponent.velocity, slopeDirection);

        velocityComponent.velocity.x = GetNewVelocity(projectedVelocity.x, -maxFallSpeed * slopeDirection.x, gravity * slopeDirection.x);
        velocityComponent.velocity.y = GetNewVelocity(projectedVelocity.y, -maxFallSpeed * slopeDirection.y, gravity * slopeDirection.y);
    }

    protected float GetNewVelocity(float currentVelocity, float targetVelocity, float accelerationRate)
    {
        int direction = currentVelocity > targetVelocity ? -1 : 1;
        float acceleration = Mathf.Abs(accelerationRate * Time.fixedDeltaTime);
        float newVelocity = currentVelocity + acceleration * direction;
        float velocityDifference = Mathf.Abs(currentVelocity - targetVelocity);

        if (velocityDifference < acceleration)
        {
            return targetVelocity;
        }

        return newVelocity;
    }
}
