using DG.Tweening;
using Level;
using System;
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
        public float LookAhead = .035f;
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
            // TODO: Figure this one out
            var totalTime = curve.length / Speed;
            Debug.Log(totalTime);
            var ticks = totalTime / 100;
            for (float i = 0; i < 1; i += ticks)
            {
                transform.position = curve.GetPointAt(i);
                if (i + LookAhead <= 1f)
                {
                    transform.LookAt(curve.GetPointAt(i + LookAhead));
                }

                yield return new WaitForSeconds(ticks);
            }
        }
    }
}