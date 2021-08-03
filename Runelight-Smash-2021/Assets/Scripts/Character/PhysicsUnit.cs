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

    // Slope movement Variables
    public bool isOnSlope = false;
    public float leftMostSlopeAngle = 0.0f;
    public float rightMostSlopeAngle = 0.0f;
    public Vector2 leftMostSlopeDirection;
    public Vector2 rightMostSlopeDirection;
    public bool canWalkOnSlope;
    public float maxSlopeAngle = 45.0f;

    // Slope calculation variables
    private List<Vector2> slopes = new List<Vector2>();
    private ContactFilter2D filter = new ContactFilter2D();
    private float CAST_OFFSET = 0.05f;

    // Collision variables
    private ContactPoint2D[] contactPoints = new ContactPoint2D[10];
    private RaycastHit2D[] hits = new RaycastHit2D[10];

    // Events
    public UnityEvent onLandEvent = new UnityEvent();

    protected override void Start()
    {
        capsule = GetComponent<CapsuleCollider2D>();
        filter.SetLayerMask(groundLayerMask);
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
        GetSlopeCollisionPoints();

        if (slopes.Count <= 0)
        {
            return;
        }
        CalculateSlopes();

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

    private void GetSlopeCollisionPoints()
    {
        Vector2 centerPos = unitRigidbody.position + capsule.offset;
        Vector2 feetPos = centerPos - Vector2.up * (capsule.size.y - capsule.size.x) / 2;
        float distance = (centerPos - feetPos).y;
        int count = Physics2D.CircleCast(centerPos, capsule.size.x / 2, Vector2.down, filter, hits, distance + CAST_OFFSET);

        for (int i = 0; i < count; i++)
        {
            RaycastHit2D hit = hits[i];

            if (hit.point.y >= centerPos.y - distance)
            {
                continue;
            }

            Vector2 slopeDirection = AddToSlopes(hit);

            // CircleCast returns only one collision per object, so perform CircleCast again to get more collision points
            RaycastHit2D extraHit = Physics2D.CircleCast(feetPos, capsule.size.x / 2, slopeDirection.y < 0.0f ? slopeDirection : -slopeDirection, CAST_OFFSET, groundLayerMask);
            if (extraHit)
            {
                AddToSlopes(extraHit);
            }
        }
    }

    private Vector2 AddToSlopes(RaycastHit2D hit)
    {
        Vector2 normal = hit.normal;
        Vector2 slopeDirection = -Vector2.Perpendicular(normal).normalized;
        float slopeAngle = -Vector2.SignedAngle(normal, Vector2.up);

        slopes.Add(normal);
        Debug.DrawRay(hit.point, slopeDirection.y < 0.0f ? -slopeDirection : slopeDirection, Color.red);

        return slopeDirection;
    }

    private void CalculateSlopes()
    {
        if (slopes.Count <= 0)
        {
            leftMostSlopeDirection = Vector2.zero;
            rightMostSlopeDirection = Vector2.zero;
            leftMostSlopeAngle = 0.0f;
            rightMostSlopeAngle = 0.0f;
            return;
        }

        slopes.Sort(SortBySlopeAngle);

        // Get the steepest slope angle on the left and right
        Vector2 leftMost = -Vector2.Perpendicular(slopes[0]);
        Vector2 rightMost = -Vector2.Perpendicular(slopes[slopes.Count - 1]);
        float left = Vector2.SignedAngle(Vector2.right, leftMost);
        float right = Vector2.SignedAngle(Vector2.right, rightMost);

        leftMostSlopeDirection = left < 0.0f ? -leftMost : Vector2.zero;
        rightMostSlopeDirection = right > 0.0f ? rightMost : Vector2.zero;
        leftMostSlopeAngle = Vector2.SignedAngle(leftMostSlopeDirection, Vector2.left);
        rightMostSlopeAngle = -Vector2.SignedAngle(rightMostSlopeDirection, Vector2.right);
        isOnSlope = leftMostSlopeAngle > 0.0f || rightMostSlopeAngle > 0.0f;
    }

    static int SortBySlopeAngle(Vector2 normal1, Vector2 normal2)
    {
        float angle1 = -Vector2.SignedAngle(normal1, Vector2.up);
        float angle2 = -Vector2.SignedAngle(normal2, Vector2.up);

        return angle1.CompareTo(angle2);
    }

    protected virtual bool IsPhysicallyGrounded()
    {
        if (slopes.Count <= 0)
        {
            return false;
        }

        return true;
    }

    private void CheckGround()
    {
        bool isOnGround = IsPhysicallyGrounded();

        if (!isGrounded && isOnGround)
        {
            OnLand();
        }

        isGrounded = isOnGround;

        if (!isGrounded)
        {
            isOnSlope = false;
            canWalkOnSlope = false;
        }
    }

    private void ClearCollisionVariables()
    {
        slopes.Clear();
    }

    protected virtual void OnLand()
    {
        onLandEvent.Invoke();
    }

    protected void ApplyGravity()
    {
        if (!isGrounded)
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
}
