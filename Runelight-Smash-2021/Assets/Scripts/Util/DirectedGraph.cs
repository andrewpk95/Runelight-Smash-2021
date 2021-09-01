using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectedGraph<T>
{
    private Dictionary<T, HashSet<T>> graph = new Dictionary<T, HashSet<T>>();

    public void AddDirectedEdge(T node1, T node2)
    {
        GetNodeList(node1).Add(node2);
    }

    public void AddEdge(T node1, T node2)
    {
        AddDirectedEdge(node1, node2);
        AddDirectedEdge(node2, node1);
    }

    public void RemoveDirectedEdge(T node1, T node2)
    {
        GetNodeList(node1).Remove(node2);
    }

    public void RemoveEdge(T node1, T node2)
    {
        RemoveDirectedEdge(node1, node2);
        RemoveDirectedEdge(node2, node1);
    }

    public void RemoveAllDirectedEdges(T node)
    {
        GetNodeList(node).Clear();
    }

    public void RemoveAllEdges(T node)
    {
        RemoveAllDirectedEdges(node);

        foreach (HashSet<T> nodeList in graph.Values)
        {
            nodeList.Remove(node);
        }
    }

    public bool IsConnected(T node1, T node2)
    {
        return GetNodeList(node1).Contains(node2) || GetNodeList(node2).Contains(node1);
    }

    public int GetEdgeCount(T node)
    {
        return GetNodeList(node).Count;
    }

    public void Clear()
    {
        foreach (T node in graph.Keys)
        {
            GetNodeList(node).Clear();
        }
    }

    private HashSet<T> GetNodeList(T node)
    {
        HashSet<T> nodeList;

        if (!graph.TryGetValue(node, out nodeList))
        {
            nodeList = new HashSet<T>();
            graph[node] = nodeList;
        }

        return nodeList;
    }
}