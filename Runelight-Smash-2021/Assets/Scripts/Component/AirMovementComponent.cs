using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirMovementComponent : MonoBehaviour
{
    // Required Variables
    public float maxAirSpeed = 3.0f;
    public float airAccelerationRate = 8.0f;
    public float airDecelerationRate = 10.0f;

    // Required Components
    protected JoystickComponent joystickComponent;
    protected SlopeComponent slopeComponent;
    protected VelocityComponent velocityComponent;

    void Start()
    {
        slopeComponent = GetComponent<SlopeComponent>();
        joystickComponent = GetComponent<JoystickComponent>();
        velocityComponent = GetComponent<VelocityComponent>();
    }

    void FixedUpdate()
    {
        if (!slopeComponent.isGrounded)
        {
            ApplyAirMovement();
        }
    }

    protected virtual void ApplyAirMovement()
    {
        if (Mathf.Abs(joystickComponent.joystick.x) > 0)
        {
            velocityComponent.velocity.x = Velocity.GetNewVelocity(velocityComponent.velocity.x, joystickComponent.joystick.x * maxAirSpeed, airAccelerationRate);
        }
        else
        {
            velocityComponent.velocity.x = Velocity.GetNewVelocity(velocityComponent.velocity.x, 0.0f, airDecelerationRate);
        }
    }
}
