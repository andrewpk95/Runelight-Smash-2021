using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitboxHit
{
    public GameObject attacker;
    public GameObject victim;

    public HitboxInfo hitbox;
    public HurtboxInfo hurtbox;

    public HitboxHit(GameObject attacker, GameObject victim, HitboxInfo hitbox, HurtboxInfo hurtbox)
    {
        this.attacker = attacker;
        this.victim = victim;
        this.hitbox = hitbox;
        this.hurtbox = hurtbox;
    }
}