using System.Collections;
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
        public List<Vehicle> Vehicles;             // all valid vehicles to spawn procedurally in the current level
        private List<GameObject> _spawnPoints;     // all valid spawn points in the current level
        private SpawnState _spawnState;            // indicates how (or if) the NeutralVehicleManager should be spawning vehicles (defaults to spawningOff on startup)


        public void Start()
        {
            // spawnState is off by default (waiting for instructions...)
            _spawnState = SpawnState.SpawningOff;       

            // initialize spawnPoints list
            _spawnPoints = new List<GameObject>();
            foreach (Object spawn in Object.FindObjectsOfType<SpawnPointEntity>())
                _spawnPoints.Add((GameObject)spawn);

            // initialize vehicles list
            Vehicles = new List<Vehicle>();
            // TODO: populate this list with vehicles that should be spawned procedurally within the current level
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


        // given a spawn point and a vehicle prefab, spawns a new instance of vehiclePrefab at spawnPoint
        private void SpawnVehicle(GameObject spawnPoint, GameObject vehiclePrefab)
        {
            GameObject vehicle = Instantiate(vehiclePrefab, this.transform);
            vehicle.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
        }
    }
}

