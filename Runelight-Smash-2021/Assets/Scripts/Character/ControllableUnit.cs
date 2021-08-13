using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControllableUnit : PhysicsUnit
{
    // Required Components
    protected CapsuleCollider2D capsule;
    protected JoystickComponent joystickComponent;

    // Input Variables
    protected bool isInputEnabled = true;

    protected JumpEventType jumpEventType = JumpEventType.None;

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
        base.Start();
        capsule = GetComponent<CapsuleCollider2D>();
        joystickComponent = GetComponent<JoystickComponent>();
        doubleJumpLeft = maxDoubleJumpCount;
        slopeComponent.onLandEvent.AddListener(OnLand);
    }

    protected override void Tick()
    {
        ApplyControls();
        base.Tick();
        StickToSlope();
    }

    private void ApplyControls()
    {
        ApplyMovement();
        ApplyJumpMovement();
    }

    private void ApplyMovement()
    {
        if (slopeComponent.isGrounded)
        {

        }
        else
        {
            ApplyAirMovement();
        }
    }

    protected virtual void ApplyAirMovement()
    {
        if (Mathf.Abs(joystickComponent.joystick.x) > 0)
        {
            velocityComponent.velocity.x = Velocity.GetNewVelocity(velocityComponent.velocity.x, joystickComponent.joystick.x * maxAirSpeed, airAccelerationRate);
        }
        else
        {
            velocityComponent.velocity.x = Velocity.GetNewVelocity(velocityComponent.velocity.x, 0.0f, airDecelerationRate);
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
        float targetJumpSpeed = joystickComponent.joystick.x > 0 ? Mathf.Max(joystickComponent.joystick.x * maxAirSpeed, velocityComponent.velocity.x) : Mathf.Min(joystickComponent.joystick.x * maxAirSpeed, velocityComponent.velocity.x);
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

    private void StickToSlope()
    {
        if (isJumping || !slopeComponent.isGrounded || !slopeComponent.canWalkOnSlope)
        {
            return;
        }
        RaycastHit2D hit;
        float distanceToFeetPos = (capsule.size.y - capsule.size.x) / 2;
        float radius = capsule.size.x / 2;
        Vector2 centerPos = unitRigidbody.position + capsule.offset;
        Vector2 feetPos = centerPos - Vector2.up * distanceToFeetPos;
        Vector2 nextVelocityStep = velocityComponent.velocity * Time.fixedDeltaTime;
        Vector2 nextPos = centerPos + nextVelocityStep;

        hit = Physics2D.CircleCast(feetPos, radius, nextVelocityStep, nextVelocityStep.magnitude, slopeComponent.groundLayerMask);

        if (hit)
        {
            float nextSlopeAngle = Slope.GetSlopeAngle(hit.normal);

            if (nextSlopeAngle > slopeComponent.maxSlopeAngle)
            {
                slopeStickPosition = hit.centroid;
                Vector2 newSlopeStickPosition = slopeStickPosition + Vector2.up * (distanceToFeetPos - capsule.offset.y);

                Debug.DrawLine(unitRigidbody.position, newSlopeStickPosition, Color.white);
                velocityComponent.ForceToPosition(newSlopeStickPosition);
                return;
            }
        }

        hit = Physics2D.CircleCast(nextPos, radius, Vector2.down, velocityComponent.velocity.magnitude, slopeComponent.groundLayerMask);

        if (hit)
        {
            Vector2 perpendicular = Vector2.Perpendicular(hit.normal);
            Vector2 nextSlopeDirection = perpendicular.x > 0.0f ? perpendicular : -perpendicular;
            float nextSlopeAngle = Vector2.Angle(perpendicular, Vector2.left);

            if (nextSlopeAngle > slopeComponent.maxSlopeAngle)
            {
                return;
            }
            Ray2D ray1 = new Ray2D(feetPos - Vector2.up * Physics2D.defaultContactOffset, nextVelocityStep);
            Ray2D ray2 = new Ray2D(hit.centroid, velocityComponent.velocity.x < 0.0f ? nextSlopeDirection : -nextSlopeDirection);

            if (!Math2D.IsRayIntersecting(ray1, ray2))
            {
                return;
            }

            slopeStickPosition = hit.centroid;
            Vector2 newSlopeStickPosition = slopeStickPosition + Vector2.up * (distanceToFeetPos - capsule.offset.y);

            Debug.DrawLine(unitRigidbody.position, newSlopeStickPosition, Color.white);
            velocityComponent.ForceToPosition(newSlopeStickPosition);
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

    protected void OnLand()
    {
        doubleJumpLeft = maxDoubleJumpCount;
        isJumping = false;
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
        joystickComponent.joystick = Vector2.zero;
    }
}
