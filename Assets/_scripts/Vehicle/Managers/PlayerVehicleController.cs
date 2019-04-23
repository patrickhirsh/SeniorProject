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
    public List<PassengerPin> SelectedPins = new List<PassengerPin>();
    public bool HasSelectedPins => SelectedPins.Any();

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
        var pin = obj.GetComponent<PassengerPin>();
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
        else if (vehicle && HasOwnership(vehicle) && HasSelectedPins && !vehicle.HasTask)
        {
            foreach (Vehicle x in Vehicles)
            {
                x.DeactivateRing();
            }
            if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Selected Vehicle {vehicle}", vehicle);
            HandleVehicleSelect(vehicle);
        }
        else if (pin && !pin.QueuedForPickup)
        {
            var tutorialObject = GameObject.Find("TutorialManager");

            if (tutorialObject != null)
            {
                TutorialManager manager = tutorialObject.GetComponent<TutorialManager>();
                manager.PickupPassenger();
            }

            if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Selected Passengers {pin}", pin);
            HandlePinSelect(pin);
        }
    }

    private void HandleVehicleSelect(Vehicle vehicle)
    {
        //TODO: This needs to be optimized out
        var tutorialObject = GameObject.Find("TutorialManager");

        if (tutorialObject != null)
        {
            var tut = tutorialObject.GetComponent<TutorialManager>();
            tut.SelectVehicle(vehicle);
        }

        BuildTasks(vehicle);
        SelectedPins = new List<PassengerPin>();
    }

    private void BuildTasks(Vehicle vehicle)
    {
        // Get a path to pickup all selected passengers
        var selectedPassengers = SelectedPins.Select(pin => pin.Passenger).ToArray();
        foreach (var passenger in selectedPassengers)
        {
            vehicle.AddTask(new PickupPassengerTask(vehicle, true, passenger));
        }
        vehicle.PlayVehicleSelected();
        foreach (var pin in SelectedPins)
        {
            pin.QueuedForPickup = true;
        }
    }

    public void HandleNotHit(GameObject arg0)
    {

    }

    #endregion

    #region Pins

    private void HandlePinSelect(PassengerPin passengerPin)
    {
        if (!SelectedPins.Contains(passengerPin))
        {
            SelectedPins.Add(passengerPin);
            passengerPin.PlaySelected();
        }
        else
        {
            passengerPin.SetSelected(false);
            SelectedPins.Remove(passengerPin);
        }
        UpdateSelectedPins();
        UpdateVehicleRings();
    }

    private void UpdateVehicleRings()
    {
        foreach (Vehicle vehicle in Vehicles)
        {
            if (HasSelectedPins && !vehicle.HasTask) vehicle.ActivateRing();
            else vehicle.DeactivateRing();
        }
    }

    private void UpdateSelectedPins()
    {
        for (var i = 0; i < SelectedPins.Count; i++)
        {
            SelectedPins[i].SetSelected(true, i + 1);
        }
    }

    private void DeselectAllPins()
    {
        foreach (var pin in SelectedPins)
        {
            pin.SetSelected(false);
        }
    }

    public void DeselectPassengerPin(PassengerPin pin)
    {
        if (SelectedPins.Contains(pin))
        {
            pin.SetSelected(false);
            SelectedPins.Remove(pin);
        }
        UpdateSelectedPins();
        UpdateVehicleRings();
    }

    #endregion

    /// <summary>
    /// Called when a vehicle has no further tasks and becomes idle
    /// </summary>
    public override void IdleVehicle(Vehicle vehicle)
    {
        UpdateVehicleRings();
    }

}