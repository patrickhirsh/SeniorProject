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
    private Stack<IntersectionRoute> _intersections;
    private Route _end;

    private Route _previousSelectedRoute;
    private List<Route> _destinationables;

    private bool VehicleSelected => _selectedVehicle != null;

    public override void VehicleTaskCallback(TaskType type, Vehicle vehicle, bool exitStatus)
    {
        if (vehicle.CurrentRoute.HasTerminals && vehicle.CurrentRoute.Terminals.Any(terminal => terminal.HasPassenger))
        {
            vehicle.AddPassenger(vehicle.CurrentRoute.Terminals.Select(terminal => terminal.Passenger).FirstOrDefault());
        }
    }

    private void HandlePassiveAI()
    {
        ParkingRoute[] parkingSpots = FindObjectsOfType<ParkingRoute>();

        ParkingRoute nearestSpot = parkingSpots[0];
        float nearestDist = Mathf.Infinity;

        for (int i = 0; i < parkingSpots.Length; i++)
        {
            if(parkingSpots[i].Type == ParkingRouteType.Volta && !parkingSpots[i].IsOccupied)
            {
                float cDist = Vector3.Distance(_selectedVehicle.transform.position, parkingSpots[i].transform.position);

                if(cDist < nearestDist)
                {
                    nearestSpot = parkingSpots[i];
                    nearestDist = cDist;
                }
            }
        }

        Queue<Connection> connections = new Queue<Connection>();

        PathfindingManager.Instance.GetPath(_selectedVehicle.CurrentConnection, nearestSpot.Connections[0],out connections);

        _selectedVehicle.AssignTask(new VehicleTask(TaskType.PassivePlayer, connections, VehicleTaskCallback));
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
        var intersection = hitInfo.transform.GetComponent<IntersectionRoute>();
        var route = hitInfo.transform.GetComponent<Route>();
        var pin = hitInfo.transform.GetComponent<Pin>();

        if (vehicle)
        {
            if (VehicleSelected) Deselect();
            CarSelection(vehicle);
        }
        else if (intersection)
        {
            IntersectionSelection(intersection);
        }
        else if (route && route.Destinationable)
        {
            RouteSelection(route);
        }

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
    private void RouteSelection(Route route)
    {
        Queue<Connection> connections;
        var intersections = new Queue<IntersectionRoute>(_intersections.Reverse());
        if (PathfindingManager.Instance.GetPath(_start, intersections, route, out connections))
        {
            _selectedVehicle.AssignTask(new VehicleTask(TaskType.ActivePlayer, connections, VehicleTaskCallback));
        }
        else
        {
            Debug.LogWarning("Could not find a path for player input");
        }
    }

    //process a tap on an intersection
    private void IntersectionSelection(IntersectionRoute intersection)
    {
        if (_previousSelectedRoute == intersection && _intersections.Any())
        {
            // We assume that the intersection is the last in the queue if we double click it
            _end = _intersections.Pop();

            Debug.Assert(_start != null, "Start is not defined.");
            Debug.Assert(_end != null, "End is not defined.");

            Queue<Connection> connections;
            var intersections = new Queue<IntersectionRoute>(_intersections.Reverse());
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
        _intersections = new Stack<IntersectionRoute>();

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
                if (route.GetType() != typeof(IntersectionRoute))
                {
                    GetNextDestinationables(route, destinations, frontier);
                }
            }
            else
            {
                GetNextDestinationables(route, destinations, frontier);
            }
        }
    }

    #endregion
}