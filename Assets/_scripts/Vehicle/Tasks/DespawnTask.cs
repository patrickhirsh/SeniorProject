namespace RideShareLevel
{
    public class DespawnTask : VehicleTask
    {
        public Route TargetRoute;

        public DespawnTask(Vehicle vehicle, bool drawPath) : base(vehicle, drawPath)
        {
            TargetRoute = vehicle.CurrentLevel.NeutralVehicleController.GetRandomSpawnRoute();
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