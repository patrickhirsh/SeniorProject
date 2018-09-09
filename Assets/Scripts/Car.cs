using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

    public List<Node> PathNodes;

    int NodeCounter = 0;

    public float CarSpeed;

    bool moving;

    public float SpeedCap;

    // Use this for initialization
    void Start ()
    {
        moving = true;
	}
	
	// Update is called once per frame
	void Update ()
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
                Vector3 pos = Vector3.MoveTowards(transform.position, PathNodes[NodeCounter].transform.position, CarSpeed * Time.deltaTime);
                GetComponent<Rigidbody>().MovePosition(pos);
            }
            else if (NodeCounter + 1 == PathNodes.Count)
            {
                
                moving = false;
            }
            else
            {
                NodeCounter++;
            }

        }
	}
}
