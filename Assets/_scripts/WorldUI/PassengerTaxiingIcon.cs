using Game;
using RideShareLevel;
using UnityEngine;
using UnityEngine.UI;

namespace _scripts
{
    public class PassengerTaxiingIcon : MonoBehaviour
    {
        public Image Icon;
        private Passenger _passenger;

        public void SetPassenger(Passenger passenger)
        {
            _passenger = passenger;
            Icon.color = ColorKey.GetBuildingColor(passenger.GetBuildingColor());
        }

        public void Deliver()
        {
            Destroy(gameObject);
        }
    }
}