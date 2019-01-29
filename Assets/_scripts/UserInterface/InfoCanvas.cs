using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Level;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UserInterface
{
    public class InfoCanvas : MonoBehaviour
    {
        public Text Score;
        private Tweener _doFade;

        private PlayerVehicleManager Manager => PlayerVehicleManager.Instance;

        #region Unity Methods

        private void Start()
        {
            PlayerVehicleManager.Instance.HoverChanged.AddListener(UpdateSelectionInfo);
            _doFade = Score.DOFade(0f, .2f).SetAutoKill(false).Pause();
        }

        #endregion

        private void UpdateSelectionInfo(GameObject hoverGameObject)
        {
            if (hoverGameObject == null)
            {
                UpdateText(null);
                return;
            }

            var vehicle = hoverGameObject.GetComponent<Vehicle>();
            var pin = hoverGameObject.GetComponent<Pin>();

            if (vehicle && Manager.HasOwnership(vehicle) && Manager.SelectedPins.Any())
            {
                UpdateText("Send Vehicle to Pickup");
            }
            else if (vehicle && Manager.HasOwnership(vehicle))
            {
                UpdateText("No Passengers Selected");
            }
            else if (pin && !Manager.SelectedPins.Contains(pin))
            {
                UpdateText("Select Passengers");
            }
            else if (pin && Manager.SelectedPins.Contains(pin))
            {
                UpdateText("Deselect Passengers");
            }
            else
            {
                UpdateText(null);
            }
        }

        private void UpdateText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                _doFade.PlayForward();
            }
            else
            {
                if (text != Score.text)
                {
                    Score.text = text;
                }

                _doFade.PlayBackwards();
            }

        }
    }
}