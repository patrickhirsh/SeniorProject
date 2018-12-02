﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Level
{
    /// <summary>
    /// Represents a (single) state the NeutralVehicleManager can be in at any time for spawning vehicles
    /// Use setSpawnState() to change _spawnState
    /// 
    /// STATE DEFINITIONS:
    /// spawningOff =            vehicles will not spawn
    /// spawningPreDefined =     vehicles will be spawned based on SpawnDirectives within SpawnPointEntities. Spawning is set to Off when all vehicles are spawned
    /// spawningHybrid =         vehicles will be spawned based on SpawnDirectives within SpawnPointEntities. Spawning is set to spawningProcedurally when all vehicles are spawned
    /// spawningProcedurally =   vehicles will be spawned procedurally using vehicles marked valid for the level (_vehicles)
    /// </summary>
    public enum SpawnState : byte { SpawningOff, SpawningPreDefined, SpawningHybrid, SpawningProcedurally }

    public class NeutralVehicleManager : VehicleManager
    {
        #region Singleton
        private static NeutralVehicleManager _instance;
        public static NeutralVehicleManager Instance => _instance ?? (_instance = Create());

        private static NeutralVehicleManager Create()
        {
            GameObject singleton = FindObjectOfType<NeutralVehicleManager>()?.gameObject;
            if (singleton == null) singleton = new GameObject { name = typeof(NeutralVehicleManager).Name };
            singleton.AddComponent<NeutralVehicleManager>();
            return singleton.GetComponent<NeutralVehicleManager>();
        }
        #endregion

        public bool DebugMode = true;
        public List<Vehicle> Vehicles;                  // all valid vehicles to spawn procedurally in the current level
        public List<SpawnRoute> _spawnPoints;     // all valid spawn points in the current level // TODO: this really shouldn't be public...
        private SpawnState _spawnState;                 // indicates how (or if) the NeutralVehicleManager should be spawning vehicles (defaults to spawningOff on startup)

        /// <summary>
        /// _reachableSpawnPointConnections keeps a 2D dictionary that maps any SpawnPointEntity connection to a dictionary
        /// that contains all other reachable SpawnPointEntity connections as Keys, and their values being the path itself. This means
        /// any pathing that needs to be done between two SpawnPointEntities is baked directly into this datastructure at runtime!
        /// </summary>
        private Dictionary<Connection, Dictionary<Connection, List<BezierCurve>>> _reachableSpawnPointConnections;


        public void Start()
        {
            Broadcaster.Instance.AddListener(GameState.SetupConnection, Initialize);
            Broadcaster.Instance.AddListener(GameState.SetupBakedPaths, Initialize);            
        }


        /// <summary>
        /// Handles all GameEvent Broadcasts
        /// </summary>
        /// <param name="gameState"></param>
        public void Initialize(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.SetupConnection:

                    // spawnState is off by default (waiting for instructions...)
                    _spawnState = SpawnState.SpawningOff;

                    // initialize spawnPoints list
                    _spawnPoints = new List<SpawnRoute>();
                    foreach (SpawnRoute spawn in Object.FindObjectsOfType<SpawnRoute>())
                        _spawnPoints.Add(spawn);

                    // initialize vehicles list
                    Vehicles = new List<Vehicle>();
                    // TODO: populate this list with vehicles that should be spawned procedurally within the current level

                    break;


                case GameState.SetupBakedPaths:

                    populateSpawnPointReachabilityPaths();
                    break;
            }
        }


        public void Update()
        {
            switch (_spawnState)
            {
                case SpawnState.SpawningOff:
                    break;

                case SpawnState.SpawningPreDefined:
                    break;

                case SpawnState.SpawningHybrid:
                    break;

                case SpawnState.SpawningProcedurally:
                    break;
            }
        }

        /// <summary>
        /// Sets the spawnState to the given state. Only accepts states with exactly one flag set.
        /// If an invalid state is given, returns false. Otherwise, returns true.
        /// </summary>
        public bool SetSpawnState(SpawnState state)
        {
            // ensure exactly one flag is set
            if ((state & (state - 1)) != 0) { return false; }
            _spawnState = state;
            return true;
        }


        public void getReachableSpawnPoints(Connection spawnPointEntityConnection)
        {
            List<List<BezierCurve>> connections = new List<List<BezierCurve>>();

            // TODO: Refactor the output of PathfindingManager.GetPath() to be Queue<Connection> to match with
            // Vehicle's pathing parameters. That means these structures within this class need to be refactored too
            // finally, these "reachable spawnpoints" should probably return as List<Queue<Connection>>.
            // Then, the spawning system can randomly pick an index between 0 and output.length - 1 to get a valid path.
        }


        /// <summary>
        /// NeutralVehicleManager only ever assigns a single task to a neutral vehicle (on spawn) then removes the vehicle
        /// when it reaches it's destination. If this task is interrupted, unexpected behavior is occuring.
        /// </summary>
        public override void VehicleTaskCallback(TaskType type, Vehicle vehicle, bool exitStatus)
        {
            // the neutral vehicle reached it's destination spawn point.
            if (exitStatus)
                Destroy(vehicle);

            // log unexpected behavior
            else
            {
                if (DebugMode && (type != TaskType.NeutralAi)) { Debug.LogWarning("Unexpected interrupt of a neutral vehicle task with non-neutral task"); }
                if (DebugMode && (type == TaskType.NeutralAi)) { Debug.LogWarning("Unexpected interrupt of a neutral vehicle task with another neutral vehicle task"); }
            } 
        }


        /// <summary>
        /// given a spawn point and a vehicle prefab, spawns a new instance of vehiclePrefab at "spawnPoint" with "destination"
        /// </summary>
        private void SpawnVehicle(GameObject spawnPoint, GameObject destination, GameObject vehiclePrefab)
        {
            GameObject vehicle = Instantiate(vehiclePrefab, this.transform);
            vehicle.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
        }


        /// <summary>
        /// for each connection within a SpawnPointEntity, look for a path to all other SpawnPointEntity connections.
        /// We represent this data in "_reachableSpawnPointEntityConnections", which is documented above. Any connection
        /// who can neither reach any other connection NOR be reached by another connection is considered "unreachable"
        /// and will therefore be removed as a key within the base dictionary.
        /// </summary>
        private void populateSpawnPointReachabilityPaths()
        {
            // base dictionary should hold a connection key for every single connection in every single SpawnPointConnection
            foreach (SpawnRoute spawn1 in _spawnPoints)
                foreach (Connection connection1 in spawn1.Connections)
                    _reachableSpawnPointConnections.Add(connection1, new Dictionary<Connection, List<BezierCurve>>());

            // for each of these connections, check if a path exists between this connection and EVERY OTHER connection
            foreach (Connection connection1 in _reachableSpawnPointConnections.Keys)
                foreach (SpawnRoute spawn2 in _spawnPoints)
                    foreach (Connection connection2 in spawn2.Connections)
                    {
                        // look for path. If the path exists, add connection2 as a reachable connection (and add its path)
                        List<BezierCurve> path = new List<BezierCurve>();
                        if (PathfindingManager.Instance.GetPath(connection1, connection2, out path))
                            _reachableSpawnPointConnections[connection1].Add(connection2, path);
                    }

            // finally, look for connections that can't reach any other connections AND aren't reachable by any other connections
            // these connections are completely isolated from the grid and should be removed
            // we chack against <= 1 because every connection has a path to itself
            foreach (Connection connection1 in _reachableSpawnPointConnections.Keys)
                if (_reachableSpawnPointConnections[connection1].Count <= 1)
                {
                    // begin searching for other SpawnNodeEntity connections that reach this connection
                    bool connectionLocated = false;
                    foreach (Connection connection2 in _reachableSpawnPointConnections.Keys)
                    {
                        // connection2 is one of the other connections we're checking reachable spawn connections from. 
                        // connection3 is a spawn connection that connection 2 connects to
                        if (connection2 != connection1)
                            foreach (Connection connection3 in _reachableSpawnPointConnections[connection2].Keys)
                                if (connection3 == connection1)
                                {
                                    // if we get here (connection3 == connection1), connection2 has a connection to connection1!
                                    connectionLocated = true;
                                    break;
                                }

                        // connection found. no need to keep looking
                        if (connectionLocated)
                            break;
                    }

                    // if we never found another connection that can reach connection1, it's unreachable. remove it.
                    if (!connectionLocated)
                        _reachableSpawnPointConnections.Remove(connection1);
                }
        }
    }
}

