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
            var passengerTerminals = vehicle.CurrentRoute.Terminals.Where(terminal => terminal.HasPassenger);
            vehicle.AddPassenger(passengerTerminals.Select(terminal => terminal.Passenger).FirstOrDefault());
        }
        if (vehicle.HasPassenger && vehicle.CurrentRoute == vehicle.Passenger.DestinationTerminal.ParentRoute)
        {
            Destroy(vehicle.Passenger.gameObject);
            Debug.Log("PASSENGER DELIVERED");
        }
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
                float cDist = Vector3.Distance(_selectedVehicle.transform.position, parkingSpots[i].transform.position);

                if (cDist < nearestDist)
                {
                    nearestSpot = parkingSpots[i];
                    nearestDist = cDist;
                }
            }
        }

        Queue<Connection> connections = new Queue<Connection>();

        PathfindingManager.Instance.GetPath(_selectedVehicle.CurrentConnection, nearestSpot.Connections[0], out connections);

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

    public GameObject IntersectionDestinationReticle;
    public GameObject PickupDestinationReticle;

    private List<GameObject> _destinationReticles = new List<GameObject>();

    public Vector3 AdjustmentVector;

    private void DrawDestinations()
    {
        _destinationReticles.ForEach(Destroy);
        if (_destinationables != null)
        {
            foreach (var destinationable in _destinationables)
            {

                if (destinationable.HasPassenger)
                {
                    var reticle = Instantiate(PickupDestinationReticle, destinationable.transform.GetChild(0).GetChild(0).transform.position + AdjustmentVector, Quaternion.identity, destinationable.transform);
                    _destinationReticles.Add(reticle);
                }
                else if (destinationable != _selectedVehicle.CurrentRoute)
                {
                    var reticle = Instantiate(IntersectionDestinationReticle, destinationable.transform.GetChild(0).GetChild(0).transform.position + AdjustmentVector, Quaternion.identity, destinationable.transform);
                    _destinationReticles.Add(reticle);
                }
            }
        }
    }

    #endregion

    #region Selection & Destination Search

    internal void HandleHit(RaycastHit hitInfo)
    {
        var vehicle = hitInfo.transform.GetComponent<Vehicle>();
        var pin = hitInfo.transform.GetComponent<Pin>();

        if (vehicle && vehicle.Manager == this)
        {
            if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Selected Vehicle {vehicle}", vehicle);
            if (VehicleSelected) Deselect();
            CarSelection(vehicle);
        }
        else if (pin)
        {
            var route = pin.GetComponentInParent<Route>();
            if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Selected Route {route}", route);

            if (route.GetType() == typeof(IntersectionRoute))
            {
                IntersectionSelection((IntersectionRoute)route);
            }
            else if (IsDestinationable(route))
            {
                RouteSelection(route);
            }

            _previousSelectedRoute = route;
        }

        DrawDestinations();
        DrawPassengerInfo();
    }

    private void DrawPassengerInfo()
    {
        var line = GetComponent<LineRenderer>();
        Debug.Assert(line != null, $"{name} needs a line renderer");

        if (line != null && _selectedVehicle != null && _selectedVehicle.HasPassenger)
        {
            var points = new Vector3[line.positionCount];
            for (int i = 0; i < line.positionCount; i++)
            {
                var t = i / (float)line.positionCount;
                points[i] = Vector3.Lerp(_selectedVehicle.transform.position, _selectedVehicle.Passenger.DestinationTerminal.transform.position, t);
                points[i].y += .5f;
            }
            line.SetPositions(points);
        }
    }

    public void HandleNotHit()
    {
        Deselect();
        DrawDestinations();
    }

    private void Deselect()
    {
        if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log("DESELECT");
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
            Deselect();
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
            if (IsDestinationable(route))
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

    private bool IsDestinationable(Route route)
    {
        // This is an "override"
        if (route.Destinationable) return true;

        if (_selectedVehicle)
        {
            if (_selectedVehicle.HasPassenger)
            {
                return _selectedVehicle.Passenger.DestinationTerminal.ParentRoute == route;
            }
            return route.HasPassenger;
        }

        return false;
    }

    #endregion
}