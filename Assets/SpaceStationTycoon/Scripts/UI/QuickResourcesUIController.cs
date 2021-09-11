using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SST.UI.Controllers
{
    using SST.Gameplay.Controllers;

    public class QuickResourcesUIController : MonoBehaviour, ITurnListener
    {
        public ResourceStateController resourcesStateController;
        public Text creditsValueText;
        public Text reputationValueText;
        public Text visitorsValueText;
        public Text fuelValueText;
        public Text partsValueText;
        public Text goodsValueText;

        private void OnEnable() {
            FindObjectOfType<GameplayPipelineController>()?.RegisterListener(this);
        }

        private void OnDisable() {
            FindObjectOfType<GameplayPipelineController>()?.UnregisterListener(this);
        }

        public void OnEndTurn() {
            //
        }

        public void OnStartTurn() {
            creditsValueText.text = resourcesStateController.currentCredits.ToString();
            reputationValueText.text = resourcesStateController.currentReputation.ToString();
            visitorsValueText.text = "0";
            fuelValueText.text = resourcesStateController.currentFuel.ToString();
            partsValueText.text = resourcesStateController.currentParts.ToString();
            goodsValueText.text = resourcesStateController.currentGoods.ToString();
        }
    }
}