using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;


namespace Level
{
    /// <summary>
    /// Represents a (single) @event the NeutralVehicleManager can be in at any time for spawning vehicles
    /// Use setSpawnState() to change _spawnState
    /// 
    /// STATE DEFINITIONS:
    /// spawningOff =            vehicles will not spawn
    /// spawningPreDefined =     vehicles will be spawned based on SpawnDirectives within SpawnPointEntities. Spawning is set to Off when all vehicles are spawned
    /// spawningHybrid =         vehicles will be spawned based on SpawnDirectives within SpawnPointEntities. Spawning is set to spawningProcedurally when all vehicles are spawned
    /// spawningProcedurally =   vehicles will be spawned procedurally using vehicles marked valid for the level (_vehicles)
    /// </summary>
    public enum SpawnState : byte { SpawningOff, SpawningPreDefined, SpawningHybrid, SpawningProcedurally }

    [System.Serializable]
    public class SerializablePath : System.Object
    {
        [SerializeField]
        public List<int> connectionIDs;
    }

    [System.Serializable]
    public class SerializablePathData : System.Object
    {
        [SerializeField]
        public List<SerializablePath> allPaths;
    }

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

        public float AVG_SPAWN_TIMER = 1f;
        public float SPAWN_TIMER_VARIANCE = 0f;

        public List<GameObject> _neutralVehiclePrefabs;     // all neutral vehicle prefabs valid for this scene
        private List<SpawnRoute> _spawnRoutes;              // all valid spawn routes in the current level
        private SpawnState _spawnState;                     // indicates how (or if) the NeutralVehicleManager should be spawning vehicles (defaults to spawningOff on startup)
        private float proceduralSpawnTimer = 0f;            // timer used for procedural spawning

        /// <summary>
        /// _validNeutralPaths keeps a 2D dictionary that maps any SpawnPointEntity connection to a dictionary
        /// that contains all other reachable SpawnPointEntity connections as Keys, and their values being the path itself. This means
        /// any pathing that needs to be done between two SpawnPointEntities is baked directly into this datastructure at runtime!
        /// </summary>
        private Dictionary<Connection, Dictionary<Connection, Queue<Connection>>> _validNeutralPaths;

        [SerializeField]
        private SerializablePathData _serializedPaths;

        /// <summary>
        /// NeutralVehicleManager only ever assigns a single task to a neutral vehicle (on spawn) then removes the vehicle
        /// when it reaches it's destination. If this task is interrupted, unexpected behavior is occuring.
        /// </summary>
        public override void VehicleTaskCallback(TaskType type, Vehicle vehicle, bool exitStatus)
        {
            // the neutral vehicle reached it's destination spawn point. Destroy it
            if (exitStatus)
                Destroy(vehicle.gameObject);

            // log unexpected behavior
            else
            {
                if (Debugger.Profile.DebugNeutralVehicleManager && (type != TaskType.NeutralAi)) { Debug.LogWarning("Unexpected interrupt of a neutral vehicle task with non-neutral task"); }
                if (Debugger.Profile.DebugNeutralVehicleManager && (type == TaskType.NeutralAi)) { Debug.LogWarning("Unexpected interrupt of a neutral vehicle task with another neutral vehicle task"); }
            }
        }

