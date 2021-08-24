using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitboxHitResult : IComparable<HitboxHitResult>
{
    public GameObject Attacker { get { return attacker.Item1; } }
    public GameObject Victim { get { return victim.Item1; } }
    public HitboxInfo AttackerHitbox { get { return attacker.Item2; } }
    public HitboxInfo VictimHitbox { get { return victim.Item2; } }

    private Tuple<GameObject, HitboxInfo> attacker;
    private Tuple<GameObject, HitboxInfo> victim;

    public HitboxHitResult(GameObject attacker1, GameObject attacker2, HitboxInfo hitbox1, HitboxInfo hitbox2)
    {
        Tuple<GameObject, HitboxInfo> tuple1 = new Tuple<GameObject, HitboxInfo>(attacker1, hitbox1);
        Tuple<GameObject, HitboxInfo> tuple2 = new Tuple<GameObject, HitboxInfo>(attacker2, hitbox2);

        // Sorts attacker and victim in a way so that 
        // attacker with hitbox type comes before attacker with hurtbox type
        // If both hitbox types are equal (ex. Attack vs Attack should clash)
        // sort attacker with lower hashcode so that GetHashCode() returns consistently with same value
        if (hitbox1.CompareTo(hitbox2) < 0)
        {
            this.attacker = tuple1;
            this.victim = tuple2;
        }
        else if (hitbox1.CompareTo(hitbox2) > 0)
        {
            this.attacker = tuple2;
            this.victim = tuple1;
        }
        else
        {
            if (attacker1.GetHashCode() < attacker2.GetHashCode())
            {
                this.attacker = tuple1;
                this.victim = tuple2;
            }
            else
            {
                this.attacker = tuple2;
                this.victim = tuple1;
            }
        }
    }

    public int CompareTo(HitboxHitResult other)
    {
        int result = this.AttackerHitbox.CompareTo(other.AttackerHitbox);

        if (result != 0)
        {
            return result;
        }

        result = this.VictimHitbox.CompareTo(other.VictimHitbox);

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

        return 0;
    }

    public override int GetHashCode()
    {
        return (17 * this.Attacker.GetHashCode() + 23 * this.Victim.GetHashCode()) - (17 * this.AttackerHitbox.GetHashCode() + 23 * this.VictimHitbox.GetHashCode());
    }
}