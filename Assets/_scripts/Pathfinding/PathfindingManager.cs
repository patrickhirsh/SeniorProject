using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Level
{
    // TODO: Take a look at the way ConnectsTo is being used here. This needs to be re-evaluated...
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


        public bool GetPath(Route start, Queue<Intersection> intersections, Route end, out Queue<Connection> path)
        {
            // get all valid starting connections based on the "start" Route's connections


            //This should be a Dictionary<Connection, Ambiguouspath>, because it's only going to hold the single "best" path for a given connection, not a list of them
            Dictionary<Connection, AmbiguousPath> bestPaths = new Dictionary<Connection, AmbiguousPath>();

            //This should go from intersection to intersection, appending the best path from one to the next onto the end of each AmbigiousPath.path in bestpaths

            Route CurrentRoute = start;

            foreach (Intersection intersection in intersections)
            {

                List<Connection> CurrentRouteConnections = new List<Connection>();
                foreach (Connection connection in CurrentRoute.Connections)
                    if (connection.Paths.Count > 0)
                        if (connection.ConnectsTo != null)
                            CurrentRouteConnections.Add(connection.ConnectsTo);

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
                 */


                Dictionary<Connection, List<AmbiguousPath>> allPaths = new Dictionary<Connection, List<AmbiguousPath>>();

                // find the best path from each startingConnection to each intersection connection reachable from that startingConnection
                foreach (Connection startingConnection in CurrentRouteConnections)
                {
                    // get all best paths
                    Dictionary<Connection, AmbiguousPath> pathsFromStartingConnection = new Dictionary<Connection, AmbiguousPath>();
                    pathsFromStartingConnection = Explore(startingConnection, intersection);

                    // add each path to allPaths, mapped by the intersection connection they connect to
                    foreach (KeyValuePair<Connection, AmbiguousPath> pathFromStartingConnection in pathsFromStartingConnection)
                    {
                        if (!allPaths.ContainsKey(pathFromStartingConnection.Key))
                        {
                            allPaths.Add(pathFromStartingConnection.Key, new List<AmbiguousPath>());
                        }
                        allPaths[pathFromStartingConnection.Key].Add(pathFromStartingConnection.Value);
                    }
                        

                    //foreach keyvalue pair of connections to the best ambigious path found from the starting connection to the next intersection
                    foreach(KeyValuePair<Connection, AmbiguousPath> valuePair in selectBestPaths(allPaths))
                    {
                        //Add all of the connections onto the end corresponding ambiguous path in the best paths dict
                        //Best paths should already hold the best path for the corresponding connection, and we've just found the best path from the starting connection to the best intersection
                        //So adding onto the end should be fine
                       foreach(Connection x in valuePair.Value.path)
                        {
                            if (!bestPaths.ContainsKey(valuePair.Key))
                            {
                                bestPaths.Add(valuePair.Key, new AmbiguousPath());
                            }
                            bestPaths[valuePair.Key].path.Add(x);
                        }
                    }
                }

                CurrentRoute = intersection;
                
            }

            //Now we should have a Dictionary with all the connections and their best paths, so what we need to do is find the shortest ambigious path
            //and then enque all those connections, starting with the key, then adding all the Ambigious.path connections

            //Use ambgious path comparer to find shortest path from best paths
            AmbiguousPathComparer pathComparer = new AmbiguousPathComparer();

            List<AmbiguousPath> finalList = new List<AmbiguousPath>(bestPaths.Values);

            Queue<Connection> returnQ = new Queue<Connection>();

            finalList.Sort(pathComparer);
            
            //Foreach connection in the absolute shortest path  
            foreach(Connection finalConnection in finalList[0].path)
            {
                returnQ.Enqueue(finalConnection);
            }

            path = returnQ;

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
                        bestAPsFromPath = Explore(path.Connection.ConnectsTo, targetIntersection);

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

                    //if there are already connections in potential path, calculate weight
                    if (potentialPath.path.Count > 1)
                    {
                        potentialPath.weight += Vector3.Distance(potentialPath.path[potentialPath.path.Count - 1].transform.position, currentConnection.transform.position);
                        potentialPath.path.Add(currentConnection);
                        currentConnection = currentConnection.Paths[0].Connection.ConnectsTo;
                    }
                    //otherwise, need to add first connection to potential paths
                    else
                    {
                        potentialPath.path.Add(currentConnection);
                        currentConnection = currentConnection.Paths[0].Connection.ConnectsTo;
                    }
                

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
                    if (allPaths.ContainsKey(startingConnection))
                    {
                        allPaths[startingConnection].Add(potentialPath);
                    }
                    else
                    {
                        allPaths.Add(startingConnection, new List<AmbiguousPath>());
                        allPaths[startingConnection].Add(potentialPath);
                    }
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
    }
}


