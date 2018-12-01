using Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Connection = Level.Connection;

public class PlayerVehicleManager : VehicleManager
{
    private Vehicle _selectedVehicle;

    private Route _start;
    private Stack<Intersection> _intersections;
    private Route _end;

    private Route _previousSelectedRoute;
    private List<Route> _destinationables;

    private bool VehicleSelected => _selectedVehicle != null;

    public override void VehicleTaskCallback(TaskType type, Vehicle vehicle, bool exitStatus)
    {

    }

    #region Unity

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (_destinationables != null)
        {
            foreach (var destinationable in _destinationables)
            {
                Gizmos.DrawSphere(destinationable.transform.position + Vector3.up, .25f);
            }
        }
    }

    #endregion

    #region UserInterface

    public GameObject DestinationReticle;
    private List<GameObject> _destinationReticles = new List<GameObject>();

    public Vector3 adjustmentVector;

    private void DrawDestinations()
    {
        _destinationReticles.ForEach(Destroy);

        if (_destinationables != null)
        {
            foreach (var destinationable in _destinationables)
            {
                var reticle = Instantiate(DestinationReticle, destinationable.transform.position + adjustmentVector, Quaternion.identity);
//                reticle.transform.localScale = Vector3.one * GameManager.Instance.Scale;
//                GameManager.Instance.OnScaleChangeEvent.AddListener(val => { reticle.transform.localScale = Vector3.one * val; });
                _destinationReticles.Add(reticle);
            }
        }
    }

    #endregion

    #region Selection & Destination Search

    internal void HandleHit(RaycastHit hitInfo)
    {
        if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Selected {hitInfo.transform.gameObject}", hitInfo.transform.gameObject);

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

        DrawDestinations();
    }

    public void HandleNotHit()
    {
        Deselect();
        DrawDestinations();
    }

    private void Deselect()
    {
        Debug.Log("DESELECT");
        _selectedVehicle = null;
        _previousSelectedRoute = null;
        _start = null;
        _end = null;
        _destinationables = null;
    }

    //process a tap on a pickup location
    private void PickupSelection(PickupLocation pickupLocation)
    {

    }

    //process a tap on an intersection
    private void IntersectionSelection(Intersection intersection)
    {
        if (_previousSelectedRoute == intersection && _intersections.Any())
        {
            // We assume that the intersection is the last in the queue if we double click it
            _end = _intersections.Pop();

            Debug.Assert(_start != null, "Start is not defined.");
            Debug.Assert(_end != null, "End is not defined.");

            Queue<Connection> connections;
            var intersections = new Queue<Intersection>(_intersections.Reverse());
            if (PathfindingManager.Instance.GetPath(_start, intersections, _end, out connections))
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
            _intersections.Push(intersection);
            DestinationableSearch(intersection);
        }
    }

    private void CarSelection(Vehicle vehicle)
    {
        //Pick a new vehicle to control
        _selectedVehicle = vehicle;
        _start = _selectedVehicle.CurrentRoute;
        _intersections = new Stack<Intersection>();

        _selectedVehicle.HaltCurrentTask();
        DestinationableSearch(_start);
    }

    private void DestinationableSearch(Route start)
    {
        if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Get next destinationables from {start}", start);

        _destinationables = new List<Route>();
        HashSet<Route> frontier = new HashSet<Route>();
        GetNextDestinationables(start, _destinationables, frontier);
    }

    private void GetNextDestinationables(Route start, IList<Route> destinations, HashSet<Route> frontier)
    {
        foreach (var route in start.NeighborRoutes)
        {
            if (frontier.Contains(route) || route == start) continue;
            frontier.Add(route);
            if (route.Destinationable)
            {
                if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Destination Found: {route}", route);
                destinations.Add(route);
            }
            else
            {
                GetNextDestinationables(route, destinations, frontier);
            }
        }
    }

    #endregion
}