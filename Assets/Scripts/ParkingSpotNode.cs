using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingSpotNode : Node {

    public bool IsOccupied;

    private static List<GameObject> OpenSpots;
    private static List<GameObject> TakenSpots;

	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // Initializes private static parking spot node structures. Should be called upon loading a scene
    public static void InitializeParkingSpotNodes(int NumFreeSpots)
    {
        // start by obtaining all parking spots from NodeObjects
        List<GameObject> NodeObjects = Node.GetNodeObjects();

        // failsafe to ensure we don't initialize parking spot nodes prior to calling Node.InitializeNodes()
        if (NodeObjects == null)
        {
            Node.InitializeNodes();
            NodeObjects = Node.GetNodeObjects();
        }

        // construct the underlying parking lot node structures
        OpenSpots = new List<GameObject>();
        TakenSpots = new List<GameObject>();

        // all spots are "taken" by default
        foreach (GameObject Node in NodeObjects)
            if (Node.GetComponent<ParkingSpotNode>() != null)
            {
                Node.GetComponent<ParkingSpotNode>().IsOccupied = true;
                TakenSpots.Add(Node);
            }

        // select random spots to leave open
        List<int> indeces = new List<int>();

        for (int i = 0; i < NumFreeSpots; i++)
        {
            int index = UnityEngine.Random.Range(0, TakenSpots.Count);
            int trials = 0;
            while(!ValidateOpenSpot(TakenSpots[index]))
            {
                index = UnityEngine.Random.Range(0, TakenSpots.Count);
                if (trials > 50) { throw new Exception("ValidateOpenSpot() rejected 50+ parking spots"); }
                trials++;
            }

        }
    }

    // Given a parking spot game object, returns true if the parking spot is at least
    // "MinAcceptableDistance" away from the AngryCar. Returns false otherwise.
    private static bool ValidateOpenSpot(GameObject ParkingSpotNode)
    {
        if (ParkingSpotNode.GetComponent<ParkingSpotNode>() == null)
            throw new Exception("ValidateOpenSpot() was given an invalid GameObject");

        float MinAcceptableDistance = 10;
        float Distance = Vector3.Distance(ParkingSpotNode.transform.position, GameManager.ACar.transform.position);
        return Distance <= MinAcceptableDistance;
    }
}
