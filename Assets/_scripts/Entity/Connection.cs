using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level
{
    [ExecuteInEditMode]
    public class Connection : MonoBehaviour
    {
        public const float CONNECTION_DISTANCE = .5f;

        [Serializable]
        public class ConnectionPath
        {
            public Connection Connection;
            public BezierCurve Path;
        }

        [HideInInspector]
        [SerializeField]
        public Connection ConnectsTo;

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

        private Dictionary<Connection, BezierCurve> _connectionPaths;
        private bool debug = false;

        private Dictionary<Connection, BezierCurve> ConnectionPaths => _connectionPaths ?? (_connectionPaths = Paths.ToDictionary(path => path.Connection, path => path.Path));
        public Connection[] InnerConnections => ConnectionPaths.Keys.ToArray();
        
        #region Unity Methods

        private void Awake()
        {
            Broadcaster.Instance.AddListener(GameState.SetupConnection, Initialize);
        }

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

        protected void Initialize(GameState gameState)
        {
            ParentRoute = transform.GetComponentInParent<Route>();
            if(debug)
                Debug.Log("connection: " + this.transform.parent.parent.name);
            ConnectsTo = FindNeighborConnection(EntityManager.Instance.Connections);
            if(ConnectsTo != null)
            {
                if(debug)
                    Debug.Log(this.transform.parent.parent.name + "NEW CONNECTION" + ConnectsTo.transform.parent.parent.name);
            }

            
        }

        /// <summary>
        /// Finds a neighbor connection given an array of connections to look from
        /// </summary>
        private Connection FindNeighborConnection(Connection[] connections)
        {
            if(debug)
                Debug.Log("FIND NEIGHBOURS" + connections.FirstOrDefault(CanConnect));
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
            if (connection == this || connection.ParentRoute == ParentRoute)
            {
                if(debug)
                    Debug.Log("Threw a false in first CanConnect Compairson");
                return false;
            }
            if (debug)
            {
                if (Vector3.Distance(transform.position, connection.transform.position) < CONNECTION_DISTANCE)
                {
                    if ((transform.parent.parent.name == "Lane (1)" && connection.transform.parent.parent.name == "Lane (6)") || (transform.parent.parent.name == "Lane (6)" && connection.transform.parent.parent.name == "Lane (1)"))
                    {

                            Debug.Log("THIS ONE WORKED");
                            Debug.Log(transform.parent.parent.name + "     " + connection.transform.parent.parent.name);
                    }
                }

            }
            //Debug.Log(Vector3.Distance(transform.position, connection.transform.position));
            return Vector3.Distance(transform.position, connection.transform.position) < CONNECTION_DISTANCE;
        }
    }
}