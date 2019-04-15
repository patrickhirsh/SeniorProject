using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening.Plugins.Core.PathCore;
using UnityEngine;

namespace RideShareLevel
{
    [ExecuteInEditMode]
    public class Connection : LevelObject
    {
        public const float CONNECTION_DISTANCE = .5f;

        [Serializable]
        public class ConnectionPath
        {
            public Connection NextConnection;
            public BezierCurve Path;
        }

        [HideInInspector]
        public Connection ConnectsTo;
        public Connection GetConnectsTo => ConnectsTo;
        public bool ConnectsToRoute => GetConnectsTo != null;
        public bool IsInbound => Paths.Any();
        public bool IsOutbound => !IsInbound;

        [HideInInspector]
        [SerializeField]
        public Route ParentRoute;

        [ReadOnly]
        public List<ConnectionPath> Paths = new List<ConnectionPath>();

        [ReadOnly]
        public List<Terminal> Terminals = new List<Terminal>();

        public int PathCount
        {
            get
            {
                Debug.Assert(ConnectionPaths != null, "ConnectionPaths is null!");
                return ConnectionPaths.Count;
            }
        }

        // This can't be serialized, so we compute once at runtime
        private Dictionary<Connection, BezierCurve> _connectionPaths;
        private Dictionary<Connection, BezierCurve> ConnectionPaths => _connectionPaths ?? (_connectionPaths = Paths.ToDictionary(path => path.NextConnection, path => path.Path));

        #region Unity Methods

        private void OnDrawGizmos()
        {
            Gizmos.color = IsInbound ? Color.blue : Color.red;
            Gizmos.DrawSphere(transform.position, .05f);
        }

        #endregion

#if UNITY_EDITOR
        public void Bake()
        {
            UnityEditor.Undo.RecordObject(this, "Bake Connection");
            ParentRoute = GetComponentInParent<Route>();
            ConnectsTo = FindNeighborConnection(CurrentLevel.EntityController.Connections);

            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }

        public void ValidatePaths()
        {
            var uniqueConnections = Paths.Select(path => path.NextConnection).Where(connection => connection != null).Distinct().Count();
            Debug.Assert(uniqueConnections == Paths.Count, "Multiple paths to a connection detected!", gameObject);
        }
#endif

        /// <summary>
        /// Finds a neighbor connection given an array of connections to look from
        /// </summary>
        private Connection FindNeighborConnection(Connection[] connections)
        {
            return connections.FirstOrDefault(CanConnect);
        }

        public bool CanPathToConnection(Connection connection)
        {
            return ConnectionPaths.ContainsKey(connection);
        }

        /// <summary>
        /// Gets the BezierCurve path from this connection to the given connection.
        /// returns false if no such connection exists.
        /// 
        /// This method only returns paths within the current connection's entity.
        /// If you want a path that spans entities, you're probably looking for
        /// PathfinderManager.FindPath()
        /// </summary>
        public bool GetPathToConnection(Connection connection, out BezierCurve path)
        {
            path = null;

            // Check if we have a path to the connection
            if (!CanPathToConnection(connection)) return false;

            // Return the path to the connection
            path = ConnectionPaths[connection];
            return true;
        }

        private bool CanConnect(Connection connection)
        {
            if (connection == this || connection.ParentRoute == ParentRoute) return false;
            return Vector3.Distance(transform.position, connection.transform.position) < CONNECTION_DISTANCE;
        }
    }
}