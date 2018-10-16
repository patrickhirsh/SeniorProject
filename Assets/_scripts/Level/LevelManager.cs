using UnityEngine;
using System.Collections.Generic;
using Level;
using Utility;

public class LevelManager : MonoBehaviour
{
    #region Singleton
    private static LevelManager _instance;
    public static LevelManager Instance => _instance ?? (_instance = FindObjectOfType<LevelManager>());
    #endregion

    // Indexes mapped to nodes
    public Dictionary<CellIndex, Entity> Nodes;

    #region Unity Methods

    private void Start()
    {
        Nodes = new Dictionary<CellIndex, Entity>();
    }

    #endregion

    /// <summary>
    /// Returns true if a Entity is in the given CellIndex
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool ContainsEntity(CellIndex index)
    {
        return Nodes.ContainsKey(index);
    }

    public void AddNode(Entity entity)
    {
        foreach (var index in entity.GetCellIndices())
        {
            Nodes.Add(index, entity);
        }
    }

    public void RemoveNode(Entity entity)
    {
        foreach (var index in entity.GetCellIndices())
        {
            Nodes.Remove(index);
        }
    }
}
