using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

    public static List<string> CarPrefabPaths;      // stores the paths of all available car prefabs for spawning

    public List<Node> PathNodes;                    // the path of Nodes defined for the current car
    public float CarSpeed;                          // carspeed vairable
    public float Acceleration = 5.0f;               // acceleration rate
    public float SpeedCap;                          // cap for car speed
    public float RotSpeed;                          // speed that cars will rotate to face parking spots
    public Transform Front;                         // where to cast from
    public float CastDistance = 1.0f;               // how far to cast

    private int NodeCounter = 0;                    // indicates the current Node in PathNodes
    private bool moving;                            // whether the car is currently pathing
    private ParkingSpotNode LastNode;                          // Node that the car ended it's pathing in. 
    
  
    public Car()
    {

    }


    // Use this for initialization
    void Start ()
    {
        moving = false;
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
    void FixedUpdate ()
    {
        if (moving)
        {
            //Every loop should repath the car if neccessary
            PathCar();
            //This acceleartion need to be calculated based on where the car is relative to the Node it's approaching. The car should be slowing down or speeding up depending on it's postion. 
            float NewAccel;
            //Then add that acceleartion to the cars old speed, and cap at 0 or the max. 
            //CarSpeed = CarSpeed + 

            if (CarSpeed < 0)
            {
                CarSpeed = 0;
            }

            // Apply acceleration
            CarSpeed += Acceleration * Time.fixedTime;

            if(CarSpeed > SpeedCap)
            {
                CarSpeed = SpeedCap;
            }

            // Cast a ray in front of this object
            RaycastHit hit;
            Physics.Raycast(Front.position, Front.forward, out hit, CastDistance);

            // If there was a hit
            if (hit.point != Vector3.zero && (hit.collider.gameObject.tag == "Car" || hit.collider.gameObject.tag == "AngryCar"))
            {
                float dist = Vector3.Distance(Front.position, hit.point);

                float unitDistance = dist / CastDistance;

                CarSpeed = Mathf.Lerp(CarSpeed, 0, 1 - unitDistance);
            }

            if (transform.position != PathNodes[NodeCounter].transform.position)
            {
                Vector3 Pos = Vector3.MoveTowards(transform.position, PathNodes[NodeCounter].transform.position, CarSpeed * Time.deltaTime);
                Vector3 NewRot = Vector3.RotateTowards(transform.forward, PathNodes[NodeCounter].transform.position - transform.position, RotSpeed * Time.deltaTime, 0.0f);
                Debug.DrawRay(transform.position, NewRot, Color.red);
                GetComponent<Rigidbody>().MovePosition(Pos);
                transform.rotation = Quaternion.LookRotation(NewRot);
            }
            else if (NodeCounter + 1 == PathNodes.Count)
            {
                LastNode = (ParkingSpotNode) PathNodes[NodeCounter];
                PathNodes = new List<Node>();
                moving = false;
                ParkingSpotNode DestStop = LastNode.GetComponent<ParkingSpotNode>();
                if(DestStop != null)
                {
                    DestStop.SpotTaken();
                }
            }
            else
            {
                NodeCounter++;
            }
        }
	}

    //Getter for next node in path
    public virtual Node GetNextNode()
    {
        if (PathNodes.Count == 0)
        {
            return LastNode;
        }
        return PathNodes[NodeCounter];
    }

    //Returns Destination Node
    public virtual ParkingSpotNode GetDestNode()
    {
        //If pathnodes isn't empty
        if (PathNodes.Count != 0)
        {
            if (PathNodes[PathNodes.Count - 1] != null)
                return PathNodes[PathNodes.Count - 1] as ParkingSpotNode;
            else
                return LastNode as ParkingSpotNode;
        }
        else
        {
            return null;
        }
    }

    //Sets cars path
    public virtual void SetPath(List<Node> InputList)
    {
        //Set the new set of pathnodes to the given input
        PathNodes = InputList;
        //Set the iterator through pathnodes to 0
        NodeCounter = 0;
        //set moving to true to get the car moving through the pathnodes in the FixedUpdate() Loop
        moving = true;
        Debug.Log(this.gameObject.name);
        if (LastNode.GetComponent<ParkingSpotNode>() != null)
        {
            //Get the last node, which was the parking spot the car was parked in
            ParkingSpotNode psn = LastNode.GetComponent<ParkingSpotNode>();
            //Set that parking spot to open.
            psn.SpotOpened();  
        }
    }

    //Sets last node (for initilization purposes)
    internal void SetLastNode(ParkingSpotNode parkingSpotNode)
    {
        LastNode = parkingSpotNode;
    }

    // TODO: Implement AngryCar pathing within AngryCar as static methods
    // Note that this should be designed such that we can have multiple "AngryCar" GameObjects

    //Checks if car needs to repathed
    public void PathCar()
    {
        ParkingSpotNode CarDest = this.GetDestNode();
        //if destination Parking Spot Node has changed to Occupied, need to find a new destination node. 
        if (CarDest.GetIsOccupied() == true)
           CalcCarPath();
    }

    // recalculates the angrycar's path to be the empty spot nearest to the target
    public void CalcCarPath()
    {
        //create parking spot destination
        Node ParkingSpotDest = Node.GetNodeObjects()[0].GetComponent<Node>();

        // find empty parking spots and determine the spot closest to the target
        float MinDist = float.MaxValue;
        //This trusts that each gameobject we're getting from GetOpenSpots() is a open ParkingSpotNode;
        foreach(GameObject x in ParkingSpotNode.GetOpenSpots())
        {
                if (Vector3.Distance(GameManager.Target.transform.position, x.transform.position) < MinDist)
                {
                    // set the parking spot as the destination for the car
                    ParkingSpotDest = x.GetComponent<ParkingSpotNode>();
                    //set MinDist for further checks
                    MinDist = Vector3.Distance(GameManager.Target.transform.position, x.transform.position);
                }
        }

        // set the path for the car
        this.SetPath(this.GetNextNode().FindShortestPath(ParkingSpotDest.GetComponent<Node>()));
    }
}
