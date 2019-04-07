using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RideShareLevel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UserInterface
{
    public class InfoCanvas : MonoBehaviour
    {
        public Text Score;
        private Tweener _doFade;

        public Level CurrentLevel => LevelManager.Instance.CurrentLevel;

        #region Unity Methods

        private void Start()
        {
            
            InputManager.Instance.HoverChanged.AddListener(UpdateSelectionInfo);
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
            var menuBuilding = hoverGameObject.GetComponent<MenuBuilding>();

            if (vehicle && CurrentLevel.PlayerVehicleController.HasOwnership(vehicle) && CurrentLevel.PlayerVehicleController.SelectedPins.Any() && !vehicle.HasTask)
            {
                UpdateText("Send Vehicle to Pickup Passengers");
            }
            else if (vehicle && CurrentLevel.PlayerVehicleController.HasOwnership(vehicle) && !vehicle.HasTask)
            {
                UpdateText("No Passenger Selected");
            }
            else if (pin && !CurrentLevel.PlayerVehicleController.SelectedPins.Contains(pin))
            {
                UpdateText("Select Passenger");
            }
            else if (pin && CurrentLevel.PlayerVehicleController.SelectedPins.Contains(pin))
            {
                UpdateText("Deselect Passenger");
            }
            else if (menuBuilding && menuBuilding.getClicked())
            {
                UpdateText(menuBuilding.LevelText2);
            }
            else if (menuBuilding)
            {
                UpdateText(menuBuilding.LevelText);
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