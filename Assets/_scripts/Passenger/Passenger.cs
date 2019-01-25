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
        private Building.BuildingColors _color;

        public Pin PassengerPickupReticle;
        public Vector3 AdjustmentVector;

        #region Unity Methods

        private Pin _pickupPin;

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
            SpawnPickupReticle();
        }
        #endregion


        #region Reticle Methods

        public void SpawnPickupReticle()
        {
            _pickupPin = Instantiate(PassengerPickupReticle, transform, false);
            _pickupPin.transform.position += AdjustmentVector;
            _pickupPin.Passenger = this;
            _pickupPin.SpriteRenderer.color = ColorKey.GetColor(_color);
        }

        #endregion
    }
}