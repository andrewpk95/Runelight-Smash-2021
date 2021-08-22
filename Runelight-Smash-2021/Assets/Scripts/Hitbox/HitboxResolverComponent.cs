using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxResolverComponent : Singleton<HitboxResolverComponent>
{
    public List<HitboxHitResult> hits = new List<HitboxHitResult>();

    void FixedUpdate()
    {
        ResolveHitboxCollisions();
        hits.Clear();
    }

    private void ResolveHitboxCollisions()
    {
        foreach (HitboxHitResult hit in hits)
        {
            Debug.Log($"{hit.attacker1.name} hit {hit.attacker2.name}");
        }
    }
}
