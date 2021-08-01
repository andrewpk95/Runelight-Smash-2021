using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsUnit : BaseUnit
{
    protected CapsuleCollider2D capsule;
    public Transform feetPosition;
    public float groundCheckRadius;
    public LayerMask groundLayerMask;

    public float gravity = 20.0f;
    public float maxFallSpeed = 5.0f;

    [SerializeField]
    public bool isGrounded;
    [SerializeField]
    protected bool _isGrounded;

    // Slope movement Variables
    public bool isOnSlope = false;
    public float leftMostSlopeAngle = 0.0f;
    public float rightMostSlopeAngle = 0.0f;
    public Vector2 leftMostSlopeDirection;
    public Vector2 rightMostSlopeDirection;
    public bool canWalkOnSlope;
    public float maxSlopeAngle = 45.0f;

    // Slope calculation variables
    private List<Vector2> leftSideSlopes = new List<Vector2>();
    private List<Vector2> rightSideSlopes = new List<Vector2>();
    private List<Vector2> centerSlopes = new List<Vector2>();

    // Collision variables
    private ContactPoint2D[] contactPoints = new ContactPoint2D[10];
    protected List<ContactPoint2D> groundContactPoints = new List<ContactPoint2D>();
    protected float groundContactCount;

    // Events
    public UnityEvent onLandEvent = new UnityEvent();

    protected override void Start()
    {
        capsule = GetComponent<CapsuleCollider2D>();
        base.Start();
    }

    protected override void FixedUpdate()
    {
        UpdateCollision();
        base.FixedUpdate();
    }

    protected override void Tick()
    {
        UpdatePhysics();
        base.Tick();
    }

    protected void UpdateCollision()
    {
        if (!unitRigidbody.IsSleeping())
        {
            CheckSlope();
            CheckGround();
            ClearCollisionVariables();
        }
    }

    protected void UpdatePhysics()
    {
        ApplyGravity();
    }

    private void CheckSlope()
    {
        if (groundContactCount <= 0)
        {
            return;
        }
        // Sort Contact Points based on slope angle
        // Slopes with angle '\' are on the left, angle '/' are on the right, angle '--' are at the center
        foreach (ContactPoint2D contactPoint in groundContactPoints)
        {
            Vector2 point = contactPoint.point;
            Vector2 normal = contactPoint.normal;
            Vector2 slopeDirection = -Vector2.Perpendicular(normal).normalized;
            float slopeAngle = -Vector2.SignedAngle(normal, Vector2.up);

            if (slopeAngle < -float.Epsilon)
            {
                leftSideSlopes.Add(-slopeDirection);
                Debug.DrawRay(point, -slopeDirection, Color.red);
            }
            else if (slopeAngle > float.Epsilon)
            {
                rightSideSlopes.Add(slopeDirection);
                Debug.DrawRay(point, slopeDirection, Color.blue);
            }
            else
            {
                centerSlopes.Add(slopeDirection);
                Debug.DrawRay(point, slopeDirection, Color.green);
            }
        }

        // Get the steepest slope angle on the left and right
        leftSideSlopes.Sort(SortBySlopeAngle);
        rightSideSlopes.Sort(SortBySlopeAngle);
        leftMostSlopeDirection = leftSideSlopes.Count > 0 ? leftSideSlopes[0] : Vector2.zero;
        rightMostSlopeDirection = rightSideSlopes.Count > 0 ? rightSideSlopes[rightSideSlopes.Count - 1] : Vector2.zero;
        leftMostSlopeAngle = Vector2.Angle(leftMostSlopeDirection, Vector2.left);
        rightMostSlopeAngle = Vector2.Angle(rightMostSlopeDirection, Vector2.right);
        isOnSlope = leftMostSlopeAngle > 0.0f || rightMostSlopeAngle > 0.0f;

        float currentSlope = 0.0f;

        if (velocity.x < 0.0f)
        {
            currentSlope = leftMostSlopeAngle > 0.0f ? leftMostSlopeAngle : rightMostSlopeAngle;
        }
        else if (velocity.x > 0.0f)
        {
            currentSlope = rightMostSlopeAngle > 0.0f ? rightMostSlopeAngle : leftMostSlopeAngle;
        }

        canWalkOnSlope = currentSlope <= maxSlopeAngle;
    }

    static int SortBySlopeAngle(Vector2 direction1, Vector2 direction2)
    {
        float angle1 = Vector2.SignedAngle(direction1, Vector2.right);
        float angle2 = Vector2.SignedAngle(direction2, Vector2.right);

        return angle1.CompareTo(angle2);
    }

    protected virtual bool IsPhysicallyGrounded()
    {
        if (groundContactCount <= 0)
        {
            return false;
        }

        return true;
    }

    protected virtual bool IsUnitGrounded()
    {
        bool isOnGround = Physics2D.OverlapCircle(feetPosition.position, groundCheckRadius, groundLayerMask);
        bool isbetweenSlopes = leftMostSlopeAngle > 0.0f && rightMostSlopeAngle > 0.0f;

        return isOnGround && (canWalkOnSlope || isbetweenSlopes);
    }

    private void CheckGround()
    {
        bool _isOnGround = IsPhysicallyGrounded();
        bool isOnGround = IsUnitGrounded();

        if (!isGrounded && isOnGround)
        {
            OnLand();
        }

        _isGrounded = _isOnGround;
        isGrounded = isOnGround;

    }

    private void ClearCollisionVariables()
    {
        leftSideSlopes.Clear();
        rightSideSlopes.Clear();
        centerSlopes.Clear();
        groundContactPoints.Clear();
        groundContactCount = 0;
    }

    protected virtual void OnLand()
    {
        onLandEvent.Invoke();
    }

    protected void ApplyGravity()
    {
        if (!_isGrounded)
        {
            velocity.y = GetNewVelocity(velocity.y, -maxFallSpeed, gravity);
        }
        else if (isOnSlope)
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
        Vector2 slopeDirection;

        if (velocity.x < 0.0f)
        {
            slopeDirection = leftMostSlopeAngle > 0.0f ? leftMostSlopeDirection : rightMostSlopeDirection;
        }
        else
        {
            slopeDirection = rightMostSlopeAngle > 0.0f ? rightMostSlopeDirection : leftMostSlopeDirection;
        }

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

    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            HandleGroundCollision(collision);
        }
    }

    protected virtual void HandleGroundCollision(Collision2D collision)
    {
        float feetThreshold = unitRigidbody.position.y - (capsule.size.y - capsule.size.x) / 2 + capsule.offset.y;
        Vector2 feetPos = prevPosition - Vector2.up * (capsule.size.y - capsule.size.x) / 2 + capsule.offset;
        float count = collision.GetContacts(contactPoints);

        for (int i = 0; i < count; i++)
        {
            ContactPoint2D contact = contactPoints[i];

            if (contact.point.y >= feetPos.y)
            {
                continue;
            }
            groundContactPoints.Add(contactPoints[i]);
            groundContactCount++;
        }
    }
}
