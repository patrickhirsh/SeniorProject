using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlS : MonoBehaviour
{
    bool CarSelected;

    CarScript CurrentCar;

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
                    if (CarSelected)
                    {
                        CurrentCar = 
                    }
                }
                else
                {
                    Debug.Log("nopz");
                }
            }
            else
            {
                Debug.Log("No hit");
            }
            Debug.Log("Mouse is down");
        }
    }
}