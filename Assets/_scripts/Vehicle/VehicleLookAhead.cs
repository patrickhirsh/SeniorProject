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

//        public float LookAheadDistance;
        public float GodModeWait = 2f;          // how long a vehicle waits before entering godmode
        public float GodModeDuration = 2f;      // how long godmode lasts

        private List<Collider> _collidingVehicles = new List<Collider>();
        private bool IsVehicleCollision => _collidingVehicles.Any();

        private float _timer = 0;
        private bool _isInGodMode;
        private bool _isWaiting;
        private bool _isSlowed;

        private Coroutine _current;
        private Tween _speedTween;

        #region Unity Methods

        private void Update()
        {
            if (_isInGodMode)
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

        //        private void CheckCollision()
        //        {
        //            IsVehicleCollision = Physics.Linecast(transform.position, transform.position + transform.forward * LookAheadDistance, out _, 1 << 9, QueryTriggerInteraction.Collide);
        //        }


        private void OnTriggerEnter(Collider other)
        {
            //First check if we're running into a collider meant for collision or net
            var vehicle = other.GetComponent<Vehicle>();
            if (vehicle)
            {
                if (!_collidingVehicles.Contains(other) && !_isInGodMode && vehicle.HasTask)
                {
                    _collidingVehicles.Add(other);
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
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = IsVehicleCollision ? Color.red : Color.white;
            var t = transform;
            Gizmos.DrawSphere(t.position, .15f);
//            Gizmos.DrawLine(position, position + t.forward * LookAheadDistance);
        }

        #endregion

        private void KillCurrent()
        {
            if (_current != null) StopCoroutine(_current);
            _current = null;
        }

        private void SpeedUp()
        {
            _speedTween?.Kill();
            _speedTween = DOTween.To(() => Vehicle.Speed, value => Vehicle.Speed = value, Vehicle.BaseSpeed, .6f).SetEase(Ease.InSine);
            //            Vehicle.Speed = Vehicle.BaseSpeed;
            _isSlowed = false;
        }

        private void SlowDown()
        {
            _speedTween?.Kill();
            _speedTween = DOTween.To(() => Vehicle.Speed, value => Vehicle.Speed = value, 0, .5f).SetEase(Ease.OutSine);
            //            Vehicle.Speed = 0;
            _isSlowed = true;
        }

    }
}