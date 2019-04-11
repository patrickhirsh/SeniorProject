namespace RideShareLevel
{
    public class DespawnTask : VehicleTask
    {
        public Route TargetRoute;

        public DespawnTask(Vehicle vehicle) : base(vehicle)
        {
            TargetRoute = vehicle.CurrentLevel.NeutralVehicleController.GetRandomSpawnRoute();
        }

        public override bool IsComplete()
        {
            return Vehicle.CurrentRoute == TargetRoute;
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