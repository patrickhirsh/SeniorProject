using UnityEngine;

namespace Level
{
    public class VehicleLookAhead : MonoBehaviour
    {
        public Vehicle Vehicle;

        public float GodModeWait = 5f;          // how long a vehicle waits before entering godmode
        public float GodModeDuration = 2f;      // how long godmode lasts
        private float _godModeTimer = 0;
        private bool _isInGodMode = false;

        private void Update()
        {
            if(_isInGodMode)
            {
                _godModeTimer -= Time.deltaTime;

                if(_godModeTimer <= 0)
                {
                    _isInGodMode = false;
                }

            }
        }

        /// <summary>
        /// Slow the car down if another car is in front
        /// </summary>
        /// <param name="other">Other.</param>
        private void OnTriggerStay(Collider other)
        {
            var vehicle = other.GetComponent<Vehicle>();
            if (vehicle != null && !_isInGodMode && vehicle != Vehicle)
            {
                Vehicle.Speed = Mathf.Lerp(Vehicle.Speed, 0, .25f);

                // if the vehicle is stopped
                if (vehicle.Speed < 0.1f)
                {
                    _godModeTimer += Time.deltaTime;

                    if (_godModeTimer > GodModeWait)
                    {
                        _isInGodMode = true;
                        _godModeTimer = GodModeDuration;
                        vehicle.Speed = vehicle.BaseSpeed;
                    }
                }
            }


        }

        private void OnTriggerExit(Collider other)
        {
            var vehicle = other.GetComponent<Vehicle>();
            if (vehicle != null && vehicle != Vehicle)
            {
                Vehicle.Speed = Vehicle.BaseSpeed;
                _godModeTimer = 0;
            }
        }

    }
}