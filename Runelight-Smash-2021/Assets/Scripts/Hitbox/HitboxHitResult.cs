using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitboxHitResult : IEquatable<HitboxHitResult>
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

    public bool Equals(HitboxHitResult other)
    {
        bool areAttackersEqual = (this.attacker1 == other.attacker1 && this.attacker2 == other.attacker2) || (this.attacker1 == other.attacker2 && this.attacker2 == other.attacker1);
        bool areHitboxInfosEqual = (this.hitbox1 == other.hitbox1 && this.hitbox2 == other.hitbox2) || (this.hitbox1 == other.hitbox2 && this.hitbox2 == other.hitbox1);

        return areAttackersEqual && areHitboxInfosEqual;
    }

    public override int GetHashCode()
    {
        return this.attacker1.GetHashCode() * this.attacker2.GetHashCode() + this.hitbox1.GetHashCode() * this.hitbox2.GetHashCode();
    }
}