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
        private bool IsColliding => _colliding.Any();

        private float _godModeTimer = 0;
        private bool _isInGodMode;
        private bool _isWaiting;
        private bool _isSlowed;

        private Coroutine _current;
        private List<Collider> _colliding = new List<Collider>();

        private void Update()
        {
            if (_isInGodMode)
            {
                if (_isSlowed) SpeedUp();

                _godModeTimer -= Time.deltaTime;
                if (_godModeTimer <= 0)
                {
                    _isInGodMode = false;
                    _isWaiting = false;
                }
            }
            else
            {
                if (IsColliding && !_isWaiting)
                {
                    if (!_isSlowed) SlowDown();
                    _godModeTimer = GodModeWait;
                    _isWaiting = true;

                }
                else if (IsColliding && _isWaiting)
                {
                    _godModeTimer -= Time.deltaTime;

                    if (_godModeTimer <= 0)
                    {
                        _isInGodMode = true;
                        _isWaiting = false;
                        _godModeTimer = GodModeDuration;

                        if (_isSlowed) SpeedUp();
                    }
                }
                else
                {
                    _godModeTimer = GodModeWait;
                    _isWaiting = false;
                    _isInGodMode = false;

                    if (_isSlowed) SpeedUp();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Vehicle>() || other.GetComponent<Stoplight>())
            {
                if (!_colliding.Contains(other))
                {
                    _colliding.Add(other);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Vehicle>() || other.GetComponent<Stoplight>())
            {
                if (_colliding.Contains(other))
                {
                    _colliding.Remove(other);
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