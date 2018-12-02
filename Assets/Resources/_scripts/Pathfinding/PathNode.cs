using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public struct PathNode
    {
        public Connection connection;       // the connection associated with this node
        public Connection prevConnection;   // the previous connection in the current best path to this node from the start node
        public float distance;              // the total distance to get to this node from the start node following the current best path

        public PathNode(Connection connection, float distance, Connection prevConnection)
        {
            this.connection = connection;
            this.prevConnection = prevConnection;
            this.distance = distance;
        }
    }
}

