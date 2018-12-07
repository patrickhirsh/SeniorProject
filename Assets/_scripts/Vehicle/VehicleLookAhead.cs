using System.Collections;
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

        private Coroutine _current;

        private void Update()
        {
            /*
            if (_isInGodMode)
            {
                _godModeTimer -= Time.deltaTime;

                if (_godModeTimer <= 0)
                {
                    _isInGodMode = false;
                    _godModeTimer = 0;
                }

            }
            */
        }

        private void OnTriggerEnter(Collider other)
        {
            /*
            var vehicle = other.GetComponent<Vehicle>();
            if (vehicle != null && !_isInGodMode && vehicle != Vehicle)
            {
                KillCurrent();
                _current = StartCoroutine(SlowDown());
            }
            */
        }

        /// <summary>
        /// Slow the car down if another car is in front
        /// </summary>
        /// <param name="other">Other.</param>
        private void OnTriggerStay(Collider other)
        {
            /*
            var vehicle = other.GetComponent<Vehicle>();
            if (vehicle != null && !_isInGodMode && vehicle != Vehicle)
            {
                // if the vehicle is stopped
                if (Vehicle.Speed < 0.1f)
                {
                    _godModeTimer += Time.deltaTime;

                    if (_godModeTimer > GodModeWait)
                    {
                        _isInGodMode = true;
                        _godModeTimer = GodModeDuration;
                        KillCurrent();
                        _current = StartCoroutine(SpeedUp());
                    }
                }
            }
            */


        }

        private void KillCurrent()
        {
            /*
            if (_current != null) StopCoroutine(_current);
            _current = null;
            */
        }

        private void OnTriggerExit(Collider other)
        {
            /*
            var vehicle = other.GetComponent<Vehicle>();
            if (vehicle != null && !_isInGodMode && vehicle != Vehicle)
            {
                KillCurrent();
                _current = StartCoroutine(SpeedUp());
                _godModeTimer = 0;
            }
            */
        }

        private IEnumerator SpeedUp()
        {
            /*
            while (Vehicle.Speed < Vehicle.BaseSpeed)
            {
                Vehicle.Speed = Mathf.Lerp(Vehicle.Speed, Vehicle.BaseSpeed, .25f);
                yield return null;
            }
            */
            return null;
        }

        private IEnumerator SlowDown()
        {
            /*
            while (Vehicle.Speed > 0)
            {
                Vehicle.Speed = Mathf.Lerp(Vehicle.Speed, 0, .25f);
                yield return null;
            }
            */
            return null;
        }
    }
}