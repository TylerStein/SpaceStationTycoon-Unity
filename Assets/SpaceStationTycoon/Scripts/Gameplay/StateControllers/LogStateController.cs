using UnityEngine;

namespace SST.Gameplay.Controllers
{
    using Utilities;
    using Data;

    public class LogStateController : MonoBehaviour
    {
        private const int defaultHistoryLength = 100;
        [SerializeField] private LogStateData data;

        public LogStateController() {
            data = new LogStateData() {
                logHistory = HistoryTrackingUtility.CreateEmpty(defaultHistoryLength, ""),
            };
        }

        public void SetData(LogStateData data) {
            this.data = data;
        }
    }
}