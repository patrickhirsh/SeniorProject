using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VehicleEntity;

namespace Level
{
    public class ParkingEntity : Entity
    {
        public bool IsOccupied;

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

