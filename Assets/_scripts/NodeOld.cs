using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeOld : MonoBehaviour
{
    public static bool FspDebugMode;

    private static List<GameObject> _nodeObjects; // stores all Entity GameObjects in the current scene
    private static List<float> _fspWeight; // stores entity data for FindShortestPath() (Weight). Indeces correspond 1:1 with NodeObjects
    private static List<NodeOld> _fspPrevNode; // stores entity data for FindShortestPath() (PrevNode). Indeces correspond 1:1 with NodeObjects
    private int _nodeId; // stores the current entity's index within NodeObjects for faster accesses

    [SerializeField]
    public List<NodeOld> Connections; // a list of all valid paths to adjacent nodes from the current entity


    // Use this for initialization
    private void Awake()
    {
        FspDebugMode = false;
    }


    // Initialize static structures within Entity. Should be called on scene load
    public static void Initialize()
    {
        // initialize Entity class datastructures
        _nodeObjects = new List<GameObject>();
        _fspWeight = new List<float>();
        _fspPrevNode = new List<NodeOld>();

        // populate NodeObjects with all nodes in the scene
        var indexAssignment = 0;
        foreach (var obj in GameObject.FindGameObjectsWithTag("Entity"))
        {
            _nodeObjects.Add(obj);
            _nodeObjects[indexAssignment].GetComponent<NodeOld>()._nodeId = indexAssignment;
            indexAssignment++;
        }

        foreach (var obj in GameObject.FindGameObjectsWithTag("Parking Spot"))
        {
            _nodeObjects.Add(obj);
            _nodeObjects[indexAssignment].GetComponent<NodeOld>()._nodeId = indexAssignment;
            indexAssignment++;
        }

        // reset the FSPWeight & FSPPrevNode Lists
        for (var i = 0; i < _nodeObjects.Count; i++)
            _fspWeight.Add(float.MaxValue);
        for (var i = 0; i < _nodeObjects.Count; i++)
            _fspPrevNode.Add(null);

        if (FspDebugMode)
            Debug.Log("Number of NodeObjects: " + _nodeObjects.Count);
    }


    // Update is called once per frame
    private void Update()
    {
    }


    // retrieve a list of all Entity GameObjects in the current scene
    public static List<GameObject> GetNodeObjects()
    {
        return _nodeObjects;
    }


    // Given a destination Entity, FSP finds the shortest path to that entity through all valid nodes
    // The path is returned as a List with the last entity being the destination
    public List<NodeOld> FindShortestPath(NodeOld destination)
    {
        // check to make sure the destination isn't an occupied parking spot
        if (destination.GetComponent<ParkingSpotNodeOld>() != null)
            if (destination.GetComponent<ParkingSpotNodeOld>().GetIsOccupied())
                throw new Exception("FindShortestPath was given an invalid destination entity");

        // construct a list of all unvisited nodes
        var frontier = new List<GameObject>();
        for (var i = 0; i < _nodeObjects.Count; i++)
            // consider all non-parking nodes for pathfinding
            if (_nodeObjects[i].GetComponent<ParkingSpotNodeOld>() == null)
            {
                _fspWeight[i] = float.MaxValue;
                _fspPrevNode[i] = null;
                frontier.Add(_nodeObjects[i]);
            }

            // origin or destination parking spot nodes are always valid
            // note that we already checked for destination parking entity occupancy above
            else if (_nodeObjects[i] == gameObject || _nodeObjects[i] == destination.gameObject)
            {
                _fspWeight[i] = float.MaxValue;
                _fspPrevNode[i] = null;
                frontier.Add(_nodeObjects[i]);
            }

        // set start entity and begin exploring Frontier
        // these are the voyages of the Starship Enterprise...
        _fspWeight[_nodeId] = 0;
        while (frontier.Count > 0)
        {
            // sort Frontier by weight in ascending order and select the lowest weight as current
            frontier.Sort((x, y) => _fspWeight[x.GetComponent<NodeOld>()._nodeId].CompareTo(_fspWeight[y.GetComponent<NodeOld>()._nodeId]));
            var currentNodeObject = frontier[0];
            var currentNode = currentNodeObject.GetComponent<NodeOld>();
            frontier.Remove(currentNodeObject);

            // explore the CurrentNode's connections
            var connections = currentNode.Connections;
            foreach (var connection in connections)
                // only explore unexplored nodes
                if (frontier.Contains(connection.gameObject))
                {
                    // calculate new distance
                    var distance = Vector3.Distance(connection.gameObject.transform.position, currentNodeObject.transform.position);
                    distance = _fspWeight[currentNode._nodeId] + distance;

                    // if we've found a shorter path, update
                    if (distance < _fspWeight[connection._nodeId])
                    {
                        _fspWeight[connection._nodeId] = distance;
                        _fspPrevNode[connection._nodeId] = currentNode;
                    }
                }
        }

        // construct the shortest path list with a reverse traversal through stored PrevNode data
        var output = new List<NodeOld>();
        var traverse = destination;
        output.Add(traverse);

        while (_fspPrevNode[traverse._nodeId] != null)
        {
            output.Add(_fspPrevNode[traverse._nodeId]);
            traverse = _fspPrevNode[traverse._nodeId];
        }

        output.Reverse();

        if (FspDebugMode)
        {
            Debug.Log("Origin Entity: " + gameObject.name);
            Debug.Log("Destination Entity: " + destination.gameObject.name);
            Debug.Log("FSP PathnodeNode Route:");
            foreach (var node in output)
                Debug.Log(node.gameObject.name);
        }

        return output;
    }


    // returns the current entity's unique ID (useful for finding this entity in a list of nodes, since
    // List.Find()'s comparer can't differentiate Entity objects)
    public int GetNodeId()
    {
        return _nodeId;
    }
}