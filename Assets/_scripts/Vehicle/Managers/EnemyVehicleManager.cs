using Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVehicleManager : VehicleManager
{
    public override void VehicleTaskCallback(TaskType type, Vehicle vehicle, bool exitStatus)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// given a spawn point and a vehicle prefab, spawns a new instance of vehiclePrefab at "spawnPoint" with "destination"
    /// </summary>
    private void SpawnVehicle(Connection spawnPoint, Connection destination, GameObject vehiclePrefab)
    {
        // instantiate the new vehicle
        var vehicle = Instantiate(vehiclePrefab, spawnPoint.transform.position, Quaternion.identity, transform).GetComponent<Vehicle>();
        vehicle.Manager = this;

        // obtain a path to the destination
        Queue<Connection> path = new Queue<Connection>();
        PathfindingManager.Instance.GetPath(spawnPoint, destination, out path);

        // assign the pathing task to this new vehicle
        vehicle.AssignTask(new VehicleTask(TaskType.ActiveAi, path, VehicleTaskCallback));
    }
}
