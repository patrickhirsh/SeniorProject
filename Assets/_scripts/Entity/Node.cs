using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utility;
using Grid = Utility.Grid;

namespace Level
{
    /// <summary>
    /// A physical location for use in the editor
    /// </summary>
    [ExecuteInEditMode]
    public class Node : MonoBehaviour
    {
        public IEnumerable<Connection> InboundConnections => GetComponentsInChildren<Connection>().Where(connection => connection.Traveling == Connection.TravelingDirection.Inbound);
        public IEnumerable<Connection> OutBoundConnections => GetComponentsInChildren<Connection>().Where(connection => connection.Traveling == Connection.TravelingDirection.Outbound);

        public CellIndex Index => transform.CellIndex();

        public Entity Entity => transform.GetComponentInParent<Entity>();

        protected virtual void Update()
        {

            if (transform.hasChanged && !Input.GetMouseButton(0))
            {
//                transform.SnapToGrid();
                transform.hasChanged = false;
            }
        }

        /// <summary>
        /// Iterates each direction returning the point of valid connections
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Vector3> ValidConnectionPoints()
        {
            var center = Index.GetPosition();
            yield return center + Grid.CELL_HALF_Z * Vector3.forward;
            yield return center - Grid.CELL_HALF_Z * Vector3.forward;
            yield return center + Grid.CELL_HALF_X * Vector3.right;
            yield return center - Grid.CELL_HALF_X * Vector3.right;
        }
    }
}