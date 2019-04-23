using RideShareLevel;
using UnityEngine;

public class EnemyVehicleController : VehicleController
{
    // prefab to be spawned for enemy vehicles
    public GameObject VehiclePrefab;

    #region Unity Methods

    #endregion

    /// <summary>
    /// Calling this method will spawn an enemy vehicle to pick up "passenger".
    /// From here, everything is handled internally (ie. if a player picks the passenger up
    /// first, etc.). This method is designed to be called when a pasenger times out.
    /// </summary>
    public void PickupPassenger(Passenger passenger)
    {
        SpawnRoute spawnPoint = CurrentLevel.NeutralVehicleController.GetRandomSpawnRoute();
        Debug.Assert(VehiclePrefab != null);    // if this assert fails, the enemy vehicle has not been set in the inspector!

        // instantiate the new vehicle
        Vehicle vehicle = Instantiate(VehiclePrefab, spawnPoint.transform.position, Quaternion.identity, transform).GetComponent<Vehicle>();
        vehicle.CurrentRoute = spawnPoint;
        vehicle.Controller = this;
        vehicle.PlayEnemySpawn();

        // obtain a path to the passenger and assign the task
        vehicle.AddTask(new PickupPassengerTask(vehicle, true, passenger));
    }

    public override void IdleVehicle(Vehicle vehicle)
    {
        vehicle.AddTask(new DespawnTask(vehicle, false));
    }
}
