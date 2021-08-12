using UnityEngine;

public static class Slope
{
    public static Vector2 GetSignedSlopeDirection(Vector2 normal)
    {
        return -Vector2.Perpendicular(normal).normalized;
    }

    public static Vector2 GetSlopeDirection(Vector2 normal)
    {
        Vector2 signedSlopeDirection = GetSignedSlopeDirection(normal);

        return signedSlopeDirection.y < 0.0f ? -signedSlopeDirection : signedSlopeDirection;
    }

    public static float GetSignedSlopeAngle(Vector2 normal)
    {
        return -Vector2.SignedAngle(normal, Vector2.up);
    }

    public static float GetSlopeAngle(Vector2 normal)
    {
        float signedSlopeAngle = GetSignedSlopeAngle(normal);

        return Mathf.Abs(signedSlopeAngle);
    }

    public static int SortBySlopeAngle(Vector2 normal1, Vector2 normal2)
    {
        float angle1 = -Vector2.SignedAngle(normal1, Vector2.up);
        float angle2 = -Vector2.SignedAngle(normal2, Vector2.up);

        return angle1.CompareTo(angle2);
    }
}