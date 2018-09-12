using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control: MonoBehaviour
{
    bool CarSelected;

    Car CurrentCar;

    // Use this for initialization
    void Start()
    {
        CarSelected = false;

    }

    // Update is called once per frame
    void Update()
    {
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
                else if(hitInfo.transform.gameObject.tag == "AngryCar")
                {
                    //Do Nothing
                }
                else if(hitInfo.transform.gameObject.tag == "Node")
                {
                    Debug.Log("Hit Node");
                    if (CarSelected)
                    {
                        Debug.Log("Pathing to car");
                        Node NewNode = CurrentCar.GetDestNode();
                        CurrentCar.SetPath(NewNode.FindShortestPath(hitInfo.transform.gameObject));
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
        foreach(Node x in Node.NodeObjects)
        {

        }
    }
}