using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Level
{
    public class PathfindingManager : MonoBehaviour
    {
        private bool _debugMode = true;

        #region Singleton
        private static PathfindingManager _instance;
        public static PathfindingManager Instance => _instance ?? (_instance = Create());

        private static PathfindingManager Create()
        {
            GameObject singleton = FindObjectOfType<PathfindingManager>()?.gameObject;
            if (singleton == null) singleton = new GameObject { name = typeof(PathfindingManager).Name };
            singleton.AddComponent<PathfindingManager>();
            return singleton.GetComponent<PathfindingManager>();
        }
        #endregion

        /// <summary>
        /// GetPath is used to obtain the best path between two connections. 
        /// Returns a bool indicating if a path could be found.
        /// If start == end, returns true with an empty path
        /// This algorithm implements a modified version of Dijkstra's Algorithm.
        /// </summary>
        public bool GetPath(Connection start, Connection end, out List<BezierCurve> path)
        {
            #region INPUT PROCESSING/VALIDATION

            path = new List<BezierCurve>();

            // The given connections must not be null
            if ((start == null) || (end == null))
            {
                Debug.LogError("PathfindingManager.GetPath() was given a null connection");
                return false;
            }

            // If the connections are equivalent, there is no path to give
            // return true, but give an empty path
            if (start == end) return true;

            #endregion


            #region SETUP

            PathNodeComparer pathNodeComparer = new PathNodeComparer();                                 // used to compare the weights of PathNodes when sorting the frontier
            Dictionary<Connection, PathNode> processed = new Dictionary<Connection, PathNode>();        // all processed nodes. used to check if a connection has been processed already
            List<PathNode> frontier = new List<PathNode>();                                             // queue of nearby unprocessed nodes, sorted after each processing step
            PathNode current = new PathNode(start, 0, null);
            frontier.Add(current);

            #endregion


            #region CORE ALGORITHM

            // indicates that we've located the end connection entity. halt processing and exit the while loop
            bool endConnectionDiscovered = false;       

            // Begin exploring the frontier. When the frontier is empty, we've processed all reachable connections.
            // These are the voyages of the Starship Enterprise...
            while (frontier.Count > 0)
            {
                // lowest weight PathNode in Frontier is next to be evaluated
                frontier.Sort(pathNodeComparer);
                current = frontier[0];
                frontier.Remove(current);


                // CHECK THE NODE WE'RE PROCESSING

                // if we're processing the end node, we've found the shortest path to it!
                if (current.connection == end)
                { endConnectionDiscovered = true; break; }


                // if this fails, there exists a connection with a path to it, but no paths leaving it. Our runtime path "baking" algorithm failed somewhere.
                Debug.Assert(current.connection.ConnectsTo != null);                            


                // BEGIN PROCESSING

                // explore the (current connection => linked inbound connection)'s outbound connections.
                foreach (Connection.ConnectionPath connectionPath in current.connection.ConnectsTo.Paths)
                {
                    // only observe connections we haven't yet processed
                    if (!processed.ContainsKey(connectionPath.Connection))
                    {
                        PathNode discoveredNode;
                        bool newNodeDiscovered = true;
                        float distance = Vector3.Distance(current.connection.ConnectsTo.gameObject.transform.position, connectionPath.Connection.gameObject.transform.position) + current.distance;
                        // TODO: add additional calculated wieght here... (vehicles currently in path, etc.)

                        // check if this connection has already been discovered (is in the frontier)
                        foreach (PathNode node in frontier)
                            if (node.connection == connectionPath.Connection)
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
                            discoveredNode = new PathNode(connectionPath.Connection, distance, current.connection);
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
                while (true)
                {
                    // we should always be able to traverse backwards until we reach the else case and break. Otherwise, we have a fault in the best path linked list
                    Debug.Assert(current.prevConnection != null);

                    // keep an eye out for the start connection as we construct the path
                    if (current.prevConnection != start)
                    {
                        // step backwards to the previous connection, then get the curve to the current connection
                        if (!current.prevConnection.GetPathToConnection(current.connection, out curve))
                            if (_debugMode) { Debug.LogError("GetPath() Generated an invalid path"); }
                        path.Add(curve);
                        current = processed[current.prevConnection]; 
                    }

                    // we've reached the first (start) connection. Generate the last path and exit
                    else
                    {
                        if (!current.prevConnection.GetPathToConnection(current.connection, out curve))
                            if (_debugMode) { Debug.LogError("GetPath() Generated an invalid path"); }
                        path.Add(curve);
                        break;
                    }
                }
                path.Reverse();     // we generated the list in reverese order
                return true;
            }

            // we never discovered the end connection. End connection is not reachable from the start connection
            else
            {
                if (_debugMode) { Debug.LogError("PathfindingManager.GetPath() could not determine a path (unreachable)"); }
                return false;
            }

            #endregion
        }
    }
}


