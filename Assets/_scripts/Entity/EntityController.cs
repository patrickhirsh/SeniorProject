using RideShareLevel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

/// <summary>
/// Manages the state of all the entities in a level
/// </summary>
public class EntityController : LevelObject
{
    [SerializeField]
    public Entity[] Entities;

    [SerializeField]
    public Route[] Routes;

    [SerializeField]
    public Connection[] Connections;
    private Dictionary<int, Connection> _connectionsById;

    [SerializeField]
    public Building[] Buildings;

    // Indexes mapped to entities
    private Dictionary<Entity, IList<CellIndex>> _entitiesToCellIndex = new Dictionary<Entity, IList<CellIndex>>();
    private Dictionary<CellIndex, IList<Entity>> _cellIndexToEntities = new Dictionary<CellIndex, IList<Entity>>();

				private bool _initialized = false;

    //    public IEnumerable<Connection> OutboundConnections => Entities.SelectMany(entity => entity.OutboundConnections);

    //public GameObject LevelPrefab;

    #region Unity Methods

    #endregion

    public void Initialize()
    {
								_initialized = true;

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
                Entities = GetComponentsInChildren<Entity>();
#endif
            }
        }
    }

				/// <summary>
				/// Indicates whether this EntityController has been initialized
				/// </summary>
				public bool Initialized()
				{
								return _initialized;
				}

#if UNITY_EDITOR
    public void Bake()
    {
        UnityEditor.Undo.RecordObject(this, "Bake Entity Manager");
        Entities = GetComponentsInChildren<Entity>();
        Buildings = CurrentLevel.GetComponentsInChildren<Building>();
        Routes = Entities.OfType<Route>().ToArray();
        Connections = Routes.SelectMany(route => route.Connections).ToArray();
        
        foreach (var route in Routes)
        {
            route.Bake();
        }

        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
    }
#endif

    public Connection GetConnectionById(int instanceId)
    {
        if (Application.isPlaying)
        {
            if (_connectionsById != null && _connectionsById.Any())
            {
                return _connectionsById[instanceId];
            }
            _connectionsById = Connections.ToDictionary(connection => connection.GetInstanceID(), connection => connection);
            return GetConnectionById(instanceId);
        }

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