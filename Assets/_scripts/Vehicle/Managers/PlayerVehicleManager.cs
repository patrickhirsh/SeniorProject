using System;
using Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Connection = Level.Connection;
using Random = UnityEngine.Random;

public class PlayerVehicleManager : VehicleManager
{
    #region Singleton
    private static PlayerVehicleManager _instance;
    public static PlayerVehicleManager Instance => _instance ?? (_instance = Create());

    private static ScoreManagerScript SM;

    private static PlayerVehicleManager Create()
    {
        GameObject singleton = FindObjectOfType<PlayerVehicleManager>()?.gameObject;
        SM = FindObjectOfType<ScoreManagerScript>();
        if (singleton == null)
        {
            singleton = new GameObject { name = $"[{typeof(PlayerVehicleManager).Name}]" };
            singleton.AddComponent<LevelManager>();
        }
        return singleton.GetComponent<PlayerVehicleManager>();
    }
    #endregion

    private List<Passenger> _selectedPassengers = new List<Passenger>();

    private Dictionary<Vehicle, Queue<VehicleTask>> VehicleTasks = new Dictionary<Vehicle, Queue<VehicleTask>>();

    public override void VehicleTaskCallback(TaskType type, Vehicle vehicle, bool exitStatus)
    {
        if (vehicle.HasPassenger)
        {
            int numDroppedOff = 0;
            float timeLeft = 0;
            //Need to add the code here to score points
            foreach (var passenger in new List<Passenger>(vehicle.Passengers))
            {
                if (passenger.DestRoute == vehicle.CurrentRoute)
                {
                    numDroppedOff += 1;
                    timeLeft += passenger.GetTimeRemaining();
                    DeliverPassenger(vehicle, passenger);
                }
            }
            //TODO: give vehicle a CarType so that this can use vehicle.cartype instead of just std vehicles
            SM.ScorePoints(timeLeft, ScoreManagerScript.CarType.STD, numDroppedOff);

        }
        if (vehicle.CurrentRoute.HasTerminals && vehicle.CurrentRoute.Terminals.Any(terminal => terminal.HasPassenger))
        {
            PickupPassenger(vehicle);
        }

        if (VehicleTasks.ContainsKey(vehicle) && VehicleTasks[vehicle].Any())
        {
            vehicle.AssignTask(VehicleTasks[vehicle].Dequeue());
        }
        else if (vehicle.HasPassenger && !VehicleTasks[vehicle].Any())
        {
            // We have a passenger but we don't have a task to drop them off.
            Queue<Connection> connections;
            if (PathfindingManager.Instance.GetPath(vehicle.CurrentRoute, vehicle.Passengers.First().DestRoute, out connections))
            {
                vehicle.AssignTask(new VehicleTask(TaskType.PassivePlayer, connections, VehicleTaskCallback));
            }
            else
            {
                Debug.LogWarning("Could not find path to passenger for vehicle");
            }
        }

    }

    private static void PickupPassenger(Vehicle vehicle)
    {
        var passengerTerminals = vehicle.CurrentRoute.Terminals.Where(t => t.HasPassenger).ToArray();
        var terminal = passengerTerminals[Random.Range(0, passengerTerminals.Length)];
        vehicle.AddPassenger(terminal.Passenger);
        terminal.RemovePassenger();
    }

    private static void DeliverPassenger(Vehicle vehicle, Passenger passenger)
    {
        vehicle.RemovePassenger(passenger);
        
        Destroy(passenger.gameObject);
        Debug.Log("PASSENGER DELIVERED");
        //GameManager.Instance.AddScore(10);
    }

    private void HandlePassiveAi()
    {
        ParkingRoute[] parkingSpots = FindObjectsOfType<ParkingRoute>();

        ParkingRoute nearestSpot = parkingSpots[0];
        float nearestDist = Mathf.Infinity;

        for (int i = 0; i < parkingSpots.Length; i++)
        {
            if (parkingSpots[i].Type == ParkingRouteType.Volta && !parkingSpots[i].IsOccupied)
            {
                //                float cDist = Vector3.Distance(_selectedVehicle.transform.position, parkingSpots[i].transform.position);
                //
                //                if (cDist < nearestDist)
                //                {
                //                    nearestSpot = parkingSpots[i];
                //                    nearestDist = cDist;
                //                }
            }
        }

        Queue<Connection> connections = new Queue<Connection>();

        //        PathfindingManager.Instance.GetPath(_selectedVehicle.CurrentConnection, nearestSpot.Connections[0], out connections);

        //        _selectedVehicle.AssignTask(new VehicleTask(TaskType.PassivePlayer, connections, VehicleTaskCallback));
    }

