using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

	//List of nodes to path to
    public List<Node> PathNodes;
    //Iterator through pathnodes
    int NodeCounter = 0;
    //Carspeed vairable
    public float CarSpeed;
    //Whether the car is currently pathing
    bool moving;
    //Cap for car speed
    public float SpeedCap;
    //Speed that cars will rotate to face parking spots
    public float RotSpeed;
    //Node that the car ended it's pathing in. 
    Node LastNode;
    //TODO: set the LastNode to the car's spawn point on initialization
    // this is the cause of the NullReferenceException prior to pathing

    public Node GetNextNode()
    {
        if(PathNodes.Count == 0)
        {
            return LastNode;
        }
        return PathNodes[NodeCounter];
    }

    public Node GetDestNode()
    {
        if (PathNodes.Count != 0)
        {
            if (PathNodes[PathNodes.Count - 1] != null)
                return PathNodes[PathNodes.Count - 1];
            else
                return LastNode;
        }
        else
        {
            return null;
        }
    }

    public void SetPath(List<Node> InputList)
    {
        PathNodes = InputList;
        NodeCounter = 0;
        moving = true;
        if (LastNode.GetComponent<ParkingSpotNode>() != null)
        {
            ParkingSpotNode psn = LastNode.GetComponent<ParkingSpotNode>();
            psn.IsOccupied = false;
        }
    }

    // Use this for initialization
    void Start ()
    {
        moving = false;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (moving)
        {
            //This acceleartion need to be calculated based on where the car is relative to the node it's approaching. The car should be slowing down or speeding up depending on it's postion. 
            float NewAccel;
            //Then add that acceleartion to the cars old speed, and cap at 0 or the max. 
            //CarSpeed = CarSpeed + 

            if (CarSpeed < 0)
            {
                CarSpeed = 0;
            }
            if(CarSpeed > SpeedCap)
            {
                CarSpeed = SpeedCap;
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
                LastNode = PathNodes[NodeCounter];
                PathNodes = new List<Node>();
                moving = false;
                ParkingSpotNode DestStop = LastNode.GetComponent<ParkingSpotNode>();
                if(DestStop != null)
                {
                    DestStop.IsOccupied = true;
                }
            }
            else
            {
                NodeCounter++;
            }

        }
	}
}
