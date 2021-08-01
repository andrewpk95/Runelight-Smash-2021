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
        StickToSlope();
    }

    private void ApplyControls()
    {
        ApplyMovement();
        ApplyJumpMovement();
    }

    private void ApplyMovement()
    {
        if (_isGrounded)
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
        float walkSpeed = joystick.x * maxWalkSpeed;
        float acceleration = Mathf.Abs(joystick.x) > 0 ? walkAccelerationRate : groundDecelerationRate;

        if (isOnSlope)
        {
            Vector2 slopeDirection;

            if (joystick.x < 0.0f)
            {
                slopeDirection = leftMostSlopeAngle > 0.0f ? -leftMostSlopeDirection : rightMostSlopeDirection;
            }
            else
            {
                slopeDirection = rightMostSlopeAngle > 0.0f ? rightMostSlopeDirection : -leftMostSlopeDirection;
            }

            Vector2 projectedVelocity = Vector3.Project(velocity, slopeDirection);

            velocity.x = GetNewVelocity(projectedVelocity.x, walkSpeed * slopeDirection.x, acceleration * slopeDirection.x);
            velocity.y = GetNewVelocity(projectedVelocity.y, walkSpeed * slopeDirection.y, acceleration * slopeDirection.y);
        }
        else
        {
            velocity.x = GetNewVelocity(velocity.x, walkSpeed, acceleration);
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
                if (!_isGrounded)
                {
                    break;
                }
                ShortHop();
                break;
            case JumpEventType.FullHop:
                if (!_isGrounded)
                {
                    break;
                }
                FullHop();
                break;
            case JumpEventType.DoubleJump:
                if (_isGrounded)
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
        if (180 - jumpAngle < leftMostSlopeAngle + minSlopeJumpAngle)
        {
            float rotateAngle = Mathf.Deg2Rad * -((leftMostSlopeAngle - (180 - jumpAngle)) + minSlopeJumpAngle);
            float cos = Mathf.Cos(rotateAngle);
            float sin = Mathf.Sin(rotateAngle);

            jumpVelocity.x = jumpVelocity.x * cos - jumpVelocity.y * sin;
        }
        else if (jumpAngle < rightMostSlopeAngle + minSlopeJumpAngle)
        {
            float rotateAngle = Mathf.Deg2Rad * (rightMostSlopeAngle - jumpAngle + minSlopeJumpAngle);
            float cos = Mathf.Cos(rotateAngle);
            float sin = Mathf.Sin(rotateAngle);

            jumpVelocity.x = jumpVelocity.x * cos - jumpVelocity.y * sin;
        }

        velocity = jumpVelocity;
        isJumpSquatting = false;
        isJumping = true;
        _isGrounded = false;
        onJumpEvent.Invoke();
    }

    private void StickToSlope()
    {
        if (isJumping)
        {
            return;
        }
        Vector2 feetPos = unitRigidbody.position + velocity * Time.fixedDeltaTime - Vector2.up * (capsule.size.y - capsule.size.x) / 2 + capsule.offset;
        RaycastHit2D hit = Physics2D.CircleCast(feetPos, capsule.size.x / 2, Vector2.down, velocity.magnitude, groundLayerMask);

        if (hit)
        {
            slopeStickPosition = hit.point + hit.normal.normalized * capsule.size.x / 2;

            float diff = slopeStickPosition.y - feetPos.y;

            Debug.DrawLine(feetPos, slopeStickPosition, Color.white);
            unitRigidbody.MovePosition(unitRigidbody.position + velocity * Time.fixedDeltaTime + Vector2.up * diff);
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

    protected override bool IsPhysicallyGrounded()
    {
        return isJumping ? false : base.IsPhysicallyGrounded();
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
