using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RideShareLevel
{
    public abstract class Route : Entity
    {
        [SerializeField] public Transform CenterTransform;

        [ReadOnly] public Connection[] Connections;
        [ReadOnly] public BezierCurve[] VehiclePaths;
        [ReadOnly] public Terminal[] Terminals;

        // An array of all entities this entity can reach
        private Route[] _connectingRoutes;
        public Route[] ConnectingRoutes
        {
            get
            {
                // We can cache at runtime, but we don't want to cache while in the editor
                if (Application.isPlaying)
                    return _connectingRoutes ?? (_connectingRoutes = FindConnectingRoutes().ToArray());
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
            // DrawPaths();
        }

        private void OnTriggerEnter(Collider other)
        {
            var target = other.GetComponent<Vehicle>();
            if (target != null)
            {
                // target.SetCurrentRoute(this);
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
                if (NeighborRoutes != null)
                {
                    Gizmos.color = Color.green;
                    foreach (var connection in Connections.Where(connection => connection.GetConnectsTo != null))
                    {
                        Gizmos.DrawSphere(connection.GetConnectsTo.transform.position + Vector3.up * .2f, .1f);
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

        #region Baking

#if UNITY_EDITOR
        public void Bake()
        {
            Undo.RecordObject(this, "Bake Route");
            Connections = GetComponentsInChildren<Connection>();
            Nodes = GetComponentsInChildren<Node>().ToList();
            VehiclePaths = GetComponentsInChildren<BezierCurve>();
            Terminals = GetComponentsInChildren<Terminal>();

            foreach (var connection in Connections)
            {
                connection.Bake();
            }
            // Do some validation

            BakePaths(Connections, VehiclePaths);
            BakePathPoints(Connections, VehiclePaths);
            BakePickupLocations(Connections);

            foreach (var connection in Connections)
            {
                connection.ValidatePaths();
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }

        private void BakePathPoints(Connection[] connections, BezierCurve[] vehiclePaths)
        {
            foreach (var curve in vehiclePaths)
            {
                foreach (var point in curve.GetAnchorPoints())
                {
                    point.BeginBake();
                    point.Connection = connections.OrderBy(c => Vector3.Distance(c.transform.position, point.transform.position)).FirstOrDefault();
                    point.EndBake();
                }
            }
        }

        private void BakePaths(Connection[] connections, BezierCurve[] paths)
        {
            foreach (var connection in connections)
            {
                Undo.RecordObject(connection, "Bake Paths on Connection");
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
                        startConnection.Paths.Add(new Connection.ConnectionPath
                        {
                            NextConnection = endConnection,
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
            foreach (var connection in connections)
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(connection);
            }
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
#endif

        #endregion


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
            foreach (var connection in Connections.Where(connection => connection != null && connection.GetConnectsTo != null))
            {
                if (!entities.Contains(connection.GetConnectsTo.ParentRoute))
                {
                    entities.Add(connection.GetConnectsTo.ParentRoute);
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