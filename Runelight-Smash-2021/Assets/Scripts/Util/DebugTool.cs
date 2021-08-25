using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugTool
{
    public static void DrawCapsule(CapsuleCollider2D capsule, Color color)
    {
        bool isVertical = capsule.direction == CapsuleDirection2D.Vertical;
        float rotateAngle = capsule.transform.rotation.eulerAngles.z + (isVertical ? 0.0f : 90.0f);
        Quaternion rotation = Quaternion.AngleAxis(rotateAngle, Vector3.forward);
        Matrix4x4 rotatedMatrix = UnityEditor.Handles.matrix * Matrix4x4.TRS(capsule.transform.position, rotation, capsule.transform.localScale);

        Vector3 capsulePos = (Vector3)capsule.offset;
        float capsuleX = isVertical ? capsule.size.x : capsule.size.y;
        float capsuleY = isVertical ? capsule.size.y : capsule.size.x;
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