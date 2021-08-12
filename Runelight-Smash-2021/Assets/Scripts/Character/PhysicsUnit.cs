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

    // Slope calculation variables
    private List<Vector2> slopes = new List<Vector2>();
    private ContactFilter2D filter = new ContactFilter2D();
    protected float CAST_OFFSET = 0.1f;

    // Collision variables
    private ContactPoint2D[] contactPoints = new ContactPoint2D[10];
    private RaycastHit2D[] hits = new RaycastHit2D[10];

    // Events
    public UnityEvent onLandEvent = new UnityEvent();

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
            velocity.y = GetNewVelocity(velocity.y, -maxFallSpeed, gravity);
        }
        else if (slopeComponent.isOnSlope)
        {
            ApplySlopeGravity();
        }
        else
        {
            velocity.y = 0.0f;
        }
    }

    protected virtual void ApplySlopeGravity()
    {
        Vector2 slopeDirection = Mathf.Sign(slopeComponent.centerSlopeDirection.x) * slopeComponent.centerSlopeDirection;
        Vector2 projectedVelocity = Vector3.Project(velocity, slopeDirection);

        velocity.x = GetNewVelocity(projectedVelocity.x, -maxFallSpeed * slopeDirection.x, gravity * slopeDirection.x);
        velocity.y = GetNewVelocity(projectedVelocity.y, -maxFallSpeed * slopeDirection.y, gravity * slopeDirection.y);
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
