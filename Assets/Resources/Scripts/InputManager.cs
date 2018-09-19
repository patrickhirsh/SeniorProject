using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool InputDebugMode;

    bool CarSelected;                   // indicates a car is currently selected. Waiting for a dest. click for pathing...
    Car CurrentCar;                     // when CarSelected == true, this is the currently selected car


    // Use this for initialization
    void Awake()
    {
        InputDebugMode = true;
        CarSelected = false;            // reset CarSelected state       
    }


    // Update is called once per frame
    void Update()
    {
        HandleInput();
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
}