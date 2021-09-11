using System.Collections.Generic;
using UnityEngine;

namespace SST.UI.Controllers
{
    using Graphics;
    using Gameplay.Controllers;

    public class UIEconomyChartController : MonoBehaviour, ITurnListener
    {
        public ResourceStateController resourceStateController;
        public UIGridRenderer gridRenderer;

        public UnityEngine.UI.Graphic background;

        public Vector2Int chartSize;
        public Color backgroundColor = Color.white;

        public Color gridColor = Color.cyan;
        public float gridThickness = 2f;

        public int cellCountX = 50;
        public int cellCountY = 1000000;

        public UILineRenderer creditsLineRenderer;
        public UILineRenderer reputationLineRenderer;
        public UILineRenderer fuelLineRenderer;
        public UILineRenderer partsLineRenderer;
        public UILineRenderer goodsLineRenderer;

        private void OnEnable() {
            FindObjectOfType<GameplayPipelineController>()?.RegisterListener(this);
            if (Application.isPlaying) {
                UpdateAllLines();
            }
        }

        private void OnDisable() {
            FindObjectOfType<GameplayPipelineController>()?.UnregisterListener(this);
        }

        public void OnEndTurn() {
            //
        }

        public void OnStartTurn() {
            if (!isActiveAndEnabled) return;
            UpdateAllLines();
        }

        public void UpdateAllLines() {
            UpdateLineFromData(creditsLineRenderer, resourceStateController.Data.creditsHistory);
            UpdateLineFromData(reputationLineRenderer, resourceStateController.Data.reputationHistory);
            UpdateLineFromData(fuelLineRenderer, resourceStateController.Data.fuelHistory);
            UpdateLineFromData(partsLineRenderer, resourceStateController.Data.partsHistory);
            UpdateLineFromData(goodsLineRenderer, resourceStateController.Data.goodsHistory);
        }

        public void UpdateLineFromData(UILineRenderer target, List<int> data) {
            List<Vector2> creditPoints;
            Vector2 range = GetLineData(data, out creditPoints);
            target.points = creditPoints;
            target.gridSize = new Vector2Int(cellCountX, cellCountY);
            target.SetVerticesDirty();
        }

        public Vector2 GetLineData(List<int> values, out List<Vector2> points) {
            Vector2 range = new Vector2(0, 0);
            List<Vector2> results = new List<Vector2>();
            for (int i = 0; i < values.Count; i++) {
                results.Add(new Vector2(i, values[i]));
                if (values[i] < range.x) range.x = values[i];
                if (values[i] > range.y) range.y = values[i];
            }
            points = results;
            return range;
        }
    }
}