using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxResolverComponent : Singleton<HitboxResolverComponent>
{
    private SortedSet<HitboxHitResult> hits = new SortedSet<HitboxHitResult>();

    void FixedUpdate()
    {
        ResolveHitboxCollisions();
        hits.Clear();
    }

    private void ResolveHitboxCollisions()
    {
        foreach (HitboxHitResult hit in hits)
        {
            Debug.Log($"{hit.Attacker.name}'s {hit.AttackerHitbox.type.ToString()}Hitbox hit {hit.Victim.name}'s {hit.VictimHitbox.type.ToString()}Hitbox");
        }
    }

    public void AddHitResult(HitboxHitResult hit)
    {
        hits.Add(hit);
    }
}
