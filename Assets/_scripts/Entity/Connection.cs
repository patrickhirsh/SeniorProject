using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Grid = Utility.Grid;

namespace Level
{
    public class Connection : MonoBehaviour
    {
        public TravelingDirection Traveling;
        public enum TravelingDirection
        {
            Inbound,
            Outbound
        }

        public Connection ConnectsTo;
        public Entity ConnectingEntity;

        [Serializable]
        public class ConnectionPath
        {
            public Connection OutboundConnection;
            public BezierCurve Path;
        }

        public List<ConnectionPath> Paths = new List<ConnectionPath>();

        protected Node Node => GetComponentInParent<Node>();

        #region Unity Methods

        protected void Update()
        {
            if (transform.hasChanged)
            {
                SnapToValidPosition();
                transform.hasChanged = false;
            }
        }

        protected void OnValidate()
        {
            foreach (var connection in Paths)
            {
                if (connection.OutboundConnection.Traveling == TravelingDirection.Inbound)
                {
                    connection.OutboundConnection = null;
                    Debug.LogError("Can't connect to an inbound connection", gameObject);
                }
            }
        }

        #endregion

        public void Setup()
        {
            CalculateConnections();
        }

        public void CalculateConnections()
        {
            if (Traveling == TravelingDirection.Inbound) return;
            ConnectsTo = EntityManager.Instance.InboundConnections.FirstOrDefault(connection =>
                Vector3.Distance(transform.position, connection.transform.position) < Mathf.Max(Grid.CELL_SIZE_X, Grid.CELL_SIZE_Z));
            ConnectingEntity = ConnectsTo != null ? ConnectsTo.Node.Entity : null;
        }

        private void SnapToValidPosition()
        {
            Vector3 closest = Vector3.positiveInfinity;
            foreach (var point in Node.ValidConnectionPoints())
            {
                if (Vector3.Distance(point, transform.position) < Vector3.Distance(closest, transform.position))
                {
                    closest = point;
                }
            }

            transform.position = closest;
        }
    }


}