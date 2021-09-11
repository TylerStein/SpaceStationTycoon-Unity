using UnityEngine;
using UnityEngine.UI;

namespace SST.UI.Controllers
{
    using SST.Gameplay.Controllers;

    public class DateTimeUIController : MonoBehaviour, ITurnListener
    {
        private readonly int[] timeScaleValues = new int[] { 1, 2, 4, 8, 16 };
        [SerializeField] private int timeScaleIndex = 0;

        public Text yearText;
        public Text monthText;
        public Text dayText;
        public TimeStateController timeStateController;
        public Button timeButton;
        public Text timeText;

        private void OnEnable() {
            FindObjectOfType<GameplayPipelineController>()?.RegisterListener(this);
        }

        private void OnDisable() {
            FindObjectOfType<GameplayPipelineController>()?.UnregisterListener(this);
        }

        private void Start() {
            timeButton.onClick.AddListener(() => {
                timeScaleIndex++;
                if (timeScaleIndex >= timeScaleValues.Length) {
                    timeScaleIndex = 0;
                }

                timeStateController.timeScale = timeScaleValues[timeScaleIndex];
                timeText.text = $"{timeScaleValues[timeScaleIndex]}x";
            });
        }

        public void OnEndTurn() {
            //
        }

        public void OnStartTurn() {
            yearText.text = timeStateController.Year.ToString();
            monthText.text = timeStateController.Month.ToString();
            dayText.text = timeStateController.Day.ToString();
        }
    }
}