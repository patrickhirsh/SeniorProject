using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    [SerializeField]
    public List<Node> Connections;                      // a list of all valid paths to adjacent nodes from the current node
    public static bool FSPDebugMode;

    private static List<GameObject> NodeObjects;        // stores all Node GameObjects in the current scene
    private static List<float> FSPWeight;               // stores node data for FindShortestPath() (Weight). Indeces correspond 1:1 with NodeObjects
    private static List<Node> FSPPrevNode;              // stores node data for FindShortestPath() (PrevNode). Indeces correspond 1:1 with NodeObjects
    private int NodeID;                                 // stores the current node's index within NodeObjects for faster accesses


    // Use this for initialization
    void Awake()
    {
        FSPDebugMode = false;
    }


    // Initialize static structures within Node. Should be called on scene load
    public static void Initialize()
    {
        // initialize Node class datastructures
        NodeObjects = new List<GameObject>();
        FSPWeight = new List<float>();
        FSPPrevNode = new List<Node>();

        // populate NodeObjects with all nodes in the scene
        int indexAssignment = 0;
        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("Node"))
        {
            NodeObjects.Add(Obj);
            NodeObjects[indexAssignment].GetComponent<Node>().NodeID = indexAssignment;
            indexAssignment++;
        }

        foreach (GameObject Obj in GameObject.FindGameObjectsWithTag("Parking Spot"))
        {
            NodeObjects.Add(Obj);
            NodeObjects[indexAssignment].GetComponent<Node>().NodeID = indexAssignment;
            indexAssignment++;
        }

        // reset the FSPWeight & FSPPrevNode Lists
        for (int i = 0; i < NodeObjects.Count; i++)
            FSPWeight.Add(float.MaxValue);
        for (int i = 0; i < NodeObjects.Count; i++)
            FSPPrevNode.Add(null);

        if (FSPDebugMode)
            Debug.Log("Number of NodeObjects: " + NodeObjects.Count);
    }


    // Update is called once per frame
    void Update()
    {

    }


    // retrieve a list of all Node GameObjects in the current scene
    public static List<GameObject> GetNodeObjects()
    {
        return NodeObjects;
    }


    // Given a destination Node, FSP finds the shortest path to that node through all valid nodes
    // The path is returned as a List with the last node being the destination
    public List<Node> FindShortestPath(Node Destination)
    {
        // check to make sure the destination isn't an occupied parking spot
        if (Destination.GetComponent<ParkingSpotNode>() != null)
            if (Destination.GetComponent<ParkingSpotNode>().GetIsOccupied() == true)
                throw new Exception("FindShortestPath was given an invalid destination node");

        // construct a list of all unvisited nodes
        List<GameObject> Frontier = new List<GameObject>();
        for (int i = 0; i < NodeObjects.Count; i++)
        {
            // consider all non-parking nodes for pathfinding
            if (NodeObjects[i].GetComponent<ParkingSpotNode>() == null)
            {
                FSPWeight[i] = float.MaxValue;
                FSPPrevNode[i] = null;
                Frontier.Add(NodeObjects[i]);
            }

            // origin or destination parking spot nodes are always valid
            // note that we already checked for destination parking node occupancy above
            else if ((NodeObjects[i] == this.gameObject) || (NodeObjects[i] == Destination.gameObject))
            {
                FSPWeight[i] = float.MaxValue;
                FSPPrevNode[i] = null;
                Frontier.Add(NodeObjects[i]);
            }
        }

        // set start node and begin exploring Frontier
        // these are the voyages of the Starship Enterprise...
        FSPWeight[this.NodeID] = 0;
        while(Frontier.Count > 0)
        {
            // sort Frontier by weight in ascending order and select the lowest weight as current
            Frontier.Sort((x, y) => FSPWeight[x.GetComponent<Node>().NodeID].CompareTo(FSPWeight[y.GetComponent<Node>().NodeID]));
            GameObject CurrentNodeObject = Frontier[0];
            Node CurrentNode = CurrentNodeObject.GetComponent<Node>();
            Frontier.Remove(CurrentNodeObject);

            // explore the CurrentNode's connections
            List<Node> Connections = CurrentNode.Connections;
            foreach (Node Connection in Connections)
            {
                // only explore unexplored nodes
                if (Frontier.Contains(Connection.gameObject))
                {
                    // calculate new distance
                    float Distance = Vector3.Distance(Connection.gameObject.transform.position, CurrentNodeObject.transform.position);
                    Distance = FSPWeight[CurrentNode.NodeID] + Distance;

                    // if we've found a shorter path, update
                    if (Distance < FSPWeight[Connection.NodeID])
                    {
                        FSPWeight[Connection.NodeID] = Distance;
                        FSPPrevNode[Connection.NodeID] = CurrentNode;
                    }
                }
            }
        }

        // construct the shortest path list with a reverse traversal through stored PrevNode data
        List<Node> Output = new List<Node>();
        Node Traverse = Destination;
        Output.Add(Traverse);

        while (FSPPrevNode[Traverse.NodeID] != null)
        {
            Output.Add(FSPPrevNode[Traverse.NodeID]);
            Traverse = FSPPrevNode[Traverse.NodeID];
        }
        Output.Reverse();

        if (FSPDebugMode)
        {
            Debug.Log("Origin Node: " + this.gameObject.name);
            Debug.Log("Destination Node: " + Destination.gameObject.name);
            Debug.Log("FSP PathnodeNode Route:");
            foreach (Node Node in Output)
                Debug.Log(Node.gameObject.name);
        }

        return Output;
    }


    // returns the current node's unique ID (useful for finding this node in a list of nodes, since
    // List.Find()'s comparer can't differentiate Node objects)
    public int GetNodeID()
    {
        return NodeID;
    }
}
