using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingSpotNode : Node {

    private bool IsOccupied;

    private static List<GameObject> OpenSpots;      // stores GameObjects of all currently non-occupied parking spots
    private static List<GameObject> TakenSpots;     // stores GameObjects of all currently occupied parking spots
    private static GameObject CarsFolder;           // a reference to the "Cars" Object in the unity editor. Set all cars as children of this for orginized instantiation

	// Use this for initialization
	void Start ()
    {
        
    }

    // Update is called once per frame
    void Update ()
    {
		
	}


    // Initialize static structures within ParkingSpotNode. Should be called on scene load
    // Populates all parking spots that are occupied on game start with cars and performs necessary underlying opperations
    // NumFreeSpots determines the number of free spots that will be generated during initialization
    public static void Initialize(int NumFreeSpots)
    {
        // Obtain the "Cars" GameObject
        CarsFolder = GameObject.Find("Cars");

        // Obtain all parking spots from NodeObjects
        List<GameObject> NodeObjects = Node.GetNodeObjects();

        // Node.Initialize() needs to be called first to populate Node.NodeObjects
        if (NodeObjects == null)
        {
            Debug.Log("WARNING: ParkingSpotNode.Initialize() was called after Node.Initialize()");
            Node.Initialize();
            NodeObjects = Node.GetNodeObjects();
        }

        // Car.Initialize needs to be called first to populate Car.CarPrefabPaths
        if (Car.CarPrefabPaths.Count == 0)
        {
            Debug.Log("WARNING: ParkingSpotNode.Initialize() was called after Car.Initialize() (or CarPrefabPaths is empty)");
            Car.Initialize();
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

            // add the validated spot to OpenSpots and mark as vacant
            OpenSpots.Add(TakenSpots[index]);
            TakenSpots[index].GetComponent<ParkingSpotNode>().IsOccupied = false;           
        }

        // remove all vacant parking spots from TakenSpots
        TakenSpots.RemoveAll(x => x.GetComponent<ParkingSpotNode>().IsOccupied == false);

        // add cars to all taken spots
        foreach (GameObject ParkingSpot in TakenSpots)
        {
            // select a random car prefab of the available prefabs and instantiate it
            int PrefabIndex = UnityEngine.Random.Range(0, Car.CarPrefabPaths.Count);
            GameObject car = Instantiate(Resources.Load(Car.CarPrefabPaths[PrefabIndex])) as GameObject;

            // set the new object's position to it's parking spot, set it's origin pathnode, and make it a child of "Cars" (for orginization)
            car.transform.position = new Vector3(ParkingSpot.transform.position.x, ParkingSpot.transform.position.y, ParkingSpot.transform.position.z);      
            car.GetComponent<Car>().PathNodes.Add(ParkingSpot.GetComponent<Node>());
            car.transform.parent = CarsFolder.transform;
            car.GetComponent<Car>().SetLastNode(ParkingSpot.GetComponent<Node>());
        }
    }


    public static List<GameObject> GetOpenSpots()
    {
        return OpenSpots;
    }

    public static List<GameObject> GetTakenSpots()
    {
        return TakenSpots;
    }

    // calling SpotTaken() on a ParkingSpotNode indicates that (this) parking spot was just filled
    public void SpotTaken()
    {
        for (int i = 0; i < OpenSpots.Count; i++)
        {
            if (OpenSpots[i].GetComponent<Node>().GetNodeID() == this.GetNodeID())
            {
                OpenSpots[i].GetComponent<ParkingSpotNode>().IsOccupied = true;
                TakenSpots.Add(OpenSpots[i]);
                OpenSpots.RemoveAt(i);
                break;
            }
        }
    }

    // calling SpotOpened() on a ParkingSpotNode indicates that (this) parking spot was just vacated
    public void SpotOpened()
    {
        for (int i = 0; i < TakenSpots.Count; i++)
        {
            if (TakenSpots[i].GetComponent<Node>().GetNodeID() == this.GetNodeID())
            {
                TakenSpots[i].GetComponent<ParkingSpotNode>().IsOccupied = false;
                OpenSpots.Add(TakenSpots[i]);
                TakenSpots.RemoveAt(i);
                break;
            }
        }
    }


    public bool GetIsOccupied()
    {
        return IsOccupied;
    }


    // Given a parking spot game object, returns true if the parking spot is at least
    // "MinAcceptableDistance" away from the AngryCar. Returns false otherwise.
    private static bool ValidateOpenSpot(GameObject ParkingSpotNode)
    {
        float MinAcceptableDistance = 10;
        if (ParkingSpotNode.GetComponent<ParkingSpotNode>() == null)
            throw new Exception("ValidateOpenSpot() was given an invalid GameObject");

        // only consider AngryCar proximity if there is currently one in the scene
        if (GameManager.ACar != null)
        {
            float Distance = Vector3.Distance(ParkingSpotNode.transform.position, GameManager.ACar.transform.position);
            return Distance > MinAcceptableDistance;
        }

        // otherwise, accept all locations
        else
        {
            Debug.Log("WARNING: Could not locate AngryCar");
            return true;
        }
    }
}
