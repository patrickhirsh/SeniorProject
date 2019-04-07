﻿namespace RideShareLevel
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
            return TargetPassenger.PickedUp || Vehicle.PathIsComplete;
        }

        public override void Complete()
        {
            if (!TargetPassenger.PickedUp)
            {
                Vehicle.AddPassenger(TargetPassenger);
                TargetPassenger.SetVehicle(Vehicle);
            }
        }


    }
}