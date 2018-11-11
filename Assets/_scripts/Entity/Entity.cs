using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Level;
using Grid = Utility.Grid;

namespace Level
{
    public abstract class Entity : MonoBehaviour
    {
        [ReadOnly] public Node[] Nodes;
        [ReadOnly] public Connection[] Connections;
        [ReadOnly] public BezierCurve[] Paths;

        // An array of all entities this entity can reach
        private Entity[] _connectingEntities;
        public Entity[] ConnectingEntities
        {
            get
            {
                // We can cache at runtime, but we don't want to cache while in the editor
                if (Application.isPlaying) return _connectingEntities ?? (_connectingEntities = FindConnectingEntities().ToArray());
                return FindConnectingEntities().ToArray();
            }
        }

        // An array of neighbor entities this entity connects to
        private Entity[] _neighborEntities;
        public Entity[] NeighborEntities
        {
            get
            {
                // We can cache at runtime, but we don't want to cache while in the editor
                if (Application.isPlaying) return _neighborEntities ?? (_neighborEntities = FindNeighborEntities().ToArray());
                return FindNeighborEntities();
            }
        }

        #region Unity Methods
        protected virtual void Awake()
        {
            EntityManager.Instance.AddEntity(this);
        }

        protected virtual void Update()
        {
            if (transform.hasChanged)
            {
                EntityManager.Instance.UpdateEntity(this);
                transform.hasChanged = false;
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            // Draw the cells
            Gizmos.color = new Color(255, 255, 255, .1f);
            foreach (var index in GetCellIndices())
            {
                Gizmos.DrawCube(index.GetPosition(), (Vector3)Grid.CellSize / 2f);
            }
        }

        protected virtual void OnDrawGizmos()
        {
            //            if (UnityEditor.Selection.activeGameObject == gameObject) return;
            if (Connections != null)
            {
                foreach (var connection in Connections)
                {
                    Gizmos.color = connection.Paths.Any() ? Color.blue : Color.red;
                    Gizmos.DrawSphere(connection.transform.position, .05f);
                }

                if (NeighborEntities != null)
                {
                    Gizmos.color = Color.green;
                    foreach (var connection in Connections.Where(connection => connection.ConnectsTo != null))
                    {
                        Gizmos.DrawSphere(connection.ConnectsTo.transform.position + Vector3.up * .2f, .1f);
                    }
                }
            }


        }

        protected virtual void OnDestroy()
        {
            // Level Manager
            EntityManager.Instance.RemoveEntity(this);
        }

        #endregion

        public void BakePrefab()
        {
            Connections = GetComponentsInChildren<Connection>();
            Nodes = GetComponentsInChildren<Node>();
            Paths = GetComponentsInChildren<BezierCurve>();

            BakePaths(Connections, Paths);
        }

        private void BakePaths(Connection[] connections, BezierCurve[] paths)
        {
            foreach (var connection in connections)
            {
                connection.Paths = new List<Connection.ConnectionPath>();
            }
            foreach (var path in paths)
            {
                var firstPoint = path.GetAnchorPoints().FirstOrDefault();
                var lastPoint = path.GetAnchorPoints().LastOrDefault();
                if (firstPoint != null & lastPoint != null)
                {
                    var startConnection = connections.FirstOrDefault(connection => Vector3.Distance(firstPoint.position, connection.transform.position) < Connection.CONNECTION_DISTANCE);
                    var endConnection = connections.FirstOrDefault(connection => Vector3.Distance(lastPoint.position, connection.transform.position) < Connection.CONNECTION_DISTANCE);
                    if (startConnection != null & endConnection != null)
                    {
                        // Bi-directional pathing
                        startConnection.Paths.Add(new Connection.ConnectionPath
                        {
                            Connection = endConnection,
                            Path = path
                        });
//                        endConnection.Paths.Add(new Connection.ConnectionPath
//                        {
//                            Connection = startConnection,
//                            Path = path
//                        });
                    }

                }
            }
        }

        /// <summary>
        /// Called after all entities have registered with the EntityManager
        /// </summary>
        public void Setup()
        {
            foreach (var connection in Connections)
            {
                connection.Setup();
            }
        }

        public bool FindPathToEntity(Connection inboundConnection, Entity target, out BezierCurve path)
        {
            path = null;

            // Check that our target is a neighbor
            if (!NeighborEntities.Contains(target)) return false;
            //            Debug.Assert(OutboundConnectingEntities.ContainsKey(target), "target is in neighbors, but no connection exists?");

            //            var outboundConnection = OutboundConnectingEntities[target];
            //            inboundConnection.GetPathToConnection(outboundConnection, out path);
            return true;
        }

        private Entity[] FindNeighborEntities()
        {
            HashSet<Entity> entities = new HashSet<Entity>();
            foreach (var connection in Connections.Where(connection => connection != null && connection.ConnectsTo != null))
            {
                if (!entities.Contains(connection.ConnectsTo.ParentEntity))
                {
                    entities.Add(connection.ConnectsTo.ParentEntity);
                }
            }
            return entities.ToArray();
        }

        /// <summary>
        /// Returns entities that this entity can reach
        /// </summary>
        private IEnumerable<Entity> FindConnectingEntities()
        {
            // Return the already cached array of connecting entities
            if (_connectingEntities != null && _connectingEntities.Any())
            {
                foreach (var entity in _connectingEntities)
                {
                    yield return entity;
                }
            }
            else
            {
                // We can reach our neighbors
                foreach (var neighbor in NeighborEntities)
                {
                    yield return neighbor;

                    // We can reach our neighbors' neighbors, and theirs too, and theirs...
                    foreach (var connectingEntity in neighbor.ConnectingEntities)
                    {
                        yield return connectingEntity;
                    }
                }
            }
        }

        /// <summary>
        /// Returns the indexes that the entity contains
        /// </summary>
        public IEnumerable<CellIndex> GetCellIndices()
        {
            if (Nodes != null)
            {
                foreach (Node subNode in Nodes)
                {
                    yield return subNode.transform.CellIndex();
                }
            }
        }

        public abstract void HandleVehicleEnter(Vehicle vehicle);
        public abstract void HandleVehicleExit(Vehicle vehicle);
    }
}