using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    
    public GameObject target;   // the point at which AngryCar will try to park closest to
    public Car AngryCar;
    public bool InputDebugMode;

    bool CarSelected;           // indicates a car is currently selected. Waiting for a dest. click for pathing...
    Car CurrentCar;             // when CarSelected == true, this is the currently selected car

    // Use this for initialization
    void Awake()
    {
        InputDebugMode = false;
        CarSelected = false;        // reset CarSelected state
        Node.InitializeNodes();     // initialize static node structures
        //CalcAngryCarPath();         // determine AngryCar's initial path on startup        
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        //Check and path angry car as needed
        //PathAngryCar();
    }


    private void HandleInput()
    {
        // Handle MouseDown
        if (Input.GetMouseButtonDown(0))
        {
            // determine click location
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

            // if the click was valid...
            if (hit)
            {
                if (InputDebugMode) { Debug.Log("Hit " + hitInfo.transform.gameObject.name); }
                switch (hitInfo.transform.gameObject.tag)
                {
                    case "Car":
                        CurrentCar = hitInfo.transform.gameObject.GetComponent<Car>();
                        CarSelected = true;
                        break;

                    case "AngryCar":
                        break;

                    case "Node":
                        if (CarSelected)
                        {
                            if (InputDebugMode) { Debug.Log("Pathing to Node"); }
                            Node NewNode = CurrentCar.GetNextNode();
                            CurrentCar.SetPath(NewNode.FindShortestPath(hitInfo.transform.gameObject.GetComponent<Node>()));
                            CarSelected = false;
                        }
                        break;

                    case "Parking Spot":
                        if (CarSelected)
                        {
                            if (InputDebugMode) { Debug.Log("Pathing to ParkingSpotNode"); }
                            Node NewNode = CurrentCar.GetNextNode();
                            CurrentCar.SetPath(NewNode.FindShortestPath(hitInfo.transform.gameObject.GetComponent<Node>()));
                            CarSelected = false;
                        }
                        break;

                    default:
                        if (InputDebugMode) { Debug.Log("No hit"); }
                        CarSelected = false;
                        break;
                }
            }

            else
            {
                Debug.Log("No hit");
                CarSelected = false;
            }
        }
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