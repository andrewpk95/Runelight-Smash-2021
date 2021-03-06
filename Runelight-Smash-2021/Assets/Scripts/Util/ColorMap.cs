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

    public static Color GetColor(HitboxInfo info)
    {
        switch (info.type)
        {
            case HitboxType.Attack:
                return redHitbox * (5 - info.id) / 5;
            case HitboxType.Grab:
                return purpleHitbox;
            case HitboxType.Projectile:
                return orangeHitbox * (5 - info.id) / 5;
            case HitboxType.Wind:
                return blueHitbox * (5 - info.id) / 5;
            case HitboxType.Collision:
                return grayHitbox;
            case HitboxType.Damageable:
                return grayHurtbox;
            case HitboxType.Invincible:
                return greenHurtbox;
            case HitboxType.Intangible:
                return blueHurtbox;
            case HitboxType.Reflective:
                return aquaHurtbox;
            case HitboxType.Shield:
                return yellowHurtbox;
            case HitboxType.Absorbing:
                return cyanHurtbox;
            default:
                return Color.white;
        }
    }
}