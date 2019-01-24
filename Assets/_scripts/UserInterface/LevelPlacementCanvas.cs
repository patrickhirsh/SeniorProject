using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class LevelPlacementCanvas : MonoBehaviour
    {
        public Button ContinueButton;
        public Slider ScaleSlider;
        public Text HelpText;

        #region Unity Methods

        private void Awake()
        {
            HelpText.text = "";
            Broadcaster.AddListener(GameEvent.GameStateChanged, GameStateChanged);
        }

        private void Start()
        {
            ContinueButton.onClick.AddListener(HandleContinueButton);
            ScaleSlider.onValueChanged.AddListener(HandleScaleSlider);
        }

        private void GameStateChanged(GameEvent @event)
        {
            if (GameManager.CurrentGameState == GameState.LevelPlacement)
            {
                ContinueButton.gameObject.SetActive(false);
                ContinueButton.GetOrAddComponent<CanvasGroup>().alpha = 0;

                ScaleSlider.gameObject.SetActive(false);
                ScaleSlider.GetOrAddComponent<CanvasGroup>().alpha = 0;

                HelpText.gameObject.SetActive(true);
                HelpText.GetOrAddComponent<CanvasGroup>().DOFade(1f, 1f);
                HelpText.DOText("Move your camera around and find a spot to place the level", .5f);
            }
            else if (GameManager.CurrentGameState == GameState.LevelPlaced || GameManager.CurrentGameState == GameState.LevelRePlacement)
            {
                HelpText.DOText("Use the slider below to scale the level. Press continue when you are ready to begin!", .5f);

                ContinueButton.gameObject.SetActive(true);
                ContinueButton.GetOrAddComponent<CanvasGroup>().DOFade(1f, 1f);

                ScaleSlider.gameObject.SetActive(true);
                ScaleSlider.GetOrAddComponent<CanvasGroup>().DOFade(1f, 1f);
            }
        }

        #endregion

        private void HandleScaleSlider(float val)
        {
            ARScaler.Instance.Scale(val);
        }

        private void HandleContinueButton()
        {
            // Done with placement, transition to simulation
            GameManager.SetGameState(GameState.LevelSimulating);
        }
    }
}
