using System;
using System.Collections.Generic;
using DG.Tweening;
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
            Gizmos.color = Color.white;
            foreach (var index in GetIndices())
            {
                Gizmos.DrawSphere(Grid.GetCellPos(index), .1f);
            }

            // TODO: Draw subnodes
        }

        protected virtual void OnDestroy()
        {
            // Level Manager
            LevelManager.Instance.RemoveNode(this);
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
    }
    
    [Serializable]
    public class SubNode
    {
        [Flags]
        public enum Direction
        {
            Up = 1 << 0,
            Down = 1 << 1,
            Left = 1 << 2,
            Right = 1 << 3
        }

        [BitMask(typeof(Direction))]
        public Direction Connections;

        public Vector3 Index { get; private set; }

        public SubNode(Vector3 index)
        {
            Index = index;
        }
    }
}