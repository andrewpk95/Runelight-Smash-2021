using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitboxType
{
    // Hitbox Types
    Attack = 1,
    Projectile,
    Grab,
    Collision,
    Wind,

    // Hurtbox Types
    Damageable = 10,
    Invincible,
    Intangible,
    Reflective,
    Shield,
    Absorbing
}
