using DG.Tweening;
using Game;
using RideShareLevel;
using UnityEngine;
using UnityEngine.UI;

namespace _scripts
{
    public class PassengerTaxiingIcon : MonoBehaviour
    {
        public Image Icon;
        private Passenger _passenger;

        public void SetPassenger(Passenger passenger)
        {
            _passenger = passenger;
            Icon.color = ColorKey.GetBuildingColor(passenger.GetBuildingColor());
        }

        public void Deliver()
        {
            transform.SetParent(null, true);
            var tween = DOTween.Sequence();
            tween.Append(transform.DOMove(_passenger.DestBuilding.ScoreLocation, 1f));
            tween.Join(transform.DOScale(Vector3.zero, 1f));
//            tween.OnComplete(() => { Destroy(gameObject); });
            tween.Play();
        }
    }
}