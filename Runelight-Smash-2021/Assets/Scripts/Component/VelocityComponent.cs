using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class VelocityComponent : MonoBehaviour
{
    // Required Components
    private Rigidbody2D unitRigidbody;

    // Public Velocity States
    public Vector2 velocity;
    public Vector2 finalVelocity { get { return velocity + velocityModifier; } }
    public bool isMovementEnabled { get { return _isMovementEnabled; } }

    // Velocity Variables
    private Vector2 velocityModifier = Vector2.zero;
    private bool _isMovementEnabled = true;
    private bool isPositionForced = false;
    private Vector2 forcePosition;

    void Start()
    {
        unitRigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isMovementEnabled)
        {
            ApplyVelocity();
        }
    }

    private void ApplyVelocity()
    {
        Vector2 velocityStep = (velocity + velocityModifier) * Time.fixedDeltaTime;

        velocityModifier = Vector2.zero;
        if (isPositionForced)
        {
            unitRigidbody.position = forcePosition;
            isPositionForced = false;
            return;
        }
        unitRigidbody.MovePosition(unitRigidbody.position + velocityStep);
    }

    public void DisableMovement()
    {
        _isMovementEnabled = false;
        unitRigidbody.velocity = Vector2.zero;
        unitRigidbody.isKinematic = true;
    }

    public void EnableMovement()
    {
        _isMovementEnabled = true;
        unitRigidbody.velocity = Vector2.zero;
        unitRigidbody.isKinematic = false;
    }

    public void ForceToPosition(Vector2 position)
    {
        isPositionForced = true;
        forcePosition = position;
    }

    public void AddVelocityModifier(Vector2 modifier)
    {
        velocityModifier = modifier;
    }
}