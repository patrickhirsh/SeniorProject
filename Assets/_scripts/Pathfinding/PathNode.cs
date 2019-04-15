using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RideShareLevel
{
    public struct PathNode
    {
        public Connection Connection;       // the connection associated with this node
        public Connection PrevConnection;   // the previous connection in the current best path to this node from the start node
        public float Distance;              // the total distance to get to this node from the start node following the current best path

        public List<Connection.ConnectionPath> NextPaths => Connection.ConnectsTo.Paths;
        public bool HasConnections => Connection.GetConnectsTo != null;
        public Route Route => Connection.ParentRoute;

        public PathNode(Connection connection, float distance, Connection prevConnection)
        {
            Debug.Assert(connection.IsOutbound, "PATHNODE ERROR: connection should be an outbound connection");
            Debug.Assert(prevConnection == null || prevConnection.IsOutbound, "PATHNODE ERROR: prevConnection should be an outbound connection");
            Connection = connection;
            PrevConnection = prevConnection;
            Distance = distance;
        }
    }
}

