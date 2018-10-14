using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ParkingSpotNodeOld : NodeOld
{
    private static List<GameObject> _openSpots; // stores GameObjects of all currently non-occupied parking spots
    private static List<GameObject> _takenSpots; // stores GameObjects of all currently occupied parking spots
    private static GameObject _carsFolder; // a reference to the "Cars" Object in the unity editor. Set all cars as children of this for orginized instantiation

    private bool _isOccupied;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }


    // Initialize static structures within ParkingSpotNode. Should be called on scene load
    // Populates all parking spots that are occupied on game start with cars and performs necessary underlying opperations
    // NumFreeSpots determines the number of free spots that will be generated during initialization
    public static void Initialize(int numFreeSpots)
    {
        // Obtain the "Cars" GameObject
        _carsFolder = GameObject.Find("Cars");

        // Obtain all parking spots from NodeObjects
        var nodeObjects = GetNodeObjects();

        // Node.Initialize() needs to be called first to populate Node.NodeObjects
        if (nodeObjects == null)
        {
            Debug.Log("WARNING: ParkingSpotNode.Initialize() was called after Node.Initialize()");
            Initialize();
            nodeObjects = GetNodeObjects();
        }

        // Car.Initialize needs to be called first to populate Car.CarPrefabPaths
        if (Car.CarPrefabPaths.Count == 0)
        {
            Debug.Log("WARNING: ParkingSpotNode.Initialize() was called after Car.Initialize() (or CarPrefabPaths is empty)");
            Car.Initialize();
        }

        // construct the underlying parking lot node structures
        _openSpots = new List<GameObject>();
        _takenSpots = new List<GameObject>();

        // all spots are "taken" by default
        foreach (var node in nodeObjects)
            if (node.GetComponent<ParkingSpotNodeOld>() != null)
            {
                node.GetComponent<ParkingSpotNodeOld>()._isOccupied = true;
                _takenSpots.Add(node);
            }

        // select random spots to leave open
        for (var i = 0; i < numFreeSpots; i++)
        {
            var index = Random.Range(0, _takenSpots.Count);
            var trials = 0;
            while (!ValidateOpenSpot(_takenSpots[index]))
            {
                index = Random.Range(0, _takenSpots.Count);
                if (trials > 50) throw new Exception("ValidateOpenSpot() rejected 50+ parking spots");
                trials++;
            }

            // add the validated spot to OpenSpots and mark as vacant
            _openSpots.Add(_takenSpots[index]);
            _takenSpots[index].GetComponent<ParkingSpotNodeOld>()._isOccupied = false;
        }

        // remove all vacant parking spots from TakenSpots
        _takenSpots.RemoveAll(x => x.GetComponent<ParkingSpotNodeOld>()._isOccupied == false);

        // add cars to all taken spots
        foreach (var parkingSpot in _takenSpots)
        {
            // select a random car prefab of the available prefabs and instantiate it
            var prefabIndex = Random.Range(0, Car.CarPrefabPaths.Count);
            var car = Instantiate(Resources.Load(Car.CarPrefabPaths[prefabIndex])) as GameObject;

            // set the new object's position to it's parking spot, set it's origin pathnode, and make it a child of "Cars" (for orginization)
            car.transform.position = new Vector3(parkingSpot.transform.position.x, parkingSpot.transform.position.y, parkingSpot.transform.position.z);
            car.GetComponent<Car>().PathNodes.Add(parkingSpot.GetComponent<NodeOld>());
            car.transform.parent = _carsFolder.transform;
            car.GetComponent<Car>().SetLastNode(parkingSpot.GetComponent<ParkingSpotNodeOld>());
        }
    }


    public static List<GameObject> GetOpenSpots()
    {
        return _openSpots;
    }

    public static List<GameObject> GetTakenSpots()
    {
        return _takenSpots;
    }

    // calling SpotTaken() on a ParkingSpotNode indicates that (this) parking spot was just filled
    public void SpotTaken()
    {
        for (var i = 0; i < _openSpots.Count; i++)
            if (_openSpots[i].GetComponent<NodeOld>().GetNodeId() == GetNodeId())
            {
                _openSpots[i].GetComponent<ParkingSpotNodeOld>()._isOccupied = true;
                _takenSpots.Add(_openSpots[i]);
                _openSpots.RemoveAt(i);
                break;
            }
    }

    // calling SpotOpened() on a ParkingSpotNode indicates that (this) parking spot was just vacated
    public void SpotOpened()
    {
        for (var i = 0; i < _takenSpots.Count; i++)
            if (_takenSpots[i].GetComponent<NodeOld>().GetNodeId() == GetNodeId())
            {
                _takenSpots[i].GetComponent<ParkingSpotNodeOld>()._isOccupied = false;
                _openSpots.Add(_takenSpots[i]);
                _takenSpots.RemoveAt(i);
                break;
            }
    }


    public bool GetIsOccupied()
    {
        return _isOccupied;
    }


    // Given a parking spot game object, returns true if the parking spot is at least
    // "MinAcceptableDistance" away from the AngryCar. Returns false otherwise.
    private static bool ValidateOpenSpot(GameObject parkingSpotNode)
    {
        float minAcceptableDistance = 10;
        if (parkingSpotNode.GetComponent<ParkingSpotNodeOld>() == null)
            throw new Exception("ValidateOpenSpot() was given an invalid GameObject");

        // only consider AngryCar proximity if there is currently one in the scene
        if (GameManager.ACar != null)
        {
            var distance = Vector3.Distance(parkingSpotNode.transform.position, GameManager.ACar.transform.position);
            return distance > minAcceptableDistance;
        }

        // otherwise, accept all locations

        Debug.Log("WARNING: Could not locate AngryCar");
        return true;
    }
}