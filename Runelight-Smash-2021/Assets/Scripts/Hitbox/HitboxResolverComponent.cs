using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxResolverComponent : MonoBehaviour
{
    public static HitboxResolverComponent instance;

    public List<HitboxHit> hits = new List<HitboxHit>();

    void Start()
    {
        instance = this;
    }

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
