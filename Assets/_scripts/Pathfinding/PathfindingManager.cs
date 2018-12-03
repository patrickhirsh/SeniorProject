using System.Collections.Generic;
using System.Linq;
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
                if (current.connection.ConnectsTo == null)
                    { processNode = false; }                      


                // BEGIN PROCESSING

                // only processed if (processNode)
                if (processNode)
                {
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
                    reversePath.Add(current.prevConnection.ConnectsTo);
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
                if (connection.ConnectsTo != null)
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
                    if (current.connection.ConnectsTo == null)
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
                            if (current.connection.ConnectsTo.ParentRoute == intersection)
                            {
                                processNode = false;
                                destinationPathnodes.Add(current);
                                processed.Add(current.connection, current);
                            }
                        }

                        else
                        {
                            // we're looking for "end"
                            if (current.connection.ConnectsTo.ParentRoute == end)
                            {
                                processNode = false;
                                destinationPathnodes.Add(current);
                                processed.Add(current.connection, current);
                            }
                        }

                        // don't process (or explore any further) if we reach an intersection that isn't "intersection" or "end"
                        if ((current.connection.ConnectsTo.ParentRoute.GetType() == typeof(IntersectionRoute)) && (current.connection.ConnectsTo.ParentRoute != end) &&
                            (current.connection.ConnectsTo.ParentRoute != intersection))
                        {
                            processNode = false;
                            processed.Add(current.connection, current);
                        }
                    }




                    // BEGIN PROCESSING

                    if (processNode)
                    {
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
                        foreach (var pathsTo in inbound.connection.ConnectsTo.Paths)
                        {
                            // if we've already added a "path", see if this inbound gets there in a shorter path. If so, replace it with this one
                            float distance = Vector3.Distance(inbound.connection.ConnectsTo.transform.position, pathsTo.Connection.transform.position) + inbound.distance;
                            if (reachableOutbound.ContainsKey(pathsTo.Connection))
                            {
                                if (reachableOutbound[pathsTo.Connection].distance > distance)
                                {
                                    reachableOutbound.Remove(pathsTo.Connection);
                                    reachableOutbound.Add(pathsTo.Connection, new PathNode(pathsTo.Connection, distance, inbound.connection.ConnectsTo));
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
                reversePath.Add(current.prevConnection.ConnectsTo);
                reversePath.Add(current.prevConnection);
                current = processed[current.prevConnection];
            }

            // add all final connections to the queue in the proper order
            for (int i = reversePath.Count - 1; i >= 0; i--)
                path.Enqueue(reversePath[i]);

            return true;

            #endregion
        }


        #region BEZIER CURVE CONSTRUCTION
        //BezierCurve curve = new BezierCurve();

        //// traverse backwards through the best path (using prevConnection) to construct the path
        //while (true)
        //{
        //    // we should always be able to traverse backwards until we reach the else case and break. Otherwise, we have a fault in the best path linked list
        //    Debug.Assert(current.prevConnection != null);

        //    // keep an eye out for the start connection as we construct the path
        //    if (current.prevConnection != start)
        //    {
        //        // step backwards to the previous connection, then get the curve to the current connection
        //        if (!current.prevConnection.GetPathToConnection(current.connection, out curve))
        //            if (_debugMode) { Debug.LogError("GetPath() Generated an invalid path"); }
        //        path.Add(curve);
        //        current = processed[current.prevConnection]; 
        //    }

        //    // we've reached the first (start) connection. Generate the last path and exit
        //    else
        //    {
        //        if (!current.prevConnection.GetPathToConnection(current.connection, out curve))
        //            if (_debugMode) { Debug.LogError("GetPath() Generated an invalid path"); }
        //        path.Add(curve);
        //        break;
        //    }
        //}
        //path.Reverse();     // we generated the list in reverese order
        //return true;
        #endregion

        #region AMBIGUOUS PATHFINDING
        /*
        public bool GetPath(Route start, Queue<Intersection> intersections, Route end, out Queue<Connection> path)
        {
            // get all valid starting connections based on the "start" Route's connections
            List<Connection> startingConnections = new List<Connection>();
            foreach (Connection connection in start.Connections)
                if (connection.Paths.Count == 0)
                    if (connection.ConnectsTo != null)
                        startingConnections.Add(connection.ConnectsTo);

            Dictionary<Connection, List<AmbiguousPath>> bestPaths = new Dictionary<Connection, List<AmbiguousPath>>();

            foreach (Intersection intersection in intersections)
            {

                // TODO:
                /*
                 * - within the below foreach loop, pop the associated KeyValuePair<Connection, List<AmbiguousPath>> from bestPaths
                 *   and merge it with all paths returned from Explore. Add these paths back into bestPaths under the same Connection key
                 * 
                 * - do this for all startingConnections. Keep in mind, a startingConnection can be associated with the end of multiple existing bestPaths...
                 *   maybe cache these and add them all at once so we can keep the original state of bestPaths while we need it?
                 * 
                 * - once all merged varients have been added, call the in-place selectBestPaths() to remove all but the best paths
                 * 
                 * - set starting connections to all reachable connections from the keys in bestPaths. Maybe cache which connection goes where, or something,
                 *   so it's easier to find out which startingConnection is associated with which Connection key in bestPath?
                 *   
                 *   
                 *   after we've iterated through all intersections...
                 * 
                 * - design some kind of special case function for finding the last Route object (if it's an intersection, we can just do the same thing, maybe?
                 * 
                 * - if bestPaths is ever empty, there isn't a route to end (I think?)
                 


                Dictionary<Connection, List<AmbiguousPath>> allPaths = new Dictionary<Connection, List<AmbiguousPath>>();

                // find the best path from each startingConnection to each intersection connection reachable from that startingConnection
                foreach (Connection startingConnection in startingConnections)
                {
                    // get all best paths
                    Dictionary<Connection, AmbiguousPath> pathsFromStartingConnection = new Dictionary<Connection, AmbiguousPath>();
                    pathsFromStartingConnection = Explore(startingConnection, intersection);

                    // add each path to allPaths, mapped by the intersection connection they connect to
                    foreach (KeyValuePair<Connection, AmbiguousPath> pathFromStartingConnection in pathsFromStartingConnection)
                        allPaths[pathFromStartingConnection.Key].Add(pathFromStartingConnection.Value);
                }

                
            }
            return true;
            
            
        }


        /// <summary>
        /// Given a starting connection and a target intersection, Explore returns a dictionary that maps
        /// each reachable connection in the targetIntersection to its best AmbiguousPath.
        /// </summary>
        private Dictionary<Connection, AmbiguousPath> Explore(Connection startingConnection, Intersection targetIntersection)
        {
            // all paths that reach the targetIntersection, mapped to the connection on the targetIntersection that they're reaching
            // value is stored as a List<AP> because a split in the ambiguous path could cause two different paths to reach the same connection in the target
            Dictionary<Connection, List<AmbiguousPath>> allPaths = new Dictionary<Connection, List<AmbiguousPath>>();

            AmbiguousPath potentialPath = new AmbiguousPath();      // the ambiguous path we're currently constructing
            Connection currentConnection = startingConnection;      // the connection we're currently evaluating. (potentialPath's last connection).ConnectsTo
            bool reachedIntersection = false;

            while (true)
            {
                // we've reached an intersection! break and check below if it's our target intersection
                if (currentConnection.ParentRoute.GetType() == typeof(Intersection))
                {
                    reachedIntersection = true;
                    break;
                }                  

                // we've reached a non-intersection connection that has more than one path leaving it (ie. 2 adjacent lanes with a lane change possibility)
                if (currentConnection.Paths.Count > 1)
                {
                    // will populate with the best APs from all paths in currentConnection.Paths
                    Dictionary<Connection, List<AmbiguousPath>> bestAPsFromPaths = new Dictionary<Connection, List<AmbiguousPath>>();

                    // recursively explore currentConnection's paths
                    foreach (var path in currentConnection.Paths)
                    {
                        // explore path's connections and get the best APs to all reachable connection on the targetIntersection
                        Dictionary<Connection, AmbiguousPath> bestAPsFromPath = new Dictionary<Connection, AmbiguousPath>();
                        bestAPsFromPath =  Explore(path.Connection.ConnectsTo, targetIntersection);

                        // apply the weight from the current connection to each of the start connections in these paths
                        foreach (KeyValuePair<Connection, AmbiguousPath> bestPath in bestAPsFromPath)
                            bestPath.Value.weight += Vector3.Distance(currentConnection.transform.position, bestPath.Value.path[0].transform.position);

                        // merge each of these paths with the path in progress. Since these paths all reach targetIntersection, we're done!
                        foreach (KeyValuePair<Connection, AmbiguousPath> mergedPath in AmbiguousPath.MergePaths(potentialPath, bestAPsFromPath))
                            allPaths[mergedPath.Key].Add(mergedPath.Value);

                        break;
                    }
                }

                // we've reached a non-intersection connection that has 0 or 1 paths
                else
                {
                    // currentConnection does not have any paths leaving it
                    if (currentConnection.Paths.Count <= 0)
                    {
                        // this is a dead end... It shouldn't ever happen, but if it does, don't consider this path (break immediately)
                        reachedIntersection = false;
                        break;
                    }

                    // exactly one path is leaving this connection. Add its weight, traverse forward, and keep searching
                    potentialPath.weight += Vector3.Distance(potentialPath.path[potentialPath.path.Count - 1].transform.position, currentConnection.transform.position);
                    potentialPath.path.Add(currentConnection);
                    currentConnection = currentConnection.Paths[0].Connection.ConnectsTo;
                }
            }

            // we've found an intersection. Check if this intersection is the target intersection
            if (reachedIntersection)
            {
                // we found the target!
                if (currentConnection.ParentRoute == targetIntersection)
                {
                    // add the weight of the final connection, add that connection to the potentialPath, and add the potentialPath to the Dictionary of valid paths!
                    potentialPath.weight += Vector3.Distance(potentialPath.path[potentialPath.path.Count - 1].transform.position, currentConnection.transform.position);
                    potentialPath.path.Add(currentConnection);
                    allPaths[startingConnection].Add(potentialPath);
                }
            }

            // find the best path that leads to each reachable connection in the targetIntersection from startingConnection and remove the rest
            return selectBestPaths(allPaths);
        }


        /// <summary>
        /// Given a dictionary with multiple AmbiguousPaths to each end connection, selectBestPaths()
        /// removes all AmbiguousPaths that lead to the same connection except for the one with the lowest weight.
        /// </summary>
        public Dictionary<Connection, AmbiguousPath> selectBestPaths(Dictionary<Connection, List<AmbiguousPath>> paths)
        {
            Dictionary<Connection, AmbiguousPath> output = new Dictionary<Connection, AmbiguousPath>();
            AmbiguousPathComparer comparer = new AmbiguousPathComparer();

            foreach (KeyValuePair<Connection, List<AmbiguousPath>> pathsToConnection in paths)
            {
                pathsToConnection.Value.Sort(comparer);
                output.Add(pathsToConnection.Key, pathsToConnection.Value[0]);
            }

            return output;
        }


        /// <summary>
        /// Given a dictionary with multiple AmbiguousPaths to each end connection, selectBestPaths()
        /// removes all AmbiguousPaths that lead to the same connection except for the one with the lowest weight.
        /// This varient of selectBestPaths performs the operation in-place
        /// </summary>
        public void selectBestPaths(ref Dictionary<Connection, List<AmbiguousPath>> paths)
        {
            AmbiguousPathComparer comparer = new AmbiguousPathComparer();

            foreach (KeyValuePair<Connection, List<AmbiguousPath>> pathsToConnection in paths)
            {
                pathsToConnection.Value.Sort(comparer);
                for (int i = pathsToConnection.Value.Count - 1; i > 0; i--)
                    pathsToConnection.Value.RemoveAt(i);
            }
        }

        */
        #endregion


        #region CURVE GENERATION

        /// <summary>
        /// Creates a BezierCurve component with a path generated by passing a queue of connections
        /// </summary>
        /// <returns>A BezierCurve component</returns>
        public BezierCurve GeneratePath(Queue<Connection> connections)
        {
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

        public void DrawPath(BezierCurve curve, LineRenderer lineRenderer)
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


        public void DrawPath(BezierCurve curve, LineRenderer lineRenderer, float startFrom)
        {
            if (curve.GetAnchorPoints().Any())
            {
                var points = new Vector3[lineRenderer.positionCount];
                for (int i = 0; i < lineRenderer.positionCount; i++)
                {
                    points[i] = curve.GetPointAt(startFrom + i / (float)(lineRenderer.positionCount - 1));
                }

                lineRenderer.SetPositions(points);
            }
        }

        #endregion
    }
}


