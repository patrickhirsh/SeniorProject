namespace RideShareLevel
{
    public class DropoffPassengerTask : VehicleTask
    {
        public Passenger TargetPassenger;

        public DropoffPassengerTask(Vehicle vehicle, Passenger passenger) : base(vehicle)
        {
            TargetPassenger = passenger;
        }

        public override bool IsComplete()
        {
            return Vehicle.PathIsComplete && Vehicle.CurrentRoute == TargetPassenger.DestRoute;
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