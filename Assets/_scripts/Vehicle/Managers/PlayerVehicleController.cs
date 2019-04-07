using System;
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


    #region Unity Methods

    private void Awake()
    {
        if (!Vehicles.Any())
        {
            Debug.LogWarning("This level is missing vehicles or needs to be baked!");
        }
        InputManager.Instance.Hit.AddListener(HandleHit);
        InputManager.Instance.NoHit.AddListener(HandleNotHit);

    }

    #endregion


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
        else if (vehicle && HasOwnership(vehicle) && SelectedPins.Any() && !vehicle.HasTask)
        {
            foreach (Vehicle x in Vehicles)
            {
                x.DeactivateRing();
            }
            if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Selected Vehicle {vehicle}", vehicle);
            HandleVehicleSelect(vehicle);
        }
        else if (pin)
        {
            foreach (Vehicle v in Vehicles)
            {
                if (!v.HasTask) { v.ActivateRing(); }
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
            if (SelectedPins.Count == 0)
            {
                foreach (Vehicle x in Vehicles)
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
        BuildTasks(vehicle);
        SelectedPins = new List<Pin>();
    }

    private void BuildTasks(Vehicle vehicle)
    {
        // Get a path to pickup all selected passengers
        var selectedPassengers = SelectedPins.Select(pin => pin.Passenger).ToArray();
        foreach (var passenger in selectedPassengers)
        {
            vehicle.AddTask(new PickupPassengerTask(vehicle, passenger));
        }
    }

    public void HandleNotHit(GameObject arg0)
    {

    }

    private void Deselect()
    {
        if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log("DESELECT");
    }

    #endregion

    public bool HasOwnership(Vehicle vehicle)
    {
        return Vehicles.Contains(vehicle);
    }

    public override void IdleVehicle(Vehicle vehicle)
    {
        // Do Nothing for Player Vehicle
    }

}