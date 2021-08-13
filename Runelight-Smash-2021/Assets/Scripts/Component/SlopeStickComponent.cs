using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AirMovementComponent))]
[RequireComponent(typeof(GravityComponent))]
[RequireComponent(typeof(JoystickComponent))]
[RequireComponent(typeof(JumpComponent))]
[RequireComponent(typeof(SlopeComponent))]
[RequireComponent(typeof(VelocityComponent))]

public class SlopeStickComponent : MonoBehaviour
{
    // Required Components
    protected CapsuleCollider2D capsule;
    protected Rigidbody2D unitRigidbody;
    protected AirMovementComponent airMovementComponent;
    protected GravityComponent gravityComponent;
    protected JoystickComponent joystickComponent;
    protected JumpComponent jumpComponent;
    protected SlopeComponent slopeComponent;
    protected VelocityComponent velocityComponent;

    // Slope Stick Variable
    private Vector2 slopeStickPosition;

    void Start()
    {
        capsule = GetComponent<CapsuleCollider2D>();
        unitRigidbody = GetComponent<Rigidbody2D>();
        airMovementComponent = GetComponent<AirMovementComponent>();
        gravityComponent = GetComponent<GravityComponent>();
        joystickComponent = GetComponent<JoystickComponent>();
        jumpComponent = GetComponent<JumpComponent>();
        slopeComponent = GetComponent<SlopeComponent>();
        velocityComponent = GetComponent<VelocityComponent>();
    }

    void FixedUpdate()
    {
        StickToSlope();
    }

    private void StickToSlope()
    {
        if (jumpComponent.isJumping || !slopeComponent.isGrounded || !slopeComponent.canWalkOnSlope)
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
}
