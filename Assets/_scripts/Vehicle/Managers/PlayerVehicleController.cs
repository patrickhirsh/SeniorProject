﻿using System;
using RideShareLevel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utility;
using Connection = RideShareLevel.Connection;
using Random = UnityEngine.Random;

public class PlayerVehicleController : VehicleController
{
    public List<Pin> SelectedPins = new List<Pin>();

    private Dictionary<Vehicle, Queue<VehicleTask>> VehicleTasks = new Dictionary<Vehicle, Queue<VehicleTask>>();

    public Vehicle[] PlayerVehicles;

    #region Unity Methods

    private void Awake()
    {
        if (!PlayerVehicles.Any())
        {
            Debug.LogWarning("This level is missing vehicles or needs to be baked!");
        }
        InputManager.Instance.Hit.AddListener(HandleHit);
        InputManager.Instance.NoHit.AddListener(HandleNotHit);
    }

    #endregion

    #region Bake

#if UNITY_EDITOR
    public void Bake(Level level)
    {
        PlayerVehicles = GetComponentsInChildren<Vehicle>();
    }
#endif

    #endregion

    public override void VehicleTaskCallback(TaskType type, Vehicle vehicle, bool exitStatus)
    {

        if (vehicle.HasPassenger)
        {
            int numDroppedOff = 0;

            Building.BuildingColors PassColor = Building.BuildingColors.Red;
            //Need to add the code here to score points
            foreach (var passenger in new List<Passenger>(vehicle.Passengers))
            {
                if (passenger.DestRoute == vehicle.CurrentRoute)
                {
                    PassColor = passenger.GetColor();

                    numDroppedOff += 1;

                    DeliverPassenger(vehicle, passenger);
                }
            }

            //TODO: give vehicle a CarType so that this can use vehicle.cartype instead of just std vehicles
            ScoreManager.Instance.ScorePoints(PassColor, numDroppedOff);
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
        //terminal.Passenger.DestroyRing();
        terminal.RemovePassenger();
    }

    private static void DeliverPassenger(Vehicle vehicle, Passenger passenger)
    {
        vehicle.RemovePassenger(passenger);

        Destroy(passenger.gameObject);

        Debug.Log("PASSENGER DELIVERED");
        //GameManager.Instance.AddScore(10);
    }

    private static void SwitchLevelPrefab(GameObject levelPrefab)
    {
        //TODO: Add in Lazy Loading to load desired prefab into position.
        throw new NotImplementedException();
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

    #region Selection & Destination Search

    internal void HandleHit(GameObject obj)
    {
        if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Hit: {obj.transform.gameObject}", obj.transform.gameObject);

        var vehicle = obj.GetComponent<Vehicle>();
        var pin = obj.GetComponent<Pin>();
        var menuBuilding = obj.GetComponent<MenuBuilding>();


        if (menuBuilding)
        {
            //Transition functions here
            //have a bool to require 2 clicks on a building to transition levels.
            if (menuBuilding.getClicked())
            {
                LevelManager.Instance.TransitionLevel(menuBuilding);
            }
            else
            {
                menuBuilding.setClicked(true);
            }
        }
        else if (vehicle && HasOwnership(vehicle) && SelectedPins.Any())
        {
            foreach (Vehicle x in PlayerVehicles)
            {
                x.DeactivateRing();
            }
            if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Selected Vehicle {vehicle}", vehicle);
            HandleVehicleSelect(vehicle);
        }
        else if (pin)
        {
            foreach (Vehicle x in PlayerVehicles)
            {
                Debug.Log("IN FOR LOOPS");
                x.ActivateRing();
            }
            if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Selected Passengers {pin}", pin);
            HandlePinSelect(pin);
        }
    }

    private void HandlePinSelect(Pin pin)
    {
        if (!SelectedPins.Contains(pin))
        {
            SelectedPins.Add(pin);
        }
        else
        {
            pin.SetSelected(false);
            SelectedPins.Remove(pin);
            if(SelectedPins.Count == 0)
            {
                foreach(Vehicle x in PlayerVehicles)
                {
                    x.DeactivateRing();
                }
            }
        }
        UpdateSelectedPins();
    }

    private void UpdateSelectedPins()
    {
        for (var i = 0; i < SelectedPins.Count; i++)
        {
            SelectedPins[i].SetSelected(true, i + 1);
        }
    }

    private void HandleVehicleSelect(Vehicle vehicle)
    {
        vehicle.HaltCurrentTask();
        if (!VehicleTasks.ContainsKey(vehicle)) VehicleTasks[vehicle] = new Queue<VehicleTask>();
        BuildTasks(vehicle);
        SelectedPins = new List<Pin>();
    }

    private void BuildTasks(Vehicle vehicle)
    {
        // Get a path to pickup all selected passengers
        var current = vehicle.CurrentRoute;
        var selectedPassengers = SelectedPins.Select(pin => pin.Passenger).ToArray();
        foreach (var passenger in selectedPassengers)
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
        foreach (var passenger in selectedPassengers)
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

    public bool HasOwnership(Vehicle vehicle)
    {
        return PlayerVehicles.Contains(vehicle);
    }

    public void HandleNotHit(GameObject arg0)
    {
        //foreach (Vehicle x in PlayerVehicles)
        //{
        //    x.DeactivateRing();
        //}
        //Deselect();
    }

    private void Deselect()
    {
        if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log("DESELECT");
    }

    #endregion

}