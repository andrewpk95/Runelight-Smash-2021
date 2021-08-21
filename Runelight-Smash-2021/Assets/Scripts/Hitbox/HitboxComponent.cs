using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]

public class HitboxComponent : MonoBehaviour
{
    // Required Variables
    public HitboxInfo hitboxInfo;

    // Public Hitbox Collision States
    public HashSet<GameObject> hits = new HashSet<GameObject>();

    // Hitbox Collision variables
    private GameObject attacker;
    private Collider2D hitboxCollider;
    private ContactFilter2D contactFilter = new ContactFilter2D();
    private Collider2D[] colliders = new Collider2D[100];

    void Start()
    {
        hitboxInfo = new HitboxInfo(0, 0, HitboxType.Attack);

        attacker = gameObject;
        hitboxCollider = GetComponent<Collider2D>();
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useTriggers = true;
    }

    void FixedUpdate()
    {
        hits.Clear();
        CheckHurtboxCollision();
        SendCollisionResults();
    }

    private void CheckHurtboxCollision()
    {
        int count = Physics2D.OverlapCollider(hitboxCollider, contactFilter, colliders);

        for (int i = 0; i < count; i++)
        {
            Collider2D col = colliders[i];
            GameObject victim = col.gameObject;

            if (attacker == victim)
            {
                continue;
            }
            hits.Add(col.gameObject);
        }
    }

    private void SendCollisionResults()
    {
        foreach (GameObject hurtbox in hits)
        {
            HurtboxComponent hurtboxComponent = hurtbox.GetComponent<HurtboxComponent>();

            if (hurtboxComponent == null)
            {
                continue;
            }

            GameObject victim = hurtbox.transform.root.gameObject;
            HurtboxInfo hurtboxInfo = hurtboxComponent.hurtboxInfo;
            HitboxHit hit = new HitboxHit(attacker, victim, hitboxInfo, hurtboxInfo);

            HitboxResolverComponent.instance.hits.Add(hit);
        }
    }
}
