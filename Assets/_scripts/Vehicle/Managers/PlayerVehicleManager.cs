using System;
using System.Collections.Generic;
using System.Linq;
using Level;
using UnityEngine;
using Connection = Level.Connection;

public class PlayerVehicleManager : VehicleManager
{
    private Vehicle _selectedVehicle;

    private Route _start;
    private Queue<Intersection> _intersections;
    private Route _end;

    private Route _previousSelectedRoute;
    private List<Route> _destinationables;

    private bool VehicleSelected => _selectedVehicle != null;

    public override void VehicleTaskCallback(TaskType type, Vehicle vehicle, bool exitStatus)
    {

    }

    internal void HandleHit(RaycastHit hitInfo)
    {
        var vehicle = hitInfo.transform.GetComponent<Vehicle>();
        var intersection = hitInfo.transform.GetComponent<Intersection>();
        var pickupLocation = hitInfo.transform.GetComponent<PickupLocation>();

        if (vehicle)
        {
            if (VehicleSelected) Deselect();
            CarSelection(vehicle);
        }
        else if (intersection)
        {
            IntersectionSelection(intersection);
        }
        else if (pickupLocation)
        {
            PickupSelection(pickupLocation);
        }

        var route = hitInfo.transform.GetComponent<Route>();
        if (route != null)
        {
            _previousSelectedRoute = route;
        }
    }

    public void HandleNotHit()
    {
        Deselect();
    }

    private void Deselect()
    {
        Debug.Log("DESELECT");
        _selectedVehicle = null;
        _previousSelectedRoute = null;
        _start = null;
        _end = null;
    }

    //process a tap on a pickup location
    private void PickupSelection(PickupLocation pickupLocation)
    {

    }

    //process a tap on an intersection
    private void IntersectionSelection(Intersection intersection)
    {
        if (_previousSelectedRoute == intersection)
        {
            // We assume that the intersection is the last in the queue if we double click it
            _end = _intersections.Dequeue();

            Debug.Assert(_start != null, "Start is not defined.");
            Debug.Assert(_end != null, "End is not defined.");

            Queue<Connection> connections;
            if (PathfindingManager.Instance.GetPath(_start, _intersections, _end, out connections))
            {
                _selectedVehicle.AssignTask(new VehicleTask(TaskType.ActivePlayer, connections, VehicleTaskCallback));
            }
            else
            {
                Debug.LogWarning("Could not find a path for player input");
            }

            Deselect();
        }
        else if (_destinationables != null && _destinationables.Contains(intersection))
        {
            _intersections.Enqueue(intersection);

            _destinationables = new List<Route>();
            HashSet<Route> frontier = new HashSet<Route>();
            GetNextDestinationables(intersection, _destinationables, frontier);
        }
    }

    private void CarSelection(Vehicle vehicle)
    {
        //Pick a new vehicle to control
        _selectedVehicle = vehicle;
        _start = _selectedVehicle.CurrentRoute;
        _intersections = new Queue<Intersection>();

        _destinationables = new List<Route>();
        HashSet<Route> frontier = new HashSet<Route>();
        GetNextDestinationables(_start, _destinationables, frontier);
    }

    private void GetNextDestinationables(Route start, IList<Route> destinations, HashSet<Route> frontier)
    {
        foreach (var route in start.NeighborRoutes)
        {
            if (frontier.Contains(route)) return;
            frontier.Add(route);
            if (route.Destinationable)
            {
                destinations.Add(route);
            }
            else
            {
                GetNextDestinationables(route, destinations, frontier);
            }
        }
    }
}