        public void Awake()
        {
            Broadcaster.AddListener(GameEvent.GameStateChanged, GameStateChanged);
            Broadcaster.AddListener(GameEvent.SetupBakedPaths, BakePaths);
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
                    handleProceduralSpawning();
                    break;
            }
        }

        private void GameStateChanged(GameEvent @event)
        {
            // Begin the simulation
            if (GameManager.CurrentGameState == GameState.LevelSimulating)
            {
                // spawnState is off by default (waiting for instructions...)
                //_spawnState = SpawnState.SpawningOff;
                _spawnState = SpawnState.SpawningProcedurally;

                // ensure neutral vehicles have been set in the inspector
                Debug.Assert(_neutralVehiclePrefabs != null);
            }
            else
            {
                _spawnState = SpawnState.SpawningOff;
            }
        }

        /// <summary>
        /// returns a random spawn route from the list of all spawn routes
        /// TODO: Spawn Route management should NOT be here
        /// </summary>
        public SpawnRoute GetRandomSpawnRoute()
        {
            int index = Random.Range(0, _spawnRoutes.Count);
            return _spawnRoutes[index];
        }


        /// <summary>
        /// Handles all GameEvent Broadcasts
        /// </summary>
        /// <param name="gameEvent"></param>
        public void BakePaths(GameEvent gameEvent)
        {
            switch (gameEvent)
            {
                case GameEvent.SetupBakedPaths:

                    // initialize spawnPoints list
                    _spawnRoutes = new List<SpawnRoute>();
                    foreach (SpawnRoute spawn in Object.FindObjectsOfType<SpawnRoute>())
                        _spawnRoutes.Add(spawn);

                    // deserialize pathing data 
                    //DeserializeNeutralPaths();
                    bakeNeutralPaths();
                    break;
            }
        }

        /// <summary>
        /// Sets the spawnState to the given @event. Only accepts states with exactly one flag set.
        /// If an invalid @event is given, returns false. Otherwise, returns true.
        /// </summary>
        public bool SetSpawnState(SpawnState state)
        {
            // ensure exactly one flag is set
            if ((state & (state - 1)) != 0) { return false; }
            _spawnState = state;
            return true;
        }


        /// <summary>
        /// given a spawn point and a vehicle prefab, spawns a new instance of vehiclePrefab at "spawnPoint" with "destination"
        /// </summary>
        private void SpawnVehicle(Connection spawnPoint, Connection destination, GameObject vehiclePrefab)
        {
            // instantiate the new vehicle
            var vehicle = Instantiate(vehiclePrefab, spawnPoint.transform.position, Quaternion.identity, transform).GetComponent<Vehicle>();
            vehicle.Manager = this;

            // construct a copy of the cached connection Queue
            Queue<Connection> path = new Queue<Connection>();
            foreach (Connection connection in _validNeutralPaths[spawnPoint][destination])
                path.Enqueue(connection);

            // assign the pathing task to this new vehicle
            vehicle.AssignTask(new VehicleTask(TaskType.NeutralAi, path, VehicleTaskCallback));
        }


        /// <summary>
        /// spawns a new neutral vehicle every (AVG_SPAWN_TIMER + (deviation determined by SPAWN_TIMER_VARIANCE)) seconds.
        /// This method keeps track of the current spawn timer and should be called every frame update.
        /// </summary>
        private void handleProceduralSpawning()
        {
            // decriment timer
            proceduralSpawnTimer -= Time.deltaTime;

            if (proceduralSpawnTimer < 0)
            {
                // reset spawn timer
                proceduralSpawnTimer = AVG_SPAWN_TIMER + Random.Range(SPAWN_TIMER_VARIANCE * -1, SPAWN_TIMER_VARIANCE);

                // get a random prefab, and random start/end connections
                int randomPrefabIndex = Random.Range(0, _neutralVehiclePrefabs.Count);
                int startIndex = Random.Range(0, _validNeutralPaths.Count);
                Connection startConnection = _validNeutralPaths.Keys.ToArray()[startIndex];
                int destinationIndex = Random.Range(0, _validNeutralPaths[startConnection].Count);
                Connection endConnection = _validNeutralPaths[startConnection].Keys.ToArray()[destinationIndex];

                // spawn the vehicle
                SpawnVehicle(startConnection, endConnection, _neutralVehiclePrefabs[randomPrefabIndex]);
                if (Debugger.Profile.DebugNeutralVehicleManager) { Debug.Log("Spawning vehicle"); }
            }
        }


        /// <summary>
        /// for each connection within a SpawnPointEntity, look for a path to all other SpawnPointEntity connections.
        /// We represent this data in "_validNeutralPaths", which is documented above. Any connection
        /// who can't reach any other connection is considered "unPathable" and will therefore be removed as a key within the base dictionary.
        /// This method caches all pathing information for every single spawn point.
        /// </summary>
        public void bakeNeutralPaths()
        {
            _validNeutralPaths = new Dictionary<Connection, Dictionary<Connection, Queue<Connection>>>();

            // initialize spawnPoints list
            _spawnRoutes = new List<SpawnRoute>();
            foreach (SpawnRoute spawn in Object.FindObjectsOfType<SpawnRoute>())
                _spawnRoutes.Add(spawn);

            // observe all spawn points as potential starting points
            foreach (SpawnRoute spawn1 in _spawnRoutes)
            {
                // observe all connections within these spawn points as potential starting points
                foreach (Connection connection1 in spawn1.Connections)
                {
                    // only observe "outbound" connections in the source
                    if (connection1.Paths.Count == 0)
                        _validNeutralPaths.Add(connection1, new Dictionary<Connection, Queue<Connection>>());
                }
            }

            // for each of these connections, check if a path exists between this connection and EVERY OTHER connection
            List<Connection> noPaths = new List<Connection>();
            foreach (Connection connection1 in _validNeutralPaths.Keys)
            {
                // observe all other spawn points
                foreach (SpawnRoute spawn2 in _spawnRoutes)
                {
                    // only observe connections not inside our current spawn point
                    if (connection1.ParentRoute != spawn2)
                    {
                        // observe all connections within each of these other spawn points
                        foreach (Connection connection2 in spawn2.Connections)
                        {
                            // look for path. If the path exists, add connection2 as a reachable connection (and add its path)
                            Queue<Connection> path = new Queue<Connection>();
                            if (PathfindingManager.Instance.GetPath(connection1, connection2.GetConnectsTo, out path))
                            { _validNeutralPaths[connection1].Add(connection2.GetConnectsTo, path); }
                        }
                    }
                }

                // if the only reachable connection from this spawnpoint is itself, mark for removal
                if (_validNeutralPaths[connection1].Count <= 1)
                    noPaths.Add(connection1);
            }

            // remove all unreachable connections
            foreach (Connection connection in noPaths)
                _validNeutralPaths.Remove(connection);

            // convert NeutralPaths to a serializable format
            //SerializeNeutralPaths();
            //UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }


        /// <summary>
        /// Converts data from _validNeutralPaths into a serializable format.
        /// Should be called from the Inspector
        /// </summary>
        private void SerializeNeutralPaths()
        {
            var pathSets = _validNeutralPaths.Values;
            _serializedPaths = new SerializablePathData();
            _serializedPaths.allPaths = new List<SerializablePath>();
            foreach (Dictionary <Connection, Queue<Connection>> pathSet in pathSets)
            {
                foreach (Queue<Connection> path in pathSet.Values)
                {
                    SerializablePath serializablePath = new SerializablePath();
                    serializablePath.connectionIDs = new List<int>();
                    while(path.Count > 0) { serializablePath.connectionIDs.Add(path.Dequeue().gameObject.GetInstanceID()); }
                    _serializedPaths.allPaths.Add(serializablePath);
                }
            }
        }


        /// <summary>
        /// Deserializes _serializedPaths and uses the data to populate _validNeutralPaths
        /// Should be called at runtime
        /// </summary>
        private void DeserializeNeutralPaths()
        {
            _validNeutralPaths = new Dictionary<Connection, Dictionary<Connection, Queue<Connection>>>();
            foreach (SerializablePath serializedPath in _serializedPaths.allPaths)
            {
                Connection start = EntityManager.Instance.GetConnectionById(serializedPath.connectionIDs[0]);
                Connection end = EntityManager.Instance.GetConnectionById(serializedPath.connectionIDs[serializedPath.connectionIDs.Count - 1]);

                // if we haven't yet added any paths from this connection (key), initialize its value
                if (!_validNeutralPaths.ContainsKey(start))
                {
                    _validNeutralPaths[start] = new Dictionary<Connection, Queue<Connection>>();
                }                   

                // populate the path queue at the proper hash in _validNeutralPaths
                Queue<Connection> path = new Queue<Connection>();
                _validNeutralPaths[start][end] = new Queue<Connection>();
                foreach (int objectID in serializedPath.connectionIDs)
                {
                    _validNeutralPaths[start][end].Enqueue(EntityManager.Instance.GetConnectionById(objectID));
                }
            }
        }
    }
}

