using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxResolverComponent : Singleton<HitboxResolverComponent>
{
    public List<HitboxHit> hits = new List<HitboxHit>();

    void FixedUpdate()
    {
        ResolveHitboxCollisions();
        hits.Clear();
    }

    private void ResolveHitboxCollisions()
    {
        foreach (HitboxHit hit in hits)
        {
            Debug.Log($"{hit.attacker.name} hit {hit.victim.name}");
        }
    }
}
