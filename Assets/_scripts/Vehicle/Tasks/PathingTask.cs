using System.Collections.Generic;

namespace RideShareLevel
{
    public class PathingTask : VehicleTask
    {
        public Queue<Connection> Path { get; private set; }

        public PathingTask(Vehicle vehicle, Queue<Connection> path) : base(vehicle)
        {
            Path = path;
        }

        public override bool IsComplete()
        {
            return Vehicle.PathIsComplete;
        }

        public override void Complete()
        {
            Vehicle.Despawn();
        }
    }
}