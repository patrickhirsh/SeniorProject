using System.CodeDom;

namespace RideShareLevel
{
    public abstract class VehicleTask
    {
        public bool IsDonePathing { get; private set; }
        public abstract bool IsComplete();
        public Vehicle Vehicle { get; private set; }

        protected VehicleTask(Vehicle vehicle)
        {
            Vehicle = vehicle;
        }

        public void SetPathComplete(bool val)
        {
            IsDonePathing = val;
        }

        public abstract void Complete();
    }
}