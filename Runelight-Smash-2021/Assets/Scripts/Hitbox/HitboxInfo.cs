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

    public HitboxInfo(short id, short groupId, HitboxType type, Vector2 position)
    {
        this.id = id;
        this.groupId = groupId;
        this.type = type;
        this.position = position;
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
