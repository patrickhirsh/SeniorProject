using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;

namespace Level
{
    [ExecuteInEditMode]
    public abstract class Entity : MonoBehaviour
    {
        public Transform NodeContainer;

        #region Unity Methods
        protected virtual void Awake()
        {
            // Level Manager
            LevelManager.Instance.AddNode(this);
        }

        protected virtual void Update()
        {
            if (transform.hasChanged)
            {
//                transform.SnapToGrid();

                //TODO: Make this work in editor
                var rotation = transform.eulerAngles;
                rotation.y = Mathf.Round(rotation.y / 90) * 90;
//                transform.eulerAngles = rotation;

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
            foreach (var point in GetInboundConnectionPoints())
            {
                Gizmos.DrawSphere(point, .05f);
            }

            // Draw the inbound connection points.
            Gizmos.color = Color.blue;
            foreach (var point in GetOutboundConnectionPoints())
            {
                Gizmos.DrawSphere(point, .05f);
            }
        }

        protected virtual void OnDestroy()
        {
            // Level Manager
            LevelManager.Instance.RemoveNode(this);
        }

        protected virtual void OnValidate()
        {
            // Constraint on the Nodes Size
//            Array.Resize(ref Nodes, (int)(Dimension.x * Dimension.y * Dimension.z));

//            // Maps the indices to the Nodes
//            var indices = GetCellIndices().ToArray();
//            for (int i = 0; i < indices.Length; i++)
//            {
//                Nodes[i].SetIndex(indices[i]);
//            }
        }
        #endregion

        /// <summary>
        /// Returns the indexes that the entity contains
        /// </summary>
        public IEnumerable<Vector3> GetInboundConnectionPoints()
        {
            return Nodes().SelectMany(node => node.InboundConnectionPoints());
        }

        /// <summary>
        /// Returns the indexes that the entity contains
        /// </summary>
        public IEnumerable<Vector3> GetOutboundConnectionPoints()
        {
            return Nodes().SelectMany(node => node.OutBoundConnectionPoints());
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