using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Grid = Utility.Grid;

namespace Level
{
    [ExecuteInEditMode]
    public class Node : MonoBehaviour
    {
        public CellIndex Index => transform.CellIndex();
        #region Direction
        [BitMask(typeof(Direction))]
        public Direction InboundConnections;
        [BitMask(typeof(Direction))]
        public Direction OutBoundConnections;

        [Flags]
        public enum Direction
        {
            Up = 1 << 0,
            Down = 1 << 1,
            Left = 1 << 2,
            Right = 1 << 3
        }
        #endregion

        protected virtual void Update()
        {
            if (transform.hasChanged)
            {
                transform.SnapToGrid();
                transform.hasChanged = false;
            }
        }

        /// <summary>
        /// Iterates each direction returning the point of inbound connections
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Vector3> InboundConnectionPoints()
        {
            foreach (var direction in Enum.GetValues(typeof(Direction)))
            {
                if (InboundConnections.HasFlag((Enum)direction))
                {
                    yield return GetDirectionPoint((Direction)direction);
                }
            }
        }

        /// <summary>
        /// Iterates each direction returning the point of outbound connections
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Vector3> OutBoundConnectionPoints()
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                if (OutBoundConnections.HasFlag(direction))
                {
                    yield return GetDirectionPoint(direction);
                }
            }
        }

        protected virtual void OnValidate()
        {
            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                if (InboundConnections.HasFlag(direction) && OutBoundConnections.HasFlag(direction))
                {
                    Debug.LogError($"Node has an inbound and outbound connection on {direction}", gameObject);
                }
            }
        }

        public Vector3 GetDirectionPoint(Direction direction)
        {
            var center = Index.GetPosition();
            switch (direction)
            {
                case Direction.Up:
                    center += Grid.CellHalfSizeZ * Vector3.forward;
                    break;
                case Direction.Down:
                    center -= Grid.CellHalfSizeZ * Vector3.forward;
                    break;
                case Direction.Left:
                    center -= Grid.CellHalfSizeX * Vector3.right;
                    break;
                case Direction.Right:
                    center += Grid.CellHalfSizeX * Vector3.right;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return center;
        }
    }
}