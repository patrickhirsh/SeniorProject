using DG.Tweening;
using Level;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VehicleEntity
{
    public class Vehicle : Entity
    {
        public Entity Target;
        public float LookAhead = .035f;
        public float Speed = 20;

        protected IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            PathToTarget(Target);
        }

        private void PathToTarget(Entity target)
        {
            List<BezierCurve> curves;
            if (EntityManager.Instance.FindPath(this, target, out curves))
            {
                // TODO: Create a custom animation curve? Calculate when to slow down on turns.
                var curve = transform.GetOrAddComponent<BezierCurve>();
                foreach (var point in curves.SelectMany(b => b.GetAnchorPoints()))
                {
                    curve.AddPoint(point);
                }

                StartCoroutine(TravelPath(curve));
            }
            else
            {
                Debug.Log("Couldn't reach path!");
            }
        }

        private IEnumerator TravelPath(BezierCurve curve)
        {
//            var tween = transform.DOMove(curve.GetPointAt(0), 1f);
//            yield return tween.WaitForCompletion();
            var speed = .0001f * Speed;
            for (float i = 0; i < 1; i += speed)
            {
                transform.position = curve.GetPointAt(i);
                if (i <= 1f - LookAhead)
                {
                    transform.LookAt(curve.GetPointAt(i + LookAhead));
                }

                yield return new WaitForSeconds(speed);
            }
        }
    }
}