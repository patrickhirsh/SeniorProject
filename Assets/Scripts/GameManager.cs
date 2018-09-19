using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameObject Target;   // need to swap all references to Control.Target with this one
    public static AngryCar ACar;

	// Use this for initialization
	void Start ()
    {
        // initialize static structures for multi-object classes
        Node.InitializeNodes();                         
        ParkingSpotNode.InitializeParkingSpotNodes(3);

        // obtain a reference to AngryCar
        ACar = FindObjectOfType<AngryCar>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
