using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxComponent : Poolable
{
    // Required Variables
    public HitboxInfo hitboxInfo;

    // Public Hitbox States
    public GameObject owner;
    public Vector2 hitboxPosition;
    public Vector2 hitboxSize;
    public CapsuleDirection2D hitboxDirection;
    public float hitboxRotation;
    public bool isHitboxEnabled;

    // Hitbox variables
    public Transform hitboxTransform;
    public CapsuleCollider2D hitboxCollider;

    // Hitbox Interpolation variables
    [SerializeField]
    private Vector3 prevHitboxPosition;
    private Vector3 currentHitboxPosition;

    void Start()
    {
        RefreshHitbox();
        // TODO: Get actual hitbox owner
        owner = transform.root.gameObject;
    }

    protected override void Tick()
    {
        // TODO: Get actual hitbox owner
        owner = transform.root.gameObject;

        UpdateHitboxShape();

        if (!hitboxInfo.isHitboxType())
        {
            return;
        }

        InterpolateHitbox();
    }

    private void InterpolateHitbox()
    {
        if (hitboxInfo.isCapsule)
        {
            return;
        }

        if (prevHitboxPosition == Vector3.zero)
        {
            prevHitboxPosition = hitboxTransform.position;
        }

        currentHitboxPosition = hitboxTransform.position;
        Vector3 lengthVector = currentHitboxPosition - prevHitboxPosition;
        Vector3 centerPos = (prevHitboxPosition + currentHitboxPosition) / 2;
        float length = lengthVector.magnitude;
        float thickness = hitboxInfo.radius * 2.0f;

        hitboxPosition = centerPos;
        hitboxRotation = Vector2.SignedAngle(Vector2.up, lengthVector);
        hitboxSize = new Vector2(thickness, thickness + length);
        hitboxCollider.transform.position = hitboxPosition;
        hitboxCollider.transform.eulerAngles = Vector3.forward * hitboxRotation;
        hitboxCollider.size = hitboxSize;

        prevHitboxPosition = currentHitboxPosition;
    }

    private void RefreshHitbox()
    {
        UpdateHitboxShape();
        UpdateHitboxLayer();
    }

    private void UpdateHitboxShape()
    {
        float thickness = hitboxInfo.radius * 2.0f;

        hitboxTransform.localPosition = (Vector3)hitboxInfo.position;
        hitboxPosition = hitboxTransform.position;
        hitboxRotation = hitboxTransform.eulerAngles.z;
        hitboxSize = new Vector2(thickness, thickness + hitboxInfo.length);
        hitboxCollider.transform.position = hitboxPosition;
        hitboxCollider.transform.eulerAngles = Vector3.forward * hitboxRotation;
        hitboxCollider.size = hitboxSize;

        if (hitboxInfo.isCapsule)
        {
            hitboxDirection = hitboxInfo.direction;
            hitboxSize = hitboxInfo.direction == CapsuleDirection2D.Vertical ? new Vector2(thickness, thickness + hitboxInfo.length) : new Vector2(thickness + hitboxInfo.length, thickness);
            hitboxCollider.direction = hitboxDirection;
            hitboxCollider.size = hitboxSize;
        }
        else
        {
            hitboxSize = Vector2.one * thickness;
            hitboxCollider.size = hitboxSize;
        }
    }

    private void UpdateHitboxLayer()
    {
        int hitboxLayer = 0;

        if (!IsObjectEnabled)
        {
            hitboxCollider.gameObject.layer = LayerMask.NameToLayer("Default");
            return;
        }

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
    }

    public void SetHitboxInfo(HitboxInfo hitboxInfo)
    {
        this.hitboxInfo = hitboxInfo;
        RefreshHitbox();
    }

    public override void Reset()
    {
        prevHitboxPosition = Vector3.zero;
        hitboxTransform.position = Vector3.zero;
        hitboxCollider.transform.position = Vector3.zero;
        hitboxCollider.transform.localEulerAngles = Vector3.zero;
        owner = null;
    }

    void OnValidate()
    {
        RefreshHitbox();
    }

    void OnDrawGizmos()
    {
        if (!IsObjectEnabled)
        {
            return;
        }

        Color hitboxColor = ColorMap.GetColor(hitboxInfo);

        DebugTool.DrawCapsule(hitboxCollider.transform.position, hitboxSize, hitboxDirection, hitboxRotation, hitboxColor);
    }
}
