using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxDetectionComponent : MonoBehaviour
{
    // Required Variables
    [SerializeField]
    private HitboxComponent hitboxComponent;

    // Public Hitbox Collision States
    public HashSet<GameObject> hitObjects = new HashSet<GameObject>();
    public HashSet<GameObject> prevHitObjects = new HashSet<GameObject>();

    // Hitbox Collision variables
    private ContactFilter2D contactFilter = new ContactFilter2D();
    private Collider2D[] colliders = new Collider2D[100];

    void Start()
    {
        hitboxComponent = GetComponent<HitboxComponent>();

        contactFilter.useTriggers = true;
    }

    void FixedUpdate()
    {
        if (!hitboxComponent.hitboxInfo.isHitboxType())
        {
            return;
        }
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(hitboxComponent.hitboxCollider.gameObject.layer));
        CheckHurtboxCollision();
        SendCollisionResults();
    }

    private void CheckHurtboxCollision()
    {
        int count = Physics2D.OverlapCapsule(hitboxComponent.hitboxPosition, hitboxComponent.hitboxSize, hitboxComponent.hitboxDirection, hitboxComponent.hitboxRotation, contactFilter, colliders);

        for (int i = 0; i < count; i++)
        {
            Collider2D col = colliders[i];
            GameObject hitObject = col.gameObject;

            hitObjects.Add(hitObject);
        }
    }

    private void SendCollisionResults()
    {
        foreach (GameObject hitObject in hitObjects)
        {
            // TODO: Get actual hitbox/hurtbox owner
            GameObject owner = hitObject.transform.root.gameObject;

            if (hitboxComponent.attacker == owner || prevHitObjects.Contains(hitObject))
            {
                continue;
            }

            HitboxComponent other = hitObject.transform.parent.parent.gameObject.GetComponent<HitboxComponent>();

            if (!other)
            {
                continue;
            }

            HitboxHitResult hit = new HitboxHitResult(hitboxComponent.attacker, owner, hitboxComponent, other, hitboxComponent.hitboxInfo, other.hitboxInfo);

            HitboxResolverComponent.Instance.AddHitResult(hit);
            prevHitObjects.Add(hitObject);
        }
    }

    public void Reset()
    {
        hitObjects.Clear();
        prevHitObjects.Clear();
    }

    void OnDisable()
    {
        Reset();
    }
}
