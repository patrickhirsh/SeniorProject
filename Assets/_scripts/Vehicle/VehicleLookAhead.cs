using UnityEngine;

namespace Level
{
    public class VehicleLookAhead : MonoBehaviour
    {
        public Vehicle Vehicle;

        public float GodModeWait = 5f;          // how long a vehicle waits before entering godmode
        public float GodModeDuration = 2f;      // how long godmode lasts
        private float _GodModeTimer = 0;
        private bool _IsInGodMode = false;

        private void Update()
        {
            if(_IsInGodMode)
            {
                _GodModeTimer -= Time.deltaTime;

                if(_GodModeTimer <= 0)
                {
                    _IsInGodMode = false;
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
            if (vehicle != null && !_IsInGodMode)
            {
                Vehicle.Speed = Mathf.Lerp(Vehicle.Speed, 0, .25f);

                // if the vehicle is stopped
                if (vehicle.Speed < 0.1f)
                {
                    _GodModeTimer += Time.deltaTime;

                    if (_GodModeTimer > GodModeWait)
                    {
                        _IsInGodMode = true;
                        _GodModeTimer = GodModeDuration;
                        vehicle.Speed = vehicle.BaseSpeed;
                    }
                }
            }


        }

        private void OnTriggerExit(Collider other)
        {
            var vehicle = other.GetComponent<Vehicle>();
            if (vehicle != null)
            {
                Vehicle.Speed = Vehicle.BaseSpeed;
                _GodModeTimer = 0;
            }
        }

    }
}