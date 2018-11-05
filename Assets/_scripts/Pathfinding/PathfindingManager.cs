using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Level
{
    public class PathfindingManager : MonoBehaviour
    {
        private bool debugMode = true;
        EntityManager _entityManager;    // PathfindingManager utilizes structures from the scene's EntityManager


        // Use this for initialization
        void Start()
        {
            _entityManager = EntityManager.Instance;
        }



        /// <summary>
        /// GetExternalPath is used to obtain a path between any two Inbound/Outbound connections. This should be used
        /// to find paths between entities.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool GetExternalPath(Connection start, Connection end, out List<BezierCurve> path)
        {
            path = new List<BezierCurve>();


            #region INPUT PROCESSING/VALIDATION
            
            // The given connections must not be null
            if ((start == null) || (end == null))
            {
                Debug.LogError("GetPath was given a null connection");
                return false;
            }

            // Start connection must be either inbound or outbound
            if ((start.Type != Connection.ConnectionType.Inbound) && (start.Type != Connection.ConnectionType.Outbound))
            {
                if (debugMode) { Debug.LogError("GetPath was given an invalid starting connection"); }
                return false;
            }

            // Start connection must be either inbound or outbound
            if ((end.Type != Connection.ConnectionType.Inbound) && (end.Type != Connection.ConnectionType.Outbound))
            {
                if (debugMode) { Debug.LogError("GetPath was given an invalid ending connection"); }
                return false;
            }            

            // if we're given an outbound connection, start at the next inbound connection (since that's the only place we can move to)
            if (start.Type == Connection.ConnectionType.Outbound)
            {
                start = start.ConnectsTo;
                if (start == null) return false;    // we were given an outbound connection with no linked inbound connection
            }

            #endregion


            #region SETUP

            Dictionary<Connection, PathNode> visited = new Dictionary<Connection, PathNode>();      // all visited nodes. used to check if a connection has been visited
            SortedSet<PathNode> frontier = new SortedSet<PathNode>(new PathNodeComparer());         // queue of nearby unvisited nodes, stored as PathNodes so was can sort by weight
            PathNode current = new PathNode(start, 0, null);
            visited.Add(start, current);
            frontier.Add(current);

            #endregion

            // TODO: Support more connection types than just inbound/outbound

            // How do I handle differences between inbound and outbound? does it matter?
            // is "Paths" only relevant for inbound connections? what does "Paths" look like for outbound connections?

            while (frontier.Count > 0)
            {
                // lowest weight PathNode in Frontier is next to be evaluated
                current = frontier.Min;
                frontier.Remove(current);
            }

            return false;
        }
    }
}


