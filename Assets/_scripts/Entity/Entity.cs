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
        public Entity[] ConnectingEntities;

        // An array of neighbor entities this entity connects to
        public Entity[] NeighborEntities;

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
            foreach (var point in GetInboundConnectionPoints())
            {
                Gizmos.DrawSphere(point.transform.position, .05f);
            }

            // Draw the inbound connection points.
            Gizmos.color = Color.blue;
            foreach (var point in GetOutboundConnectionPoints())
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
            foreach (var connection in GetOutboundConnectionPoints())
            {
                connection.Setup();
            }
            // We MUST setup the neighbor entities first
            NeighborEntities = FindNeighborEntities().ToArray();

            // AFTER setting up neighbors, calculate a list of all the entities we can reach
            ConnectingEntities = FindConnectingEntities().ToArray();
        }

        /// <summary>
        /// Returns all connecting neighbor entities
        /// </summary>
        private IEnumerable<Entity> FindNeighborEntities()
        {
            return GetOutboundConnectionPoints().Select(connection => connection.ConnectingEntity).Where(entity => entity != null);
        }

        /// <summary>
        /// Returns entities that this entity can reach
        /// </summary>
        private IEnumerable<Entity> FindConnectingEntities()
        {

            // Return the already cached array of connecting entities
//            if (ConnectingEntities.Any())
//            {
//                foreach (var entity in ConnectingEntities)
//                {
//                    yield return entity;
//                }
//
//                // That's all folks
//                yield break;
//            }

            // We can reach our neighbors
            foreach (var entity in NeighborEntities)
            {
                yield return entity;
            }

            // We can reach our neighbors' neighbors, and theirs too, and theirs...
            foreach (var entity in NeighborEntities.SelectMany(entity => entity.FindConnectingEntities()))
            {
                yield return entity;
            }
        }

        /// <summary>
        /// Returns the indexes that the entity contains
        /// </summary>
        public IEnumerable<Connection> GetInboundConnectionPoints()
        {
            return Nodes().SelectMany(node => node.InboundConnections);
        }

        /// <summary>
        /// Returns the indexes that the entity contains
        /// </summary>
        public IEnumerable<Connection> GetOutboundConnectionPoints()
        {
            return Nodes().SelectMany(node => node.OutBoundConnections);
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