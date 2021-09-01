using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugTool
{
    public static void DrawCapsule(Vector2 origin, Vector2 size, CapsuleDirection2D capsuleDirection, float angle, Color color)
    {
        bool isVertical = capsuleDirection == CapsuleDirection2D.Vertical;
        float rotateAngle = angle + (isVertical ? 0.0f : 90.0f);
        Quaternion rotation = Quaternion.AngleAxis(rotateAngle, Vector3.forward);
        Matrix4x4 rotatedMatrix = UnityEditor.Handles.matrix * Matrix4x4.TRS((Vector3)origin, rotation, Vector3.one);

        Vector3 capsulePos = Vector3.zero;
        float capsuleX = isVertical ? size.x : size.y;
        float capsuleY = isVertical ? size.y : size.x;
        float radius = capsuleX / 2;
        float capsuleLength = Mathf.Max(0.0f, capsuleY - capsuleX);
        Vector3 topCenter = capsulePos + Vector3.up * capsuleLength / 2;
        Vector3 bottomCenter = capsulePos - Vector3.up * capsuleLength / 2;
        Rect rect = new Rect(capsulePos.x - capsuleX / 2, capsulePos.y - capsuleLength / 2, capsuleX, capsuleLength);

        using (new UnityEditor.Handles.DrawingScope(color, rotatedMatrix))
        {
            UnityEditor.Handles.DrawSolidArc(topCenter, Vector3.forward, Vector3.right, 180.0f, radius);
            UnityEditor.Handles.DrawSolidRectangleWithOutline(rect, Color.white, Color.clear);
            UnityEditor.Handles.DrawSolidArc(bottomCenter, Vector3.forward, Vector3.left, 180.0f, radius);
        }
    }
}