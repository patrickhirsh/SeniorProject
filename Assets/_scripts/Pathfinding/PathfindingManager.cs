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
        /// GetPath is used to obtain the best path between two connections of any type. 
        /// Returns a bool indicating if a path could be found.
        /// If start == end or start is an outbound connection where start.ConnectsTo == end, returns true with an empty path
        /// This algorithm implements a modified version of Dijkstra's Algorithm.
        /// </summary>
//        public bool GetPath(Connection start, Connection end, out List<BezierCurve> path)
//        {
//            #region INPUT PROCESSING/VALIDATION
//
//            path = new List<BezierCurve>();
//
//            // The given connections must not be null
//            if ((start == null) || (end == null))
//            {
//                Debug.LogError("PathfindingManager.GetPath() was given a null connection");
//                return false;
//            }
//
//            // if we're given an outbound start connection, start at the next inbound connection (since that's the only place we can move to)
//            // the starting connection is the ONLY connection stored as an inbound (or internal) connection
//            if (start.Type == Connection.ConnectionType.Outbound)
//            {
//                start = start.ConnectsTo;
//                if (start == null) return false;    // we were given an outbound connection with no linked inbound connection
//            }
//
//            // If the connections are equivalent, there is no path to give
//            // return true, but give an empty path
//            if (start == end) return true;
//
//            // if the start and end connections are in the same entity, try to get a path between them
//            // this covers the case where we would need to check internalPaths on the first "exploration" (done in SETUP)
//            if (start.ParentEntity == end.ParentEntity)
//            {
//                BezierCurve curve = new BezierCurve();
//                if (start.GetPathToConnection(end, out curve))
//                {
//                    path.Add(curve);
//                    return true;
//                }
//                // if we don't find a path here, the only scenario is that start is an inbound connection in the same entity as end,
//                // however a path does not exist directly between them. Continue the algorithm and see if we can exit, then
//                // re-enter the entity on a different connection that connects to end.
//            }
//
//            #endregion
//
//
//            #region SETUP
//
//            PathNodeComparer pathNodeComparer = new PathNodeComparer();                                 // used to compare the weights of PathNodes when sorting the frontier
//            Dictionary<Connection, PathNode> processed = new Dictionary<Connection, PathNode>();        // all processed nodes. used to check if a connection has been processed already
//            List<PathNode> frontier = new List<PathNode>();                                             // queue of nearby unprocessed nodes, sorted after each processing step
//            PathNode current = new PathNode(start, 0, null);
//            processed.Add(start, current);
//
//            // we process the start connection by hand, since it's a special case (an inbound or internal connection)
//            foreach (Connection.ConnectionPath connectionPath in current.connection.Paths)
//            {
//                // we don't care about internal connections here. See the final check in INPUT PROCESSING/VALIDATION for more info
//                if (connectionPath.Connection.Type == Connection.ConnectionType.Outbound)
//                {
//                    float distance = Vector3.Distance(current.connection.gameObject.transform.position, connectionPath.Connection.gameObject.transform.position);
//                    // TODO: add additional calculated wieght here... (vehicles currently in path, etc.)
//                    PathNode discoveredNode = new PathNode(connectionPath.Connection, distance, current.connection);
//                    frontier.Add(discoveredNode);
//                }               
//            }
//
//            #endregion
//
//
//            #region CORE ALGORITHM
//
//            bool endConnectionDiscovered = false;       // indicates that we've located the end connection entity. halt processing and exit the while loop
//            bool inSameEntityAsEnd = false;             // acts as a switch that indicates we need to check internal connections (since we're in the same entity as end, and end could be an internal connection)
//        
//            // at this point, we've processed the start connection, removed it from the frontier, and added all its neighboring
//            // connections. Begin exploring the frontier. When the frontier is empty, we've processed all reachable connections.
//            // These are the voyages of the Starship Enterprise...
//            while (frontier.Count > 0)
//            {
//                // lowest weight PathNode in Frontier is next to be evaluated
//                frontier.Sort(pathNodeComparer);
//                current = frontier[0];
//                frontier.Remove(current);
//
//
//                // CHECK THE NODE WE'RE PROCESSING
//
//                // if we're processing the end node, we've found the shortest path to it!
//                if (current.connection == end)
//                    { endConnectionDiscovered = true; break; }
//
//                // we've found a connection in the same entity as end. Search for end in the below foreach loop
//                if (current.connection.ParentEntity == end.ParentEntity) { inSameEntityAsEnd = true; }                    
//                else { inSameEntityAsEnd = false; }
//
//                Debug.Assert(current.connection.Type == Connection.ConnectionType.Outbound);    // we should never be processing a non-outbound connection
//                Debug.Assert(current.connection.ConnectsTo != null);                            // if this fails, there exists an outbound connection with a path to it, but no linked inbound connection (ConnectsTo) going from it
//
//
//                // BEGIN PROCESSING
//
//                // explore the (current connection => linked inbound connection)'s outbound connections.
//                foreach (Connection.ConnectionPath connectionPath in current.connection.ConnectsTo.Paths)
//                {
//                    // only observe outbound connections UNLESS we're in the same entity as end. Then also check internal connections for end
//                    if ((connectionPath.Connection.Type == Connection.ConnectionType.Outbound) ||
//                        ((inSameEntityAsEnd) && (connectionPath.Connection == end)))
//                    {
//                        // only observe connections we haven't yet processed
//                        if (!processed.ContainsKey(connectionPath.Connection))
//                        {
//                            PathNode discoveredNode;
//                            bool newNodeDiscovered = true;
//                            float distance = Vector3.Distance(current.connection.ConnectsTo.gameObject.transform.position, connectionPath.Connection.gameObject.transform.position) + current.distance;
//                            // TODO: add additional calculated wieght here... (vehicles currently in path, etc.)
//
//                            // check if this connection has already been discovered (is in the frontier)
//                            foreach (PathNode node in frontier)
//                                if (node.connection == connectionPath.Connection)
//                                {
//                                    // we've already discovered this node!
//                                    discoveredNode = node;
//                                    newNodeDiscovered = false;
//
//                                    // is this path better than its current path? If so, change its best path to this one. If not, move on
//                                    if (discoveredNode.distance > distance)
//                                    {
//                                        discoveredNode.distance = distance;
//                                        discoveredNode.prevConnection = current.connection;
//                                    }
//                                    break;
//                                }
//
//                            // this connection has never been discovered before. Add it to the frontier!
//                            if (newNodeDiscovered)
//                            {
//                                discoveredNode = new PathNode(connectionPath.Connection, distance, current.connection);
//                                frontier.Add(discoveredNode);
//                            }
//                        }
//                    } 
//                }
//
//                // processing for this connection is complete. Add to processed and continue
//                processed.Add(current.connection, current);
//            }
//
//            #endregion
//
//
//            #region PATH CONSTRUCTION
//
//            // if we discovered the end connection, construct the best path
//            if (endConnectionDiscovered)
//            {
//                BezierCurve curve = new BezierCurve();
//
//                // traverse backwards through the best path (using prevConnection) to construct the path
//                while (true)
//                {
//                    // we should always be able to traverse backwards until we reach the else case and break. Otherwise, we have a fault in the best path linked list
//                    Debug.Assert(current.prevConnection != null);
//
//                    // all backwards-traveresed connections after end should be outbound EXCEPT the first (start) connection
//                    if (current.prevConnection.Type == Connection.ConnectionType.Outbound)
//                    {
//                        // step backwards to the previous Outbound, then step into its inbound connection to get the curve to the current outbound connection
//                        if (!current.prevConnection.ConnectsTo.GetPathToConnection(current.connection, out curve))
//                            if (_debugMode) { Debug.LogError("GetPath() Generated an invalid path"); }
//                        path.Add(curve);
//                        current = processed[current.prevConnection];    // move on to the next 
//                    }
//
//                    // we've reached the first (start) connection. Generate the last path and exit
//                    else
//                    {
//                        Debug.Assert((current.prevConnection.Type == Connection.ConnectionType.Inbound) || (current.prevConnection.Type == Connection.ConnectionType.Internal));
//                        current.prevConnection.GetPathToConnection(current.connection, out curve);
//                        path.Add(curve);
//                        current = processed[current.prevConnection];
//                        break;
//                    }                   
//                }
//                path.Reverse();     // we generated the list in reverese order. Reverse it for proper output.
//                return true;
//            }
//
//            // we never discovered the end connection. End connection is not reachable from the start connection
//            else
//            {
//                if (_debugMode) { Debug.LogError("PathfindingManager.GetPath() could not determine a path (unreachable)"); }
//                return false;
//            }
//
//            #endregion
//        }
    }
}


