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
        if (vehicle.HasPassenger && vehicle.CurrentRoute == vehicle.Passenger.DestRoute)
        {
            DeliverPassenger(vehicle);
            vehicle.Passenger.DespawnDestinationReticle();
        }
        if (!vehicle.HasPassenger && vehicle.CurrentRoute.HasTerminals && vehicle.CurrentRoute.Terminals.Any(terminal => terminal.HasPassenger))
        {
            PickupPassenger(vehicle);
        }
    }

    private static void PickupPassenger(Vehicle vehicle)
    {
        var passengerTerminals = vehicle.CurrentRoute.Terminals.Where(t => t.HasPassenger).ToArray();
        var terminal = passengerTerminals[Random.Range(0, passengerTerminals.Length)];
        vehicle.AddPassenger(terminal.Passenger);
        terminal.RemovePassenger();
        vehicle.Passenger.SpawnDestinationReticle();
    }

    private static void DeliverPassenger(Vehicle vehicle)
    {
        var hasPin = vehicle.CurrentRoute.GetComponentInChildren<Pin>();
        if (hasPin)
        {
            Destroy(hasPin.gameObject);
        }
        Destroy(vehicle.Passenger.gameObject);
        Debug.Log("PASSENGER DELIVERED");
        GameManager.Instance.AddScore(10);
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

    #region Unity Methods

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
    public GameObject PassengerDeliveryReticle;

    private List<GameObject> _destinationReticles = new List<GameObject>();

    public Vector3 AdjustmentVector;

    private void DrawDestinations()
    {
        _destinationReticles.ForEach(Destroy);
        if (_destinationables != null)
        {
            foreach (var destinationable in _destinationables)
            {
                if (_selectedVehicle.HasPassenger && destinationable == _selectedVehicle.Passenger.DestRoute)
                {
                    var reticle = Instantiate(PassengerDeliveryReticle, destinationable.transform.GetChild(0).GetChild(0).transform.position + AdjustmentVector, Quaternion.identity, destinationable.transform);
                    _destinationReticles.Add(reticle);
                }
                else if (destinationable.HasPassenger)
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
        if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Hit: {hitInfo.transform.gameObject}", hitInfo.transform.gameObject);

        var vehicle = hitInfo.transform.GetComponent<Vehicle>();
        var pin = hitInfo.transform.GetComponent<Pin>();

        if (vehicle && vehicle.Manager == this)
        {
            //if (Debugger.Profile.DebugPlayerVehicleManager) 
            Debug.Log($"Selected Vehicle {vehicle}", vehicle);
            if (VehicleSelected) Deselect();
            CarSelection(vehicle);
            if (_selectedVehicle.HasPassenger)
            {
                _selectedVehicle.Passenger.SetDestReticle(true);
            }
        }
        else if (pin)
        {
            var route = pin.GetComponentInParent<Route>();
            if (Debugger.Profile.DebugPlayerVehicleManager) Debug.Log($"Selected Route {route}", route);
            /*
            if (_selectedVehicle.HasPassenger && route == _selectedVehicle.Passenger.DestinationTerminal.ParentRoute)
            {
                RouteSelection(route);
            }
            else */
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
        var curve = GetComponent<BezierCurve>();
        Debug.Assert(line != null, $"{name} needs a line renderer");
        Debug.Assert(curve != null, $"{name} needs a bezier curve");

        if (line != null && curve != null && _selectedVehicle != null && _selectedVehicle.HasPassenger)
        {
            var offsetY = 1f;
            curve.Clear(true);

            var start = _selectedVehicle.transform.position;
            var end = _selectedVehicle.Passenger.DestRoute.transform.position;
            start.y += offsetY;
            end.y = start.y;

            var half = Vector3.Lerp(start, end, .5f);

            var handle1 = Vector3.Lerp(start, half, .5f);
            handle1.y = start.y + offsetY;

            curve.AddPointAt(start);
            var mid = curve.AddPointAt(half + Vector3.up);
            mid.globalHandle1 = handle1;
            curve.AddPointAt(end);

            line.positionCount = 20;
            PathfindingManager.Instance.DrawCurve(curve, line);
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
        //_selectedVehicle.Passenger.SetDestReticle(false);
        _selectedVehicle = null;
        _previousSelectedRoute = null;
        _start = null;
        _end = null;
        _destinationables = null;
        GetComponent<LineRenderer>().positionCount = 0;
    }

    //process a tap on a pickup location
    private void RouteSelection(Route route)
    {
        Queue<Connection> connections;

        if (PathfindingManager.Instance.GetPath(_start, route, out connections))
//        if (PathfindingManager.Instance.GetPath(_start, intersections, route, out connections))
        {
            _selectedVehicle.AssignTask(new VehicleTask(TaskType.ActivePlayer, connections, VehicleTaskCallback));
            //_selectedVehicle.Passenger.SetDestReticle(false);
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

        if (_selectedVehicle != null)
        {
            if (_selectedVehicle.HasPassenger)
            {
                return _selectedVehicle.Passenger.DestRoute == route;
            }
            return route.HasPassenger;
        }

        return false;
    }

    #endregion
}