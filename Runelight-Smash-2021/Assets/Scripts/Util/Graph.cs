using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph<T>
{
    private Dictionary<T, HashSet<T>> graph = new Dictionary<T, HashSet<T>>();

    public void AddEdge(T node1, T node2)
    {
        AddToNodeList(node1, node2);
        AddToNodeList(node2, node1);
    }

    public void RemoveEdge(T node1, T node2)
    {
        RemoveFromNodeList(node1, node2);
        RemoveFromNodeList(node2, node1);
    }

    public bool IsConnected(T node1, T node2)
    {
        HashSet<T> nodeList;

        if (!graph.TryGetValue(node1, out nodeList))
        {
            return false;
        }

        return nodeList.Contains(node2);
    }

    public void Clear()
    {
        foreach (HashSet<T> nodeList in graph.Values)
        {
            nodeList.Clear();
        }
    }

    private void AddToNodeList(T node1, T node2)
    {
        HashSet<T> nodeList;

        if (!graph.TryGetValue(node1, out nodeList))
        {
            nodeList = new HashSet<T>();
            graph[node1] = nodeList;
        }
        nodeList.Add(node2);
    }

    private void RemoveFromNodeList(T node1, T node2)
    {
        HashSet<T> nodeList;

        if (!graph.TryGetValue(node1, out nodeList))
        {
            return;
        }
        nodeList.Remove(node2);
    }
}