using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(SlopeComponent))]
[RequireComponent(typeof(VelocityComponent))]

public class WallCollisionComponent : MonoBehaviour
{
    // Required Components
    protected Rigidbody2D unitRigidbody;
    protected CapsuleCollider2D capsule;
    protected SlopeComponent slopeComponent;
    protected VelocityComponent velocityComponent;

    // Public Wall Collision State
    public bool isCollidingWithWall;

    // Wall Collision Variables
    private RaycastHit2D hit;

    void Start()
    {
        unitRigidbody = GetComponent<Rigidbody2D>();
        capsule = GetComponent<CapsuleCollider2D>();
        slopeComponent = GetComponent<SlopeComponent>();
        velocityComponent = GetComponent<VelocityComponent>();
    }

    void FixedUpdate()
    {
        CheckWallCollision();
        StickToWall();
    }

    private void CheckWallCollision()
    {
        float distanceToFeetPos = (capsule.size.y - capsule.size.x) / 2;
        Vector2 centerPos = unitRigidbody.position + capsule.offset;
        Vector2 nextVelocityStep = velocityComponent.velocity * Time.fixedDeltaTime;
        Vector2 nextPos = centerPos + nextVelocityStep;

        hit = Physics2D.CapsuleCast(centerPos, capsule.size, capsule.direction, 0.0f, nextVelocityStep, nextVelocityStep.magnitude, slopeComponent.groundLayerMask);
    }

    private void StickToWall()
    {
        if (hit)
        {
            float nextSlopeAngle = Slope.GetSlopeAngle(hit.normal);

            if (nextSlopeAngle > slopeComponent.maxSlopeAngle)
            {
                Vector2 newSlopeStickPosition = hit.centroid - capsule.offset;

                velocityComponent.ForceToPosition(newSlopeStickPosition);
                velocityComponent.velocity = Vector2.zero;
                isCollidingWithWall = true;
            }
        }
        else
        {
            isCollidingWithWall = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (capsule != null && hit)
        {
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawWireDisc(hit.centroid, Vector3.forward, capsule.size.x / 2);
        }
    }
}
