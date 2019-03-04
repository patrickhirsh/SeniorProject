using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game;
using UnityEngine;

namespace Level
{
    public class Passenger : MonoBehaviour
    {
        public Terminal StartTerminal;

        public Route StartRoute => StartTerminal.ParentRoute;


        public Route DestRoute;
        public Pin PassengerPickupReticle;
        public Vector3 AdjustmentVector;
        public bool PickedUp;
        public bool EnemyVehicleEnroute;
        public Gradient RingColorGradient;
        public GameObject RingPrefab;
        public GameObject LevelPrefab { get; internal set; }

        private Building.BuildingColors _color;
        private float _timeRemaining;
        private Pin _pickupPin;
        private GameObject Ring;

        #region Unity Methods
        private void Awake()
        {
            Broadcaster.AddListener(GameEvent.Reset, Reset);
        }

        private void Reset(GameEvent @event)
        {
            Destroy(gameObject);

        }

        public void Start()
        {
            _color = LevelManager.Instance.GetValidColor();
            DestRoute = LevelManager.Instance.GetBuildingRoute(_color);
            _timeRemaining = PassengerManager.PassengerTimeout;
            PickedUp = false;
            EnemyVehicleEnroute = false;
            SpawnPickupReticle();

        }

        private GameObject SpawnRing(Color color, float speed)
        {
            GameObject spawnedObj = Instantiate(RingPrefab, transform, false);
            Material ringMat = spawnedObj.GetComponent<Renderer>().material;
            ringMat.SetColor("_Color", color);
            ringMat.SetFloat("_Speed", speed);
            return spawnedObj;
        }

        public void Update()
        {
            // track passenger timeout
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                if (_timeRemaining <= 0) { _timeRemaining = 0; }
            }

            // spawn an enemy vehicle if the passenger times out and hasn't yet been picked up
            if (_timeRemaining == 0 && !PickedUp && !EnemyVehicleEnroute)
            {
                EnemyVehicleManager.Instance.PickupPassenger(this);
                Debug.Log("Enemy Vehicle Spawned!");
                EnemyVehicleEnroute = true;
                Ring.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
            }


            //TODO: visualize passenger time remaining here...
            if(Ring == null && !PickedUp)
            {
                Ring = SpawnRing(Color.red, 3);
            }
                
            else if(!PickedUp)
            {
                float time = _timeRemaining / PassengerManager.PassengerTimeout;
                Color newRingColor = RingColorGradient.Evaluate(1 - time);
                Ring.GetComponent<Renderer>().material.SetColor("_Color", newRingColor);
//                Debug.Log("Time is" + time);
                if(_timeRemaining > 0)
                    Ring.GetComponent<Renderer>().material.SetFloat("_Speed", 6-(time*5));
            }

            // NOTE: PickedUp == true when ANY vehicle has picked it up. Once it's picked up, don't show
            // any visual queues (or, maybe show the customers value above the vehicle? idk..)

            // NOTE: you might want to check if(_timeRemaining > 0 && !PickedUp) to display visual "time remaining"
            // pulsing. once if(_timeRemaining == 0 && !PickedUp) is true, maybe switch to a slow pulse of a different color
            // to indicate that the passenger timed out and an enemy vehicle is now on its way to pick it up instead...

        }

        public void DestroyRing()
        {
            Destroy(Ring);
        }
        #endregion

        public float GetTimeRemaining()
        {
            return _timeRemaining;
        }

        public Building.BuildingColors GetColor()
        {
            return _color;
        }
        #region Reticle Methods
        public void SpawnPickupReticle()
        {
            _pickupPin = Instantiate(PassengerPickupReticle, transform, false);
            _pickupPin.transform.position += AdjustmentVector;
            _pickupPin.Passenger = this;
            _pickupPin.SetColor(ColorKey.GetColor(_color));
        }
        #endregion
    }
}