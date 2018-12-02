using System.Linq;
using Level;
using UnityEngine;

namespace Level
{
    public class Lane : Route
    {
        public override bool Destinationable =>  Terminals.Any(terminal => terminal.HasPassenger);

        public override void HandleVehicleEnter(Vehicle vehicle)
        {
        }

        public override void HandleVehicleExit(Vehicle vehicle)
        {
        }
    }
}
