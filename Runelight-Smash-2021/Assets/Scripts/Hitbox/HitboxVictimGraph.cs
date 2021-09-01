using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxVictimGraph
{
    private DirectedGraph<GameObject> victimGraph = new DirectedGraph<GameObject>();
    private Dictionary<int, DirectedGraph<GameObject>> groupIdVictimGraph = new Dictionary<int, DirectedGraph<GameObject>>();

    public void AddEdge(HitboxComponent attacker, HitboxComponent victim)
    {
        if (attacker.hitboxInfo.isHitboxType() && victim.hitboxInfo.isHitboxType())
        {
            victimGraph.AddEdge(attacker.owner, victim.owner);
            return;
        }

        int groupId = attacker.hitboxInfo.groupId;

        GetGroupIdVictimGraph(groupId).AddDirectedEdge(attacker.owner, victim.owner);
    }

    public void RemoveEdge(HitboxComponent attacker, HitboxComponent victim)
    {
        if (attacker.hitboxInfo.isHitboxType() && victim.hitboxInfo.isHitboxType())
        {
            victimGraph.RemoveEdge(attacker.owner, victim.owner);
            return;
        }

        int groupId = attacker.hitboxInfo.groupId;

        GetGroupIdVictimGraph(groupId).RemoveDirectedEdge(attacker.owner, victim.owner);
    }

    public void RemoveAllEdges(GameObject attacker)
    {
        foreach (DirectedGraph<GameObject> graph in groupIdVictimGraph.Values)
        {
            graph.RemoveAllDirectedEdges(attacker);
        }

        victimGraph.RemoveAllDirectedEdges(attacker);
    }

    public bool IsConnected(HitboxComponent attacker, HitboxComponent victim)
    {
        int groupId = attacker.hitboxInfo.groupId;

        return victimGraph.IsConnected(attacker.owner, victim.owner) || GetGroupIdVictimGraph(groupId).IsConnected(attacker.owner, victim.owner);
    }

    public void Clear()
    {
        victimGraph.Clear();

        foreach (DirectedGraph<GameObject> graph in groupIdVictimGraph.Values)
        {
            graph.Clear();
        }
    }

    private DirectedGraph<GameObject> GetGroupIdVictimGraph(int groupId)
    {
        DirectedGraph<GameObject> graph;

        if (!groupIdVictimGraph.TryGetValue(groupId, out graph))
        {
            graph = new DirectedGraph<GameObject>();
            groupIdVictimGraph.Add(groupId, graph);
        }

        return graph;
    }
}
