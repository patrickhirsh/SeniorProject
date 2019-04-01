﻿namespace RideShareLevel
{
    public enum ParkingRouteType { Volta, Enemy }

    public class ParkingRoute : Route
    {
        public ParkingRouteType Type;
        public bool IsOccupied;

        public override bool Destinationable => false;

        public override void HandleVehicleEnter(Vehicle vehicle)
        {
            IsOccupied = true;
        }

        public override void HandleVehicleExit(Vehicle vehicle)
        {
            IsOccupied = false;
        }
    }
}
