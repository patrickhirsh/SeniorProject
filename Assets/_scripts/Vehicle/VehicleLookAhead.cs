using UnityEngine;

namespace Level
{
    public class VehicleLookAhead : MonoBehaviour
    {
        public Vehicle Vehicle;

        /// <summary>
        /// Slow the car down if another car is in front
        /// </summary>
        /// <param name="other">Other.</param>
        private void OnTriggerStay(Collider other)
        {
            var vehicle = other.GetComponent<Vehicle>();
            if (vehicle != null)
            {
                Vehicle.Speed = Mathf.Lerp(Vehicle.Speed, 0, .25f);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var vehicle = other.GetComponent<Vehicle>();
            if (vehicle != null)
            {
                Vehicle.Speed = Vehicle.BaseSpeed;
            }
        }

    }
}