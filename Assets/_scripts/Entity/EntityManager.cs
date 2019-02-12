using Level;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

/// <summary>
/// Manages the state of all the entities in a level
/// </summary>
public class EntityManager : Singleton<EntityManager>
{
    [HideInInspector]
    private Entity[] _entities;

    public Entity[] Entities => Application.isPlaying ? _entities : GetComponentsInChildren<Entity>();

    private Route[] _routes;
    public Route[] Routes
    {
        get
        {
            if (Application.isPlaying)
            {
                return _routes ?? (_routes = Entities.OfType<Route>().ToArray());
            }
            return Entities.OfType<Route>().ToArray();
        }
    }

    private Connection[] _connections;
    public Connection[] Connections
    {
        get
        {
            if (Application.isPlaying)
            {
                return _connections ?? (_connections = Routes.SelectMany(route => route.Connections).ToArray());
            }
            return Routes.SelectMany(route => route.Connections).ToArray();
        }
    }

    // Indexes mapped to entities
    private Dictionary<Entity, IList<CellIndex>> _entitiesToCellIndex = new Dictionary<Entity, IList<CellIndex>>();
    private Dictionary<CellIndex, IList<Entity>> _cellIndexToEntities = new Dictionary<CellIndex, IList<Entity>>();

    //    public IEnumerable<Connection> OutboundConnections => Entities.SelectMany(entity => entity.OutboundConnections);

    public GameObject LevelPrefab;

    #region Unity Methods

    #endregion

    public void Initialize()
    {
        // Verify that we have baked all the entities in the scene
        if (Entities.Length != GetComponentsInChildren<Entity>().Length)
        {
            Debug.LogError($"Entity Manager for level needs to be baked. Entities: {Entities.Length} | Children: {GetComponentsInChildren<Entity>().Length}");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            if (Entities == null || !Entities.Any())
            {
#if UNITY_EDITOR
                Debug.LogError("EntityManager does not have any entities. Did you bake?");
#else
                // Recover if not Unity Editor
                _entities = GetComponentsInChildren<Entity>();
#endif
            }
        }

        Broadcaster.Broadcast(GameEvent.SetupConnection);
        Broadcaster.Broadcast(GameEvent.SetupBakedPaths);
    }

    public void Bake()
    {
        _entities = GetComponentsInChildren<Entity>();
    }

    public Connection GetConnectionById(int instanceId)
    {
        return Connections.FirstOrDefault(connection => connection.GetInstanceID() == instanceId);
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
    }

    public void SpawnLevel(Vector3 pos)
    {
        throw new NotImplementedException();
        //        transform.position = pos;
        //        var spawnedLevel = Instantiate(LevelPrefab, pos, Quaternion.identity, transform);
        //        spawnedLevel.transform.localScale = scale;
        //        Initialize();
    }
}