using DG.Tweening;
using Game;
using RideShareLevel;
using UnityEngine;
using UnityEngine.UI;

namespace _scripts
{
    public class PassengerTaxiingIcon : LevelObject
    {
        public Image Icon;
        private Passenger _passenger;

        public void SetPassenger(Passenger passenger)
        {
            _passenger = passenger;
            Icon.color = ColorKey.GetBuildingColor(passenger.GetBuildingColor());
        }

        private void Update()
        {
            if (!MainCamera) return;
            if (gameObject.activeInHierarchy)
            {
                transform.LookAt(MainCamera.transform);
            }
        }

        public void Deliver()
        {
            var tween = DOTween.Sequence();
            tween.Append(transform.DOMove(_passenger.DestBuilding.ScoreLocation, 0.75f).SetEase(Ease.InSine));
            tween.Join(transform.DOScale(Vector3.zero, 0.25f).SetDelay(0.5f));
            tween.OnComplete(() => { Destroy(gameObject); });
            tween.Play();
        }
    }
}