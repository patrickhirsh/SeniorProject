using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class LevelPlacementCanvas : MonoBehaviour
    {
        public Button ContinueButton;
        public Slider ScaleSlider;
        public Slider RotationSlider;
        public Text HelpText;

        private Sequence _sequence;

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
            RotationSlider.onValueChanged.AddListener(HandleRotationSlider);
        }

        private void GameStateChanged(GameEvent @event)
        {

            if (GameManager.CurrentGameState == GameState.LevelPlacement)
            {
                KillSequence();
                _sequence = DOTween.Sequence();
                _sequence.OnStart(() =>
                {
                    ScaleSlider.gameObject.SetActive(false);
                    HelpText.gameObject.SetActive(true);
                    ContinueButton.gameObject.SetActive(false);
                    ContinueButton.GetOrAddComponent<CanvasGroup>().alpha = 0;
                    ScaleSlider.GetOrAddComponent<CanvasGroup>().alpha = 0;
                });
                _sequence.Append(HelpText.GetOrAddComponent<CanvasGroup>().DOFade(1f, 1f));
                _sequence.Join(HelpText.DOText("Move your camera around and find a spot to place the level", .5f));
                _sequence.Play();
            }
            else if (GameManager.CurrentGameState == GameState.LevelPlaced || GameManager.CurrentGameState == GameState.LevelRePlacement)
            {
                KillSequence();
                _sequence = DOTween.Sequence();
                _sequence.OnStart(() =>
                {
                    ContinueButton.gameObject.SetActive(true);
                    ScaleSlider.gameObject.SetActive(true);
                });

                HelpText.DOText("Use the slider below to scale the level. Press continue when you are ready to begin!", .5f);
                _sequence.Append(ContinueButton.GetOrAddComponent<CanvasGroup>().DOFade(1f, 1f));
                _sequence.Join(ScaleSlider.GetOrAddComponent<CanvasGroup>().DOFade(1f, 1f));
                _sequence.Play();
            }
        }

        private void KillSequence()
        {
            if (_sequence != null && !_sequence.IsComplete())
            {
                _sequence.Kill();
            }
        }

        #endregion

        private void HandleScaleSlider(float val)
        {
            ARScaler.Instance.Scale(val);
        }

        private void HandleRotationSlider(float val)
        {
            ARScaler.Instance.Rotate(val);
        }

        private void HandleContinueButton()
        {
            // Done with placement, transition to simulation
            GameManager.SetGameState(GameState.LevelSimulating);
        }

        public void SetRotation(float rotationY)
        {
            RotationSlider.value = rotationY;
        }
    }
}
