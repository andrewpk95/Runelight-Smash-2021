using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider2D))]

public class HurtboxComponent : MonoBehaviour
{
    // Required Variables
    public HurtboxInfo hurtboxInfo;

    private CapsuleCollider2D hurtboxCollider;

    void Start()
    {
        hurtboxCollider = GetComponent<CapsuleCollider2D>();
        UpdateHurtboxLayer();
    }

    private void UpdateHurtboxLayer()
    {
        switch (hurtboxInfo.type)
        {
            case HurtboxType.Damageable:
                gameObject.layer = LayerMask.NameToLayer("DamageableHurtbox");
                break;
            case HurtboxType.Invincible:
                gameObject.layer = LayerMask.NameToLayer("InvincibleHurtbox");
                break;
            case HurtboxType.Intangible:
                gameObject.layer = LayerMask.NameToLayer("IntangibleHurtbox");
                break;
            case HurtboxType.Reflective:
                gameObject.layer = LayerMask.NameToLayer("ReflectiveHurtbox");
                break;
            case HurtboxType.Shield:
                gameObject.layer = LayerMask.NameToLayer("ShieldHurtbox");
                break;
            case HurtboxType.Absorbing:
                gameObject.layer = LayerMask.NameToLayer("AbsorbingHurtbox");
                break;
            default:
                break;
        }
    }

    public void SetHurtboxType(HurtboxType type)
    {
        hurtboxInfo.type = type;
        UpdateHurtboxLayer();
    }

    public void OnValidate()
    {
        UpdateHurtboxLayer();
    }

    void OnDrawGizmos()
    {
        if (!hurtboxCollider)
        {
            hurtboxCollider = GetComponent<CapsuleCollider2D>();
        }
        CapsuleCollider2D capsule = hurtboxCollider;
        Color hitboxColor = ColorMap.GetColor(hurtboxInfo.type);

        DebugTool.DrawCapsule(capsule, hitboxColor);
    }
}
