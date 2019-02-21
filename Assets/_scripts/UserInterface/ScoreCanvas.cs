using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UserInterface
{
    public class ScoreCanvas : MonoBehaviour
    {
        public Button HideButton;
        public Button RestartButton;

        private int _score;

        #region Unity Methods

        private void Start()
        {
            RestartButton.onClick.AddListener(HandleRestartButton);
            HideButton.onClick.AddListener(HandleHideButton);
            
        }

        #endregion

        private void HandleHideButton()
        {
            UserInterfaceManager.Instance.HideUI();
        }

        private void HandleRestartButton()
        {
            Broadcaster.Broadcast(GameEvent.Reset);
            GameManager.SetGameState(GameState.LevelRePlacement);
        }
    }

    //    private void UpdateScore(int score)
    //    {
    //        StartCoroutine(AnimateScore());
    //    }

    //    private IEnumerator AnimateScore()
    //    {
    //        var currentScore = GameManager.Instance.CurrentScore;
    //        while (_score != currentScore)
    //        {
    //            if (_score > currentScore)
    //            {
    //                _score--;
    //            }
    //            else
    //            {
    //                _score++;
    //            }

    //            Score.text = _score.ToString();
    //            yield return new WaitForSeconds(0.1f);
    //        }
    //    }
    //}
}
