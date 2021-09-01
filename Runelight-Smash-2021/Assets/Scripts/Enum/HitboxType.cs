using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lower number means higher priority
// Ex)  If a hitbox hits Shield(10) and Damageable(14) hurtbox at the same time,
//      The game should register as Shield collision and ignore Damageable collision. 
public enum HitboxType
{
    // Hitbox Types
    Collision = 0,
    Grab = 1,
    Attack = 2,
    Projectile = 3,
    Wind = 4,

    // Hurtbox Types
    Shield = 10,
    Reflective = 11,
    Absorbing = 12,
    Invincible = 13,
    Damageable = 14,
    Intangible = 15,
}
