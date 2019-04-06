using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace RideShareLevel
{
    public class VehicleLookAhead : MonoBehaviour
    {
        public Vehicle Vehicle;

        public float GodModeWait = 2f;          // how long a vehicle waits before entering godmode
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
        private Tween _speedTween;

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
                if (!_collidingVehicles.Contains(other) && !_isInGodMode && (other.GetComponent<Vehicle>().GetCurrentTask() != null))
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
            _speedTween?.Kill();
            _speedTween = DOTween.To(() => Vehicle.Speed, value => Vehicle.Speed = value, Vehicle.BaseSpeed, .3f).SetEase(Ease.InSine);
//            Vehicle.Speed = Vehicle.BaseSpeed;
            _isSlowed = false;
        }

        private void SlowDown()
        {
            _speedTween?.Kill();
            _speedTween = DOTween.To(() => Vehicle.Speed, value => Vehicle.Speed = value, 0, .3f).SetEase(Ease.OutSine);
//            Vehicle.Speed = 0;
            _isSlowed = true;
        }

    }
}