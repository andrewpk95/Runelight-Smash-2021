using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(VelocityComponent))]

public class VerticalSinMovementComponent : MonoBehaviour
{
    // Required Variables
    public float moveDistance = 4.0f;
    public float frequencySpeed = 4.0f;
    public float angle;

    private Rigidbody2D unitRigidbody;
    private VelocityComponent velocityComponent;
    private Vector2 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        unitRigidbody = GetComponent<Rigidbody2D>();
        velocityComponent = GetComponent<VelocityComponent>();
        originalPosition = unitRigidbody.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (angle > 360.0f)
        {
            angle -= 360.0f;
        }
        angle += 360.0f * Time.fixedDeltaTime * (1 / frequencySpeed);
        velocityComponent.velocity = moveDistance * Vector2.right * Mathf.Sin(Mathf.Deg2Rad * angle);
    }
}
