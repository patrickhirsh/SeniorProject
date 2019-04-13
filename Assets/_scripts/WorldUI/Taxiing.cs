using System;
using System.Collections.Generic;
using DG.Tweening;
using Game;
using RideShareLevel;
using UnityEngine;
using UnityEngine.UI;

namespace _scripts
{
    public class Taxiing : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;
        public Transform PassengerContainer;
        public Image PassengerIconPrefab;

        private Camera _camera;
        #region Unity Methods

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (CanvasGroup.gameObject.activeInHierarchy)
            {
                CanvasGroup.transform.LookAt(_camera.transform);
            }
        }
        #endregion

        public void SetPassengers(IEnumerable<Passenger> passengers)
        {
            // Clear all of them out
            foreach (Transform child in PassengerContainer)
            {
                Destroy(child.gameObject);
            }

            // Create icons
            foreach (var passenger in passengers)
            {
                var passengerIcon = Instantiate(PassengerIconPrefab, PassengerContainer, false);
                passengerIcon.color = ColorKey.GetBuildingColor(passenger.GetBuildingColor());
            }
        }
    }
}