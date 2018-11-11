using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Level
{
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
        private List<GameObject> _vehicles;         // all valid vehicles to spawn in the current level
        private GameObject activeVehicles;          // parent object of all vehicles in the current scene
        private SpawnState spawnState;              // indicates how (or if) the VehicleManager should be spawning vehicles


        public void Start()
        {
            // spawnState is off by default (waiting for instructions...)
            spawnState = SpawnState.spawningOff;

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
            switch (spawnState)
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


        // given a spawn point and a vehicle prefab, spawns a new instance of vehiclePrefab at spawnPoint
        private void spawnVehicle(GameObject spawnPoint, GameObject vehiclePrefab)
        {
            GameObject vehicle = Instantiate(vehiclePrefab, this.transform);
            vehicle.transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
        }
    }
}

