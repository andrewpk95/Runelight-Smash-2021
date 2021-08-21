using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorMap
{
    public static float ALPHA = 0.3f;

    // Hitbox Colors
    public static Color redHitbox = new Color(0.9f, 0.0f, 0.0f, ALPHA);
    public static Color purpleHitbox = new Color(0.353f, 0.0f, 0.353f, ALPHA);
    public static Color orangeHitbox = new Color(0.9f, 0.3f, 0.0f, ALPHA);
    public static Color blueHitbox = new Color(0.0f, 0.71f, 0.71f, ALPHA);
    public static Color grayHitbox = new Color(0.5f, 0.5f, 0.5f, ALPHA);

    // Hurtbox Colors
    public static Color grayHurtbox = new Color(0.7f, 0.7f, 0.7f, ALPHA);
    public static Color greenHurtbox = new Color(0.0f, 1.0f, 0.5f, ALPHA);
    public static Color blueHurtbox = new Color(0.0f, 0.0f, 0.5f, ALPHA);
    public static Color aquaHurtbox = new Color(0.25f, 1.0f, 0.75f, ALPHA);
    public static Color yellowHurtbox = new Color(0.8f, 0.8f, 0.0f, ALPHA);
    public static Color cyanHurtbox = new Color(0.3f, 0.75f, 1.0f, ALPHA);

    public static Color GetColor(HitboxType type)
    {
        switch (type)
        {
            case HitboxType.Attack:
                return redHitbox;
            case HitboxType.Grab:
                return purpleHitbox;
            case HitboxType.Projectile:
                return orangeHitbox;
            case HitboxType.Wind:
                return blueHitbox;
            case HitboxType.Collision:
                return grayHitbox;
            default:
                return Color.white;
        }
    }

    public static Color GetColor(HurtboxType type)
    {
        switch (type)
        {
            case HurtboxType.Damageable:
                return grayHurtbox;
            case HurtboxType.Invincible:
                return greenHurtbox;
            case HurtboxType.Intangible:
                return blueHurtbox;
            case HurtboxType.Reflective:
                return aquaHurtbox;
            case HurtboxType.Shield:
                return yellowHurtbox;
            case HurtboxType.Absorbing:
                return cyanHurtbox;
            default:
                return Color.white;
        }
    }
}