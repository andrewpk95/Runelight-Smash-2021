using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxVictimGraph
{
    private Dictionary<int, Graph<GameObject>> victimGraph = new Dictionary<int, Graph<GameObject>>();

    public void AddEdge(GameObject attacker, GameObject victim, int groupId)
    {
        GetGraph(groupId).AddEdge(attacker, victim);
    }

    public void RemoveEdge(GameObject attacker, GameObject victim, int groupId)
    {
        GetGraph(groupId).RemoveEdge(attacker, victim);
    }

    public void RemoveNode(GameObject attacker, int groupId)
    {
        GetGraph(groupId).RemoveNode(attacker);
    }

    public bool IsConnected(GameObject attacker, GameObject victim, int groupId)
    {
        return GetGraph(groupId).IsConnected(attacker, victim);
    }

    public void Clear()
    {
        foreach (Graph<GameObject> graph in victimGraph.Values)
        {
            graph.Clear();
        }
    }

    private Graph<GameObject> GetGraph(int groupId)
    {
        Graph<GameObject> graph;

        if (!victimGraph.TryGetValue(groupId, out graph))
        {
            graph = new Graph<GameObject>();
            victimGraph.Add(groupId, graph);
        }

        return graph;
    }
}
