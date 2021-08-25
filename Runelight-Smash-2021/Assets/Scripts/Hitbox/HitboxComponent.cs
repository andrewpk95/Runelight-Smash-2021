using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxComponent : MonoBehaviour
{
    // Required Variables
    [SerializeField]
    private HitboxInfo hitboxInfo;

    // Public Hitbox Collision States
    public HashSet<GameObject> hitObjects = new HashSet<GameObject>();
    public HashSet<GameObject> prevHitObjects = new HashSet<GameObject>();

    // Hitbox Collision variables
    public GameObject attacker;
    public CapsuleCollider2D hitboxCollider;
    private ContactFilter2D contactFilter = new ContactFilter2D();
    private Collider2D[] colliders = new Collider2D[100];

    // Hitbox Interpolation variables
    [SerializeField]
    private Vector3 prevHitboxPosition;
    private Vector3 currentHitboxPosition;
    private CapsuleCollider2D interpolatedCapsule;

    void Start()
    {
        contactFilter.useTriggers = true;

        RefreshHitbox();
    }

    void FixedUpdate()
    {
        if (!hitboxInfo.isHitboxType())
        {
            return;
        }
        // TODO: Get actual hitbox owner
        attacker = transform.root.gameObject;

        InterpolateHitbox();
        CheckHurtboxCollision();
        SendCollisionResults();
    }

    private void InterpolateHitbox()
    {
        if (hitboxInfo.isCapsule)
        {
            hitboxCollider.transform.localPosition = Vector3.zero;
            return;
        }

        Vector3 currentHitboxPosition = transform.position;
        Vector3 lengthVector = currentHitboxPosition - prevHitboxPosition;
        Vector3 centerPos = (prevHitboxPosition + currentHitboxPosition) / 2;
        float length = lengthVector.magnitude;
        float thickness = hitboxInfo.radius * 2.0f;

        hitboxCollider.transform.position = centerPos;
        hitboxCollider.transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, lengthVector);
        hitboxCollider.size = new Vector2(thickness, thickness + length);

        prevHitboxPosition = currentHitboxPosition;
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

    private void RefreshHitbox()
    {
        UpdateHitboxShape();
        UpdateHitboxLayer();
    }

    private void UpdateHitboxShape()
    {
        float thickness = hitboxInfo.radius * 2.0f;

        transform.localPosition = (Vector3)hitboxInfo.position;
        hitboxCollider.transform.localPosition = Vector3.zero;
        hitboxCollider.transform.eulerAngles = Vector3.zero;

        if (hitboxInfo.isCapsule)
        {
            hitboxCollider.direction = hitboxInfo.direction;
            hitboxCollider.size = hitboxInfo.direction == CapsuleDirection2D.Vertical ? new Vector2(thickness, thickness + hitboxInfo.length) : new Vector2(thickness + hitboxInfo.length, thickness);
        }
        else
        {
            hitboxCollider.size = Vector2.one * thickness;
        }
        prevHitboxPosition = hitboxCollider.transform.position;
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
        RefreshHitbox();
    }

    public void Reset()
    {
        hitObjects.Clear();
        prevHitObjects.Clear();
    }

    void OnEnable()
    {
        prevHitboxPosition = transform.position;
    }

    void OnDisable()
    {
        Reset();
    }

    void OnValidate()
    {
        if (!hitboxCollider)
        {
            hitboxCollider = GetComponent<CapsuleCollider2D>();
        }
        RefreshHitbox();
    }

    void OnDrawGizmos()
    {
        if (!hitboxCollider)
        {
            hitboxCollider = GetComponent<CapsuleCollider2D>();
        }
        CapsuleCollider2D capsule = hitboxCollider;
        Color hitboxColor = ColorMap.GetColor(hitboxInfo);

        DebugTool.DrawCapsule(capsule, hitboxColor);
    }
}
