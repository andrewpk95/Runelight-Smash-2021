using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitboxHitResult : IComparable<HitboxHitResult>
{
    public GameObject Attacker { get { return attacker.owner; } }
    public GameObject Victim { get { return victim.owner; } }
    public HitboxComponent AttackerHitbox { get { return attacker; } }
    public HitboxComponent VictimHitbox { get { return victim; } }
    public HitboxInfo AttackerHitboxInfo { get { return attacker.hitboxInfo; } }
    public HitboxInfo VictimHitboxInfo { get { return victim.hitboxInfo; } }

    private HitboxComponent attacker;
    private HitboxComponent victim;

    public HitboxHitResult(HitboxComponent attacker, HitboxComponent victim)
    {
        // Sorts attacker and victim in a way so that 
        // attacker with hitbox type comes before attacker with hurtbox type
        // If both hitbox types are equal (ex. Attack vs Attack should clash)
        // sort attacker with lower hashcode so that GetHashCode() returns consistently with same value
        int result = attacker.hitboxInfo.CompareTo(victim.hitboxInfo);

        if (result < 0)
        {
            this.attacker = attacker;
            this.victim = victim;
        }
        else if (result > 0)
        {
            this.attacker = victim;
            this.victim = attacker;
        }
        else
        {
            if (attacker.owner.GetHashCode() < victim.owner.GetHashCode())
            {
                this.attacker = attacker;
                this.victim = victim;
            }
            else
            {
                this.attacker = victim;
                this.victim = attacker;
            }
        }
    }

    public int CompareTo(HitboxHitResult other)
    {
        int result = this.VictimHitboxInfo.CompareTo(other.VictimHitboxInfo);

        if (result != 0)
        {
            return result;
        }

        result = this.AttackerHitboxInfo.CompareTo(other.AttackerHitboxInfo);

        if (result != 0)
        {
            return result;
        }

        result = other.Attacker.GetHashCode() - this.Attacker.GetHashCode();

        if (result != 0)
        {
            return result;
        }

        result = other.Victim.GetHashCode() - this.Victim.GetHashCode();

        if (result != 0)
        {
            return result;
        }

        result = other.AttackerHitbox.GetHashCode() - this.AttackerHitbox.GetHashCode();

        if (result != 0)
        {
            return result;
        }

        result = other.VictimHitbox.GetHashCode() - this.VictimHitbox.GetHashCode();

        if (result != 0)
        {
            return result;
        }

        return 0;
    }

    public override int GetHashCode()
    {
        return (17 * this.Attacker.GetHashCode() + 23 * this.Victim.GetHashCode()) - (17 * this.AttackerHitbox.GetHashCode() + 23 * this.VictimHitbox.GetHashCode());
    }
}