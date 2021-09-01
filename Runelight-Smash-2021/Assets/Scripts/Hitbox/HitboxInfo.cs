using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct HitboxInfo : IComparable<HitboxInfo>
{
    public short id;
    public short groupId;
    public HitboxType type;
    public Vector2 position;
    public float radius;
    public bool isCapsule;
    public float length;
    public CapsuleDirection2D direction;

    public HitboxInfo(short id, short groupId, HitboxType type, Vector2 position, float radius)
    {
        this.id = id;
        this.groupId = groupId;
        this.type = type;
        this.position = position;
        this.radius = radius;

        this.isCapsule = false;
        this.length = 0;
        this.direction = 0;
    }

    public HitboxInfo(short id, short groupId, HitboxType type, Vector2 position, float radius, float length, CapsuleDirection2D direction)
    {
        this.id = id;
        this.groupId = groupId;
        this.type = type;
        this.position = position;
        this.radius = radius;

        this.isCapsule = true;
        this.length = length;
        this.direction = direction;
    }

    public bool isHitboxType()
    {
        return (int)type < 10;
    }

    public int CompareTo(HitboxInfo other)
    {
        int result = this.type - other.type;

        if (result != 0)
        {
            return result;
        }
        result = this.groupId - other.groupId;

        if (result != 0)
        {
            return result;
        }

        result = this.id - other.id;

        if (result != 0)
        {
            return result;
        }

        return 0;
    }
}
