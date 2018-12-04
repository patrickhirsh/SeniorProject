using System.Linq;
using Level;
using UnityEngine;

namespace Level
{
    public class LaneRoute : Route
    {
        public override bool Destinationable => false;

        public override void HandleVehicleEnter(Vehicle vehicle)
        {

        }

        public override void HandleVehicleExit(Vehicle vehicle)
        {

        }
    }
}
