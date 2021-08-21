using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HurtboxInfo
{
    public HurtboxType type;

    public HurtboxInfo(HurtboxType type)
    {
        this.type = type;
    }
}
