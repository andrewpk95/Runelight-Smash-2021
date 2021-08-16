using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(SlopeComponent))]
[RequireComponent(typeof(VelocityComponent))]

public class WallCollisionComponent : MonoBehaviour
{
    // Required Variables
    public float maxCeilingAngle = 30.0f;
    public LayerMask wallLayerMask;

    // Required Components
    protected Rigidbody2D unitRigidbody;
    protected CapsuleCollider2D capsule;
    protected SlopeComponent slopeComponent;
    protected VelocityComponent velocityComponent;

    // Public Wall Collision State
    public bool isCollidingWithWall;

    // Wall Collision Variables
    private RaycastHit2D hit;
    private Vector2 newSlopeStickPosition;

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

        hit = Physics2D.CapsuleCast(centerPos, capsule.size, capsule.direction, 0.0f, nextVelocityStep, nextVelocityStep.magnitude, wallLayerMask);
    }

    private void StickToWall()
    {
        if (hit)
        {
            if (slopeComponent.isGrounded)
            {
                GroundStickToWall();
            }
            else
            {
                AirStickToWall();
            }
        }
        else
        {
            isCollidingWithWall = false;
        }
    }

    private void GroundStickToWall()
    {
        float nextSlopeAngle = Slope.GetSlopeAngle(hit.normal);

        if (nextSlopeAngle > slopeComponent.maxSlopeAngle)
        {
            newSlopeStickPosition = hit.centroid - capsule.offset;

            velocityComponent.ForceToPosition(newSlopeStickPosition);
            velocityComponent.velocity = Vector2.zero;
            isCollidingWithWall = true;
        }
    }

    private void AirStickToWall()
    {
        Vector2 slopeStickPosition = hit.centroid - capsule.offset;
        Vector2 hitDistanceVector = slopeStickPosition - unitRigidbody.position;
        Vector2 remainingVelocity = velocityComponent.velocity * Time.fixedDeltaTime - hitDistanceVector;
        float nextSlopeAngle = Slope.GetSlopeAngle(hit.normal);
        float signedSlopeAngle = Slope.GetSignedSlopeAngle(hit.normal);

        if (180.0f - nextSlopeAngle < maxCeilingAngle)
        {
            float y = remainingVelocity.x * Mathf.Tan(Mathf.Deg2Rad * signedSlopeAngle);

            newSlopeStickPosition = slopeStickPosition + new Vector2(remainingVelocity.x, y);
            velocityComponent.ForceToPosition(newSlopeStickPosition);
            velocityComponent.velocity.y = Mathf.Min(velocityComponent.velocity.y, 0.0f);
            isCollidingWithWall = true;
        }
        else if (nextSlopeAngle > slopeComponent.maxSlopeAngle)
        {
            Vector2 slopeDirection = Slope.GetSlopeDirection(hit.normal);
            float x = remainingVelocity.y / Mathf.Tan(Mathf.Deg2Rad * signedSlopeAngle);

            newSlopeStickPosition = slopeStickPosition + new Vector2(x, remainingVelocity.y);
            velocityComponent.ForceToPosition(newSlopeStickPosition);
            isCollidingWithWall = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (capsule != null)
        {
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.DrawWireDisc(newSlopeStickPosition, Vector3.forward, capsule.size.x / 2);
        }
    }
}
