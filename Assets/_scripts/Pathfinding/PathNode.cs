using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public struct PathNode
    {
        public Connection connection;
        public Connection prevConnection;
        public float weight;

        public PathNode(Connection connection, float weight, Connection prevConnection)
        {
            this.connection = connection;
            this.prevConnection = prevConnection;
            this.weight = weight;
        }
    }
}

