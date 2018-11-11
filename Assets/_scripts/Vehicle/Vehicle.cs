using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level
{
    public class Vehicle : MonoBehaviour
    {
        public bool debugMode;
        public Connection CurrentConnection;
        public float LookAhead = .2f;
        public float Speed = 5f;

        private Coroutine _animationTween;
        private Queue<Connection> _connectionsPath;

        protected void Start()
        {
            // TODO: Determine the starting connection (probably on vehicle spawn).
            CurrentConnection = EntityManager.Instance.Connections.Where(connection => connection.Paths.Any())
                .OrderBy(connection => Vector3.Distance(transform.position, connection.transform.position))
                .FirstOrDefault();
        }


        #region VEHICLE PATHING

        /// <summary>
        /// Halts this vehicle's pathing (if any is being done)
        /// </summary>
        public void StopTraveling()
        {
            if (_animationTween != null) StopCoroutine(_animationTween);
        }

        /// <summary>
        /// Moves this vehicle along the given path of connections.
        /// Sets this vehicles _connectionsPath to the new "connections" path, then begins
        /// traversing it by starting the TravelPath() coroutine.
        /// </summary>
        public void StartTraveling(Queue<Connection> connections)
        {
            _connectionsPath = connections;
            StartCoroutine(TravelPath());
        }

        /// <summary>
        /// A Coroutine that traverses through all paths in _connectionsPath, moving this vehicle along
        /// each path as it goes.
        /// </summary>
        private IEnumerator TravelPath()
        {
            // traverse each path in _connectionsPath
            while (_connectionsPath.Any())
            {             
                // validate the current path
                BezierCurve curve;
                Connection target = _connectionsPath.Dequeue();
                if (!CurrentConnection.GetPathToConnection(target, out curve))
                    if (debugMode) { Debug.LogWarning($"Could not find path"); }

                // leaving the previous entity
                CurrentConnection.ParentEntity.HandleVehicleExit(this);

                // traverse the path
                float position = 0.0f;
                while (position <= 1)
                {
                    position += (Speed * Time.deltaTime) / curve.length;
                    transform.position = curve.GetPointAt(position);

                    if (position + LookAhead <= 1f)
                        transform.LookAt(curve.GetPointAt(position + LookAhead));

                    yield return new WaitForSeconds(0);
                }

                // entering the next entity
                CurrentConnection = target.ConnectsTo;
                CurrentConnection.ParentEntity.HandleVehicleEnter(this);
                if (debugMode) { Debug.Log($"Vehicle is entering {CurrentConnection.ParentEntity}", CurrentConnection.ParentEntity); }

                // delete the drawn path
                var lineRenderer = this.GetOrAddComponent<LineRenderer>();
                lineRenderer.positionCount = 0;
            }         
        }

        #endregion


        #region Old Stuff That Should Probably Be Removed When Jaden Says "Yeah That's Cool Remove That"
        /*
        public void TravelPath(Queue<Connection> connections)
        {
            _connectionsPath = connections;
            TravelToConnection(_connectionsPath.Dequeue());
        }


        /// <summary>
        /// TravelPath's coroutine. updates each frame until the path has been fully traversed.
        /// </summary>
        private IEnumerator TravelPath(BezierCurve curve, Connection target)
        {
            float position = 0.0f;
            CurrentConnection.ParentEntity.HandleVehicleExit(this);

            // core coroutine - updates vehicle position/roation each frame until the curve has been traversed
            while (position <= 1)
            {
                position += (Speed * Time.deltaTime) / curve.length;
                transform.position = curve.GetPointAt(position);

                if (position + LookAhead <= 1f)
                    transform.LookAt(curve.GetPointAt(position + LookAhead));

                yield return new WaitForSeconds(0);
            }

            // update current to target
            CurrentConnection = target.ConnectsTo;

            Debug.Log($"Vehicle is entering {CurrentConnection.ParentEntity}", CurrentConnection.ParentEntity);
            CurrentConnection.ParentEntity.HandleVehicleEnter(this);

            // delete the drawn path
            var lineRenderer = this.GetOrAddComponent<LineRenderer>();
            lineRenderer.positionCount = 0;

            if (_connectionsPath.Any())
                TravelToConnection(_connectionsPath.Dequeue());
        }


        // Unecessary?
        private BezierCurve SetupCurve(BezierCurve curveToTravel)
        {
            var curve = transform.GetOrAddComponent<BezierCurve>();
            curve.Clear();

            foreach (var point in curveToTravel.GetAnchorPoints())
                curve.AddPoint(point);

            return curve;
        }


        private void TravelToConnection(Connection target)
        {
            BezierCurve curve;

            if (CurrentConnection.GetPathToConnection(target, out curve))
            {
                //var vehicleCurve = SetupCurve(curve);
                _animationTween = StartCoroutine(TravelPath(curve, target));
            }

            else
            {
                Debug.LogWarning($"Could not find path");
            }
        }
        */
        #endregion
    }
}