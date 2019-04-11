using System.CodeDom;

namespace RideShareLevel
{
    public abstract class VehicleTask
    {
        public abstract bool IsComplete();
        public Vehicle Vehicle { get; }

        protected VehicleTask(Vehicle vehicle)
        {
            Vehicle = vehicle;
        }

        public abstract bool ShouldStart();
        public abstract void Complete();
    }
}