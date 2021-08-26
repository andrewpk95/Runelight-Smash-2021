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
    public Transform hitboxTransform;
    public CapsuleCollider2D hitboxCollider;
    private ContactFilter2D contactFilter = new ContactFilter2D();
    private Collider2D[] colliders = new Collider2D[100];

    // Hitbox Interpolation variables
    [SerializeField]
    private Vector3 prevHitboxPosition;
    private Vector3 currentHitboxPosition;

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
            hitboxTransform.localPosition = (Vector3)hitboxInfo.position;
            return;
        }

        Vector3 currentHitboxPosition = hitboxTransform.position;
        Vector3 lengthVector = currentHitboxPosition - prevHitboxPosition;
        Vector3 centerPos = (prevHitboxPosition + currentHitboxPosition) / 2;
        float length = lengthVector.magnitude;
        float thickness = hitboxInfo.radius * 2.0f;

        hitboxCollider.transform.position = centerPos;
        hitboxCollider.transform.eulerAngles = Vector3.forward * Vector2.SignedAngle(Vector2.up, lengthVector);
        hitboxCollider.size = new Vector2(thickness, thickness + length);

        Debug.DrawLine(prevHitboxPosition, currentHitboxPosition, Color.red);

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

            HitboxComponent hitboxComponent = hitObject.transform.parent.parent.gameObject.GetComponent<HitboxComponent>();

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
        Reset();
        UpdateHitboxShape();
        UpdateHitboxLayer();
    }

    private void UpdateHitboxShape()
    {
        float thickness = hitboxInfo.radius * 2.0f;

        hitboxTransform.localPosition = (Vector3)hitboxInfo.position;
        hitboxCollider.transform.localPosition = Vector3.zero;

        if (hitboxInfo.isCapsule)
        {
            hitboxCollider.direction = hitboxInfo.direction;
            hitboxCollider.size = hitboxInfo.direction == CapsuleDirection2D.Vertical ? new Vector2(thickness, thickness + hitboxInfo.length) : new Vector2(thickness + hitboxInfo.length, thickness);
        }
        else
        {
            hitboxCollider.size = Vector2.one * thickness;
        }
        prevHitboxPosition = hitboxTransform.position;
    }

    private void UpdateHitboxLayer()
    {
        int hitboxLayer = 0;

        switch (hitboxInfo.type)
        {
            case HitboxType.Attack:
                hitboxLayer = LayerMask.NameToLayer("AttackHitbox");
                break;
            case HitboxType.Projectile:
                hitboxLayer = LayerMask.NameToLayer("ProjectileHitbox");
                break;
            case HitboxType.Grab:
                hitboxLayer = LayerMask.NameToLayer("GrabHitbox");
                break;
            case HitboxType.Collision:
                hitboxLayer = LayerMask.NameToLayer("CollisionHitbox");
                break;
            case HitboxType.Wind:
                hitboxLayer = LayerMask.NameToLayer("WindHitbox");
                break;
            case HitboxType.Damageable:
                hitboxLayer = LayerMask.NameToLayer("DamageableHitbox");
                break;
            case HitboxType.Invincible:
                hitboxLayer = LayerMask.NameToLayer("InvincibleHitbox");
                break;
            case HitboxType.Intangible:
                hitboxLayer = LayerMask.NameToLayer("IntangibleHitbox");
                break;
            case HitboxType.Reflective:
                hitboxLayer = LayerMask.NameToLayer("ReflectiveHitbox");
                break;
            case HitboxType.Shield:
                hitboxLayer = LayerMask.NameToLayer("ShieldHitbox");
                break;
            case HitboxType.Absorbing:
                hitboxLayer = LayerMask.NameToLayer("AbsorbingHitbox");
                break;
            default:
                break;
        }

        hitboxCollider.gameObject.layer = hitboxLayer;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(hitboxCollider.gameObject.layer));
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
        prevHitboxPosition = Vector3.zero;
        hitboxTransform.position = Vector3.zero;
        hitboxCollider.transform.position = Vector3.zero;
        hitboxCollider.transform.localEulerAngles = Vector3.zero;
    }

    void OnEnable()
    {
        prevHitboxPosition = hitboxTransform.position;
    }

    void OnDisable()
    {
        Reset();
    }

    void OnValidate()
    {
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
