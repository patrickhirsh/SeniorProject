using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level
{
    public class VehicleLookAhead : MonoBehaviour
    {
        public Vehicle Vehicle;

        public float GodModeWait = 5f;          // how long a vehicle waits before entering godmode
        public float GodModeDuration = 2f;      // how long godmode lasts
        private bool IsVehicleCollision => _collidingVehicles.Any();
        private bool IsAtIntersection => _collidingIntersections.Any();

        private float _timer = 0;
        private bool _isInGodMode;
        private bool _isWaiting;
        private bool _isSlowed;

        private Coroutine _current;
        private List<Collider> _collidingVehicles = new List<Collider>();
        private List<Collider> _collidingIntersections = new List<Collider>();

        private void Update()
        {

            if (IsAtIntersection)
            {
                if (!_isSlowed) SlowDown();
                _isInGodMode = false;
                _isWaiting = false;
                foreach (var intersection in _collidingIntersections.ToArray())
                {
                    if (!intersection.enabled)
                    {
                        _collidingIntersections.Remove(intersection);
                    }
                }
            }
            else if (_isInGodMode)
            {
                if (_isSlowed) SpeedUp();

                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    _isInGodMode = false;
                    _isWaiting = false;
                }
            }
            else if (IsVehicleCollision)
            {
                if (!_isWaiting)
                {
                    if (!_isSlowed) SlowDown();
                    _timer = GodModeWait;
                    _isWaiting = true;
                }
                else if (_isWaiting)
                {
                    _timer -= Time.deltaTime;

                    if (_timer <= 0)
                    {
                        _isInGodMode = true;
                        _isWaiting = false;
                        _timer = GodModeDuration;
                    }
                }
            }
            else
            {
                _timer = GodModeWait;
                _isWaiting = false;
                _isInGodMode = false;

                if (_isSlowed) SpeedUp();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Vehicle>())
            {
                if (!_collidingVehicles.Contains(other) && !_isInGodMode)
                {
                    _collidingVehicles.Add(other);
                }
            }

            if (other.GetComponent<Stoplight>())
            {
                if (!_collidingIntersections.Contains(other))
                {
                    _collidingIntersections.Add(other);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Vehicle>())
            {
                if (_collidingVehicles.Contains(other))
                {
                    _collidingVehicles.Remove(other);
                }
            }

            if (other.GetComponent<Stoplight>())
            {
                if (_collidingIntersections.Contains(other))
                {
                    _collidingIntersections.Remove(other);
                }
            }
        }

        private void KillCurrent()
        {
            if (_current != null) StopCoroutine(_current);
            _current = null;
        }

        private void SpeedUp()
        {
            Vehicle.Speed = Vehicle.BaseSpeed;
            _isSlowed = false;
        }

        private void SlowDown()
        {
            Vehicle.Speed = 0;
            _isSlowed = true;
        }
    }
}