using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UserInterface
{
    public class UserInterfaceManager : MonoBehaviour
    {
        public LevelPlacementCanvas LevelPlacementCanvas;
        public ScoreCanvas ScoreCanvas;
        public InfoCanvas InfoCanvas;

        private Component[] _all;
        private Sequence _disableSequence;
        private Sequence _enableSequence;

        private void Awake()
        {
            Broadcaster.AddListener(GameEvent.GameStateChanged, GameStateChanged);
            _all = new Component[] { LevelPlacementCanvas, ScoreCanvas, InfoCanvas };
        }

        private void GameStateChanged(GameEvent state)
        {
            KillDisableSequence();
            KillEnableSequence();
            switch (GameManager.CurrentGameState)
            {
                case GameState.LevelPlaced:
                case GameState.LevelPlacement:
                case GameState.LevelRePlacement:
                    DisableCanvases(_all.Except(new Component[] { LevelPlacementCanvas }));
                    EnableCanvases(new Component[] { LevelPlacementCanvas });
                    break;

                case GameState.LevelSimulating:
                    DisableCanvases(_all.Except(new Component[] { ScoreCanvas, InfoCanvas }));
                    EnableCanvases(new Component[] { ScoreCanvas, InfoCanvas });
                    break;

                default:
                    DisableCanvases(_all);
                    break;
            }
        }

        private void KillDisableSequence()
        {
            if (_disableSequence != null && !_disableSequence.IsComplete())
            {
                _disableSequence.Kill(true);
            }
        }

        private void KillEnableSequence()
        {
            if (_enableSequence != null && !_enableSequence.IsComplete())
            {
                _enableSequence.Kill();
            }
        }

        private void DisableCanvases(IEnumerable<Component> canvases)
        {
            _disableSequence = DOTween.Sequence();
            foreach (var canvas in canvases)
            {
                var group = canvas.GetOrAddComponent<CanvasGroup>();

                _disableSequence.OnStart(() =>
                {
                    group.interactable = false;
                    group.blocksRaycasts = false;
                });
                _disableSequence.Append(group.DOFade(0f, .76f));
                _disableSequence.Play();
            }
            if (Time.timeSinceLevelLoad < 2f) _disableSequence.Complete(true);
        }

        private void EnableCanvases(IEnumerable<Component> canvases)
        {
            _enableSequence = DOTween.Sequence();
            foreach (var canvas in canvases)
            {
                var group = canvas.GetOrAddComponent<CanvasGroup>();
                _enableSequence.OnComplete(() =>
                {
                    group.interactable = true;
                    group.blocksRaycasts = true;
                });
                _enableSequence.Append(group.DOFade(1f, .76f).SetDelay(0.76f));
                _enableSequence.Play();
            }
            if (Time.timeSinceLevelLoad < 2f) _enableSequence.Complete(true);
        }
    }
}