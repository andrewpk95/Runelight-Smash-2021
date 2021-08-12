using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FighterUnit : ControllableUnit
{
    protected override float groundSpeed { get { return isDashing ? maxDashSpeed : base.groundSpeed; } }
    protected override float groundAccelerationRate { get { return isDashing ? dashAccelerationRate : walkAccelerationRate; } }
    // Dash 
    public bool isDashing = false;
    public bool isDashStarted = false;
    public float initialDashSpeed = 6.0f;
    public float dashAccelerationRate = 40.0f;
    public float maxDashSpeed = 12.0f;

    protected override void ApplyGroundMovement()
    {
        if (isDashing && !isDashStarted)
        {
            Vector2 normalized = velocityComponent.velocity.normalized;

            velocityComponent.velocity.x = initialDashSpeed * (joystickComponent.joystick.x > 0 ? Mathf.Abs(normalized.x) : -Mathf.Abs(normalized.x));
            velocityComponent.velocity.y = initialDashSpeed * normalized.y;

            isDashStarted = true;
        }
        base.ApplyGroundMovement();
    }

    public void SetDashInput(float direction)
    {
        if (direction < 0)
        {
            if (!isDashing)
            {
                Debug.Log("Dash Left");
            }
            isDashing = true;
        }
        else if (direction > 0)
        {
            if (!isDashing)
            {
                Debug.Log("Dash Right");
            }
            isDashing = true;
        }
        else
        {
            if (isDashing)
            {
                Debug.Log("Stop");
                isDashing = false;
                isDashStarted = false;
            }
        }
    }
}