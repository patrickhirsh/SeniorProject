using Level;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

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

    private Route[] _routes;
    public Route[] Routes => _routes ?? (_routes = Entities.OfType<Route>().ToArray()); // Linq is so cool

    private Connection[] _connections;
    public Connection[] Connections => _connections ?? (_connections = Routes.SelectMany(route => route.Connections).ToArray()); // Linq is so cool

    // Indexes mapped to entities
    private Dictionary<Entity, IList<CellIndex>> _entitiesToCellIndex = new Dictionary<Entity, IList<CellIndex>>();
    private Dictionary<CellIndex, IList<Entity>> _cellIndexToEntities = new Dictionary<CellIndex, IList<Entity>>();

    //    public IEnumerable<Connection> OutboundConnections => Entities.SelectMany(entity => entity.OutboundConnections);

    public GameObject LevelPrefab;

    #region Unity Methods

    #endregion

    public void Initialize()
    {
        Debug.Log("Initialize EntityManager", gameObject);
        Entities = GetComponentsInChildren<Entity>();
        Broadcaster.Instance.Broadcast(GameState.SetupConnection);
        Broadcaster.Instance.Broadcast(GameState.SetupBakedPaths);
    }

    public void Bake()
    {
        Entities = GetComponentsInChildren<Entity>();
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