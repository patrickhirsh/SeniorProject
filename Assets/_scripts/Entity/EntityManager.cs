using Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using VehicleEntity;

/// <summary>
/// Manages the state of all the entities in a level
/// </summary>
public class EntityManager : MonoBehaviour
{
    #region Singleton
    private static EntityManager _instance;
    public static EntityManager Instance => _instance ?? (_instance = FindObjectOfType<EntityManager>());
    #endregion

    // Indexes mapped to entities
    private Dictionary<Entity, IList<CellIndex>> _entitiesToCellIndex = new Dictionary<Entity, IList<CellIndex>>();
    private Dictionary<CellIndex, IList<Entity>> _cellIndexToEntities = new Dictionary<CellIndex, IList<Entity>>();

    public IEnumerable<Entity> Entities => _entitiesToCellIndex.Keys.ToList();
    public IEnumerable<Connection> OutboundConnections => Entities.SelectMany(entity => entity.OutboundConnections);
    public IEnumerable<Connection> InboundConnections => Entities.SelectMany(entity => entity.InboundConnections);

    #region Unity Methods

    #endregion

    public void Setup()
    {
        CalculateEntities();
    }

    private void CalculateEntities()
    {
        // Setup all child Entities
        foreach (Transform child in transform)
        {
            child.GetComponent<Entity>()?.Setup();
        }
    }

    /// <summary>
    /// Returns entities in a given cell index
    /// </summary>
    public IEnumerable<Entity> EntitiesAtCellIndex(CellIndex index)
    {
        return _cellIndexToEntities[index];
    }

    public void AddEntity(Entity entity)
    {
        // Create a new list in the _entitiesToCellIndex if it doesn't exist
        if (!_entitiesToCellIndex.ContainsKey(entity)) _entitiesToCellIndex.Add(entity, new List<CellIndex>());

        // Adds the entity to the cells its currently in
        foreach (var cellIndex in entity.GetCellIndices())
        {
            _entitiesToCellIndex[entity].Add(cellIndex);

            if (!_cellIndexToEntities.ContainsKey(cellIndex))
            {
                _cellIndexToEntities.Add(cellIndex, new List<Entity> { entity });
            }
            else
            {
                _cellIndexToEntities[cellIndex].Add(entity);
            }
        }
    }

    public void RemoveEntity(Entity entity)
    {
        if (_entitiesToCellIndex == null) return;
        if (!_entitiesToCellIndex.ContainsKey(entity)) throw new UnityException($"Entity not found when removing.");

        // Remove the entity from the cells it's registered in
        foreach (var cellIndex in _entitiesToCellIndex[entity])
        {
            _cellIndexToEntities[cellIndex].Remove(entity);
        }

        // Remove the entity
        _entitiesToCellIndex.Remove(entity);
    }

    public void UpdateEntity(Entity entity)
    {
        // This could be optimized more, but will probably do fine
        RemoveEntity(entity);
        AddEntity(entity);
        CalculateEntities();
    }

    // TODO: This could be done in a much better way me thinks
    /// <summary>
    /// Computes a path for a vehicle to an entity. Returns a boolean whether the path was found.
    /// </summary>
    public bool FindPath(Vehicle vehicle, Entity toEntity, out List<BezierCurve> path)
    {
        path = new List<BezierCurve>();
        if (toEntity == null) return false;

        var startCell = vehicle.GetCellIndices().First();
        var start = _cellIndexToEntities[startCell].FirstOrDefault(entity => entity.GetType() != typeof(Vehicle));
        var current = start;

        // Check if we can reach the destination
        if (current == null || !current.ConnectingEntities.Contains(toEntity)) return false;

        while (current != toEntity)
        {
            // Find the next entity
            var next = current.NeighborEntities.First(entity => entity == toEntity || entity.ConnectingEntities.Contains(toEntity));

            BezierCurve curve;
            // From current (and the inbound connection), find a path to next
            if (current.FindPathToEntity(current.InboundConnections.First(), next, out curve))
            {
                path.Add(curve);
            }
            else
            {
                return false;
            }

            current = next;
        }
        // path.Add(toEntity.transform.position);
        Debug.Log(path.Count);
        return true;
    }
}

public class EntityContainer<T>
{
}
