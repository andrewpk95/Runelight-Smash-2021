using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxDetectionComponent : MonoBehaviour
{
    // Required Variables
    [SerializeField]
    private HitboxComponent hitboxComponent;

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
    }

    private void CheckHurtboxCollision()
    {
        int count = Physics2D.OverlapCapsule(hitboxComponent.hitboxPosition, hitboxComponent.hitboxSize, hitboxComponent.hitboxDirection, hitboxComponent.hitboxRotation, contactFilter, colliders);

        for (int i = 0; i < count; i++)
        {
            Collider2D col = colliders[i];
            GameObject hitObject = col.gameObject;

            SendCollisionResult(hitObject);
        }
    }

    private void SendCollisionResult(GameObject hitObject)
    {
        HitboxComponent other = hitObject.transform.parent.parent.gameObject.GetComponent<HitboxComponent>();

        if (!other)
        {
            return;
        }

        if (hitboxComponent.owner == other.owner)
        {
            return;
        }

        HitboxHitResult hit = new HitboxHitResult(hitboxComponent, other);

        HitboxResolverComponent.Instance.AddHitResult(hit);
    }

    void OnDisable()
    {
        GameObject owner = hitboxComponent?.owner;

        if (!owner)
        {
            return;
        }

        HitboxResolverComponent.Instance.ResetVictim(hitboxComponent.owner, hitboxComponent.hitboxInfo.groupId);
    }
}
