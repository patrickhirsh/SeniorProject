using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Level;

namespace Level
{
    public class ParkingRoute : Route
    {
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

