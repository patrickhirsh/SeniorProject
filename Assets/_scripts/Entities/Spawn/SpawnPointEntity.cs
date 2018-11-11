using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class SpawnPointEntity : MonoBehaviour
    {

        /// <summary>
        /// A SpawnDirective represents a pre-determined vehicle spawn. SpawnDirectives
        /// are stored in a queue within each SpawnPointEntity. These are
        /// added prior to runtime (within the level editor) and will
        /// only be used if the level spawn mode is set to "pre-determined".
        /// At runtime, vehicles will be spawned based on each SpawnDirective's fields
        /// </summary>
        public class SpawnDirective
        {
            public GameObject vehicle;     // The template vehicle used to instantiate the new vehicle
            public float time;             // Time in seconds after the start of the game that the vehicle should spawn

            public SpawnDirective(GameObject vehicleTemplate, float time)
            {
                this.vehicle = vehicleTemplate;
                this.time = time;
            }
        }

        /// <summary>
        /// Compares SpawnDirectives based on SpawnDirective.time
        /// Sorts performed with this comparer will result in a sorting order of Smallest -> Largest
        /// </summary>
        public class SpawnDirectiveComparer : IComparer<SpawnDirective>
        {
            public int Compare(SpawnDirective directive1, SpawnDirective directive2)
            {
                if (directive1.time < directive2.time)
                    return -1;
                else if (directive1.time > directive2.time)
                    return 1;
                else if (directive1.time == directive2.time)
                    return 0;

                else return 0;
            }
        }

        private static SpawnDirectiveComparer spawnDirectiveComparer = new SpawnDirectiveComparer();    // used to sort _spawnQueue
        private List<SpawnDirective> _spawnQueue;                                                       // keeps a sorted record of SpawnDirectives (lowest time -> highest time) for non-procedural spawning


        public void Start()
        {
            _spawnQueue = new List<SpawnDirective>();
        }


        /// <summary>
        /// Given a reference vehicle (vehicleTemplate) and a time in seconds,
        /// Creates a new spawn directive at this spawn point with a new instance of the provided vehicle
        /// The vehicle will be spawned (time) seconds into the game.
        /// Returns true if the SpawnDirective was added successfully or false if invalid parameters were given
        /// </summary>
        public bool AddSpawnDirective(GameObject vehicleTemplate, float time)
        {
            if (time < 0) { return false; }
            if (vehicleTemplate == null) { return false; }

            _spawnQueue.Add(new SpawnDirective(vehicleTemplate, time));
            _spawnQueue.Sort(spawnDirectiveComparer);
            return true;
        }


        /// <summary>
        /// Returns the next lowest-time SpawnDirective and removes it from _spawnQueue.
        /// Returns null if _spawnQueue is empty
        /// </summary>
        /// <returns></returns>
        public SpawnDirective PopNextSpawnDirective()
        {
            if (_spawnQueue.Count < 1)
                return null;

            SpawnDirective next = new SpawnDirective(_spawnQueue[0].vehicle, _spawnQueue[0].time);
            _spawnQueue.RemoveAt(0);
            return next;
        }
    }
}

