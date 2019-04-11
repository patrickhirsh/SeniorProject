using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace RideShareLevel
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


        #region PATH GENERATION

        /// <summary>
        /// Populates "connections" with the best path between "start" and "destination" (start/destination inclusive).
        /// This method will consider any connection out of "start" and find a path to any connection within "destination".
        /// Returns true if a path is found and false otherwise. If start == destination, "connections" will be left empty (returns true).
        /// </summary>
        public bool GetPath(Route start, Route destination, out Queue<Connection> connections)
        {
            // input validation
            connections = new Queue<Connection>();
            if ((start == null) || (destination == null)) { Debug.LogError("PathfindingManager.GetPath() was given a null connection"); return false; }
            if (start == destination) { return true; }

            PathNodeComparer pathNodeComparer = new PathNodeComparer();                                 // used to compare the weights of PathNodes when sorting the frontier
            Dictionary<Connection, PathNode> processed = new Dictionary<Connection, PathNode>();        // all processed nodes. used to check if a connection has been processed already
            List<PathNode> frontier = new List<PathNode>();                                             // queue of nearby unprocessed nodes, sorted after each processing step. NOTE: stores "outbound" connections

            // add all valid starting connections leaving the "start" Route to frontier
            foreach (Connection connection in start.Connections)
                if (connection.GetConnectsTo != null)
                    frontier.Add(new PathNode(connection, 0, null));

            PathNode current = frontier[0];
            bool endRouteDiscovered = false;

            // These are the voyages of the Starship Enterprise...
            while (frontier.Count > 0)
            {
                // lowest weight PathNode in Frontier is next to be evaluated
                frontier.Sort(pathNodeComparer);
                current = frontier[0];
                frontier.Remove(current);
                bool processNode = true;

                // if we're processing the end node, we've found the shortest path to it!
                if (current.connection.ParentRoute == destination)
                { endRouteDiscovered = true; break; }

                // if this is true, there exists a connection with a path to it, but no paths leaving it. Ignore
                if (current.connection.GetConnectsTo == null)
                { processNode = false; }

                // not all nodes need to be processed (see above) but ALL nodes should be added to procesed at this stage
                if (processNode) { ProcessNode(ref current, ref processed, ref frontier); }
                else { processed.Add(current.connection, current); }
            }

            if (endRouteDiscovered) { connections = ConstructPath(ref current, ref processed); return true; }
            else { return false; }
        }


        /// <summary>
        /// Processes "current" in Dijkstra's fashion. This method requires references to the caller's processed and frontier
        /// data structures in order to mutate them during processing.
        /// </summary>
        private void ProcessNode(ref PathNode current, ref Dictionary<Connection, PathNode> processed, ref List<PathNode> frontier)
        {
            // explore the (current connection => linked inbound connection)'s outbound connections.
            foreach (Connection.ConnectionPath connectionPath in current.connection.GetConnectsTo.Paths)
            {
                // only observe connections we haven't yet processed
                if (!processed.ContainsKey(connectionPath.Connection))
                {
                    PathNode discoveredNode;
                    bool newNodeDiscovered = true;
                    float distance = Vector3.Distance(current.connection.GetConnectsTo.transform.position, connectionPath.Connection.transform.position) + current.distance;
                    // TODO: add additional calculated weight here... (vehicles currently in path, etc.)

                    // check if this connection has already been discovered (is it in the frontier?)
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
            processed.Add(current.connection, current);
        }


        /// <summary>
        /// This method traverses backwards through current's previous connections to construct the contained path.
        /// The "processed" dictionary associated with the caller GetPath() method is required to map each PathNode to its connection.
        /// </summary>
        private Queue<Connection> ConstructPath(ref PathNode current, ref Dictionary<Connection, PathNode> processed)
        {
            // temporarily store the reversed path
            List<Connection> reversePath = new List<Connection>();
            reversePath.Add(current.connection);

            // traverse backwards through the best path (using prevConnection) to construct the path
            while (current.prevConnection != null)
            {
                reversePath.Add(current.prevConnection.GetConnectsTo);
                reversePath.Add(current.prevConnection);
                current = processed[current.prevConnection];
            }

            // add all final connections to the queue in the proper order
            Queue<Connection> path = new Queue<Connection>();
            for (int i = reversePath.Count - 1; i >= 0; i--)
                path.Enqueue(reversePath[i]);

            return path;
        }

        #endregion

        #region LEGACY PATH GENERATION

        /// <summary>
        /// GetPath is used to obtain the best path between two connections. 
        /// Returns a bool indicating if a path could be found.
        /// If start == end, returns true with an empty path
        /// This algorithm implements a modified version of Dijkstra's Algorithm.
        /// </summary>
        public bool GetPath(Connection start, Connection end, out Queue<Connection> path)
        {
            #region INPUT PROCESSING/VALIDATION

            path = new Queue<Connection>();

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

                // we may not want to process a connection for multiple reasons: it's in "intersection"/"end", it doesn't have a ConnectsTo, it doesn't have paths leaving it, etc..
                bool processNode = true;

                // if we're processing the end node, we've found the shortest path to it!
                if (current.connection == end)
                { endConnectionDiscovered = true; break; }

                // if this is true, there exists a connection with a path to it, but no paths leaving it. Ignore
                if (current.connection.GetConnectsTo == null)
                { processNode = false; }


                // BEGIN PROCESSING

                // only processed if (processNode)
                if (processNode)
                {
                    // explore the (current connection => linked inbound connection)'s outbound connections.
                    foreach (Connection.ConnectionPath connectionPath in current.connection.GetConnectsTo.Paths)
                    {
                        // only observe connections we haven't yet processed
                        if (!processed.ContainsKey(connectionPath.Connection))
                        {
                            PathNode discoveredNode;
                            bool newNodeDiscovered = true;
                            float distance = Vector3.Distance(current.connection.GetConnectsTo.gameObject.transform.position, connectionPath.Connection.gameObject.transform.position) + current.distance;
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
                }


                // processing for this connection is complete. Add to processed and continue
                processed.Add(current.connection, current);
            }

            #endregion


            #region PATH CONSTRUCTION

            // if we discovered the end connection, construct the best path
            if (endConnectionDiscovered)
            {

                // temporarily store the reversed path
                List<Connection> reversePath = new List<Connection>();
                reversePath.Add(current.connection);

                // traverse backwards through the best path (using prevConnection) to construct the path
                while (current.prevConnection != null)
                {
                    reversePath.Add(current.prevConnection.GetConnectsTo);
                    reversePath.Add(current.prevConnection);
                    current = processed[current.prevConnection];
                }

                // add all final connections to the queue in the proper order
                for (int i = reversePath.Count - 1; i >= 0; i--)
                    path.Enqueue(reversePath[i]);

                return true;
            }

            // we never discovered the end connection. End connection is not reachable from the start connection
            else
            {
                return false;
            }

            #endregion
        }

        /// <summary>
        /// GetPath is used to obtain the best path between two Routes with the
        /// requirement that the path must go through the intersections in "intersections"
        /// (and ONLY these intersections). This method currently doesn't support a series
        /// of intersections that loop in a circle.
        /// </summary>
        public bool GetPath(Route start, Queue<IntersectionRoute> intersections, Route end, out Queue<Connection> path)
        {
            #region INPUT PROCESSING/VALIDATION

            path = new Queue<Connection>();

            // The given routes must not be null
            if ((start == null) || (end == null))
            {
                Debug.LogError("PathfindingManager.GetPath() was given a null connection");
                return false;
            }

            // If the routes are equivalent, there is no path to give
            // return true, but give an empty path
            if (start == end) return true;

            #endregion


            #region SETUP

            PathNodeComparer pathNodeComparer = new PathNodeComparer();                                 // used to compare the weights of PathNodes when sorting the frontier
            Dictionary<Connection, PathNode> processed = new Dictionary<Connection, PathNode>();        // all processed nodes. used to check if a connection has been processed already
            List<PathNode> frontier = new List<PathNode>();                                             // queue of nearby unprocessed nodes, sorted after each processing step. NOTE: stores "outbound" connections

            // TODO: Don't consider all paths here... only the paths leaving the source Route?
            // get all valid starting connections based on the "start" Route's connections and add to frontier
            foreach (Connection connection in start.Connections)
                if (connection.GetConnectsTo != null)
                    frontier.Add(new PathNode(connection, 0, null));

            PathNode current = frontier[0];

            #endregion


            #region CORE ALGORITHM


            // TODO: Don't initalize monobehavior Intersection like this...
            bool lookForEnd = false;                                        // indicates we've gone through all intersections and are now looking for "end"
            IntersectionRoute intersection = new IntersectionRoute();                 // the current intersection we're looking for (if we aren't looking for "end")
            List<PathNode> destinationPathnodes = new List<PathNode>();     // stores a list of all connections we've tried to process (reached) within the current destination

            while (true)
            {
                // are we looking for an intersection, or the end route?
                if (intersections.Count > 0)
                    intersection = intersections.Dequeue();
                else
                    lookForEnd = true;

                // we may not want to process a connection for multiple reasons: it's in "intersection"/"end", it doesn't have a ConnectsTo, it doesn't have paths leaving it, etc..
                bool processNode = true;

                // reset list of all connections we've tried to process (reached) within the current destination
                destinationPathnodes = new List<PathNode>();

                // upon moving to the next intersection, we didn't find any new connections to explore
                // a path does not exist through the given intersections. Return false
                if (frontier.Count == 0)
                {
                    if (_debugMode) { Debug.LogError("PathfindingManager.GetPath() could not determine a path (unreachable)"); }
                    return false;
                }

                // Begin exploring the frontier. When the frontier is empty, we've processed all reachable connections.
                // These are the voyages of the Starship Enterprise...
                while (frontier.Count > 0)
                {
                    // lowest weight PathNode in Frontier is next to be evaluated
                    frontier.Sort(pathNodeComparer);
                    current = frontier[0];
                    frontier.Remove(current);
                    processNode = true;


                    // CHECK THE NODE WE'RE PROCESSING

                    // if this is true, there exists a connection with a path to it, but no paths leaving it. Ignore
                    if (current.connection.GetConnectsTo == null)
                    {
                        processNode = false;
                        processed.Add(current.connection, current);
                    }

                    else
                    {
                        // search for the next "destination". if we find it, don't process it, but store it in "destinationPathnodes"
                        if (!lookForEnd)
                        {
                            // we're looking for "intersection"
                            if (current.connection.GetConnectsTo.ParentRoute == intersection)
                            {
                                processNode = false;
                                destinationPathnodes.Add(current);
                                processed.Add(current.connection, current);
                            }
                        }

                        else
                        {
                            // we're looking for "end"
                            if (current.connection.GetConnectsTo.ParentRoute == end)
                            {
                                processNode = false;
                                destinationPathnodes.Add(current);
                                processed.Add(current.connection, current);
                            }
                        }

                        // don't process (or explore any further) if we reach an intersection that isn't "intersection" or "end"
                        if ((current.connection.GetConnectsTo.ParentRoute.GetType() == typeof(IntersectionRoute)) && (current.connection.GetConnectsTo.ParentRoute != end) &&
                            (current.connection.GetConnectsTo.ParentRoute != intersection))
                        {
                            processNode = false;
                            processed.Add(current.connection, current);
                        }
                    }




                    // BEGIN PROCESSING

                    if (processNode)
                    {
                        // explore the (current connection => linked inbound connection)'s outbound connections.
                        foreach (Connection.ConnectionPath connectionPath in current.connection.GetConnectsTo.Paths)
                        {
                            // only observe connections we haven't yet processed
                            if (!processed.ContainsKey(connectionPath.Connection))
                            {
                                PathNode discoveredNode;
                                bool newNodeDiscovered = true;
                                float distance = Vector3.Distance(current.connection.GetConnectsTo.gameObject.transform.position, connectionPath.Connection.gameObject.transform.position) + current.distance;
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
                }


                // TRANSITION TO NEXT DESTINATION

                // was the destination == "end" && we found paths to it? if so, we're done!
                if (lookForEnd && (destinationPathnodes.Count > 0))
                    break;

                // we couldn't find a path to the next destination
                if (destinationPathnodes.Count == 0)
                {
                    if (_debugMode) { Debug.LogError("PathfindingManager.GetPath() could not determine a path (unreachable)"); }
                    return false;
                }

                // evaluate the best paths to each reachableOutbound connection in intersection
                else
                {
                    // store and update all reachableOutbound connections within the intersection here.
                    Dictionary<Connection, PathNode> reachableOutbound = new Dictionary<Connection, PathNode>();

                    // note that inbound is actually holding the outbound connection outside the target. Use ConnectsTo to find paths
                    foreach (PathNode inbound in destinationPathnodes)
                    {
                        foreach (var pathsTo in inbound.connection.GetConnectsTo.Paths)
                        {
                            // if we've already added a "path", see if this inbound gets there in a shorter path. If so, replace it with this one
                            float distance = Vector3.Distance(inbound.connection.GetConnectsTo.transform.position, pathsTo.Connection.transform.position) + inbound.distance;
                            if (reachableOutbound.ContainsKey(pathsTo.Connection))
                            {
                                if (reachableOutbound[pathsTo.Connection].distance > distance)
                                {
                                    reachableOutbound.Remove(pathsTo.Connection);
                                    reachableOutbound.Add(pathsTo.Connection, new PathNode(pathsTo.Connection, distance, inbound.connection.GetConnectsTo));
                                }
                            }

                            // otherwise, this is the first inbound that reaches this outbound. Add the path
                            else
                                reachableOutbound.Add(pathsTo.Connection, new PathNode(pathsTo.Connection, distance, inbound.connection));
                        }
                    }

                    // add these new "starting points" to the frontier
                    foreach (PathNode outboundConnection in reachableOutbound.Values)
                        frontier.Add(outboundConnection);
                }
            }

            #endregion


            #region PATH CONSTRUCTION

            // sort the destinationPathnodes by best weight. This "best path" is the output
            destinationPathnodes.Sort(pathNodeComparer);
            current = destinationPathnodes[0];

            // temporarily store the reversed path
            List<Connection> reversePath = new List<Connection>();
            reversePath.Add(current.connection);

            // traverse backwards through the best path (using prevConnection) to construct the path
            while (current.prevConnection != null)
            {
                reversePath.Add(current.prevConnection.GetConnectsTo);
                reversePath.Add(current.prevConnection);
                current = processed[current.prevConnection];
            }

            // add all final connections to the queue in the proper order
            for (int i = reversePath.Count - 1; i >= 0; i--)
                path.Enqueue(reversePath[i]);

            return true;

            #endregion
        }

        #endregion

        #region CURVE GENERATION

        /// <summary>
        /// Creates a BezierCurve component with a path generated by passing a queue of connections
        /// </summary>
        /// <returns>A BezierCurve component</returns>
        public BezierCurve GenerateCurves(Queue<Connection> connections)
        {
            connections = new Queue<Connection>(connections);
            var obj = new GameObject("BezierCurve", typeof(BezierCurve));
            var objCurve = obj.GetComponent<BezierCurve>();

            if (connections.Peek().Paths.Count <= 0)
            {
                connections.Dequeue();
            }

            // traverse each path in _connectionsPath
            while (connections.Count > 0)
            {
                BezierCurve curve;
                Connection current = connections.Dequeue();
                Connection target = connections.Dequeue();

                Debug.Assert(current != null, "Current is null");
                Debug.Assert(target != null, "Target is null");

                // get path between this connection and the next connection
                if (current.GetPathToConnection(target, out curve))
                {
                    objCurve.AddCurve(curve);
                }
                else
                {
                    // no path between two adjacent connections in the queue
                    Debug.LogWarning($"Could not find path between connections");
                }
            }
            return objCurve;
        }

        public void DrawCurve(BezierCurve curve, LineRenderer lineRenderer)
        {
            if (curve.GetAnchorPoints().Any())
            {
                var points = new Vector3[lineRenderer.positionCount];
                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    points[i] = curve.GetPointAt(i / (float)(lineRenderer.positionCount - 1));
                }

                lineRenderer.SetPositions(points);
            }
        }

        public void DrawCurve(BezierCurve curve, LineRenderer lineRenderer, float startFrom)
        {
            if (curve.GetAnchorPoints().Any())
            {
                var points = new Vector3[lineRenderer.positionCount];
                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    points[i] = curve.GetPointAt(startFrom + i / (float)(lineRenderer.positionCount - 1));
                    points[i].y += .1f;
                }

                lineRenderer.SetPositions(points);
            }
        }

        #endregion
    }
}


