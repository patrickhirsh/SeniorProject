using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RideShareLevel;
using UnityEngine;

namespace _scripts
{
    public class Taxiing : MonoBehaviour
    {
        public CanvasGroup CanvasGroup;
        public Transform PassengerContainer;
        public PassengerTaxiingIcon PassengerIconPrefab;
        public LineRaycaster Line;

        private Camera _camera;
        private Dictionary<Passenger, PassengerTaxiingIcon> _icons = new Dictionary<Passenger, PassengerTaxiingIcon>();
        private Sequence _sequence;

        #region Unity Methods

        private void Awake()
        {
            _camera = Camera.main;
            Line.gameObject.SetActive(false);
            CanvasGroup.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (CanvasGroup.gameObject.activeInHierarchy)
            {
                CanvasGroup.transform.LookAt(_camera.transform);
            }
        }
        #endregion

        public void AddPassenger(Passenger passenger)
        {
            var passengerIcon = Instantiate(PassengerIconPrefab, PassengerContainer, false);
            passengerIcon.SetPassenger(passenger);
            _icons[passenger] = passengerIcon;

            CheckVisibility();
        }

        public void RemovePassenger(Passenger passenger)
        {
            Debug.Assert(_icons.ContainsKey(passenger), "Passenger not in icon", gameObject);
            var icon = _icons[passenger];
            icon.Deliver();
            
            // Remove at end
            _icons.Remove(passenger);
            CheckVisibility();
        }

        private void CheckVisibility()
        {
            _sequence.Kill();
            if (!_icons.Any())
            {
                _sequence = DOTween.Sequence();
                _sequence.Append(CanvasGroup.DOFade(0, .5f));
                _sequence.OnComplete(() =>
                {
                    Line.gameObject.SetActive(false);
                    CanvasGroup.gameObject.SetActive(false);
                });
            }
            else
            {
                Line.gameObject.SetActive(true);
                CanvasGroup.alpha = 0;
                _sequence = DOTween.Sequence();
                _sequence.Append(CanvasGroup.DOFade(1, .5f));
                _sequence.OnComplete(() =>
                {
                    CanvasGroup.gameObject.SetActive(true);
                });
            }
        }
    }
}