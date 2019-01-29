using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level
{
    public class IntersectionRoute : Route
    {
        public override bool Destinationable => true;

        public List<ColliderGroup> ColliderGroups;

        [Serializable]
        public class ColliderGroup
        {
            public List<Collider> Colliders;

            public void SetActive(bool active)
            {
                foreach (var c in Colliders)
                {
                    c.enabled = active;
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            StartCoroutine(RunIntersection());
//            foreach (var colliderGroup in ColliderGroups)
//            {
//                colliderGroup.SetActive(false);
//            }
        }

        private IEnumerator RunIntersection()
        {
            if (ColliderGroups.Any())
            {
                // Enable all to start (Red)
                foreach (var colliderGroup in ColliderGroups)
                {
                    colliderGroup.SetActive(true);
                }

                var activeGroup = ColliderGroups.First();
                while (isActiveAndEnabled)
                {
                    foreach (var colliderGroup in ColliderGroups)
                    {
                        // Set Red
                        activeGroup.SetActive(true);

                        //Move to next group and set Green
                        activeGroup = colliderGroup;
                        yield return new WaitForSeconds(2f);

                        activeGroup.SetActive(false);

                        yield return new WaitForSeconds(Random.Range(4f, 10f));
                    }
                }
            }
        }

        public override void HandleVehicleEnter(Vehicle vehicle)
        {

        }

        public override void HandleVehicleExit(Vehicle vehicle)
        {

        }
    }
}
