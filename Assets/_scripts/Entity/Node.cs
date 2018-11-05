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
        public CellIndex Index => transform.CellIndex();

        public Entity Entity => transform.GetComponentInParent<Entity>();

        #region Connections
        private Connection[] _inboundConnections;
        public IEnumerable<Connection> InboundConnections => _inboundConnections ??
                                                             (_inboundConnections = GetComponentsInChildren<Connection>()
                                                                 .Where(connection => connection.Type == Connection.ConnectionType.Inbound)
                                                                 .ToArray());

        private Connection[] _outboundConnections;
        public IEnumerable<Connection> OutBoundConnections => _outboundConnections ?? (
                                                                  _outboundConnections = GetComponentsInChildren<Connection>()
                                                                      .Where(connection => connection.Type == Connection.ConnectionType.Outbound)
                                                                      .ToArray());
        #endregion

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