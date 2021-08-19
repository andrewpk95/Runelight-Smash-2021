using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(VelocityComponent))]

public class SlopeComponent : MonoBehaviour
{
    // Required Variables
    public LayerMask groundLayerMask;
    public LayerMask platformLayerMask;
    public float maxSlopeAngle = 45.0f;
    public int fallThroughTimeInFrames = 30;

    // Required Components
    private CapsuleCollider2D capsule;
    private Rigidbody2D unitRigidbody;
    private VelocityComponent velocityComponent;

    // Public Slope States 
    // TODO: Change to property getter for variables that need to be protected
    public bool isGrounded;
    public bool isOnSlope = false;
    public float leftMostSlopeAngle = 0.0f;
    public float centerSlopeAngle = 0.0f;
    public float rightMostSlopeAngle = 0.0f;
    public Vector2 leftMostSlopeDirection;
    public Vector2 centerSlopeDirection;
    public Vector2 rightMostSlopeDirection;
    public bool canWalkOnSlope;
    public LayerMask currentLayerMask;

    // Events
    public UnityEvent onLandEvent = new UnityEvent();

    // Slope calculation variables
    private List<Vector2> slopes = new List<Vector2>();
    private ContactFilter2D filter = new ContactFilter2D();
    private float CAST_OFFSET = 0.1f;

    // Collision variables
    private ContactPoint2D[] contactPoints = new ContactPoint2D[10];
    private RaycastHit2D[] hits = new RaycastHit2D[10];
    [SerializeField]
    private GameObject currentGround;
    private GameObject newGround;

    // Fall Through variables
    private int fallThroughTimeleft;

    void Start()
    {
        unitRigidbody = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        velocityComponent = GetComponent<VelocityComponent>();

        StopFallThrough();
    }

    void FixedUpdate()
    {
        if (fallThroughTimeleft > 0)
        {
            fallThroughTimeleft--;
        }
        else
        {
            StopFallThrough();
        }
        if (!unitRigidbody.IsSleeping())
        {
            CheckSlope();
            CheckPlatform();
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
        int count = Physics2D.CapsuleCast(centerPos, capsule.size, capsule.direction, 0.0f, Vector2.down, filter, hits, CAST_OFFSET);
        GameObject ground = null;

        for (int i = 0; i < count; i++)
        {
            RaycastHit2D hit = hits[i];

            if (hit.point.y >= centerPos.y - distance)
            {
                continue;
            }

            if (((1 << hit.collider.gameObject.layer) & platformLayerMask.value) != 0)
            {
                Vector2 platformSlope = hit.normal;

                Debug.DrawRay(hit.centroid, platformSlope, Color.magenta);
                if (!isGrounded && velocityComponent.velocity.sqrMagnitude > 0.0f && Vector2.Angle(platformSlope, velocityComponent.velocity) < 89.0f)
                {
                    continue;
                }
            }

            Vector2 slopeDirection = AddToSlopes(hit);
            ground = hit.collider.gameObject;

            // CircleCast returns only one collision per object, so perform CircleCast again to get more collision points
            RaycastHit2D extraHit = Physics2D.CapsuleCast(centerPos, capsule.size, capsule.direction, 0.0f, slopeDirection.y < 0.0f ? slopeDirection : -slopeDirection, CAST_OFFSET, currentLayerMask);
            if (extraHit)
            {
                AddToSlopes(extraHit);
            }
        }
        newGround = ground;
    }

    private bool IsPhysicallyGrounded()
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

        InvokeGroundEvents(isOnGround);
        isGrounded = isOnGround;
    }

    private void InvokeGroundEvents(bool isOnGround)
    {
        if (!isGrounded && isOnGround)
        {
            OnLand();
        }
    }

    private void CheckPlatform()
    {
        if (currentGround != newGround)
        {
            LeavePlatform(currentGround);
            LandOnPlatform(newGround);
        }
        currentGround = newGround;
    }

    private void LandOnPlatform(GameObject platformObject)
    {
        if (platformObject == null)
        {
            return;
        }
        MovingPlatformComponent platform = platformObject.GetComponent<MovingPlatformComponent>();

        if (platform)
        {
            platform.AddToPassengers(velocityComponent);
            Debug.Log($"Landed on {platform.gameObject.name}");
        }
    }

    private void LeavePlatform(GameObject platformObject)
    {
        if (platformObject == null)
        {
            return;
        }
        MovingPlatformComponent platform = platformObject.GetComponent<MovingPlatformComponent>();

        if (platform)
        {
            platform.RemoveFromPassengers(velocityComponent);
            Debug.Log($"Left {platform.gameObject.name}");
        }
    }


    private void OnLand()
    {
        onLandEvent.Invoke();
        StopFallThrough();
    }

    private Vector2 AddToSlopes(RaycastHit2D hit)
    {
        Vector2 normal = hit.normal;
        Vector2 slopeDirection = Slope.GetSignedSlopeDirection(normal);

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
            centerSlopeDirection = Vector2.zero;
            leftMostSlopeAngle = 0.0f;
            rightMostSlopeAngle = 0.0f;
            centerSlopeAngle = 0.0f;
            return;
        }

        slopes.Sort(Slope.SortBySlopeAngle);

        // Get the steepest slope angle on the left and right
        Vector2 leftMostNormal = slopes[0];
        Vector2 rightMostNormal = slopes[slopes.Count - 1];
        Vector2 centerNormal = Vector2.up;
        Vector2 leftMost = Slope.GetSignedSlopeDirection(leftMostNormal);
        Vector2 rightMost = Slope.GetSignedSlopeDirection(rightMostNormal);
        float left = Slope.GetSignedSlopeAngle(leftMostNormal);
        float right = Slope.GetSignedSlopeAngle(rightMostNormal);

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

        leftMostSlopeDirection = left < 0.0f ? Slope.GetSlopeDirection(leftMostNormal) : Vector2.zero;
        rightMostSlopeDirection = right > 0.0f ? Slope.GetSlopeDirection(rightMostNormal) : Vector2.zero;
        leftMostSlopeAngle = Vector2.SignedAngle(leftMostSlopeDirection, Vector2.left);
        rightMostSlopeAngle = -Vector2.SignedAngle(rightMostSlopeDirection, Vector2.right);
        centerSlopeDirection = Slope.GetSlopeDirection(centerNormal);
        centerSlopeAngle = Slope.GetSlopeAngle(centerNormal);
        isOnSlope = centerSlopeAngle > 0.0f;
    }

    private void ClearCollisionVariables()
    {
        slopes.Clear();
    }

    public void FallThrough()
    {
        currentLayerMask = groundLayerMask;
        filter.SetLayerMask(currentLayerMask);
        gameObject.layer = LayerMask.NameToLayer("FallThroughUnit");
        fallThroughTimeleft = fallThroughTimeInFrames;
    }

    public void StopFallThrough()
    {
        currentLayerMask = groundLayerMask + platformLayerMask;
        filter.SetLayerMask(currentLayerMask);
        gameObject.layer = LayerMask.NameToLayer("Unit");
        fallThroughTimeleft = 0;
    }

    public void SetGrounded(bool isOnGround)
    {
        InvokeGroundEvents(isOnGround);
        isGrounded = isOnGround;
    }
}