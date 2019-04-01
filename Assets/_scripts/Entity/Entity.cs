using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Grid = Utility.Grid;

namespace RideShareLevel
{
    public abstract class Entity : LevelObject
    {
        [ReadOnly] public List<Node> Nodes = new List<Node>();

        #region Unity Methods
        protected virtual void Awake()
        {
            EntityController.AddEntity(this);
        }

        protected virtual void OnDestroy()
        {
            // Level Manager
            EntityController.RemoveEntity(this);
        }

        protected virtual void OnDrawGizmos()
        {
            // Draw the cells
            Gizmos.color = new Color(100, 100, 100, .05f);
            foreach (var index in GetCellIndices())
            {
                Gizmos.DrawCube(index.GetPosition(), (Vector3)Grid.CellSize / 2f);
            }
        }
        #endregion

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
    }
}