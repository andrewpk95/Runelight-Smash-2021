using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class VerticalSinMovementComponent : MonoBehaviour
{
    // Required Variables
    public float moveDistance = 4.0f;
    public float frequencySpeed = 4.0f;
    public float angle;

    protected Rigidbody2D unitRigidbody;
    protected Vector2 originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        unitRigidbody = GetComponent<Rigidbody2D>();
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
        unitRigidbody.position = originalPosition + Vector2.right * Mathf.Sin(Mathf.Deg2Rad * angle);
    }
}
