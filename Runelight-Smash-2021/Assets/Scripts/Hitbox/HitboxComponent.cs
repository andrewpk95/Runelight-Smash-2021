using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]

public class HitboxComponent : MonoBehaviour
{
    // Required Variables
    [SerializeField]
    private HitboxInfo hitboxInfo;

    // Public Hitbox Collision States
    public HashSet<GameObject> hitObjects = new HashSet<GameObject>();
    public HashSet<GameObject> prevHitObjects = new HashSet<GameObject>();

    // Hitbox Collision variables
    private GameObject attacker;
    private CapsuleCollider2D hitboxCollider;
    private ContactFilter2D contactFilter = new ContactFilter2D();
    private Collider2D[] colliders = new Collider2D[100];

    void Start()
    {
        // TODO: Get actual hitbox owner
        attacker = transform.root.gameObject;

        hitboxCollider = GetComponent<CapsuleCollider2D>();

        contactFilter.useTriggers = true;

        UpdateHitboxLayer();
    }

    void FixedUpdate()
    {
        if (!hitboxInfo.isHitboxType())
        {
            return;
        }
        CheckHurtboxCollision();
        SendCollisionResults();
    }

    private void CheckHurtboxCollision()
    {
        int count = Physics2D.OverlapCollider(hitboxCollider, contactFilter, colliders);

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

            if (attacker == owner || prevHitObjects.Contains(hitObject))
            {
                continue;
            }

            HitboxComponent hitboxComponent = hitObject.GetComponent<HitboxComponent>();

            if (!hitboxComponent)
            {
                continue;
            }

            HitboxHitResult hit = new HitboxHitResult(attacker, owner, hitboxInfo, hitboxComponent.hitboxInfo);

            HitboxResolverComponent.Instance.AddHitResult(hit);
            prevHitObjects.Add(hitObject);
        }
    }

    private void UpdateHitboxLayer()
    {
        switch (hitboxInfo.type)
        {
            case HitboxType.Attack:
                gameObject.layer = LayerMask.NameToLayer("AttackHitbox");
                break;
            case HitboxType.Projectile:
                gameObject.layer = LayerMask.NameToLayer("ProjectileHitbox");
                break;
            case HitboxType.Grab:
                gameObject.layer = LayerMask.NameToLayer("GrabHitbox");
                break;
            case HitboxType.Collision:
                gameObject.layer = LayerMask.NameToLayer("CollisionHitbox");
                break;
            case HitboxType.Wind:
                gameObject.layer = LayerMask.NameToLayer("WindHitbox");
                break;
            case HitboxType.Damageable:
                gameObject.layer = LayerMask.NameToLayer("DamageableHitbox");
                break;
            case HitboxType.Invincible:
                gameObject.layer = LayerMask.NameToLayer("InvincibleHitbox");
                break;
            case HitboxType.Intangible:
                gameObject.layer = LayerMask.NameToLayer("IntangibleHitbox");
                break;
            case HitboxType.Reflective:
                gameObject.layer = LayerMask.NameToLayer("ReflectiveHitbox");
                break;
            case HitboxType.Shield:
                gameObject.layer = LayerMask.NameToLayer("ShieldHitbox");
                break;
            case HitboxType.Absorbing:
                gameObject.layer = LayerMask.NameToLayer("AbsorbingHitbox");
                break;
            default:
                break;
        }
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
    }

    public void SetHitboxInfo(HitboxInfo hitboxInfo)
    {
        this.hitboxInfo = hitboxInfo;
        UpdateHitboxLayer();
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

    void OnValidate()
    {
        UpdateHitboxLayer();
    }

    void OnDrawGizmos()
    {
        if (!hitboxCollider)
        {
            hitboxCollider = GetComponent<CapsuleCollider2D>();
        }
        CapsuleCollider2D capsule = hitboxCollider;
        Color hitboxColor = ColorMap.GetColor(hitboxInfo.type);

        DebugTool.DrawCapsule(capsule, hitboxColor);
    }
}
