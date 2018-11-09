using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level
{
    [ExecuteInEditMode]
    public class Connection : MonoBehaviour
    {
        public const float CONNECTION_DISTANCE = .25f;

        [Serializable]
        public class ConnectionPath
        {
            public Connection Connection;       // per the spec below, this would now just be "connection"
            public BezierCurve Path;
        }

        [HideInInspector]
        [SerializeField]
        public Connection ConnectsTo;

        [HideInInspector]
        [SerializeField]
        public Entity ParentEntity;

        [ReadOnly]
        public List<ConnectionPath> Paths = new List<ConnectionPath>();

        private Dictionary<Connection, BezierCurve> _connectionPaths;
        private Dictionary<Connection, BezierCurve> ConnectionPaths => _connectionPaths ?? (_connectionPaths = Paths.ToDictionary(path => path.Connection, path => path.Path));
        public Connection[] InnerConnections => ConnectionPaths.Keys.ToArray();
        #region Unity Methods

        protected virtual void OnDrawGizmosSelected()
        {
            foreach (var connectionPath in Paths)
            {
                if (Paths.Count(path => path.Connection == connectionPath.Connection) > 1)
                {
                    Debug.LogError($"Multiple paths to a connection detected!", gameObject);
                }
            }
        }

        private void OnValidate()
        {
            foreach (var connectionPath in Paths)
            {
                if (Paths.Count(path => path == connectionPath) > 1)
                {
                    Paths.Remove(connectionPath);
                    Debug.LogError($"Removed duplicate path on {gameObject}");
                }
            }
        }
        #endregion

        public void Bake()
        {
            ConnectsTo = EntityManager.Instance.Connections.FirstOrDefault(CanConnect);
            ParentEntity = transform.GetComponentInParent<Entity>();
        }

        public void Setup()
        {
            ConnectsTo = EntityManager.Instance.Connections.FirstOrDefault(CanConnect);
            ParentEntity = transform.GetComponentInParent<Entity>();
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
        /// a function in PathfinderManager
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
            if (connection == this || connection.ParentEntity == ParentEntity) return false;
            return Vector3.Distance(transform.position, connection.transform.position) < CONNECTION_DISTANCE;
        }
    }


}