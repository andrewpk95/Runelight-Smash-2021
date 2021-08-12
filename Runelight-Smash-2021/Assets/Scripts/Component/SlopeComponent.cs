using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlopeComponent : MonoBehaviour
{
    // Required Variables
    public LayerMask groundLayerMask;
    public float maxSlopeAngle = 45.0f;

    // Required Components
    protected CapsuleCollider2D capsule;
    protected Rigidbody2D unitRigidbody;

    // Public Slope States
    public bool isGrounded;
    public bool isOnSlope = false;
    public float leftMostSlopeAngle = 0.0f;
    public float centerSlopeAngle = 0.0f;
    public float rightMostSlopeAngle = 0.0f;
    public Vector2 leftMostSlopeDirection;
    public Vector2 centerSlopeDirection;
    public Vector2 rightMostSlopeDirection;
    public bool canWalkOnSlope;

    // Events
    public UnityEvent onLandEvent = new UnityEvent();

    // Slope calculation variables
    private List<Vector2> slopes = new List<Vector2>();
    private ContactFilter2D filter = new ContactFilter2D();
    protected float CAST_OFFSET = 0.1f;

    // Collision variables
    private ContactPoint2D[] contactPoints = new ContactPoint2D[10];
    private RaycastHit2D[] hits = new RaycastHit2D[10];

    void Start()
    {
        unitRigidbody = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        filter.SetLayerMask(groundLayerMask);
    }

    void FixedUpdate()
    {
        if (!unitRigidbody.IsSleeping())
        {
            CheckSlope();
            CheckGround();
            ClearCollisionVariables();
        }
    }

    private void CheckSlope()
    {
        GetSlopeCollisionPoints();

        if (slopes.Count <= 0)
        {
            return;
        }
        CalculateSlopes();

        canWalkOnSlope = centerSlopeAngle <= maxSlopeAngle;
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

    protected virtual bool IsPhysicallyGrounded()
    {
        if (slopes.Count <= 0 || !canWalkOnSlope)
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
    }

    protected virtual void OnLand()
    {
        onLandEvent.Invoke();
    }

    private Vector2 AddToSlopes(RaycastHit2D hit)
    {
        Vector2 normal = hit.normal;
        Vector2 slopeDirection = GetSignedSlopeDirection(normal);

        slopes.Add(normal);
        Debug.DrawRay(hit.point, slopeDirection.y < 0.0f ? -slopeDirection : slopeDirection, Color.red);

        return slopeDirection;
    }

    public Vector2 GetSignedSlopeDirection(Vector2 normal)
    {
        return -Vector2.Perpendicular(normal).normalized;
    }

    public Vector2 GetSlopeDirection(Vector2 normal)
    {
        Vector2 signedSlopeDirection = GetSignedSlopeDirection(normal);

        return signedSlopeDirection.y < 0.0f ? -signedSlopeDirection : signedSlopeDirection;
    }

    public float GetSignedSlopeAngle(Vector2 normal)
    {
        return -Vector2.SignedAngle(normal, Vector2.up);
    }

    public float GetSlopeAngle(Vector2 normal)
    {
        float signedSlopeAngle = GetSignedSlopeAngle(normal);

        return Mathf.Abs(signedSlopeAngle);
    }

    private void CalculateSlopes()
    {
        if (slopes.Count <= 0)
        {
            leftMostSlopeDirection = Vector2.zero;
            rightMostSlopeDirection = Vector2.zero;
            centerSlopeDirection = Vector2.zero;
            leftMostSlopeAngle = 0.0f;
            rightMostSlopeAngle = 0.0f;
            centerSlopeAngle = 0.0f;
            return;
        }

        slopes.Sort(SortBySlopeAngle);

        // Get the steepest slope angle on the left and right
        Vector2 leftMostNormal = slopes[0];
        Vector2 rightMostNormal = slopes[slopes.Count - 1];
        Vector2 centerNormal = Vector2.up;
        Vector2 leftMost = GetSignedSlopeDirection(leftMostNormal);
        Vector2 rightMost = GetSignedSlopeDirection(rightMostNormal);
        float left = GetSignedSlopeAngle(leftMostNormal);
        float right = GetSignedSlopeAngle(rightMostNormal);

        if (left < 0.0f && right > 0.0f)
        {
            centerNormal = leftMostNormal + rightMostNormal;
        }
        else if (left < 0.0f)
        {
            centerNormal = rightMostNormal;
        }
        else if (right > 0.0f)
        {
            centerNormal = leftMostNormal;
        }

        leftMostSlopeDirection = left < 0.0f ? GetSlopeDirection(leftMostNormal) : Vector2.zero;
        rightMostSlopeDirection = right > 0.0f ? GetSlopeDirection(rightMostNormal) : Vector2.zero;
        leftMostSlopeAngle = Vector2.SignedAngle(leftMostSlopeDirection, Vector2.left);
        rightMostSlopeAngle = -Vector2.SignedAngle(rightMostSlopeDirection, Vector2.right);
        centerSlopeDirection = GetSlopeDirection(centerNormal);
        centerSlopeAngle = GetSlopeAngle(centerNormal);
        isOnSlope = centerSlopeAngle > 0.0f;
    }

    static int SortBySlopeAngle(Vector2 normal1, Vector2 normal2)
    {
        float angle1 = -Vector2.SignedAngle(normal1, Vector2.up);
        float angle2 = -Vector2.SignedAngle(normal2, Vector2.up);

        return angle1.CompareTo(angle2);
    }

    private void ClearCollisionVariables()
    {
        slopes.Clear();
    }
}