using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FighterUnit : ControllableUnit
{
    protected override float groundSpeed { get { return isDashing ? maxDashSpeed : maxWalkSpeed; } }
    protected override float groundAccelerationRate { get { return isDashing ? dashAccelerationRate : walkAccelerationRate; } }
    // Dash 
    public bool isDashing;
    public float initialDashSpeed = 6.0f;
    public float dashAccelerationRate = 40.0f;
    public float maxDashSpeed = 12.0f;

    protected override void ApplyGroundMovement()
    {
        if (isDashing && velocity.magnitude < initialDashSpeed)
        {
            Vector2 normalized = velocity.normalized;

            velocity.x = initialDashSpeed * (joystick.x > 0 ? Mathf.Abs(normalized.x) : -Mathf.Abs(normalized.x));
            velocity.y = initialDashSpeed * normalized.y;
        }
        base.ApplyGroundMovement();
    }

    public void SetDashInput(float direction)
    {
        if (direction < 0)
        {
            Debug.Log("Dash Left");
            isDashing = true;
        }
        else if (direction > 0)
        {
            Debug.Log("Dash Right");
            isDashing = true;
        }
        else
        {
            if (isDashing)
            {
                Debug.Log("Stop");
                isDashing = false;
            }
        }
    }
}