using System.Collections.Generic;

namespace RideShareLevel
{
    public class NeutralPathingTask : VehicleTask
    {
        public Queue<Connection> Path { get; private set; }

        public NeutralPathingTask(Vehicle vehicle, Queue<Connection> path) : base(vehicle)
        {
            Path = path;
        }

        public override bool IsComplete()
        {
            return Vehicle.PathIsComplete;
        }

        public override bool ShouldStart()
        {
            return true;
        }

        public override void Complete()
        {
            Vehicle.Despawn();
        }
    }
}