namespace RideShareLevel
{
    public class DropoffPassengerTask : VehicleTask
    {
        public Passenger TargetPassenger { get; }

        public DropoffPassengerTask(Vehicle vehicle, Passenger passenger) : base(vehicle)
        {
            TargetPassenger = passenger;
        }

        public override bool IsComplete()
        {
            return Vehicle.PathIsComplete && Vehicle.CurrentRoute == TargetPassenger.DestRoute;
        }

        public override bool ShouldStart()
        {
            return Vehicle.HasPassenger(TargetPassenger);
        }

        public override void Complete()
        {
            Vehicle.RemovePassenger(TargetPassenger);
            TargetPassenger.Deliver(Vehicle);
            if (Vehicle.PlayerControlled)
            {
                ParticleManager.Instance.GenerateFirework(Vehicle.transform.position);
            }
        }
    }
}