namespace RideShareLevel
{
    public class PickupPassengerTask : VehicleTask
    {

        public Passenger TargetPassenger;

        public PickupPassengerTask(Vehicle vehicle, Passenger passenger) : base(vehicle)
        {
            TargetPassenger = passenger;
        }

        public override bool IsComplete()
        {
            return (TargetPassenger != null && TargetPassenger.PickedUp) || Vehicle.PathIsComplete;
        }

        public override bool ShouldStart()
        {
            return TargetPassenger != null && !TargetPassenger.PickedUp;
        }

        public override void Complete()
        {
            if (TargetPassenger != null && !TargetPassenger.PickedUp)
            {
                Vehicle.AddPassenger(TargetPassenger);
                TargetPassenger.SetVehicle(Vehicle);
            }
        }


    }
}