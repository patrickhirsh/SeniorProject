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
        private Building.BuildingColors Color;

        public Pin PassengerPickupReticle;
        public Vector3 AdjustmentVector;

        #region Unity Methods

        private Pin pickupPin;

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
            Color = LevelManager.Instance.GetValidColor();
            DestRoute = LevelManager.Instance.GetBuildingRoute(Color);
            SpawnPickupReticle();
        }
        #endregion


        #region Reticle Methods

        public void SpawnPickupReticle()
        {
            pickupPin = Instantiate(PassengerPickupReticle, transform, false);
            pickupPin.transform.position += AdjustmentVector;
            pickupPin.Passenger = this;
            pickupPin.SpriteRenderer.color = ColorKey.GetColor(Color);
        }

        #endregion
    }
}