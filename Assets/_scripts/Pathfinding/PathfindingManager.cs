using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Level
{
    public class PathfindingManager : MonoBehaviour
    {

        EntityManager _entityManager;    // PathfindingManager utilizes structures from the scene's EntityManager


        // Use this for initialization
        void Start()
        {
            _entityManager = EntityManager.Instance;
        }



        /// <summary>
        /// Given a starting point and an end point, populate "path" with the best path between
        /// two connection points. 
        /// 
        /// USAGE: 
        /// 
        /// 
        /// NOTE: This algorithm follows a few invarients to ensure we always get the best path:
        /// - If start is an outbound connection, we always convert it to its connecting inbound connection. 
        ///     Other types (ie. ParkingSpot) are left as-is
        /// - After the initial connection, our explore will ONLY consider Connections of the type: Inbound or Outbound
        ///     - The exception to this is the end Connection, which is individually checked and can be any type
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool GetPath(Connection start, Connection end, out List<BezierCurve> path)
        {
            // clear output path and ensure the parameters passed are valid
            path = new List<BezierCurve>();
            if ((start == null) || (end == null)) return false;
            // TODO: Check to ensure that if the end connection is a parking spot, it isn't occupied

            // if we're given an outbound connection, start at the next inbound connection (since that's the only place we can move to)
            if (start.Traveling == Connection.TravelingDirection.Outbound)
            {
                start = start.ConnectsTo;
                if (start == null) return false;    // we were given an outbound connection with no linked inbound connection
            }

            Dictionary<Connection, PathNode> visited = new Dictionary<Connection, PathNode>();      // all visited nodes. used to check if a connection has been visited
            SortedSet<PathNode> frontier = new SortedSet<PathNode>(new PathNodeComparer());         // queue of nearby unvisited nodes, stored as PathNodes so was can sort by weight
            PathNode current = new PathNode(start, 0, null);
            visited.Add(start, current);
            frontier.Add(current);

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


