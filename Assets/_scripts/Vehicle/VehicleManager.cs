using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Level
{
    /// <summary>
    /// Represents a (single) state the VehicleManager can be in at any time for spawning vehicles
    /// Use setSpawnState() to change _spawnState
    /// 
    /// STATE DEFINITIONS:
    /// spawningOff =            vehicles will not spawn
    /// spawningPreDefined =     vehicles will be spawned based on SpawnDirectives within SpawnPointEntities. Spawning is set to Off when all vehicles are spawned
    /// spawningHybrid =         vehicles will be spawned based on SpawnDirectives within SpawnPointEntities. Spawning is set to spawningProcedurally when all vehicles are spawned
    /// spawningProcedurally =   vehicles will be spawned procedurally using vehicles marked valid for the level (_vehicles)
    /// </summary>
    public enum SpawnState : byte { spawningOff, spawningPreDefined, spawningHybrid, spawningProcedurally }

    public class VehicleManager : MonoBehaviour
    {
        #region Singleton
        private static VehicleManager _instance;
        public static VehicleManager Instance => _instance ?? (_instance = Create());

        private static VehicleManager Create()
        {
            GameObject singleton = FindObjectOfType<VehicleManager>()?.gameObject;
            if (singleton == null) singleton = new GameObject { name = typeof(VehicleManager).Name };
            singleton.AddComponent<VehicleManager>();
            return singleton.GetComponent<VehicleManager>();
        }
        #endregion

        private List<GameObject> _spawnPoints;      // all valid spawn points in the current level
        private List<GameObject> _vehicles;         // all valid vehicles to spawn procedurally in the current level
        private SpawnState _spawnState;             // indicates how (or if) the VehicleManager should be spawning vehicles (defaults to spawningOff on startup)


        public void Start()
        {
            // spawnState is off by default (waiting for instructions...)
            _spawnState = SpawnState.spawningOff;

            

            // initialize spawnPoints list
            _spawnPoints = new List<GameObject>();
            foreach (Object spawn in Object.FindObjectsOfType<SpawnPointEntity>())
                _spawnPoints.Add((GameObject)spawn);

            // initialize vehicles list
            _vehicles = new List<GameObject>();
            // TODO: populate this list with vehicles that should be spawned procedurally within the current level
        }


        public void Update()
        {
            switch (_spawnState)
            {
                case SpawnState.spawningOff:
                    break;

                case SpawnState.spawningPreDefined:
                    break;

                case SpawnState.spawningHybrid:
                    break;

                case SpawnState.spawningProcedurally:
                    break;
            }
        }

        /// <summary>
        /// Sets the spawnState to the given state. Only accepts states with exactly one flag set.
        /// If an invalid state is given, returns false. Otherwise, returns true.
        /// </summary>
        public bool setSpawnState(SpawnState state)
        {
            // ensure exactly one flag is set
            if ((state & (state - 1)) != 0) { return false; }
            _spawnState = state;
            return true;
        }


        // given a spawn point and a vehicle prefab, spawns a new instance of vehiclePrefab at spawnPoint
        private void spawnVehicle(GameObject spawnPoint, GameObject vehiclePrefab)
        {
            GameObject vehicle = Instantiate(vehiclePrefab, this.transform);
            vehicle.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
        }
    }
}

