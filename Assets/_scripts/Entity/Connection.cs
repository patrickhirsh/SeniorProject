using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grid = Utility.Grid;

namespace Level
{
    public class Connection : MonoBehaviour
    {
        public ConnectionType Type;
        public enum ConnectionType
        {
            Inbound = 0,
            Outbound = 1,
            Internal = 2
        }

        public Connection ConnectsTo;
        public Entity parentEntity;
        public Entity ConnectingEntity;

        [Serializable]
        public class ConnectionPath
        {
            public Connection OutboundConnection;       // per the spec below, this would now just be "connection"
            public BezierCurve Path;
        }

        // TODO: include internal connection types for applicable entities (parking, for now)
        // Pathfinding needs to check Paths within a Parking entity's inbound connection to find it's inbound connections
        // NOTE: This means Paths will no longer ONLY contain associated outbound connections. It could also contain internal connections.
        // this means Austin's path drawing probably needs an aditional if statement to make sure he's only looking at outbound connections
        public List<ConnectionPath> Paths = new List<ConnectionPath>();

        private Dictionary<Connection, BezierCurve> _connectionPaths;
        private Dictionary<Connection, BezierCurve> ConnectionPaths => _connectionPaths ?? (_connectionPaths = Paths.ToDictionary(path => path.OutboundConnection, path => path.Path));

        protected Node Node => GetComponentInParent<Node>();

        #region Unity Methods

        protected void Update()
        {
            if (transform.hasChanged)
            {
                SnapToValidPosition();
                transform.hasChanged = false;
            }
        }

        protected void OnValidate()
        {
            if (Type == ConnectionType.Inbound)
            {
                foreach (var connection in Paths)
                {
                    if (connection.OutboundConnection.Type == ConnectionType.Inbound)
                    {
                        connection.OutboundConnection = null;
                        Debug.LogError("Can't connect to an inbound connection", gameObject);
                    }
                }
            }

        }

        #endregion

        public void Setup(Entity parent)
        {
            this.parentEntity = parent;
            CalculateConnections();
        }

        public Connection[] ConnectedConnections()
        {
            return ConnectionPaths.Keys.ToArray();
        }

        /// <summary>
        /// Gets the BezierCurve path from this connection to the given connection.
        /// returns false if no such connection exists.
        /// 
        /// This method only returns paths within the current connection's entity.
        /// If you want a path that spans entities, you're probably looking for
        /// a function in PathfinderManager
        /// </summary>
        public bool GetPathToConnection(Connection connection, out BezierCurve path)
        {
            path = null;
            if (connection.Type == ConnectionType.Inbound)
            {
                Debug.LogError("Can't get path to an inbound connection");
                return false;
            }

            // Check if we have a path to the connection
            if (!ConnectionPaths.ContainsKey(connection)) return false;

            // Return the path to the connection
            path = ConnectionPaths[connection];
            return true;
        }

        public void CalculateConnections()
        {
            if (Type == ConnectionType.Inbound) return;

            ConnectsTo = EntityManager.Instance.InboundConnections.FirstOrDefault(connection =>
                Vector3.Distance(transform.position, connection.transform.position)
                < Mathf.Max(Grid.CELL_SIZE_X, Grid.CELL_SIZE_Z));

            ConnectingEntity = ConnectsTo?.Node.Entity;
        }

        private void SnapToValidPosition()
        {
            Vector3 closest = Vector3.positiveInfinity;
            foreach (var point in Node.ValidConnectionPoints())
            {
                if (Vector3.Distance(point, transform.position) < Vector3.Distance(closest, transform.position))
                {
                    closest = point;
                }
            }

            transform.position = closest;
        }
    }


}