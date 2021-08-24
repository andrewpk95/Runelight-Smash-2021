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

    public HitboxInfo(short id, short groupId, HitboxType type)
    {
        this.id = id;
        this.groupId = groupId;
        this.type = type;
    }

    public bool isHitboxType()
    {
        return (int)type < 10;
    }

    public int CompareTo(HitboxInfo other)
    {
        return this.type - other.type;
    }
}
