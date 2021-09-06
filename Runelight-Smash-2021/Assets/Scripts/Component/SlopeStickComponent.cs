using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AirMovementComponent))]
[RequireComponent(typeof(GravityComponent))]
[RequireComponent(typeof(InputComponent))]
[RequireComponent(typeof(JumpComponent))]
[RequireComponent(typeof(SlopeComponent))]
[RequireComponent(typeof(VelocityComponent))]
[RequireComponent(typeof(WallCollisionComponent))]

public class SlopeStickComponent : MonoBehaviour
{
    // Required Components
    private CapsuleCollider2D capsule;
    private Rigidbody2D unitRigidbody;
    private AirMovementComponent airMovementComponent;
    private GravityComponent gravityComponent;
    private InputComponent inputComponent;
    private JumpComponent jumpComponent;
    private SlopeComponent slopeComponent;
    private VelocityComponent velocityComponent;
    private WallCollisionComponent wallCollisionComponent;

    // Slope Stick Variable
    private Vector2 slopeStickPosition;

    void Start()
    {
        capsule = GetComponent<CapsuleCollider2D>();
        unitRigidbody = GetComponent<Rigidbody2D>();
        airMovementComponent = GetComponent<AirMovementComponent>();
        gravityComponent = GetComponent<GravityComponent>();
        inputComponent = GetComponent<InputComponent>();
        jumpComponent = GetComponent<JumpComponent>();
        slopeComponent = GetComponent<SlopeComponent>();
        velocityComponent = GetComponent<VelocityComponent>();
        wallCollisionComponent = GetComponent<WallCollisionComponent>();
    }

    void FixedUpdate()
    {
        StickToSlope();
    }

    private void StickToSlope()
    {
        if (jumpComponent.isJumping || !slopeComponent.isGrounded || !slopeComponent.canWalkOnSlope || wallCollisionComponent.isCollidingWithWall)
        {
            return;
        }
        RaycastHit2D hit;
        Vector2 centerPos = unitRigidbody.position + capsule.offset;
        Vector2 nextVelocityStep = velocityComponent.velocity * Time.fixedDeltaTime;
        Vector2 nextPos = centerPos + nextVelocityStep;

        hit = Physics2D.CapsuleCast(nextPos, capsule.size, capsule.direction, 0.0f, Vector2.down, velocityComponent.velocity.magnitude, slopeComponent.currentLayerMask);

        if (hit)
        {
            Vector2 perpendicular = Vector2.Perpendicular(hit.normal);
            Vector2 nextSlopeDirection = perpendicular.x > 0.0f ? perpendicular : -perpendicular;
            float nextSlopeAngle = Vector2.Angle(perpendicular, Vector2.left);

            if (nextSlopeAngle > slopeComponent.maxSlopeAngle)
            {
                return;
            }
            Ray2D ray1 = new Ray2D(centerPos - Vector2.up * Physics2D.defaultContactOffset, nextVelocityStep);
            Ray2D ray2 = new Ray2D(hit.centroid, velocityComponent.velocity.x < 0.0f ? nextSlopeDirection : -nextSlopeDirection);

            if (!Math2D.IsRayIntersecting(ray1, ray2))
            {
                return;
            }

            slopeStickPosition = hit.centroid;
            Vector2 newSlopeStickPosition = slopeStickPosition - Vector2.up * capsule.offset.y;

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
}
