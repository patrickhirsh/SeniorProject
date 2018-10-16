using System.Collections.Generic;
using UnityEngine;

public class AngryCar : Car
{
    //Last entity parking spot
    private ParkingSpotNodeOld _lastNodeOld;

    //bool for movement
    private bool _moving;

    // TODO: Make AngryCar inherit from Car, and refactor this code to contain all of (any) angry cars' functionality
    //Int for iterating through pathnodes
    private int _nodeCounter;

    public GameObject Indicator;

    // Use this for initialization
    private void Start()
    {
        _moving = false;
        _lastNodeOld = GameManager.StartNodeOld;
        CalcCarPath();
    }

    //Get destination parking spot entity
    public override ParkingSpotNodeOld GetDestNode()
    {
        if (PathNodes.Count == 0) return _lastNodeOld;
        return (ParkingSpotNodeOld) PathNodes[PathNodes.Count - 1];
    }

    //Get next entity in pathnodes
    public override NodeOld GetNextNode()
    {
        if (PathNodes.Count == 0) return _lastNodeOld;
        return PathNodes[_nodeCounter];
    }

    //Fixed Update is called on a fixed interval
    private void FixedUpdate()
    {
        PathCar();

        if (_moving)
        {
            //This acceleartion need to be calculated based on where the car is relative to the entity it's approaching. The car should be slowing down or speeding up depending on it's postion. 
            float newAccel;
            //Then add that acceleartion to the cars old speed, and cap at 0 or the max. 
            //CarSpeed = CarSpeed + 

            if (CarSpeed < 0) CarSpeed = 0;
            if (CarSpeed > SpeedCap) CarSpeed = SpeedCap;
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
            }
            else
            {
                _nodeCounter++;
            }
        }
    }

    public override void SetPath(List<NodeOld> inputList)
    {
        //Set the new set of pathnodes to the given input
        PathNodes = inputList;
        //Set the iterator through pathnodes to 0
        _nodeCounter = 0;
        //set moving to true to get the car moving through the pathnodes in the FixedUpdate() Loop
        _moving = true;

        var transformF = PathNodes[PathNodes.Count - 1].transform;

        Indicator.transform.position = transformF.position;
    }
    ////Checks if car needs to repathed
    //private void PathCar()
    //{
    //    ParkingSpotNode CarDest = this.GetDestNode();
    //    //if destination Parking Spot Entity has changed to Occupied, need to find a new destination entity. 
    //    if (CarDest.GetIsOccupied() == true)
    //        CalcCarPath();
    //}

    //// recalculates the angrycar's path to be the empty spot nearest to the target
    //private void CalcCarPath()
    //{
    //    //create parking spot destination
    //    Entity ParkingSpotDest = GameManager.StartNode;

    //    // find empty parking spots and determine the spot closest to the target
    //    float MinDist = float.MaxValue;
    //    //This trusts that each gameobject we're getting from GetOpenSpots() is a open ParkingSpotNode;
    //    foreach (GameObject x in ParkingSpotNode.GetOpenSpots())
    //    {
    //        if (Vector3.Distance(GameManager.Target.transform.position, x.transform.position) < MinDist)
    //        {
    //            // set the parking spot as the destination for the car
    //            ParkingSpotDest = x.GetComponent<ParkingSpotNode>();
    //            //set MinDist for further checks
    //            MinDist = Vector3.Distance(GameManager.Target.transform.position, x.transform.position);
    //        }
    //    }


    //    Entity foo = this.GetNextNode();
    //    // set the path for the car
    //    this.SetPath(this.GetNextNode().FindShortestPath(ParkingSpotDest.GetComponent<ParkingSpotNode>()));
    //}
}