using Level;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VehicleEntity
{
    public class Vehicle : MonoBehaviour
    {
        public Connection CurrentConnection;
        public Entity Target;
        public float LookAhead = .2f;
        private float Speed = 5f;
        private Coroutine _animationTween;

        private Queue<Connection> _connectionsPath;

        protected void Start()
        {
            // TODO: Determine the starting connection (probably on vehicle spawn).
            CurrentConnection = EntityManager.Instance.Connections.Where(connection => connection.Paths.Any())
                .OrderBy(connection => Vector3.Distance(transform.position, connection.transform.position))
                .FirstOrDefault();
        }

//        private void PathToTarget(Entity target)
//        {
//            List<BezierCurve> curves;
//            if (EntityManager.Instance.FindPath(this, target, out curves))
//            {
//                TravelPath(curves);
//            }
//            else
//            {
//                Debug.Log("Couldn't reach path!");
//            }
//        }

        public void StopTraveling()
        {
            if (_animationTween != null) StopCoroutine(_animationTween);
        }

        public BezierCurve SetupCurve(BezierCurve curveToTravel)
        {
            var curve = transform.GetOrAddComponent<BezierCurve>();
            curve.Clear();

            foreach (var point in curveToTravel.GetAnchorPoints())
            {
                curve.AddPoint(point);
            }

            return curve;
        }

//        public void TravelPath(IList<BezierCurve> curves)
//        {
//            var curve = transform.GetOrAddComponent<BezierCurve>();
//            curve.Clear();
//
//            foreach (var point in curves.SelectMany(b => b.GetAnchorPoints()))
//                curve.AddPoint(point);
//
//            _animationTween = StartCoroutine(TravelPath(curve));
//        }

        public IEnumerator TravelPath(BezierCurve curve, Connection target)
        {
            float position = 0.0f;
            //float speedScale = 0.05f;
            CurrentConnection.ParentEntity.HandleVehicleExit(this);

            while (position <= 1)
            {
                position += (Speed * Time.deltaTime) / curve.length;
                transform.position = curve.GetPointAt(position);

                if (position + LookAhead <= 1f)
                {
                    transform.LookAt(curve.GetPointAt(position + LookAhead));
                }

                yield return new WaitForSeconds(0);
            }

            // Update current to target
            CurrentConnection = target.ConnectsTo;

            Debug.Log($"Vehicle is entering {CurrentConnection.ParentEntity}", CurrentConnection.ParentEntity);
            CurrentConnection.ParentEntity.HandleVehicleEnter(this);

            // Delete the drawn path
            var lineRenderer = this.GetOrAddComponent<LineRenderer>();
            lineRenderer.positionCount = 0;

            if (_connectionsPath.Any())
            {
                TravelToConnection(_connectionsPath.Dequeue());
            }
        }

        public void TravelPath(Queue<Connection> connections)
        {
            _connectionsPath = connections;
            TravelToConnection(_connectionsPath.Dequeue());
        }

        private void TravelToConnection(Connection target)
        {
            BezierCurve curve;
            if (CurrentConnection.GetPathToConnection(target, out curve))
            {
                var vehicleCurve = SetupCurve(curve);
                _animationTween = StartCoroutine(TravelPath(vehicleCurve, target));
            }
            else
            {
                Debug.LogWarning($"Could not find path");
            }
        }
    }
}