using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngryCar : Car
{



    // TODO: Make AngryCar inherit from Car, and refactor this code to contain all of (any) angry cars' functionality



    //public List<Node> PathNodes;

    int NodeCounter = 0;

    //public float CarSpeed;

    bool moving;

    //public float SpeedCap;

    //public float RotSpeed;

    Node LastNode;

    public AngryCar()
    {
    }

    //public Node GetDestNode()
    //{
    //    if (PathNodes.Count == 0)
    //    {
    //        return LastNode;
    //    }
    //    return PathNodes[NodeCounter];
    //}

    //public void SetPath(List<Node> InputList)
    //{
    //    PathNodes = InputList;
    //    NodeCounter = 0;
    //    moving = true;
    //}

    // Use this for initialization
    void Start()
    {
        moving = false;
        //CalcAngryCarPath();         // determine AngryCar's initial path on startup 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PathCar();

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
            if (CarSpeed > SpeedCap)
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
            }
            else
            {
                NodeCounter++;
            }

        }
        
    }

    private void PathCar()
    {
        throw new NotImplementedException();
    }
}
