using UnityEngine;

namespace SST.Gameplay.Controllers
{
    using Utilities;
    using Data;

    [System.Serializable]
    public class ResourceStateController : MonoBehaviour, ITurnListener
    {
        public readonly int defaultHistoryLength = 50;
        [SerializeField] private ResourceStateData data;

        public ResourceStateData Data => data;

        public int currentFuel;
        public int currentGoods;
        public int currentParts;
        public int currentCredits;
        public int currentReputation;

        private void OnEnable() {
            FindObjectOfType<GameplayPipelineController>()?.RegisterListener(this);
            data = new ResourceStateData() {
                creditsHistory = HistoryTrackingUtility.CreateEmpty(defaultHistoryLength, currentCredits),
                reputationHistory = HistoryTrackingUtility.CreateEmpty(defaultHistoryLength, currentReputation),
                fuelHistory = HistoryTrackingUtility.CreateEmpty(defaultHistoryLength, currentFuel),
                goodsHistory = HistoryTrackingUtility.CreateEmpty(defaultHistoryLength, currentGoods),
                partsHistory = HistoryTrackingUtility.CreateEmpty(defaultHistoryLength, currentParts),
            };
        }

        private void OnDisable() {
            FindObjectOfType<GameplayPipelineController>()?.UnregisterListener(this);
        }

        public void SetData(ResourceStateData data) {
            this.data = data;
        }

        public void OnStartTurn() {
            currentCredits = HistoryTrackingUtility.Peek(data.creditsHistory, 0);
            currentReputation = HistoryTrackingUtility.Peek(data.reputationHistory, 0);
            currentFuel = HistoryTrackingUtility.Peek(data.fuelHistory, 0);
            currentParts = HistoryTrackingUtility.Peek(data.partsHistory, 0);
            currentGoods = HistoryTrackingUtility.Peek(data.goodsHistory, 0);
        }

        public void OnEndTurn() {
            HistoryTrackingUtility.Push(ref data.creditsHistory, currentCredits);
            HistoryTrackingUtility.Push(ref data.reputationHistory, currentReputation);
            HistoryTrackingUtility.Push(ref data.fuelHistory, currentFuel);
            HistoryTrackingUtility.Push(ref data.partsHistory, currentParts);
            HistoryTrackingUtility.Push(ref data.goodsHistory, currentGoods);
        }
    }
}