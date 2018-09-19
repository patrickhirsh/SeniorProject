using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
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
                // determine the object we've selected and act accordingly
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

    // checks if AngryCar's destination is still vacant. Should be ran every update
    private void PathAngryCar()
    {
        Node acardest = AngryCar.GetDestNode();
        if (!acardest.gameObject.activeSelf)
            CalcAngryCarPath();
    }

    // recalculates the AngryCar's path to be the empty spot nearest to the target
    private void CalcAngryCarPath()
    {
        //Create parking spot Destination
        Node parkingSpotDest = Node.GetNodeObjects()[0].GetComponent<Node>();

        // find empty parking spots and determine the spot closest to the target
        float MinDist = float.MaxValue;
        foreach (GameObject x in Node.GetNodeObjects())
        {
            if ((x.tag == "Parking Spot") && !(x.GetComponent<ParkingSpotNode>().IsOccupied))
            {
                if (Vector3.Distance(target.transform.position, x.transform.position) < MinDist)
                {
                    // set the parking spot as the destination for the angry car
                    parkingSpotDest = x.GetComponent<Node>();
                    MinDist = Vector3.Distance(target.transform.position, x.transform.position);
                }
            }
        }

        // set the path for the angry car
        AngryCar.SetPath(AngryCar.GetNextNode().FindShortestPath(parkingSpotDest.transform.gameObject.GetComponent<Node>()));
    }
}