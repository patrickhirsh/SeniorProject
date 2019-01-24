using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    public class SpawnRoute : Route
    {
        public override bool Destinationable => false;

        /// <summary>
        /// A SpawnDirective represents a pre-determined vehicle spawn. SpawnDirectives
        /// are stored in a queue within each SpawnPointEntity. These are
        /// added prior to runtime (within the level editor) and will
        /// only be used if the level spawn mode is set to "pre-determined".
        /// At runtime, vehicles will be spawned based on each SpawnDirective's fields
        /// </summary>
        public class SpawnDirective
        {
            public GameObject vehicle { get; private set; }             // The template vehicle used to instantiate the new vehicle
            public GameObject destination { get; private set; }         // The spawned vehicle's destination. Must have a SpawnPointEntity component
            public float time { get; private set;}                      // Time in seconds after the start of the game that the vehicle should spawn


            public SpawnDirective(GameObject vehicleTemplate, GameObject destination, float time)
            {
                this.vehicle = vehicleTemplate;
                this.time = time;
                this.destination = destination;

                if (vehicle.GetComponent<Vehicle>() == null)
                    Debug.LogWarning("SpawnDirective created with a vehicleTemplate that doesn't have a Vehicle Component (vehicleTemplate must be a vehicle)");

                if (destination.GetComponent<SpawnRoute>() == null)
                    Debug.LogWarning("SpawnDirective created with a destination that isn't a SpawnNodeEntity (destination must be another SpawnNodeEntity)");
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


        private List<SpawnRoute> reachableSpawnPoints;                                            // All SpawnPoints reachable from this SpawnPoint
        private List<SpawnRoute> reachingSpawnPoints;                                             // All SpawnPoints that can reach this SpawnPoint
        private static SpawnDirectiveComparer spawnDirectiveComparer = new SpawnDirectiveComparer();    // used to sort _spawnQueue
        private List<SpawnDirective> _spawnQueue;                                                       // keeps a sorted record of SpawnDirectives (lowest time -> highest time) for non-procedural spawning


        protected override void Start()
        {
            base.Start();
            Broadcaster.AddListener(GameEvent.SetupConnection, Initialize);
        }

        /// <summary>
        /// Handles all GameEvent Broadcasts
        /// </summary>
        public void Initialize(GameEvent gameEvent)
        {
            switch (gameEvent)
            {
                case GameEvent.SetupConnection:
                    _spawnQueue = new List<SpawnDirective>();
                    break;
            }
        }


        /// <summary>
        /// Given a reference vehicle (vehicleTemplate) and a time in seconds,
        /// Creates a new spawn directive at this spawn point with a new instance of the provided vehicle
        /// The vehicle will be spawned (time) seconds into the game.
        /// Returns true if the SpawnDirective was added successfully or false if invalid parameters were given
        /// </summary>
        public bool AddSpawnDirective(GameObject vehicleTemplate, GameObject destination, float time)
        {
            if (time < 0) { return false; }
            if (vehicleTemplate == null) { return false; }

            _spawnQueue.Add(new SpawnDirective(vehicleTemplate, destination, time));
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

            SpawnDirective next = new SpawnDirective(_spawnQueue[0].vehicle, _spawnQueue[0].destination, _spawnQueue[0].time);
            _spawnQueue.RemoveAt(0);
            return next;
        }



        public override void HandleVehicleEnter(Vehicle vehicle)
        {
            return;
        }

        public override void HandleVehicleExit(Vehicle vehicle)
        {
            return;
        }
    }
}

