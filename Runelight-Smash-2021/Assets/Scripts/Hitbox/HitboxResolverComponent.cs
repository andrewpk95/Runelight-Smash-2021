using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxResolverComponent : Singleton<HitboxResolverComponent>
{
    private SortedSet<HitboxHitResult> hits = new SortedSet<HitboxHitResult>();
    private Graph<GameObject> resolvedPairs = new Graph<GameObject>();
    private HitboxVictimGraph victimGraph = new HitboxVictimGraph();

    void FixedUpdate()
    {
        ResolveHitboxCollisions();
        Reset();
    }

    private void ResolveHitboxCollisions()
    {
        foreach (HitboxHitResult hit in hits)
        {
            // Debug.Log($"{hit.Attacker.name}'s {hit.AttackerHitbox.name} (id: {hit.AttackerHitboxInfo.id}, groupId: {hit.AttackerHitboxInfo.groupId}) hit {hit.Victim.name}'s {hit.VictimHitbox.name}");
        }
        foreach (HitboxHitResult hit in hits)
        {
            if (resolvedPairs.IsConnected(hit.Attacker, hit.Victim))
            {
                continue;
            }
            if (victimGraph.IsConnected(hit.Attacker, hit.Victim, hit.AttackerHitboxInfo.groupId))
            {
                continue;
            }
            Resolve(hit);

            // TODO: If resolve returns false (ex. An attack out-prioritizes the other and must hit the other's hitbox), do not add edge
            resolvedPairs.AddEdge(hit.Attacker, hit.Victim);
            victimGraph.AddEdge(hit.Attacker, hit.Victim, hit.AttackerHitboxInfo.groupId);
        }
    }

    private void Resolve(HitboxHitResult hit)
    {
        switch ((hit.AttackerHitboxInfo.type, hit.VictimHitboxInfo.type))
        {
            case (HitboxType.Collision, _):
                Debug.Log($"[Collision Event] {hit.Attacker.name} collided with {hit.Victim.name}");
                break;
            case (HitboxType.Grab, HitboxType.Grab):
                Debug.Log($"[Grab Clash Event] {hit.Attacker.name} clashed with {hit.Victim.name}");
                break;
            case (HitboxType.Grab, HitboxType.Damageable):
                Debug.Log($"[Grab Event] {hit.Attacker.name} grabbed {hit.Victim.name}");
                break;
            case (HitboxType.Attack, HitboxType.Attack):
            case (HitboxType.Attack, HitboxType.Projectile):
            case (HitboxType.Projectile, HitboxType.Projectile):
                Debug.Log($"[Attack Clash Event] {hit.Attacker.name} (id: {hit.AttackerHitboxInfo.id}, groupId: {hit.AttackerHitboxInfo.groupId}) clashed with {hit.Victim.name} (id: {hit.VictimHitboxInfo.id}, groupId: {hit.VictimHitboxInfo.groupId}), one may out-prioritize the other");
                break;
            case (HitboxType.Attack, HitboxType.Shield):
            case (HitboxType.Projectile, HitboxType.Shield):
                Debug.Log($"[Shield Event] {hit.Attacker.name} (id: {hit.AttackerHitboxInfo.id}, groupId: {hit.AttackerHitboxInfo.groupId}) attacked {hit.Victim.name}'s shield, may be unblockable");
                break;
            case (HitboxType.Projectile, HitboxType.Absorbing):
                Debug.Log($"[Absorb Event] {hit.Attacker.name}'s projectile was absorbed by {hit.Victim.name}");
                break;
            case (HitboxType.Projectile, HitboxType.Reflective):
                Debug.Log($"[Reflect Event] {hit.Attacker.name}'s projectile was reflected by {hit.Victim.name}, may not be able to reflect");
                break;
            case (HitboxType.Attack, HitboxType.Invincible):
            case (HitboxType.Projectile, HitboxType.Invincible):
                Debug.Log($"[Invincible hit Event] {hit.Attacker.name} (id: {hit.AttackerHitboxInfo.id}, groupId: {hit.AttackerHitboxInfo.groupId}) attacked invincible {hit.Victim.name}");
                break;
            case (HitboxType.Attack, HitboxType.Damageable):
            case (HitboxType.Projectile, HitboxType.Damageable):
                Debug.Log($"[Damage Event] {hit.Attacker.name} (id: {hit.AttackerHitboxInfo.id}, groupId: {hit.AttackerHitboxInfo.groupId}) damaged {hit.Victim.name}");
                break;
            case (HitboxType.Wind, HitboxType.Shield):
                Debug.Log($"[Wind Event] {hit.Attacker.name} is pushing {hit.Victim.name}'s shield");
                break;
            case (HitboxType.Wind, HitboxType.Damageable):
                Debug.Log($"[Wind Event] {hit.Attacker.name} is pushing {hit.Victim.name}");
                break;
            default:
                break;
        }
    }

    public void Reset()
    {
        hits.Clear();
        resolvedPairs.Clear();
    }

    public void AddHitResult(HitboxHitResult hit)
    {
        hits.Add(hit);
    }

    public void ResetVictim(GameObject attacker, int groupId)
    {
        victimGraph.RemoveNode(attacker, groupId);
    }
}
