using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxResolverComponent : Singleton<HitboxResolverComponent>
{
    private HashSet<HitboxHitResult> hits = new HashSet<HitboxHitResult>();

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

    public void AddHitResult(HitboxHitResult hit)
    {
        hits.Add(hit);
    }
}
