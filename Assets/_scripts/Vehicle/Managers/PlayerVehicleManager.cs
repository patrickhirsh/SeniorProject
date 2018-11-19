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
    private Queue<Connection> _finalPath;
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
    }

    //process a tap on a pickup location
    private void PickupSelection(PickupLocation pickupLocation)
    {

    }

    //process a tap on an intersection
    private void IntersectionSelection(Intersection intersection)
    {
        Debug.Log($"{intersection} selected", intersection);

        if (_currentIntersection == intersection)
        {
            Debug.Log($"{_selectedVehicle}", _selectedVehicle.gameObject);
            Debug.Log($"{_finalPath.Count}" + "FINAL PATH HIT");
            _selectedVehicle.AssignTask(new VehicleTask(TaskType.ActivePlayer, new Queue<Connection>(_finalPath), VehicleTaskCallback));
            Deselect();
        }
        else if (_validPaths != null && _validPaths.ContainsKey(intersection))
        {
            //Add the task of navigating to this intersection to the que of things to do
            foreach (var connection in _validPaths[intersection])
            {
                _finalPath.Enqueue(connection);
            }
            GetNextValidIntersections(_finalPath.Last());
        }

        _currentIntersection = intersection;
    }
    //process a tap on a car
    private void CarSelection(Vehicle vehicle)
    {
        Debug.Log("VEHICLE SELECTED");
        //Pick a new vehicle to control
        _selectedVehicle = vehicle;
        _finalPath = new Queue<Connection>();

        GetNextValidIntersections(vehicle.CurrentConnection);
    }

    private void GetNextValidIntersections(Connection connection)
    {
        _validPaths = new Dictionary<Route, IList<Connection>>();
        foreach (var nextConnection in connection.Paths.Select(path => path.Connection))
        {
            var connections = new List<Connection>();
            var current = nextConnection.ConnectsTo;
            Debug.Log("NEXT", current);

            while (current != null && current.ParentRoute.GetType() == typeof(Intersection))
            {
                connections.Add(current);
                var next = current.Paths.First().Connection;
                connections.Add(next);
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
                    Debug.Log($"{route} Added", route);
                    _validPaths.Add(route, connections);
                }
            }
        }
    }


}
