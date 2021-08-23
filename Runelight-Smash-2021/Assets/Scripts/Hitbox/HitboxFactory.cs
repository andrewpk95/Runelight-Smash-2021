using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxFactory : ObjectPool<HitboxComponent>
{
    protected override void ResetObject(HitboxComponent obj)
    {
        base.ResetObject(obj);

        obj.Reset();
    }
}
