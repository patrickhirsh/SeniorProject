using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level
{
    public abstract class Route : Entity
    {
        [ReadOnly] public Connection[] Connections;
        [ReadOnly] public BezierCurve[] VehiclePaths;
        public Terminal[] Terminals => Connections.SelectMany(connection => connection.Terminals).ToArray();

        // An array of all entities this entity can reach
        private Route[] _connectingRoutes;
        public Route[] ConnectingRoutes
        {
            get
            {
                // We can cache at runtime, but we don't want to cache while in the editor
                if (Application.isPlaying) return _connectingRoutes ?? (_connectingRoutes = FindConnectingRoutes().ToArray());
                return FindConnectingRoutes().ToArray();
            }
        }

        // An array of neighbor entities this entity connects to
        private Route[] _neighborRoutes;
        public Route[] NeighborRoutes
        {
            get
            {
                // We can cache at runtime, but we don't want to cache while in the editor
                if (Application.isPlaying) return _neighborRoutes ?? (_neighborRoutes = FindNeighborRoutes().ToArray());
                return FindNeighborRoutes();
            }
        }

        public abstract bool Destinationable { get; }
        public bool HasTerminals => Terminals.Length > 0;
        public bool HasPassenger => HasTerminals && Terminals.Any(terminal => terminal.HasPassenger);

        #region Unity Methods

        protected virtual void Start()
        {
//            DrawPaths();
        }

        private void OnTriggerEnter(Collider other)
        {
            var target = other.GetComponent<Vehicle>();
            if (target != null)
            {
//                target.SetCurrentRoute(this);
                HandleVehicleEnter(target);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var target = other.GetComponent<Vehicle>();
            if (target != null)
            {
                HandleVehicleExit(target);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (Connections != null)
            {
                foreach (var connection in Connections)
                {
                    Gizmos.color = connection.Paths.Any() ? Color.blue : Color.red;
                    Gizmos.DrawSphere(connection.transform.position, .05f);
                }

                if (NeighborRoutes != null)
                {
                    Gizmos.color = Color.green;
                    foreach (var connection in Connections.Where(connection => connection.ConnectsTo != null))
                    {
                        Gizmos.DrawSphere(connection.ConnectsTo.transform.position + Vector3.up * .2f, .1f);
                    }
                }
            }
        }

        #endregion

//        private void DrawPaths()
//        {
//            foreach (var curve in VehiclePaths)
//            {
//                if (curve.GetAnchorPoints().Any())
//                {
//                    var child = new GameObject("Line");
//                    child.transform.SetParent(transform);
//                    var lineRenderer = child.AddComponent<LineRenderer>();
//                    int lengthOfLineRenderer = 20;
//                    lineRenderer.positionCount = lengthOfLineRenderer;
//                    lineRenderer.widthMultiplier = .06f;
//
//                    lineRenderer.numCapVertices = 2;
//                    lineRenderer.numCornerVertices = 2;
//                    lineRenderer.useWorldSpace = false;
//                    PathfindingManager.Instance.DrawCurve(curve, lineRenderer);
//                }
//            }
//        }

        public void BakePrefab()
        {
            Connections = GetComponentsInChildren<Connection>();
            Nodes = GetComponentsInChildren<Node>().ToList();
            VehiclePaths = GetComponentsInChildren<BezierCurve>();

            BakePaths(Connections, VehiclePaths);
            BakePickupLocations(Connections);
        }

        private void BakePickupLocations(Connection[] connections)
        {
            foreach (var connection in connections)
            {
                connection.Terminals = new List<Terminal>();
            }
            foreach (var terminal in GetComponentsInChildren<Terminal>())
            {
                var connection = connections.OrderBy(c => Vector3.Distance(c.transform.position, terminal.transform.position)).FirstOrDefault();
                if (connection != null)
                {
                    connection.Terminals.Add(terminal);
                    terminal.Connection = connection;
                }
            }
        }

        private void BakePaths(Connection[] connections, BezierCurve[] paths)
        {
            foreach (var connection in connections)
            {
                connection.Paths = new List<Connection.ConnectionPath>();
            }
            foreach (var path in paths)
            {
                var firstPoint = path.GetAnchorPoints().FirstOrDefault();
                var lastPoint = path.GetAnchorPoints().LastOrDefault();
                if (firstPoint != null & lastPoint != null)
                {
                    var startConnection = connections.FirstOrDefault(connection => Vector3.Distance(firstPoint.position, connection.transform.position) < Connection.CONNECTION_DISTANCE);
                    var endConnection = connections.FirstOrDefault(connection => Vector3.Distance(lastPoint.position, connection.transform.position) < Connection.CONNECTION_DISTANCE);
                    if (startConnection != null & endConnection != null)
                    {
                        // Bi-directional pathing
                        startConnection.Paths.Add(new Connection.ConnectionPath
                        {
                            Connection = endConnection,
                            Path = path
                        });
                        //                        endConnection.VehiclePaths.Add(new Connection.ConnectionPath
                        //                        {
                        //                            Connection = startConnection,
                        //                            Path = path
                        //                        });
                    }

                }
            }
        }

        public bool FindPathToEntity(Connection inboundConnection, Entity target, out BezierCurve path)
        {
            path = null;

            // Check that our target is a neighbor
            if (!NeighborRoutes.Contains(target)) return false;
            //            Debug.Assert(OutboundConnectingEntities.ContainsKey(target), "target is in neighbors, but no connection exists?");

            //            var outboundConnection = OutboundConnectingEntities[target];
            //            inboundConnection.GetPathToConnection(outboundConnection, out path);
            return true;
        }

        private Route[] FindNeighborRoutes()
        {
            HashSet<Route> entities = new HashSet<Route>();
            foreach (var connection in Connections.Where(connection => connection != null && connection.ConnectsTo != null))
            {
                if (!entities.Contains(connection.ConnectsTo.ParentRoute))
                {
                    entities.Add(connection.ConnectsTo.ParentRoute);
                }
            }
            return entities.ToArray();
        }

        /// <summary>
        /// Returns entities that this entity can reach
        /// </summary>
        private IEnumerable<Route> FindConnectingRoutes()
        {
            // Return the already cached array of connecting entities
            if (_connectingRoutes != null && _connectingRoutes.Any())
            {
                foreach (var entity in _connectingRoutes)
                {
                    yield return entity;
                }
            }
            else
            {
                // We can reach our neighbors
                foreach (var neighbor in NeighborRoutes)
                {
                    yield return neighbor;

                    // We can reach our neighbors' neighbors, and theirs too, and theirs...
                    foreach (var connectingEntity in neighbor.ConnectingRoutes)
                    {
                        yield return connectingEntity;
                    }
                }
            }
        }

        public abstract void HandleVehicleEnter(Vehicle vehicle);
        public abstract void HandleVehicleExit(Vehicle vehicle);
    }
}