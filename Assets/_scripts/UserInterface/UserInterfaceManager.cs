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
        private Component[] _all;

        private void Awake()
        {
            Broadcaster.AddListener(GameEvent.GameStateChanged, GameStateChanged);
            _all = new Component[] { LevelPlacementCanvas, ScoreCanvas };
        }

        private void GameStateChanged(GameEvent state)
        {
            switch (GameManager.CurrentGameState)
            {
                case GameState.LevelPlaced:
                case GameState.LevelPlacement:
                case GameState.LevelRePlacement:
                    DisableCanvases(_all.Except(new Component[] { LevelPlacementCanvas }));
                    EnableCanvases(new Component[] { LevelPlacementCanvas });
                    break;

                case GameState.LevelSimulating:
                    DisableCanvases(_all.Except(new Component[] { ScoreCanvas }));
                    EnableCanvases(new Component[] { ScoreCanvas });
                    break;

                default:
                    DisableCanvases(_all);
                    break;
            }
        }

        private void DisableCanvases(IEnumerable<Component> canvases)
        {
            foreach (var canvas in canvases)
            {
                var group = canvas.GetOrAddComponent<CanvasGroup>();
                group.interactable = false;
                group.blocksRaycasts = false;
                var tween = group.DOFade(0f, .76f);
                if (Time.timeSinceLevelLoad < 2f) tween.Complete(true);
            }
        }

        private void EnableCanvases(IEnumerable<Component> canvases)
        {
            foreach (var canvas in canvases)
            {
                var group = canvas.GetOrAddComponent<CanvasGroup>();
                group.interactable = true;
                group.blocksRaycasts = true;
                var tween = group.DOFade(1f, .76f).SetDelay(0.76f);
                if (Time.timeSinceLevelLoad < 2f) tween.Complete(true);
            }
        }
    }
}