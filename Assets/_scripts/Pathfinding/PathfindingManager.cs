using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Level
{
    public class PathfindingManager : MonoBehaviour
    {
        private bool _debugMode = true;
        EntityManager _entityManager;    // PathfindingManager utilizes structures from the scene's EntityManager


        // Use this for initialization
        void Start()
        {
            _entityManager = EntityManager.Instance;
        }


        /// <summary>
        /// GetExternalPath is used to obtain a path between any two Inbound/Outbound connections. Returns a bool
        /// indicating if a path could be found. This function should be used to find paths across entities. 
        /// To find a path within an entity (using interior connection types) see GetInternalPath. 
        /// This algorithm implements a modified version of Dijkstra's Algorithm.
        /// </summary>
        public bool GetExternalPath(Connection start, Connection end, out List<BezierCurve> path)
        {
            #region INPUT PROCESSING/VALIDATION

            path = new List<BezierCurve>();

            // The given connections must not be null
            if ((start == null) || (end == null))
            {
                Debug.LogError("PathfindingManager.GetExternalPath() was given a null connection");
                return false;
            }

            // Start connection must be either inbound or outbound
            if ((start.Type != Connection.ConnectionType.Inbound) && (start.Type != Connection.ConnectionType.Outbound))
            {
                if (_debugMode) { Debug.LogError("PathfindingManager.GetExternalPath() was given an invalid starting connection"); }
                return false;
            }

            // Start connection must be either inbound or outbound
            if ((end.Type != Connection.ConnectionType.Inbound) && (end.Type != Connection.ConnectionType.Outbound))
            {
                if (_debugMode) { Debug.LogError("PathfindingManager.GetExternalPath() was given an invalid ending connection"); }
                return false;
            }            

            // if we're given an outbound connection, start at the next inbound connection (since that's the only place we can move to)
            // the starting connection is the ONLY connection stored as an inbound connection
            if (start.Type == Connection.ConnectionType.Outbound)
            {
                start = start.ConnectsTo;
                if (start == null) return false;    // we were given an outbound connection with no linked inbound connection
            }

            #endregion


            #region SETUP

            PathNodeComparer pathNodeComparer = new PathNodeComparer();                                 // used to compare the weights of PathNodes when sorting the frontier
            Dictionary<Connection, PathNode> processed = new Dictionary<Connection, PathNode>();        // all processed nodes. used to check if a connection has been processed already
            List<PathNode> frontier = new List<PathNode>();                                             // queue of nearby unprocessed nodes, sorted after each processing step
            PathNode current = new PathNode(start, 0, null);
            processed.Add(start, current);

            // we process the start connection by hand, since it's a special case (an inbound connection)
            foreach (Connection.ConnectionPath connectionPath in current.connection.Paths)
            {
                float distance = Vector3.Distance(current.connection.gameObject.transform.position, connectionPath.OutboundConnection.gameObject.transform.position);
                // TODO: add additional calculated wieght here... (vehicles currently in path, etc.)
                PathNode discoveredNode = new PathNode(connectionPath.OutboundConnection, distance, current.connection);
                frontier.Add(discoveredNode);
            }

            #endregion


            #region CORE ALGORITHM

            // at this point, we've processed the start connection, removed it from the frontier, and added all its neighboring
            // connections. Begin exploring the frontier. When the frontier is empty, we've processed all reachable connections.
            // These are the voyages of the Starship Enterprise...
            bool endConnectionDiscovered = false;
            while (frontier.Count > 0)
            {
                // lowest weight PathNode in Frontier is next to be evaluated
                frontier.Sort(pathNodeComparer);
                current = frontier[0];
                frontier.Remove(current);

                // if we're processing the end node, we've found the shortest path to it!
                // if we exit the while loop without discovering the end node, there's no possible way to reach it
                if (current.connection == end)
                    { endConnectionDiscovered = true; break; }

                // connections stored in frontier should ALWAYS be outbound connections
                Debug.Assert(current.connection.Type == Connection.ConnectionType.Outbound);

                // explore the (current connection => linked inbound connection in next Entity)'s outbound connections.
                foreach (Connection.ConnectionPath connectionPath in current.connection.ConnectsTo.Paths)
                {
                    // only observe connections we haven't yet processed
                    if (!processed.ContainsKey(connectionPath.OutboundConnection))
                    {
                        PathNode discoveredNode;
                        bool newNodeDiscovered = true;
                        float distance = Vector3.Distance(current.connection.ConnectsTo.gameObject.transform.position, connectionPath.OutboundConnection.gameObject.transform.position) + current.distance;
                        // TODO: add additional calculated wieght here... (vehicles currently in path, etc.)
                        
                        // check if this connection has already been discovered (is in the frontier)
                        foreach (PathNode node in frontier)
                            if (node.connection == connectionPath.OutboundConnection)
                            {
                                // we've already discovered this node!
                                discoveredNode = node;
                                newNodeDiscovered = false;

                                // is this path better than its current path? If so, change its best path to this one. If not, move on
                                if (discoveredNode.distance > distance)
                                {
                                    discoveredNode.distance = distance;
                                    discoveredNode.prevConnection = current.connection;
                                }
                                break;
                            }

                        // this connection has never been discovered before. Add it to the frontier!
                        if (newNodeDiscovered)
                        {
                            discoveredNode = new PathNode(connectionPath.OutboundConnection, distance, current.connection);
                            frontier.Add(discoveredNode);
                        }                             
                    }
                }

                // processing for this connection is complete. Add to processed and continue
                processed.Add(current.connection, current);
            }

            #endregion


            #region PATH CONSTRUCTION

            // if we discovered the end connection, construct the best path
            if (endConnectionDiscovered)
            {
                BezierCurve curve = new BezierCurve();

                // traverse backwards through the best path (using prevConnection) to construct the path
                while (current.connection.Type == Connection.ConnectionType.Outbound)
                {
                    // if either of these asserts fail, we were unable to correctly traverse backwards through the best path.
                    // the algorithm has failed to properly construct a linked list for best path
                    Debug.Assert(current.prevConnection != null);
                    Debug.Assert((current.prevConnection.Type == Connection.ConnectionType.Inbound) || (current.prevConnection.Type == Connection.ConnectionType.Outbound));

                    // all connections should be outbound EXCEPT the first (start) connection
                    if (current.prevConnection.Type == Connection.ConnectionType.Outbound)
                    {
                        // step backwards to the previous Outbound, then step into its inbound connection to get the curve to the current outbound connection
                        current.prevConnection.ConnectsTo.FindPathToConnection(current.connection, out curve);
                        path.Add(curve);
                        current = processed[current.prevConnection];    // move on to the next 
                    }

                    // we've reached the first (start) connection. Generate the last path and exit
                    else
                    {
                        Debug.Assert(current.prevConnection.Type == Connection.ConnectionType.Inbound);
                        current.prevConnection.FindPathToConnection(current.connection, out curve);
                        path.Add(curve);
                        current = processed[current.prevConnection];
                        break;
                    }                   
                }
                path.Reverse();     // we generated the list in reverese order
                return true;
            }

            // we never discovered the end connection. End connection is not reachable from the start connection
            else
            {
                if (_debugMode) { Debug.LogError("PathfindingManager.GetExternalPath() could not determine a path (unreachable)"); }
                return false;
            }

            #endregion
        }
    }
}


