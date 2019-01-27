using Level;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EnemyVehicleStatus { pathingToPassenger, pathingToDestination, pathingToDespawn };

public class EnemyVehicleManager : VehicleManager
{

    #region Singleton
    private static EnemyVehicleManager _instance;
    public static EnemyVehicleManager Instance => _instance ?? (_instance = Create());

    private static EnemyVehicleManager Create()
    {
        GameObject singleton = FindObjectOfType<EnemyVehicleManager>()?.gameObject;
        if (singleton == null)
        {
            singleton = new GameObject { name = $"[{typeof(EnemyVehicleManager).Name}]" };
            singleton.AddComponent<EnemyVehicleManager>();
        }
        return singleton.GetComponent<EnemyVehicleManager>();
    }
    #endregion

    // prefab to be spawned for enemy vehicles
    public GameObject VehiclePrefab;

    // stores all currently active enenmy vehicls and their associated tasks
    private Dictionary<Vehicle, EnemyVehicleTask> _enemyVehicles;

    /// <summary>
    /// Container to store information on the associated enemy vehicle's task.
    /// Used as a value in _enemyVehicles
    /// </summary>
    public class EnemyVehicleTask
    {
        public EnemyVehicleStatus status;
        public Passenger passenger;
    }

    #region Unity Methods & Broadcast Handlers
    public void Awake()
    {
        Broadcaster.AddListener(GameEvent.Reset, Reset);
    }

    public void Start()
    {
        _enemyVehicles = new Dictionary<Vehicle, EnemyVehicleTask>();
    }

    private void Reset(GameEvent @event)
    {
        _enemyVehicles = new Dictionary<Vehicle, EnemyVehicleTask>();
    }
    #endregion

    public override void VehicleTaskCallback(TaskType type, Vehicle vehicle, bool exitStatus)
    {
        switch(_enemyVehicles[vehicle].status)
        {
            case EnemyVehicleStatus.pathingToPassenger:
                // has the passenger been picked up by the player during pathing?
                bool passengerStillThere = false;  
                var passengerTerminals = vehicle.CurrentRoute.Terminals.Where(t => t.HasPassenger).ToArray();
                foreach (Terminal terminal in passengerTerminals)
                {
                    if (terminal.Passenger == _enemyVehicles[vehicle].passenger)
                    {
                        vehicle.AddPassenger(terminal.Passenger);
                        terminal.RemovePassenger();
                        DropOffPassenger(vehicle);
                        _enemyVehicles[vehicle].status = EnemyVehicleStatus.pathingToDestination;
                        passengerStillThere = true;
                    }
                }
                // the passenger was picked up already. RIP. We'll get em next time boys. head to despawn.
                if (!passengerStillThere)
                {
                    PathToDespawn(vehicle);
                    _enemyVehicles[vehicle].status = EnemyVehicleStatus.pathingToDespawn;
                }
                break;

            case EnemyVehicleStatus.pathingToDestination:
                vehicle.RemovePassenger(_enemyVehicles[vehicle].passenger);
                Destroy(_enemyVehicles[vehicle].passenger.gameObject);
                PathToDespawn(vehicle);
                _enemyVehicles[vehicle].status = EnemyVehicleStatus.pathingToDespawn;
                break;

            case EnemyVehicleStatus.pathingToDespawn:
                _enemyVehicles.Remove(vehicle);
                Destroy(vehicle);
                break;
        }
    }

    /// <summary>
    /// Calling this method will spawn an enemy vehicle to pick up "passenger".
    /// From here, everything is handled internally (ie. if a player picks the passenger up
    /// first, etc.). This method is designed to be called when a pasenger times out.
    /// </summary>
    public void PickupPassenger(Passenger passenger)
    {
        SpawnRoute spawnPoint = NeutralVehicleManager.Instance.GetRandomSpawnRoute();
        Debug.Assert(VehiclePrefab != null);    // if this assert fails, the enemy vehicle has not been set in the inspector!

        // instantiate the new vehicle
        Vehicle vehicle = Instantiate(VehiclePrefab, spawnPoint.transform.position, Quaternion.identity, transform).GetComponent<Vehicle>();
        vehicle.Manager = this;

        // track the vehicle with a mapping to its task
        EnemyVehicleTask task = new EnemyVehicleTask();
        task.status = EnemyVehicleStatus.pathingToPassenger;
        task.passenger = passenger;
        _enemyVehicles[vehicle] = task;

        // obtain a path to the passenger and assign the task
        Queue<Connection> path = new Queue<Connection>();
        PathfindingManager.Instance.GetPath(spawnPoint, passenger.StartRoute, out path);
        vehicle.AssignTask(new VehicleTask(TaskType.ActiveAi, path, VehicleTaskCallback));
    }

    /// <summary>
    /// Given an enemy vehicle with a passenger picked up (assumes exactly ONE passenger),
    /// drops that passenger off at their destination.
    /// </summary>
    private void DropOffPassenger(Vehicle vehicle)
    {
        // obtain a path to the passenger's destination and assign the task
        Queue<Connection> path = new Queue<Connection>();
        PathfindingManager.Instance.GetPath(vehicle.CurrentRoute, vehicle.Passengers[0].DestRoute, out path);
        vehicle.AssignTask(new VehicleTask(TaskType.ActiveAi, path, VehicleTaskCallback));
    }

    /// <summary>
    /// Paths an enemy vehicle to its despawn point and despawns.
    /// </summary>
    private void PathToDespawn(Vehicle vehicle)
    {
        Debug.Assert(!vehicle.HasPassenger);

        // obtain a path to the vehicle's despawn point and assign the task
        SpawnRoute despawnPoint = NeutralVehicleManager.Instance.GetRandomSpawnRoute();
        Queue<Connection> path = new Queue<Connection>();
        PathfindingManager.Instance.GetPath(despawnPoint, vehicle.CurrentRoute, out path);
        vehicle.AssignTask(new VehicleTask(TaskType.ActiveAi, path, VehicleTaskCallback));
    }
}
