using Game;
using UnityEngine;
using UnityEngine.UI;

namespace RideShareLevel
{
    public class Passenger : LevelObject
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

        private Building.BuildingColors _color;
        private float _timeRemaining;
        private float _totalTime;
        public Pin _pickupPin;
        private GameObject Ring;
        private Image _RadialTimer;


        #region Unity Methods
        private void Awake()
        {
            Broadcaster.AddListener(GameEvent.Reset, Reset);
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
                CurrentLevel.EnemyVehicleController.PickupPassenger(this);
                EnemyVehicleEnroute = true;
                //                Ring.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
            }


            //TODO: visualize passenger time remaining here...

            if (!PickedUp && _RadialTimer != null)
            {
                _RadialTimer.fillAmount = _timeRemaining / _totalTime;
            }
            else if (_RadialTimer != null)
            {
                _RadialTimer.fillAmount = 0;
            }

            if (Ring == null && !PickedUp)
            {
                //                Ring = SpawnRing(Color.red, 3);
            }

            else if (!PickedUp)
            {
                float time = _timeRemaining / _totalTime;
                Color newRingColor = RingColorGradient.Evaluate(1 - time);
                Ring.GetComponent<Renderer>().material.SetColor("_Color", newRingColor);
                //                Debug.Log("Time is" + time);
                if (_timeRemaining > 0)
                    Ring.GetComponent<Renderer>().material.SetFloat("_Speed", 6 - (time * 5));
            }

            // NOTE: PickedUp == true when ANY vehicle has picked it up. Once it's picked up, don't show
            // any visual queues (or, maybe show the customers value above the vehicle? idk..)

            // NOTE: you might want to check if(_timeRemaining > 0 && !PickedUp) to display visual "time remaining"
            // pulsing. once if(_timeRemaining == 0 && !PickedUp) is true, maybe switch to a slow pulse of a different color
            // to indicate that the passenger timed out and an enemy vehicle is now on its way to pick it up instead...

        }
        #endregion

        public void SetVehicle(Vehicle vehicle)
        {
            PickedUp = true;
            DestroyRing();
            SetPickupReticleActive(false);
        }

        private void Reset(GameEvent @event)
        {
            Destroy(gameObject);
        }

        private GameObject SpawnRing(Color color, float speed)
        {
            GameObject spawnedObj = Instantiate(RingPrefab, transform, false);
            Material ringMat = spawnedObj.GetComponent<Renderer>().material;
            ringMat.SetColor("_Color", color);
            ringMat.SetFloat("_Speed", speed);
            return spawnedObj;
        }

        public void SetPassengerType(PassengerTypes type)
        {
            _totalTime = type.PassengerTimer;
            _timeRemaining = type.PassengerTimer;
            _color = type.PassColor;

            DestRoute = CurrentLevel.PassengerController.GetBuildingRoute(_color);

            PickedUp = false;
            EnemyVehicleEnroute = false;
            CreatePickupReticle();
        }

        public void DestroyRing()
        {
            Destroy(Ring);
        }

        public float GetTimeRemaining()
        {
            return _timeRemaining;
        }

        public Building.BuildingColors GetBuildingColor()
        {
            return _color;
        }

        #region Reticle Methods
        public void CreatePickupReticle()
        {
            _pickupPin = Instantiate(PassengerPickupReticle, transform, false);
            _pickupPin.transform.position += AdjustmentVector;
            _pickupPin.Passenger = this;
            _pickupPin.SetColor(ColorKey.GetBuildingColor(_color));

            _RadialTimer = _pickupPin.RadialTimerImg;
        }

        public void SetPickupReticleActive(bool value)
        {
            _pickupPin.gameObject.SetActive(value);
        }

        #endregion

        public void Deliver(Vehicle vehicle)
        {
            CurrentLevel.PassengerController.PassengerDelivered(this, vehicle.PlayerControlled);

            GameObject tutorialObject = GameObject.Find("TutorialManager");

            if (tutorialObject != null)
            {
                tutorialObject.GetComponent<TutorialManager>().DeliverPassenger();
            }

            // Remove passenger from game
            Destroy(gameObject);
        }
    }

}