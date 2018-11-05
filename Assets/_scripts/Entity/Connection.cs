using DG.Tweening;
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
            Inbound,
            Outbound
        }

        public Connection ConnectsTo;
        public Entity parentEntity;
        public Entity ConnectingEntity;

        [Serializable]
        public class ConnectionPath
        {
            public Connection OutboundConnection;
            public BezierCurve Path;
        }

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
            foreach (var connection in Paths)
            {
                if (connection.OutboundConnection.Type == ConnectionType.Inbound)
                {
                    connection.OutboundConnection = null;
                    Debug.LogError("Can't connect to an inbound connection", gameObject);
                }
            }
        }

        #endregion

        public void Setup(Entity parent)
        {
            this.parentEntity = parent;
            CalculateConnections();
        }

        /// <summary>
        /// Finds a path from this connection to connection
        /// </summary>
        public bool FindPathToConnection(Connection connection, out BezierCurve path)
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