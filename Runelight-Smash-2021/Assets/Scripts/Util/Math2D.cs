using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Math2D
{
    private static float EPSILON = 0.05f;

    public static bool IsRayIntersecting(Ray2D ray1, Ray2D ray2)
    {
        Debug.DrawLine(ray1.origin, ray1.origin + ray1.direction, Color.green);
        Debug.DrawLine(ray2.origin, ray2.origin + ray2.direction, Color.blue);

        float dx = ray2.origin.x - ray1.origin.x;
        float dy = ray2.origin.y - ray1.origin.y;
        float det = ray2.direction.x * ray1.direction.y - ray2.direction.y * ray1.direction.x;

        if (det == 0.0f)
        {
            Vector2 diff = ray2.origin - ray1.origin;

            return Mathf.Abs(ray1.direction.x * diff.y - ray1.direction.y * diff.x) < EPSILON;
        }
        float u = (dy * ray2.direction.x - dx * ray2.direction.y) / det;
        float v = (dy * ray1.direction.x - dx * ray1.direction.y) / det;
        u = Mathf.Abs(u) < EPSILON ? EPSILON : u;
        v = Mathf.Abs(v) < EPSILON ? EPSILON : v;

        return u * v >= 0.0f;
    }
}