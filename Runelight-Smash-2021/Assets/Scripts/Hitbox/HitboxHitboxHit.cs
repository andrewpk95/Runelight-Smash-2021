using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitboxHitResult
{
    public GameObject attacker1;
    public GameObject attacker2;

    public HitboxInfo hitbox1;
    public HitboxInfo hitbox2;

    public HitboxHitResult(GameObject attacker1, GameObject attacker2, HitboxInfo hitbox1, HitboxInfo hitbox2)
    {
        this.attacker1 = attacker1;
        this.attacker2 = attacker2;
        this.hitbox1 = hitbox1;
        this.hitbox2 = hitbox2;
    }
}