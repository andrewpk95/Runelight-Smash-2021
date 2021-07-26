using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    protected Collider2D unitCollider;
    protected Rigidbody2D unitRigidbody;

    [SerializeField]
    protected Vector2 velocity;
    protected bool isMovementEnabled { get { return _isMovementEnabled; } }
    private bool _isMovementEnabled = true;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        unitCollider = GetComponent<Collider2D>();
        unitRigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (isMovementEnabled)
        {
            ApplyVelocity();
        }
    }

    private void ApplyVelocity()
    {
        unitRigidbody.MovePosition(unitRigidbody.position + velocity * Time.fixedDeltaTime);
    }

    protected void DisableMovement()
    {
        _isMovementEnabled = false;
        unitRigidbody.velocity = Vector2.zero;
        unitRigidbody.isKinematic = true;
    }

    protected void EnableMovement()
    {
        _isMovementEnabled = true;
        unitRigidbody.velocity = Vector2.zero;
        unitRigidbody.isKinematic = false;
    }
}
