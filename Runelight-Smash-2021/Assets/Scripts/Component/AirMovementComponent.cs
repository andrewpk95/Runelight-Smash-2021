using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputComponent))]
[RequireComponent(typeof(SlopeComponent))]
[RequireComponent(typeof(VelocityComponent))]

public class AirMovementComponent : MonoBehaviour
{
    // Required Variables
    public float maxAirSpeed = 3.0f;
    public float airAccelerationRate = 8.0f;
    public float airDecelerationRate = 10.0f;

    // Required Components
    private InputComponent inputComponent;
    private SlopeComponent slopeComponent;
    private VelocityComponent velocityComponent;

    void Start()
    {
        slopeComponent = GetComponent<SlopeComponent>();
        inputComponent = GetComponent<InputComponent>();
        velocityComponent = GetComponent<VelocityComponent>();
    }

    void FixedUpdate()
    {
        if (!slopeComponent.isGrounded)
        {
            ApplyAirMovement();
        }
    }

    private void ApplyAirMovement()
    {
        if (Mathf.Abs(inputComponent.joystick.x) > 0)
        {
            velocityComponent.velocity.x = Velocity.GetNewVelocity(velocityComponent.velocity.x, inputComponent.joystick.x * maxAirSpeed, airAccelerationRate);
        }
        else
        {
            velocityComponent.velocity.x = Velocity.GetNewVelocity(velocityComponent.velocity.x, 0.0f, airDecelerationRate);
        }
    }
}
