using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class VelocityComponent : MonoBehaviour
{
    // Required Components
    private Rigidbody2D unitRigidbody;

    // Public Slope States
    public Vector2 velocity;
    public bool isMovementEnabled { get { return _isMovementEnabled; } }

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
        if (isPositionForced)
        {
            unitRigidbody.position = forcePosition;
            isPositionForced = false;
            return;
        }
        unitRigidbody.MovePosition(unitRigidbody.position + velocity * Time.fixedDeltaTime);
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
}