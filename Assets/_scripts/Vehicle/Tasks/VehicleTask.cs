using System.CodeDom;

namespace RideShareLevel
{
    public abstract class VehicleTask
    {
        public abstract bool IsComplete();
        public Vehicle Vehicle { get; }
        public bool DrawPath { get; }

        protected VehicleTask(Vehicle vehicle, bool drawPath)
        {
            Vehicle = vehicle;
            DrawPath = drawPath;
        }

        public abstract bool ShouldStart();
        public abstract void Complete();
    }
}