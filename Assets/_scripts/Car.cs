using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public static List<string> CarPrefabPaths; // stores the paths of all available car prefabs for spawning
    private ParkingSpotNodeOld _lastNodeOld; // Node that the car ended it's pathing in. 
    private bool _moving; // whether the car is currently pathing

    private int _nodeCounter; // indicates the current Node in PathNodes
    public float Acceleration = 5.0f; // acceleration rate
    public float CarSpeed; // carspeed vairable
    public float CastDistance = 1.0f; // how far to cast
    public Transform Front; // where to cast from

    public List<NodeOld> PathNodes; // the path of Nodes defined for the current car
    public float RotSpeed; // speed that cars will rotate to face parking spots
    public float SpeedCap; // cap for car speed


    // Use this for initialization
    private void Start()
    {
        _moving = false;
    }


    // Initialize static structures within Car. Should be called on scene load
    public static void Initialize()
    {
        // populates CarPrefabPaths with paths(relative to Resources) to all available car prefabs
        CarPrefabPaths = new List<string>();
        CarPrefabPaths.Add("Prefabs/Cars/GreenCar");
        CarPrefabPaths.Add("Prefabs/Cars/OrangeCar");
        CarPrefabPaths.Add("Prefabs/Cars/RedCar");
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_moving)
        {
            //Every loop should repath the car if neccessary
            PathCar();
            //This acceleartion need to be calculated based on where the car is relative to the Node it's approaching. The car should be slowing down or speeding up depending on it's postion. 
            float newAccel;
            //Then add that acceleartion to the cars old speed, and cap at 0 or the max. 
            //CarSpeed = CarSpeed + 

            if (CarSpeed < 0) CarSpeed = 0;

            // Apply acceleration
            CarSpeed += Acceleration * Time.fixedTime;

            if (CarSpeed > SpeedCap) CarSpeed = SpeedCap;

            // Cast a ray in front of this object
            RaycastHit hit;
            Physics.Raycast(Front.position, Front.forward, out hit, CastDistance);

            // If there was a hit
            if (hit.point != Vector3.zero && (hit.collider.gameObject.tag == "Car" || hit.collider.gameObject.tag == "AngryCar"))
            {
                var dist = Vector3.Distance(Front.position, hit.point);

                var unitDistance = dist / CastDistance;

                CarSpeed = Mathf.Lerp(CarSpeed, 0, 1 - unitDistance);
            }

            if (transform.position != PathNodes[_nodeCounter].transform.position)
            {
                var pos = Vector3.MoveTowards(transform.position, PathNodes[_nodeCounter].transform.position, CarSpeed * Time.deltaTime);
                var newRot = Vector3.RotateTowards(transform.forward, PathNodes[_nodeCounter].transform.position - transform.position, RotSpeed * Time.deltaTime, 0.0f);
                Debug.DrawRay(transform.position, newRot, Color.red);
                GetComponent<Rigidbody>().MovePosition(pos);
                transform.rotation = Quaternion.LookRotation(newRot);
            }
            else if (_nodeCounter + 1 == PathNodes.Count)
            {
                _lastNodeOld = (ParkingSpotNodeOld) PathNodes[_nodeCounter];
                PathNodes = new List<NodeOld>();
                _moving = false;
                var destStop = _lastNodeOld.GetComponent<ParkingSpotNodeOld>();
                if (destStop != null) destStop.SpotTaken();
            }
            else
            {
                _nodeCounter++;
            }
        }
    }

    //Getter for next node in path
    public virtual NodeOld GetNextNode()
    {
        if (PathNodes.Count == 0) return _lastNodeOld;
        return PathNodes[_nodeCounter];
    }

    //Returns Destination Node
    public virtual ParkingSpotNodeOld GetDestNode()
    {
        //If pathnodes isn't empty
        if (PathNodes.Count != 0)
        {
            if (PathNodes[PathNodes.Count - 1] != null)
                return PathNodes[PathNodes.Count - 1] as ParkingSpotNodeOld;
            return _lastNodeOld;
        }

        return null;
    }

    //Sets cars path
    public virtual void SetPath(List<NodeOld> inputList)
    {
        //Set the new set of pathnodes to the given input
        PathNodes = inputList;
        //Set the iterator through pathnodes to 0
        _nodeCounter = 0;
        //set moving to true to get the car moving through the pathnodes in the FixedUpdate() Loop
        _moving = true;
        Debug.Log(gameObject.name);
        if (_lastNodeOld.GetComponent<ParkingSpotNodeOld>() != null)
        {
            //Get the last node, which was the parking spot the car was parked in
            var psn = _lastNodeOld.GetComponent<ParkingSpotNodeOld>();
            //Set that parking spot to open.
            psn.SpotOpened();
        }
    }

    //Sets last node (for initilization purposes)
    internal void SetLastNode(ParkingSpotNodeOld parkingSpotNodeOld)
    {
        _lastNodeOld = parkingSpotNodeOld;
    }

    // TODO: Implement AngryCar pathing within AngryCar as static methods
    // Note that this should be designed such that we can have multiple "AngryCar" GameObjects

    //Checks if car needs to repathed
    public void PathCar()
    {
        var carDest = GetDestNode();
        //if destination Parking Spot Node has changed to Occupied, need to find a new destination node. 
        if (carDest.GetIsOccupied())
            CalcCarPath();
    }

    // recalculates the angrycar's path to be the empty spot nearest to the target
    public void CalcCarPath()
    {
        //create parking spot destination
        var parkingSpotDest = NodeOld.GetNodeObjects()[0].GetComponent<NodeOld>();

        // find empty parking spots and determine the spot closest to the target
        var minDist = float.MaxValue;
        //This trusts that each gameobject we're getting from GetOpenSpots() is a open ParkingSpotNode;
        foreach (var x in ParkingSpotNodeOld.GetOpenSpots())
            if (Vector3.Distance(GameManager.Target.transform.position, x.transform.position) < minDist)
            {
                // set the parking spot as the destination for the car
                parkingSpotDest = x.GetComponent<ParkingSpotNodeOld>();
                //set MinDist for further checks
                minDist = Vector3.Distance(GameManager.Target.transform.position, x.transform.position);
            }

        // set the path for the car
        SetPath(GetNextNode().FindShortestPath(parkingSpotDest.GetComponent<NodeOld>()));
    }
}