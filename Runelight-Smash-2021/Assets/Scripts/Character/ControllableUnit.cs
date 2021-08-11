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
    protected virtual float groundSpeed { get { return Mathf.Abs(joystick.x) * maxWalkSpeed; } }
    protected virtual float groundAccelerationRate { get { return walkAccelerationRate; } }
    public float maxWalkSpeed = 5.0f;
    public float walkAccelerationRate = 15.0f;
    public float groundDecelerationRate = 20.0f;
    private Vector2 slopeStickPosition;

    // Air Movement Variables
    public float maxAirSpeed = 3.0f;
    public float airAccelerationRate = 8.0f;
    public float airDecelerationRate = 10.0f;

    // Jump Variables
    protected bool isJumpSquatting;
    protected bool isJumping;
    public float shortHopHeight = 1.5f;
    public float fullHopHeight = 3.0f;
    public float doubleJumpHeight = 3.0f;
    public int maxDoubleJumpCount = 1;
    protected int doubleJumpLeft;
    public bool canJumpChangeDirection = true;
    public float minSlopeJumpAngle = 15.0f;

    // Events
    public UnityEvent onJumpEvent = new UnityEvent();

    protected override void Start()
    {
        doubleJumpLeft = maxDoubleJumpCount;
        base.Start();
    }

    protected override void Tick()
    {
        ApplyControls();
        base.Tick();
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
        if (!canWalkOnSlope)
        {
            return;
        }
        float speed = Mathf.Sign(joystick.x) * groundSpeed;
        float acceleration = Mathf.Abs(joystick.x) > 0 ? groundAccelerationRate : groundDecelerationRate;

        if (isOnSlope)
        {
            Vector2 slopeDirection = centerSlopeDirection.x > 0.0f ? centerSlopeDirection : -centerSlopeDirection;
            Vector2 projectedVelocity = Vector3.Project(velocity, slopeDirection);

            velocity.x = GetNewVelocity(projectedVelocity.x, speed * slopeDirection.x, acceleration * slopeDirection.x);
            velocity.y = GetNewVelocity(projectedVelocity.y, speed * slopeDirection.y, acceleration * slopeDirection.y);
        }
        else
        {
            velocity.x = GetNewVelocity(velocity.x, speed, acceleration);
        }

        if (velocity.x < 0.0f && leftMostSlopeAngle > maxSlopeAngle)
        {
            velocity = Vector2.zero;
            return;
        }
        else if (velocity.x > 0.0f && rightMostSlopeAngle > maxSlopeAngle)
        {
            velocity = Vector2.zero;
            return;
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

    protected override void ApplySlopeGravity()
    {
        if (isOnSlope && canWalkOnSlope)
        {
            return;
        }
        base.ApplySlopeGravity();
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
        Vector2 jumpVelocity = new Vector2(canJumpChangeDirection ? joystick.x * maxAirSpeed : velocity.x, Mathf.Sqrt(2.0f * gravity * jumpHeight));
        float jumpAngle = Vector2.Angle(Vector2.right, jumpVelocity);

        // Make sure jump velocity is away from the grounds
        if (180 - jumpAngle < centerSlopeAngle + minSlopeJumpAngle)
        {
            float rotateAngle = Mathf.Deg2Rad * -((centerSlopeAngle - (180 - jumpAngle)) + minSlopeJumpAngle);
            float cos = Mathf.Cos(rotateAngle);
            float sin = Mathf.Sin(rotateAngle);

            jumpVelocity.x = jumpVelocity.x * cos - jumpVelocity.y * sin;
        }
        else if (jumpAngle < centerSlopeAngle + minSlopeJumpAngle)
        {
            float rotateAngle = Mathf.Deg2Rad * (centerSlopeAngle - jumpAngle + minSlopeJumpAngle);
            float cos = Mathf.Cos(rotateAngle);
            float sin = Mathf.Sin(rotateAngle);

            jumpVelocity.x = jumpVelocity.x * cos - jumpVelocity.y * sin;
        }

        velocity = jumpVelocity;
        isJumpSquatting = false;
        isJumping = true;
        isGrounded = false;
        onJumpEvent.Invoke();
    }

    protected override void ApplyVelocity()
    {
        StickToSlope();
    }

    private void StickToSlope()
    {
        if (isJumping || !isGrounded || !canWalkOnSlope)
        {
            base.ApplyVelocity();
            return;
        }
        RaycastHit2D hit;
        float distanceToFeetPos = (capsule.size.y - capsule.size.x) / 2;
        float radius = capsule.size.x / 2;
        Vector2 centerPos = unitRigidbody.position + capsule.offset;
        Vector2 feetPos = centerPos - Vector2.up * distanceToFeetPos;
        Vector2 nextVelocityStep = velocity * Time.fixedDeltaTime;
        Vector2 nextPos = centerPos + nextVelocityStep;

        hit = Physics2D.CircleCast(feetPos, radius, nextVelocityStep, nextVelocityStep.magnitude, groundLayerMask);

        if (hit)
        {
            float nextSlopeAngle = GetSlopeAngle(hit.normal);

            if (nextSlopeAngle > maxSlopeAngle)
            {
                slopeStickPosition = hit.centroid;
                Vector2 newSlopeStickPosition = slopeStickPosition + Vector2.up * (distanceToFeetPos - capsule.offset.y);

                Debug.DrawLine(unitRigidbody.position, newSlopeStickPosition, Color.white);
                unitRigidbody.position = newSlopeStickPosition;
                return;
            }
        }

        hit = Physics2D.CircleCast(nextPos, radius, Vector2.down, velocity.magnitude, groundLayerMask);

        if (hit)
        {
            Vector2 perpendicular = Vector2.Perpendicular(hit.normal);
            Vector2 nextSlopeDirection = perpendicular.x > 0.0f ? perpendicular : -perpendicular;
            float nextSlopeAngle = Vector2.Angle(perpendicular, Vector2.left);

            if (nextSlopeAngle > maxSlopeAngle)
            {
                base.ApplyVelocity();
                return;
            }
            Ray2D ray1 = new Ray2D(feetPos - Vector2.up * Physics2D.defaultContactOffset, nextVelocityStep);
            Ray2D ray2 = new Ray2D(hit.centroid, velocity.x < 0.0f ? nextSlopeDirection : -nextSlopeDirection);

            if (!Math2D.IsRayIntersecting(ray1, ray2))
            {
                base.ApplyVelocity();
                return;
            }

            slopeStickPosition = hit.centroid;
            Vector2 newSlopeStickPosition = slopeStickPosition + Vector2.up * (distanceToFeetPos - capsule.offset.y);

            Debug.DrawLine(unitRigidbody.position, newSlopeStickPosition, Color.white);
            unitRigidbody.position = newSlopeStickPosition;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (capsule != null)
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(slopeStickPosition, Vector3.forward, capsule.size.x / 2);
        }
    }

    protected override void OnLand()
    {
        doubleJumpLeft = maxDoubleJumpCount;
        isJumping = false;
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
