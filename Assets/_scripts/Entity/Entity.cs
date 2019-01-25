using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Grid = Utility.Grid;

namespace Level
{
    public abstract class Entity : MonoBehaviour
    {
        [ReadOnly] public List<Node> Nodes = new List<Node>();

        #region Unity Methods
        protected virtual void Awake()
        {
            EntityManager.Instance.AddEntity(this);
        }

        protected virtual void Update()
        {
            if (transform.hasChanged)
            {
//                EntityManager.Instance.UpdateEntity(this);
                transform.hasChanged = false;
            }
        }

        protected virtual void OnDestroy()
        {
            // Level Manager
            EntityManager.Instance.RemoveEntity(this);
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