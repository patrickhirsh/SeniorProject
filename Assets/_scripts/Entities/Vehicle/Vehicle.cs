using Level;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VehicleEntity
{
    public class Vehicle : Entity
    {
        public Connection ClosestInbound => EntityManager.Instance.InboundConnections
            .OrderBy(connection => Vector3.Distance(transform.position, connection.transform.position))
            .FirstOrDefault();
        public Entity Target;
        public float LookAhead = 5.0f;
        public float Speed = 20;
        private Coroutine _animationTween;

        protected IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            //            PathToTarget(Target);
        }

        private void PathToTarget(Entity target)
        {
            List<BezierCurve> curves;
            if (EntityManager.Instance.FindPath(this, target, out curves))
            {
                TravelPath(curves);
            }
            else
            {
                Debug.Log("Couldn't reach path!");
            }
        }

        public void StopTraveling()
        {
            if (_animationTween != null) StopCoroutine(_animationTween);
        }

        public void TravelPath(IList<BezierCurve> curves)
        {
            var curve = transform.GetOrAddComponent<BezierCurve>();
            curve.Clear();

            foreach (var point in curves.SelectMany(b => b.GetAnchorPoints()))
            {
                curve.AddPoint(point);
            }

            _animationTween = StartCoroutine(TravelPath(curve));
        }

        public IEnumerator TravelPath(BezierCurve curve)
        {
            DrawPath(curve);

            // TODO: Figure this one out
            var totalTime = curve.length / Speed;
            Debug.Log(totalTime);
            var Pos = 0.0f;

            var ticks = .004f / totalTime;
            while (transform.position != curve.GetPointAt(1))
            {
                transform.position = curve.GetPointAt(Pos);
                if (Pos + LookAhead <= 1f)
                {
                    transform.LookAt(curve.GetPointAt(Pos + LookAhead));
                }

                Pos += ticks;
                yield return new WaitForSeconds(ticks);
            }

            // Delete the drawn path
            var lineRenderer = this.GetOrAddComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            /*
            for (float i = 0; i < totalTime; i += ticks)
            {
                transform.position = curve.GetPointAt(i);
                if (i + LookAhead <= 1f)
                {
                    transform.LookAt(curve.GetPointAt(i + LookAhead));
                }

                yield return new WaitForSeconds(ticks);
            }
            */
        }

        private void DrawPath(BezierCurve curve)
        {
            var lineRenderer = this.GetOrAddComponent<LineRenderer>();
            int lengthOfLineRenderer = 200;
            lineRenderer.positionCount = lengthOfLineRenderer;
            lineRenderer.widthMultiplier = .2f;
            lineRenderer.numCapVertices = 10;
            lineRenderer.numCornerVertices = 10;
            var points = new Vector3[lengthOfLineRenderer];

            for (int i = 0; i < lengthOfLineRenderer; i++)
            {
                points[i] = curve.GetPointAt(i / (float)(lengthOfLineRenderer - 1));
                points[i] += Vector3.up * .2f;
            }

            lineRenderer.SetPositions(points);
        }
    }
}