using UnityEngine;
using System.Collections.Generic;
using Level;

public class LevelManager : MonoBehaviour
{
    #region Singleton
    private static LevelManager _instance;
    public static LevelManager Instance => _instance ?? (_instance = FindObjectOfType<LevelManager>());
    #endregion

    // Indexes mapped to nodes
    public Dictionary<Vector3, Node> Nodes;

    #region Unity Methods

    private void Start()
    {
        Nodes = new Dictionary<Vector3, Node>();
    }

    #endregion


    public void AddNode(Node node)
    {
        foreach (var index in node.GetIndices())
        {
            Nodes.Add(index, node);
        }
    }

    public void RemoveNode(Node node)
    {
        foreach (var index in node.GetIndices())
        {
            Nodes.Remove(index);
        }
    }
}
