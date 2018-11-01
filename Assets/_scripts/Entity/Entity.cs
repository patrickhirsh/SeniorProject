using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace Level
{
    public abstract class Entity : MonoBehaviour
    {
        // The Transform that holds nodes
        public Transform NodeContainer;

        // An array of all entities this entity can reach
        private Entity[] _connectingEntities;
        public Entity[] ConnectingEntities => _connectingEntities ?? (_connectingEntities = FindConnectingEntities().ToArray());

        // An array of neighbor entities this entity connects to
        private Entity[] _neighborEntities;
        public Entity[] NeighborEntities => _neighborEntities ?? (_neighborEntities = FindNeighborEntities().ToArray());

        private Dictionary<Entity, Connection> _outboundConnectingEntities;
        public Dictionary<Entity, Connection> OutboundConnectingEntities => _outboundConnectingEntities ?? (_outboundConnectingEntities = OutboundConnections.ToDictionary(connection => connection.ConnectingEntity));

        private Connection[] _outboundConnections;
        public Connection[] OutboundConnections => _outboundConnections ?? (_outboundConnections = Nodes().SelectMany(node => node.OutBoundConnections).ToArray());

        private Connection[] _inboundConnections;
        public Connection[] InboundConnections => _inboundConnections ?? (_inboundConnections = Nodes().SelectMany(node => node.InboundConnections).ToArray());

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

        protected virtual void OnDrawGizmos()
        {
            // Draw the cells
            Gizmos.color = Color.white;
            foreach (var index in GetCellIndices())
            {
                Gizmos.DrawSphere(index.GetPosition(), .05f);
            }

            // Draw the inbound connection points.
            Gizmos.color = Color.green;
            foreach (var point in InboundConnections)
            {
                Gizmos.DrawSphere(point.transform.position, .05f);
            }

            // Draw the inbound connection points.
            Gizmos.color = Color.blue;
            foreach (var point in OutboundConnections)
            {
                Gizmos.DrawSphere(point.transform.position, .05f);
            }
        }

        protected virtual void OnDestroy()
        {
            // Level Manager
            EntityManager.Instance.RemoveEntity(this);
        }

        #endregion

        /// <summary>
        /// Called after all entities have registered with the EntityManager
        /// </summary>
        public void Setup()
        {
            foreach (var connection in OutboundConnections)
            {
                connection.Setup();
            }
        }

        public bool FindPathToEntity(Connection inboundConnection, Entity target, out BezierCurve path)
        {
            path = null;

            // Check that our target is a neighbor
            if (!NeighborEntities.Contains(target)) return false;
            Debug.Assert(OutboundConnectingEntities.ContainsKey(target), "target is in neighbors, but no connection exists?");

            var outboundConnection = OutboundConnectingEntities[target];
            inboundConnection.FindPathToConnection(outboundConnection, out path);
            return true;
        }

        /// <summary>
        /// Returns all connecting neighbor entities
        /// </summary>
        private IEnumerable<Entity> FindNeighborEntities()
        {
            return OutboundConnections
                .Select(connection => connection.ConnectingEntity)
                .Where(entity => entity != null);
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
            foreach (Node subNode in Nodes())
            {
                yield return subNode.transform.CellIndex();
            }
        }

        /// <summary>
        /// Iterates the connection points in the Nodes of this entity.
        /// </summary>
        public Node[] Nodes()
        {
            Debug.Assert(NodeContainer != null, $"NodeContainer needs to be setup", gameObject);
            return NodeContainer.GetComponentsInChildren<Node>();
        }
    }
}