    #region Unity Methods

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
    }

    #endregion

    #region UserInterface

    public GameObject IntersectionDestinationReticle;
    public GameObject PickupDestinationReticle;
    public GameObject PassengerDeliveryReticle;

    private List<GameObject> _destinationReticles = new List<GameObject>();

    public Vector3 AdjustmentVector;

    private void DrawDestinations()
    {
        _destinationReticles.ForEach(Destroy);
        //        if (_destinationables != null)
        //        {
        //            foreach (var destinationable in _destinationables)
        //            {
        //                if (_selectedVehicle.HasPassenger && destinationable == _selectedVehicle.Passengers.DestRoute)
        //                {
        //                    var reticle = Instantiate(PassengerDeliveryReticle, destinationable.transform.GetChild(0).GetChild(0).transform.position + AdjustmentVector, Quaternion.identity, destinationable.transform);
        //                    _destinationReticles.Add(reticle);
        //                }
        //                else if (destinationable.HasPassenger)
        //                {
        //                    var reticle = Instantiate(PickupDestinationReticle, destinationable.transform.GetChild(0).GetChild(0).transform.position + AdjustmentVector, Quaternion.identity, destinationable.transform);
        //                    _destinationReticles.Add(reticle);
        //                }
        //                else if (destinationable != _selectedVehicle.CurrentRoute)
        //                {
        //                    var reticle = Instantiate(IntersectionDestinationReticle, destinationable.transform.GetChild(0).GetChild(0).transform.position + AdjustmentVector, Quaternion.identity, destinationable.transform);
        //                    _destinationReticles.Add(reticle);
        //                }
        //            }
        //        }
    }

    #endregion

    #region Selection & Destination Search

    internal void HandleHit(RaycastHit hitInfo)
    {
        if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Hit: {hitInfo.transform.gameObject}", hitInfo.transform.gameObject);

        var vehicle = hitInfo.transform.GetComponent<Vehicle>();
        var pin = hitInfo.transform.GetComponent<Pin>();

        if (vehicle && HasOwnership(vehicle) && _selectedPassengers.Any())
        {
            if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Selected Vehicle {vehicle}", vehicle);
            HandleVehicleSelect(vehicle);
        }
        else if (pin)
        {
            var route = pin.GetComponentInParent<Route>();
            if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Selected Passengers {route}", route);
            HandlePassengerSelect(pin);
        }

        //        DrawDestinations();
        //        DrawPassengerInfo();
    }

    private void HandleVehicleSelect(Vehicle vehicle)
    {
        vehicle.HaltCurrentTask();
        if (!VehicleTasks.ContainsKey(vehicle)) VehicleTasks[vehicle] = new Queue<VehicleTask>();
        BuildTasks(vehicle);
        _selectedPassengers = new List<Passenger>();
        HoverChanged.Invoke("Sending vehicle to pick up passengers");

    }

    private void BuildTasks(Vehicle vehicle)
    {
        // Get a path to pickup all selected passengers
        var current = vehicle.CurrentRoute;
        foreach (var passenger in _selectedPassengers)
        {
            Queue<Connection> connections;
            if (PathfindingManager.Instance.GetPath(current, passenger.StartRoute, out connections))
            {
                VehicleTasks[vehicle].Enqueue(new VehicleTask(TaskType.PassivePlayer, connections, VehicleTaskCallback));
                current = passenger.StartRoute;
            }
            else
            {
                Debug.LogWarning("Could not find path to passenger for vehicle");
            }
        }

        // Get path from last picked up passenger to each destination of passengers
        foreach (var passenger in _selectedPassengers)
        {
            Queue<Connection> connections;
            Debug.Assert(passenger.DestRoute != null, "Passengers does not have a destination");
            if (PathfindingManager.Instance.GetPath(current, passenger.DestRoute, out connections))
            {
                VehicleTasks[vehicle].Enqueue(new VehicleTask(TaskType.PassivePlayer, connections, VehicleTaskCallback));
                current = passenger.DestRoute;
            }
            else
            {
                Debug.LogWarning("Could not find path for passenger to destination");
            }
        }

        vehicle.AssignTask(VehicleTasks[vehicle].Dequeue());
    }

    private void HandlePassengerSelect(Pin pin)
    {
        if (!_selectedPassengers.Contains(pin.Passenger))
        {
            _selectedPassengers.Add(pin.Passenger);
            HoverChanged.Invoke("Deselect Passengers");
        }
        else
        {
            _selectedPassengers.Remove(pin.Passenger);
            HoverChanged.Invoke("Select Passengers");
        }
    }

    private bool HasOwnership(Vehicle vehicle)
    {
        return vehicle.Manager == this;
    }

    public void HandleNotHit()
    {
        Deselect();
        DrawDestinations();
    }

    private void Deselect()
    {
        if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log("DESELECT");
        //_selectedVehicle.Passengers.SetDestReticle(false);
        GetComponent<LineRenderer>().positionCount = 0;
    }


    #endregion


    #region Hover
    [Serializable]
    public class HoverEvent : UnityEvent<string> { }
    public HoverEvent HoverChanged = new HoverEvent();
    private Transform _hoverTransform;

    public void HandleHover(bool hit, RaycastHit hitInfo)
    {
        if (hit)
        {
            if (_hoverTransform == null || _hoverTransform != hitInfo.transform)
            {
                _hoverTransform = hitInfo.transform;

                var vehicle = hitInfo.transform.GetComponent<Vehicle>();
                var pin = hitInfo.transform.GetComponent<Pin>();

                if (vehicle && HasOwnership(vehicle) && _selectedPassengers.Any())
                {
                    HoverChanged.Invoke("Send Vehicle to Pickup");
                }
                else if (vehicle && HasOwnership(vehicle))
                {
                    HoverChanged.Invoke("No Passengers Selected");
                }

                if (pin && !_selectedPassengers.Contains(pin.Passenger))
                {
                    HoverChanged.Invoke("Select Passengers");
                }
                else if (pin && _selectedPassengers.Contains(pin.Passenger))
                {
                    HoverChanged.Invoke("Deselect Passengers");
                }
            }
        }
        else
        {
            HoverChanged.Invoke(null);
            _hoverTransform = null;
        }
    }
    #endregion


}