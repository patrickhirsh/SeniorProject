using System.Collections;
using System.Collections.Generic;
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


        #region Unity Methods

        private void Start()
        {
            PlayerVehicleManager.Instance.HoverChanged.AddListener(UpdateSelectionInfo);
        }

        #endregion

        private void UpdateSelectionInfo(string text)
        {
            if (text != Score.text)
            {
                _doFade.Kill();

                if (string.IsNullOrEmpty(text))
                {
                    _doFade = Score.DOFade(0f, .2f);
                }
                else
                {
                    Score.text = text;
                    _doFade = Score.DOFade(1f, .2f);
                }
            }
        }
    }
}