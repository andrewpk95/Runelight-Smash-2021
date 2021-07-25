using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUnit : BaseUnit
{
    public Transform feetPosition;
    public float groundCheckRadius;
    public LayerMask groundLayerMask;

    public float gravity = 20.0f;
    public float maxFallSpeed = 5.0f;

    protected bool isGrounded { get { return _isGrounded; } }
    [SerializeField]
    private bool _isGrounded;

    protected override void FixedUpdate()
    {
        UpdatePhysics();
        base.FixedUpdate();
    }

    protected void UpdatePhysics()
    {
        CheckGround();
        ApplyGravity();
    }

    private void CheckGround()
    {
        if (velocity.y > 0)
        {
            _isGrounded = false;
            return;
        }

        _isGrounded = Physics2D.OverlapCircle(feetPosition.position, groundCheckRadius, groundLayerMask);

        if (isGrounded)
        {
            OnLand();
        }
    }

    protected virtual void OnLand()
    {
        velocity.y = 0;
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y = GetNewVelocity(velocity.y, -maxFallSpeed, gravity);
        }
    }

    protected float GetNewVelocity(float currentVelocity, float targetVelocity, float accelerationRate)
    {
        int direction = currentVelocity > targetVelocity ? -1 : 1;
        float acceleration = accelerationRate * Time.fixedDeltaTime;
        float newVelocity = currentVelocity + acceleration * direction;
        float velocityDifference = Mathf.Abs(currentVelocity - targetVelocity);

        if (velocityDifference < acceleration)
        {
            return targetVelocity;
        }

        return newVelocity;
    }
}
