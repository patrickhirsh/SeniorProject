using System;
using System.Collections.Generic;
using System.Linq;
using Level;
using UnityEngine;
using Connection = Level.Connection;

public class PlayerVehicleManager : VehicleManager
{
    private Vehicle _selectedVehicle;
    private Connection[] _nextValidConnections;
    private Intersection _currentIntersection;
    private Queue<Intersection> _finalPath;
    private Dictionary<Route, IList<Connection>> _validPaths;

    private bool VehicleSelected => _selectedVehicle != null;

    public override void VehicleTaskCallback(TaskType type, Vehicle vehicle, bool exitStatus)
    {

    }

    internal void HandleHit(RaycastHit hitInfo)
    {
        Debug.Log("HIT", hitInfo.transform);
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
    }

    public void HandleNotHit()
    {
        Deselect();
    }

    private void Deselect()
    {
        Debug.Log("DESELECT");
        _selectedVehicle = null;
        _finalPath?.Clear();
        _currentIntersection = null;
    }

    //process a tap on a pickup location
    private void PickupSelection(PickupLocation pickupLocation)
    {

    }

    //process a tap on an intersection
    private void IntersectionSelection(Intersection intersection)
    {
        if(_currentIntersection != null)
            Debug.Log("Current intersection: " + _currentIntersection.name);
        Debug.Log($"{intersection} selected", intersection);

        if (_currentIntersection == intersection)
        {
            Debug.Log($"{_finalPath.Count}" + "  Intersections in final path");
            _selectedVehicle.AssignTask(new VehicleTask(TaskType.ActivePlayer, _selectedVehicle.CurrentConnection.ParentRoute, new Queue<Intersection>(_finalPath), intersection.Connections[0].ParentRoute, VehicleTaskCallback));
            Deselect();
        }
        else if (_validPaths != null && _validPaths.ContainsKey(intersection))
        {
            //Add the task of navigating to this intersection to the que of things to do

            Debug.Log("Added to final path " + intersection.Connections[0].ParentRoute.name);

            _finalPath.Enqueue(intersection);

            Debug.Log("FINAL Q LAST: " + _finalPath.Last().name);

            GetNextValidIntersections(intersection);

            _currentIntersection = intersection;
        }

        
    }
    //process a tap on a car
    private void CarSelection(Vehicle vehicle)
    {
        Debug.Log("VEHICLE SELECTED");
        //Pick a new vehicle to control
        _selectedVehicle = vehicle;
        _finalPath = new Queue<Intersection>();
        _currentIntersection = null;
        GetNextValidIntersectionsBegin(vehicle.CurrentConnection);
    }

    //This function gets all the valid intersections to navigate to from the first connection
    private void GetNextValidIntersectionsBegin(Connection currentConnection)
    {
        //Rest valid paths
        _validPaths = new Dictionary<Route, IList<Connection>>();
        //fore each connection coming out of the current route
        //for each connection path in that route
        foreach (var nextConnection in currentConnection.Paths.Select(path => path.Connection))
        {
            //New list of connectiosn
            var connectionsList = new List<Connection>();
            //Follow the path from the starting connection in the route to the 
            var current = nextConnection.ConnectsTo;

            while (current != null && current.ParentRoute.GetType() != typeof(Intersection))
            {
                connectionsList.Add(current);
                var next = current.Paths.First().Connection;
                connectionsList.Add(next);
                current = next.ConnectsTo;
                Debug.Assert(current != null, "Current is null!");

            }

            if (current != null)
            {
                //connections.Add(current);

                var route = current.ParentRoute;
                Debug.Assert(route != null, "Route cannot be null");
                if (!_validPaths.ContainsKey(route))
                {
                    Debug.Log($"{route} Added to Valid Paths", route);
                    _validPaths.Add(route, connectionsList);
                }
            }
        }
    }



    //This gets all the valid intersections to navigate to 
    private void GetNextValidIntersections(Intersection currentIntersection)
    {
        //Rest valid paths
        _validPaths = new Dictionary<Route, IList<Connection>>();
        //fore each connection coming out of the current Intersection
        foreach(Connection connection in currentIntersection.Connections) {
            //for each connection path in that route
            foreach (var nextConnection in connection.Paths.Select(path => path.Connection))
            {
                //New list of connectiosn
                var connectionsList = new List<Connection>();
                //Follow the path from the starting connection in the route to the 
                var current = nextConnection.ConnectsTo;

                while (current != null && current.ParentRoute.GetType() != typeof(Intersection) && current.ParentRoute.GetType() != typeof(ParkingSpot))
                {
                    connectionsList.Add(current);
                    var next = current.Paths.First().Connection;
                    connectionsList.Add(next);
                    current = next.ConnectsTo;
                    //Debug.Assert(current != null, "Current is null!");

                }

                if (current != null && current.ParentRoute != currentIntersection && current.ParentRoute.GetType() != typeof(ParkingSpot))
                {
                    //connections.Add(current);

                    var route = current.ParentRoute;
                    Debug.Assert(route != null, "Route cannot be null");
                    if (!_validPaths.ContainsKey(route))
                    {
                        Debug.Log($"{route} Added to list of valid connections", route);
                        _validPaths.Add(route, connectionsList);
                    }
                }
            }
        }
    }

    private static Connection GetClosestConnection(Vehicle vehicle)
    {
        Connection returnConnection = new Connection();
            
        //In here we somehow need to find the closest distance wise connection, or the last passed over one. 

        return returnConnection;
    }
}
