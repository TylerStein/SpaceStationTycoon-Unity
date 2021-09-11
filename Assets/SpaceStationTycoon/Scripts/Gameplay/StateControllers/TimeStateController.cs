using UnityEngine;

namespace SST.Gameplay.Controllers
{
    using SST.Data;

    public class TimeStateController : MonoBehaviour
    {
        public int timeScale = 1;

        [SerializeField] GameplayPipelineController gameplayPipelineController;
        [SerializeField] float secondsInADay = 2;
        [SerializeField] int daysInAMonth = 30;
        [SerializeField] int monthsInAYear = 12;
        [SerializeField] private float tick = 0f;
        [SerializeField] private TimeStateData data;

        public int Day => data.day;
        public int Month => data.month;
        public int Year => data.year;

        public void SetData(TimeStateData data) {
            this.data = data;
            tick = 0;
        }

        // Update is called once per frame
        private void Update() {
            tick += Time.deltaTime * timeScale;
            if (tick >= secondsInADay) {
                tick = 0;
                IncrementDay();
                gameplayPipelineController.OnTurn();
            }
        }

        private void IncrementDay() {
            int day = data.day;
            day++;
            if (day >= daysInAMonth) {
                day = 0;
                IncrementMonth();
            }

            data.day = day;
        }

        private void IncrementMonth() {
            int month = data.month;
            month++;
            if (month >= monthsInAYear) {
                month = 0;
                IncrementYear();
            }

            data.month = month;
        }

        private void IncrementYear() {
            int year = data.year;
            year++;
            data.year = year;
        }
    }
}