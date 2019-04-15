using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;


namespace RideShareLevel
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

    public class NeutralVehicleController : VehicleController
    {
        public float AvgSpawnTimer = 1f;
        public float SpawnTimerVariance = 0f;

        public List<GameObject> NeutralVehiclePrefabs;     // all neutral vehicle prefabs valid for this scene
        private List<SpawnRoute> _spawnRoutes;              // all valid spawn routes in the current level
        private SpawnState _spawnState = SpawnState.SpawningOff;                     // indicates how (or if) the NeutralVehicleManager should be spawning vehicles (defaults to spawningOff on startup)
        private bool _canSpawn;
        private float _proceduralSpawnTimer = 0f;            // timer used for procedural spawning

        private Dictionary<Tuple<SpawnRoute, SpawnRoute>, Queue<Connection>> _validNeutralPaths;

        #region Unity Methods

        public void Awake()
        {
            Broadcaster.AddListener(GameEvent.GameStateChanged, GameStateChanged);
        }

        public override void IdleVehicle(Vehicle vehicle)
        {
            vehicle.Despawn();
        }

        public void Update()
        {
            if (!_canSpawn) return;
            switch (_spawnState)
            {
                case SpawnState.SpawningOff:
                    break;

                case SpawnState.SpawningPreDefined:
                    break;

                case SpawnState.SpawningHybrid:
                    break;

                case SpawnState.SpawningProcedurally:
                    HandleProceduralSpawning();
                    break;
            }
        }
        #endregion

        #region Initialization
        public void Initialize()
        {
            InitializePaths();
            _spawnState = SpawnState.SpawningProcedurally;

            // ensure neutral vehicles have been set in the inspector
            Debug.Assert(NeutralVehiclePrefabs != null, "Missing neutral vehicle prefabs", gameObject);
            Debug.Assert(NeutralVehiclePrefabs.Any(), "Missing neutral vehicle prefabs", gameObject);
        }

        public void InitializePaths()
        {
            // initialize spawnPoints list
            _spawnRoutes = new List<SpawnRoute>();
            foreach (SpawnRoute spawn in CurrentLevel.EntityController.Routes.OfType<SpawnRoute>())
                _spawnRoutes.Add(spawn);

            // deserialize pathing data 
            //DeserializeNeutralPaths();
            BakeNeutralPaths();
        }
        #endregion

        private void GameStateChanged(GameEvent @event)
        {
            // Begin the simulation
            _canSpawn = GameManager.CurrentGameState == GameState.LevelSimulating;
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
        /// given a spawn point and a vehicle prefab, spawns a new instance of vehiclePrefab at "spawnPoint" with "destination"
        /// </summary>
        private void SpawnVehicle(Tuple<SpawnRoute, SpawnRoute> pathKey, GameObject vehiclePrefab)
        {
            // instantiate the new vehicle
            var vehicle = Instantiate(vehiclePrefab, pathKey.Item1.transform.position, Quaternion.identity, transform).GetComponent<Vehicle>();
            vehicle.Controller = this;

            // construct a copy of the cached connection Queue
            Queue<Connection> path = new Queue<Connection>();
            foreach (Connection connection in _validNeutralPaths[pathKey])
                path.Enqueue(connection);

            // assign the pathing task to this new vehicle
            vehicle.AddTask(new NeutralPathingTask(vehicle, false, path));
        }

        /// <summary>
        /// spawns a new neutral vehicle every (AVG_SPAWN_TIMER + (deviation determined by SPAWN_TIMER_VARIANCE)) seconds.
        /// This method keeps track of the current spawn timer and should be called every frame update.
        /// </summary>
        private void HandleProceduralSpawning()
        {
            // decriment timer
            _proceduralSpawnTimer -= Time.deltaTime;

            if (_proceduralSpawnTimer < 0)
            {
                // reset spawn timer
                _proceduralSpawnTimer = AvgSpawnTimer + Random.Range(SpawnTimerVariance * -1, SpawnTimerVariance);

                var keys = _validNeutralPaths.Keys.ToArray();
                var key = keys[Random.Range(0, keys.Length)];
                    
                // spawn the vehicle
                SpawnVehicle(key, NeutralVehiclePrefabs[Random.Range(0, NeutralVehiclePrefabs.Count - 1)]);
                if (Debugger.Profile.DebugNeutralVehicleManager) { Debug.Log("Spawning vehicle"); }
            }
        }

        public void BakeNeutralPaths()
        {
            _validNeutralPaths = new Dictionary<Tuple<SpawnRoute, SpawnRoute>, Queue<Connection>>();
            _spawnRoutes = CurrentLevel.EntityController.Routes.OfType<SpawnRoute>().ToList();

            foreach (var spawnRoute in _spawnRoutes)
            {
                foreach (var route in _spawnRoutes)
                {
                    if (route == spawnRoute) continue;

                    var key = new Tuple<SpawnRoute, SpawnRoute>(spawnRoute, route);

                    if (PathfindingManager.Instance.GetPath(spawnRoute, route, out var connections))
                    {
                        _validNeutralPaths.Add(key, connections);
                    }
                }
            }
        }
    }
}

