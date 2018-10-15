using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Grid = Utility.Grid;

namespace Level
{
    [ExecuteInEditMode]
    public abstract class Node : MonoBehaviour
    {
        public Vector3 Dimension;
        public SubNode[] SubNodes;
        public Dictionary<Vector3, SubNode> SubNodeMap;

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
                transform.SnapToGrid();
                transform.hasChanged = false;
            }
        }

        protected virtual void OnDrawGizmos()
        {
            // Draw the cells
            Gizmos.color = Color.white;
            foreach (var index in GetIndices())
            {
                Gizmos.DrawSphere(Grid.GetCellPos(index), .05f);
            }

            // Draw the subnode connection points.
            Gizmos.color = Color.green;
            foreach (var point in SubNodeConnectionPoints())
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
            // Constraint on the SubNodes Size
            Array.Resize(ref SubNodes, (int)(Dimension.x * Dimension.y * Dimension.z));

            // Maps the indices to the SubNodes
            var indices = GetIndices().ToArray();
            for (int i = 0; i < indices.Length; i++)
            {
                SubNodes[i].SetIndex(indices[i]);
            }
        }
        #endregion

        /// <summary>
        /// Returns the indexes that the node contains
        /// </summary>
        public IEnumerable<Vector3> GetIndices()
        {
            var index = Grid.GetCellIndex(transform.position);
            var current = index;
            for (int x = 0; x < Dimension.x; x++)
            {
                for (int z = 0; z < Dimension.z; z++)
                {
                    yield return current;
                    current.z += 1;
                }
                current.x += 1;
                current.z = index.z;
            }
        }

        /// <summary>
        /// Iterates the connection points in the SubNodes of this node.
        /// </summary>
        public IEnumerable<Vector3> SubNodeConnectionPoints()
        {
            return SubNodes.SelectMany(node => node.GetDirectionPoints());
        }
    }

    [Serializable]
    public class SubNode
    {
        public Vector3 Index { get; private set; }
        #region Direction
        [BitMask(typeof(Direction))]
        public Direction Connections;

        [Flags]
        public enum Direction
        {
            Up = 1 << 0,
            Down = 1 << 1,
            Left = 1 << 2,
            Right = 1 << 3
        }
        #endregion

        public void SetIndex(Vector3 index)
        {
            Index = index;
        }

        /// <summary>
        /// Iterates each direction returning the point where the entrance/exit is for the cell in that direction.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Vector3> GetDirectionPoints()
        {
            foreach (var direction in Enum.GetValues(typeof(Direction)))
            {
                if (Connections.HasFlag((Enum)direction))
                {
                    yield return GetDirectionPoint((Direction)direction);
                }
            }
        }

        public Vector3 GetDirectionPoint(Direction direction)
        {
            var center = Grid.GetCellPos(Index);
            switch (direction)
            {
                case Direction.Up:
                    center += Grid.GridHalfZ * Vector3.forward;
                    break;
                case Direction.Down:
                    center -= Grid.GridHalfZ * Vector3.forward;
                    break;
                case Direction.Left:
                    center -= Grid.GridHalfX * Vector3.right;
                    break;
                case Direction.Right:
                    center += Grid.GridHalfX * Vector3.right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return center;
        }
    }
}