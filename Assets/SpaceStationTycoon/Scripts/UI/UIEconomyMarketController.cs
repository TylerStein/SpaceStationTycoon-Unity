using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SST.UI.Controllers
{
    using Graphics;
    using Gameplay.Controllers;

    public class UIEconomyMarketController : MonoBehaviour, ITurnListener
    {
        public EconomyStateController economyStateController;

        public Text totalPriceText;

        public UINumberInputRow fuelInputRow;
        public Text fuelPriceText;

        public UINumberInputRow partsInputRow;
        public Text partsPriceText;

        public UINumberInputRow goodsInputRow;
        public Text goodsPriceText;

        public void Awake() {
            fuelInputRow.valueChanged.AddListener((value) => economyStateController.currentFuelPurchase = value);
            partsInputRow.valueChanged.AddListener((value) => economyStateController.currentPartsPurchase = value);
            goodsInputRow.valueChanged.AddListener((value) => economyStateController.currentGoodsPurchase = value);
        }

        private void OnEnable() {
            FindObjectOfType<GameplayPipelineController>()?.RegisterListener(this);
            if (Application.isPlaying) {
                UpdateValues();
            }
        }

        private void OnDisable() {
            FindObjectOfType<GameplayPipelineController>()?.UnregisterListener(this);
        }


        public void OnEndTurn() {
            //
        }

        public void OnStartTurn() {
            fuelPriceText.text = FormatCost(economyStateController.CurrentFuelPrice, economyStateController.currentFuelPurchase);
            partsPriceText.text = FormatCost(economyStateController.CurrentPartsPrice, economyStateController.currentPartsPurchase);
            goodsPriceText.text = FormatCost(economyStateController.CurrentGoodsPrice, economyStateController.currentGoodsPurchase);
            totalPriceText.text = $"{economyStateController.CurrentTotalCost} C";
        }

        public void UpdateValues() {
            fuelInputRow.SetValue(economyStateController.currentFuelPurchase);
            fuelPriceText.text = FormatCost(economyStateController.CurrentFuelPrice, economyStateController.currentFuelPurchase);

            partsInputRow.SetValue(economyStateController.currentPartsPurchase);
            partsPriceText.text = FormatCost(economyStateController.CurrentPartsPrice, economyStateController.currentPartsPurchase);

            goodsInputRow.SetValue(economyStateController.currentGoodsPurchase);
            goodsPriceText.text = FormatCost(economyStateController.CurrentGoodsPrice, economyStateController.currentGoodsPurchase);
        }

        public string FormatCost(int cost, int purchase) {
            return $"({cost} x {purchase} = {cost * purchase} C)";
        }
    }
}