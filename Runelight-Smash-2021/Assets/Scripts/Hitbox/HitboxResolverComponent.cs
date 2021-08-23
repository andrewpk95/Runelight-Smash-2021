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
            Debug.Log($"{hit.attacker1.name}'s {hit.hitbox1.type.ToString()}Hitbox hit {hit.attacker2.name}'s {hit.hitbox2.type.ToString()}Hitbox");
        }
    }

    public void AddHitResult(HitboxHitResult hit)
    {
        hits.Add(hit);
    }
}
