using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public bool InputDebugMode;

    bool CarSelected;                   // indicates a car is currently selected. Waiting for a dest. click for pathing...
    Car CurrentCar;                     // when CarSelected == true, this is the currently selected car


    List<Node> newpath;
    GameObject lastNodeNir;

    List<LineRenderer> lrList;

    bool drawBool;

    // Use this for initialization
    void Awake()
    {
        InputDebugMode = true;
        CarSelected = false;            // reset CarSelected state       
        lrList = new List<LineRenderer>();
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
                //    // determine the object we've selected and act accordingly
                //    if (InputDebugMode) { Debug.Log("Hit " + hitInfo.transform.gameObject.name); }
                //    switch (hitInfo.transform.gameObject.tag)
                //    {
                //        case "Car":
                //            CurrentCar = hitInfo.transform.gameObject.GetComponent<Car>();
                //            CarSelected = true;
                //            break;

                //        case "AngryCar":
                //            break;

                //        case "Node":
                //            if (CarSelected)
                //            {
                //                if (InputDebugMode) { Debug.Log("Pathing to Node"); }
                //                Node NewNode = CurrentCar.GetNextNode();
                //                CurrentCar.SetPath(NewNode.FindShortestPath(hitInfo.transform.gameObject.GetComponent<Node>()));
                //                CarSelected = false;
                //            }
                //            break;

                //        case "Parking Spot":
                //            if (CarSelected)
                //            {
                //                if (InputDebugMode) { Debug.Log("Pathing to ParkingSpotNode"); }
                //                Node NewNode = CurrentCar.GetNextNode();
                //                CurrentCar.SetPath(NewNode.FindShortestPath(hitInfo.transform.gameObject.GetComponent<Node>()));
                //                CarSelected = false;
                //            }
                //            break;

                //        default:
                //            if (InputDebugMode) { Debug.Log("No hit"); }
                //            CarSelected = false;
                //            break;
                //    }
                //}

                //else
                //{
                //    Debug.Log("No hit");
                //    CarSelected = false;
                if(hitInfo.transform.gameObject.tag == "Car")
                {
                    Debug.Log("hit a car");
                    CurrentCar = hitInfo.transform.gameObject.GetComponent<Car>();
                    CarSelected = true;
                    newpath = new List<Node>();
                    lastNodeNir = hitInfo.transform.gameObject.GetComponent<Car>().LastNode.gameObject;
                    drawBool = false;
                }



            }
        }

        if (Input.GetMouseButton(0) && CarSelected)
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if(hitInfo.transform.gameObject.tag == "Node")
            {


                if (hitInfo.transform.gameObject != lastNodeNir && lastNodeNir.GetComponent<Node>().Connections.Contains(hitInfo.transform.gameObject.GetComponent<Node>()))
                {
                    if(drawBool)
                    { 
                        var line = hitInfo.transform.gameObject.AddComponent<LineRenderer>();
                        lrList.Add(line);

                        line.sortingLayerName = "OnTop";
                        line.sortingOrder = 5;
                        line.positionCount = 2;
                        line.SetPosition(0, lastNodeNir.transform.position);
                        line.SetPosition(1, hitInfo.transform.gameObject.transform.position);
                        line.startWidth = .5f;
                        line.endWidth = .6f;
                        line.useWorldSpace = true;
                    }
                    Debug.Log("hit a node");
                    lastNodeNir = hitInfo.transform.gameObject;
                    drawBool = true;
                    newpath.Add(hitInfo.transform.gameObject.GetComponent<Node>());
                }
            }
            if (hitInfo.transform.gameObject.tag == "Parking Spot")
            {


                if (hitInfo.transform.gameObject != lastNodeNir && lastNodeNir.GetComponent<Node>().Connections.Contains(hitInfo.transform.gameObject.GetComponent<Node>()))
                {
                    if (drawBool)
                    {
                        var line = hitInfo.transform.gameObject.AddComponent<LineRenderer>();
                        lrList.Add(line);

                        line.sortingLayerName = "OnTop";
                        line.sortingOrder = 7;
                        line.positionCount = 2;
                        line.SetPosition(0, lastNodeNir.transform.position);
                        line.SetPosition(1, hitInfo.transform.gameObject.transform.position);
                        line.startWidth = .5f;
                        line.endWidth = .6f;
                        line.useWorldSpace = true;
                    }
                    Debug.Log("hit a node");
                    lastNodeNir = hitInfo.transform.gameObject;
                    newpath.Add(hitInfo.transform.gameObject.GetComponent<Node>());
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("released mouse button");

            CurrentCar.SetPath(newpath);
        }
    }
}