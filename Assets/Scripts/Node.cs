using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{

    public static List<GameObject> NodeObjects = new List<GameObject>();

    [SerializeField]
    public List<Node> Connections;

    // for pathfinding
    public float Weight;       // length of shortest path from origin
    public Node PrevNode;      // previous node in shortest path

    // Use this for initialization
    void Awake()
    {
        foreach(GameObject x in GameObject.FindGameObjectsWithTag("Node"))
        {
            NodeObjects.Add(x);
        }
        foreach (GameObject x in GameObject.FindGameObjectsWithTag("Parking Spot"))
        {
            NodeObjects.Add(x);
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public List<Node> FindShortestPath(GameObject Destination)
    {
        Debug.Log("Number of Nodes" + NodeObjects.Count);
        // construct a transform list of all unvisited nodes
        List<Transform> Frontier = new List<Transform>();
        foreach (GameObject Obj in NodeObjects)
        {
            // first, check for parking spot nodes
            if (Obj.GetComponent<ParkingSpotNode>() != null)
            {
                // only add nodes that are vacant
                if (!Obj.GetComponent<ParkingSpotNode>().IsOccupied)
                {
                    Node n = Obj.GetComponent<Node>();
                    n.Weight = int.MaxValue;
                    n.PrevNode = null;
                    Frontier.Add(Obj.transform);
                }
            }

            // otherwise, always add the node
            else
            {
                Node n = Obj.GetComponent<Node>();
                n.Weight = int.MaxValue;
                n.PrevNode = null;
                Frontier.Add(Obj.transform);
            }
        }

        // set start node and begin exploring
        this.Weight = 0;
        while(Frontier.Count > 0)
        {
            // Sort Frontier by weight in ascending order and select the lowest weight as current
            Frontier.Sort((x, y) => x.GetComponent<Node>().Weight.CompareTo(y.GetComponent<Node>().Weight));
            Transform Current = Frontier[0];
            Frontier.Remove(Current);

            // 
            Node CurrentNode = Current.GetComponent<Node>();
            foreach (Node Connection in CurrentNode.Connections)
            {
                // only explore unexplored nodes
                if (Frontier.Contains(Connection.gameObject.transform))
                {
                    // calculate new distance
                    float Distance = Vector3.Distance(Connection.gameObject.transform.position, Current.position);
                    Distance = CurrentNode.Weight + Distance;

                    // if we've found a shorter path, update
                    if (Distance < Connection.Weight)
                    {
                        Connection.Weight = Distance;
                        Connection.PrevNode = CurrentNode;
                    }
                }
            }
        }

        List<Node> output = new List<Node>();
        Node Traverse = Destination.GetComponent<Node>();
        output.Add(Traverse);

        while (Traverse.PrevNode != null)
        {
            output.Add(Traverse.PrevNode);
            Traverse = Traverse.PrevNode;
        }

        output.Reverse();
        return output;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Connections.Count; i++)
        {
           // Gizmos.DrawLine(transform.position, Connections[i].transform.position);
        }
    }
}
