using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitboxInfo
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
}
