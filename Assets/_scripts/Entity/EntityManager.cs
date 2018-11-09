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
    public static EntityManager Instance
    {
        get
        {
            if (Application.isPlaying) return _instance ?? (_instance = Create());
            return Create();
        }
    }

    private static EntityManager Create()
    {
        GameObject singleton = FindObjectOfType<EntityManager>()?.gameObject;
        if (singleton == null)
        {
            singleton = new GameObject { name = $"[{typeof(EntityManager).Name}]" };
            singleton.AddComponent<EntityManager>();
        }
        return singleton.GetComponent<EntityManager>();
    }
    #endregion

    [HideInInspector]
    public Entity[] Entities;
    [HideInInspector]
    public Connection[] Connections;

    // Indexes mapped to entities
    private Dictionary<Entity, IList<CellIndex>> _entitiesToCellIndex = new Dictionary<Entity, IList<CellIndex>>();
    private Dictionary<CellIndex, IList<Entity>> _cellIndexToEntities = new Dictionary<CellIndex, IList<Entity>>();

    //    public IEnumerable<Connection> OutboundConnections => Entities.SelectMany(entity => entity.OutboundConnections);


    #region Unity Methods

    #endregion

    public void Bake()
    {
        Entities = GetComponentsInChildren<Entity>();
        Connections = Entities.Where(entity => entity != null).SelectMany(entity => entity.Connections).ToArray();
        foreach (var connection in Connections)
        {
            connection.Bake();
        }
    }

    public void Setup()
    {
        CalculateEntities();
    }

    private void CalculateEntities()
    {
        // Setup all child Entities
        Debug.Assert(Entities != null, "Entities in EntityManager is null");
        foreach (var entity in Entities)
        {
            entity.Setup();
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

}