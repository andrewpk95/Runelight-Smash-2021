using UnityEngine;

public static class Velocity
{
    public static float GetNewVelocity(float currentVelocity, float targetVelocity, float accelerationRate)
    {
        int direction = currentVelocity > targetVelocity ? -1 : 1;
        float acceleration = Mathf.Abs(accelerationRate * Time.fixedDeltaTime);
        float newVelocity = currentVelocity + acceleration * direction;
        float velocityDifference = Mathf.Abs(currentVelocity - targetVelocity);

        if (velocityDifference < acceleration)
        {
            return targetVelocity;
        }

        return newVelocity;
    }
}