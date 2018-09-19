using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameObject Target;   // need to swap all references to Control.Target with this one
    public static AngryCar ACar;

	// Use this for initialization
	void Start ()
    {
        // obtain a reference to AngryCar
        ACar = FindObjectOfType<AngryCar>();

        // initialize static structures for non-singleton classes
        Car.Initialize();
        Node.Initialize();
        ParkingSpotNode.Initialize(3);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    
}
