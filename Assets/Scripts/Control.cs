using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    bool CarSelected;

    public GameObject target;

    public Car AngryCar;

    Car CurrentCar;

    // Use this for initialization
    void Start()
    {
        //No cars selected on startup
        CarSelected = false;
        //Angry car needs to be set to find it's path on startup
        //CalcAngryCarPath();

    }

    // Update is called once per frame
    void Update()
    {
        //get input to direct cars
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse is down");

            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                Debug.Log("Hit " + hitInfo.transform.gameObject.name);
                if (hitInfo.transform.gameObject.tag == "Car")
                {
                    CurrentCar = hitInfo.transform.gameObject.GetComponent<Car>();
                    CarSelected = true;
                }
                else if (hitInfo.transform.gameObject.tag == "AngryCar")
                {
                    //Do Nothing
                }
                else if (hitInfo.transform.gameObject.tag == "Node")
                {
                    Debug.Log("Hit Node");
                    if (CarSelected)
                    {
                        Debug.Log("Pathing to car");
                        Node NewNode = CurrentCar.GetNextNode();
                        CurrentCar.SetPath(NewNode.FindShortestPath(hitInfo.transform.gameObject.GetComponent<Node>()));
                        //route car to node somehow
                        CarSelected = false;
                    }
                    else
                    {
                        //Do Nothing
                    }
                }
                else if (hitInfo.transform.gameObject.tag == "Parking Spot")
                {
                    Debug.Log("Hit Parking Spot");
                    if (CarSelected)
                    {
                        Debug.Log("Pathing to car");
                        Node NewNode = CurrentCar.GetNextNode();
                        CurrentCar.SetPath(NewNode.FindShortestPath(hitInfo.transform.gameObject.GetComponent<Node>()));
                        //route car to node somehow
                        CarSelected = false;
                    }
                    else
                    {
                        //Do Nothing
                    }

                }
                //Allow deselection of cars
                else
                {
                    CarSelected = false;
                    Debug.Log("Car Deselected");
                }
            }
            else
            {
                Debug.Log("No hit");
                CarSelected = false;
            }
        }
        //Check and path angry car as needed
        //PathAngryCar();
    }

    private void PathAngryCar()
    {
        //get current destination of angrycar
        Node acardest = AngryCar.GetDestNode();
        //if the angry cars destination node is no longer active, set a new one
        if (!acardest.gameObject.activeSelf)  // does this work?
        {
            CalcAngryCarPath();
        }
    }

    private void CalcAngryCarPath()
    {
        //Set mindist to max
        float MinDist = float.MaxValue;
        //Create parking spot Destination
        Node parkingSpotDest = Node.GetNodeObjects()[0].GetComponent<Node>();
        //Foreach node
        foreach (GameObject x in Node.GetNodeObjects())
        {
            //If it's a parking spot
            if (x.tag == "Parking Spot")
            {
                //If the parking spot in question's distance to the target is less than the current parkingspotest
                if (Vector3.Distance(target.transform.position, x.transform.position) < MinDist)
                {
                    //set currently evaluted parking spot as the destination for the angry car
                    parkingSpotDest = x.GetComponent<Node>();
                    MinDist = Vector3.Distance(target.transform.position, x.transform.position);
                }
            }
        }
        //Set the path for the angry car
        AngryCar.SetPath(AngryCar.GetNextNode().FindShortestPath(parkingSpotDest.transform.gameObject.GetComponent<Node>()));
    }
}