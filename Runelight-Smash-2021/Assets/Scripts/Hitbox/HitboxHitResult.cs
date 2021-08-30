using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HitboxHitResult : IComparable<HitboxHitResult>
{
    public GameObject Attacker { get { return attacker.Item1; } }
    public GameObject Victim { get { return victim.Item1; } }
    public HitboxComponent AttackerHitbox { get { return attacker.Item2; } }
    public HitboxComponent VictimHitbox { get { return victim.Item2; } }
    public HitboxInfo AttackerHitboxInfo { get { return attacker.Item3; } }
    public HitboxInfo VictimHitboxInfo { get { return victim.Item3; } }

    private Tuple<GameObject, HitboxComponent, HitboxInfo> attacker;
    private Tuple<GameObject, HitboxComponent, HitboxInfo> victim;

    public HitboxHitResult(GameObject attacker1, GameObject attacker2, HitboxComponent hitbox1, HitboxComponent hitbox2, HitboxInfo info1, HitboxInfo info2)
    {
        Tuple<GameObject, HitboxComponent, HitboxInfo> tuple1 = new Tuple<GameObject, HitboxComponent, HitboxInfo>(attacker1, hitbox1, info1);
        Tuple<GameObject, HitboxComponent, HitboxInfo> tuple2 = new Tuple<GameObject, HitboxComponent, HitboxInfo>(attacker2, hitbox2, info2);

        // Sorts attacker and victim in a way so that 
        // attacker with hitbox type comes before attacker with hurtbox type
        // If both hitbox types are equal (ex. Attack vs Attack should clash)
        // sort attacker with lower hashcode so that GetHashCode() returns consistently with same value
        if (info1.CompareTo(info2) < 0)
        {
            this.attacker = tuple1;
            this.victim = tuple2;
        }
        else if (info1.CompareTo(info2) > 0)
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
        int result = this.AttackerHitboxInfo.CompareTo(other.AttackerHitboxInfo);

        if (result != 0)
        {
            return result;
        }

        result = this.VictimHitboxInfo.CompareTo(other.VictimHitboxInfo);

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