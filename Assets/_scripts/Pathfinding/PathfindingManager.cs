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
            connections = new Queue<Connection>();

            // input validation
            Debug.Assert(start != null, "Start route to GetPath should not be null");
            Debug.Assert(destination != null, "Start route to GetPath should not be null");

            if (start == destination) return true;

            PathNodeComparer pathNodeComparer = new PathNodeComparer();                                 // used to compare the weights of PathNodes when sorting the frontier
            Dictionary<Connection, PathNode> processed = new Dictionary<Connection, PathNode>();        // all processed nodes. used to check if a connection has been processed already
            List<PathNode> frontier = start.Connections
                .Where(connection => !connection.HasPaths) // Take only outbound
                .Select(connection => new PathNode(connection, 0, null)) // Convert to PathNode
                .ToList();

            Debug.Assert(frontier.Any(), "Frontier does not have any connections");

            // These are the voyages of the Starship Enterprise...
            while (frontier.Count > 0)
            {
                // lowest weight PathNode in Frontier is next to be evaluated
                frontier.Sort(pathNodeComparer);
                var current = frontier[0];
                frontier.Remove(current);

                // if we're processing the end node, we've found the shortest path to it!
                if (current.Route == destination)
                {
                    connections = ConstructPath(ref current, ref processed);
                    return true;
                }

                // If there are paths in the connecting connection, then process it
                if (current.HasConnections)
                {
                    ProcessNode(ref current, ref processed, ref frontier);
                }
                else
                {
                    // not all nodes need to be processed (see above) but ALL nodes should be added to processed at this stage
                    processed.Add(current.Connection, current);
                }
            }

            return false;
        }


        /// <summary>
        /// Processes "current" in Dijkstra's fashion. This method requires references to the caller's processed and frontier
        /// data structures in order to mutate them during processing.
        /// </summary>
        private void ProcessNode(ref PathNode current, ref Dictionary<Connection, PathNode> processed, ref List<PathNode> frontier)
        {
            // explore the (current connection => linked inbound connection)'s outbound connections.
            foreach (Connection.ConnectionPath path in current.NextPaths)
            {
                // only observe connections we haven't yet processed
                if (!processed.ContainsKey(path.NextConnection))
                {
                    PathNode discoveredNode;
                    bool newNodeDiscovered = true;
                    float distance = Vector3.Distance(current.Connection.GetConnectsTo.transform.position, path.NextConnection.transform.position) + current.Distance;
                    // TODO: add additional calculated weight here... (vehicles currently in path, etc.)

                    // check if this connection has already been discovered (is it in the frontier?)
                    foreach (PathNode node in frontier)
                        if (node.Connection == path.NextConnection)
                        {
                            // we've already discovered this node!
                            discoveredNode = node;
                            newNodeDiscovered = false;

                            // is this path better than its current path? If so, change its best path to this one. If not, move on
                            if (discoveredNode.Distance > distance)
                            {
                                discoveredNode.Distance = distance;
                                discoveredNode.PrevConnection = current.Connection;
                            }
                            break;
                        }

                    // this connection has never been discovered before. Add it to the frontier!
                    if (newNodeDiscovered)
                    {
                        discoveredNode = new PathNode(path.NextConnection, distance, current.Connection);
                        frontier.Add(discoveredNode);
                    }
                }
            }
            processed.Add(current.Connection, current);
        }


        /// <summary>
        /// This method traverses backwards through current's previous connections to construct the contained path.
        /// The "processed" dictionary associated with the caller GetPath() method is required to map each PathNode to its connection.
        /// </summary>
        private Queue<Connection> ConstructPath(ref PathNode current, ref Dictionary<Connection, PathNode> processed)
        {

            // temporarily store the reversed path
            List<Connection> reversePath = new List<Connection>();

            // traverse backwards through the best path (using prevConnection) to construct the path
            while (current.PrevConnection != null)
            {
                reversePath.Add(current.Connection);
                if (current.Connection.CanPathToConnection(current.PrevConnection))
                {
                    reversePath.Add(current.PrevConnection);
                }
                else
                {
                    reversePath.Add(current.PrevConnection.ConnectsTo);
                }
                current = processed[current.PrevConnection];
            }

            // add all final connections to the queue in the proper order
            Queue<Connection> path = new Queue<Connection>();
            for (int i = reversePath.Count - 1; i >= 0; i--)
                path.Enqueue(reversePath[i]);

            return path;
        }

        #endregion

        #region CURVE GENERATION

        /// <summary>
        /// Creates a BezierCurve component with a path generated by passing a queue of connections
        /// </summary>
        /// <returns>A BezierCurve component</returns>
        public BezierCurve GenerateCurves(Queue<Connection> connections)
        {
            Debug.Assert(connections != null, "GENERATECURVES ERROR: Connections should not be null");
            Debug.Assert(connections.Any(), "GENERATECURVES ERROR:No connections found in queue");
            Debug.Assert(connections.Count % 2 == 0, "Even amount of connections is expected");

            connections = new Queue<Connection>(connections);
            var obj = new GameObject("BezierCurve", typeof(BezierCurve));
            var bezierCurve = obj.GetComponent<BezierCurve>();

            Debug.Assert(connections.Peek().HasPaths, "First connection does not have paths");

            // traverse each path in _connectionsPath
            while (connections.Count > 2)
            {
                var current = connections.Dequeue();
                var target = connections.Dequeue();

                // get path between this connection and the next connection
                if (current.GetPathToConnection(target, out var curve))
                {
                    bezierCurve.AddCurve(curve);
                }
                else
                {
                    // no path between two adjacent connections in the queue
                    Debug.LogError($"GENERATECURVES ERROR: Could not find path between connections");
                }
            }
            return bezierCurve;
        }

        public void DrawCurve(BezierCurve curve, LineRenderer lineRenderer, float start = 0f, float end = 1f)
        {
            end = Mathf.Min(1f, end);
            start = Mathf.Max(0f, start);

            if (curve.GetAnchorPoints().Any())
            {
                var points = new Vector3[lineRenderer.positionCount];
                for (int i = 0; i < lineRenderer.positionCount; i += 1)
                {
                    var t = start + i * (end - start) / lineRenderer.positionCount;
                    points[i] = curve.GetPointAt(t);
                    points[i].y += .1f;
                }

                lineRenderer.SetPositions(points);
            }
        }

        #endregion
    }
}


