using UnityEngine;

namespace SST.Gameplay.Controllers
{
    using SST.Data;

    public class EconomyStateController : MonoBehaviour, ITurnListener
    {
        private readonly int defaultMaxFuelPrice = 100;
        private readonly int defaultMaxPartsPrice = 100;
        private readonly int defaultMaxGoodsPrice = 100;

        private readonly int defaultMinFuelPrice = 1;
        private readonly int defaultMinPartsPrice = 1;
        private readonly int defaultMinGoodsPrice = 1;

        private readonly int defaultFuelPurchase = 0;
        private readonly int defaultPartsPurchase = 0;
        private readonly int defaultGoodsPurchase = 0;

        [SerializeField] private EconomyStateData data;

        public int currentFuelPurchase;
        public int currentPartsPurchase;
        public int currentGoodsPurchase;

        private int currentFuelPrice;
        private int currentPartsPrice;
        private int currentGoodsPrice;

        public int CurrentFuelCost => currentFuelPrice * currentFuelPurchase;
        public int CurrentPartsCost => currentPartsPrice * currentPartsPurchase;
        public int CurrentGoodsCost => currentGoodsPrice * currentGoodsPurchase;
        public int CurrentTotalCost => CurrentFuelCost + CurrentPartsCost + CurrentGoodsCost;

        public int CurrentFuelPrice => currentFuelPrice;
        public int CurrentPartsPrice => currentPartsPrice;
        public int CurrentGoodsPrice => currentGoodsPrice;

        private void OnEnable() {
            FindObjectOfType<GameplayPipelineController>()?.RegisterListener(this);
        }

        private void OnDisable() {
            FindObjectOfType<GameplayPipelineController>()?.UnregisterListener(this);
        }
        public EconomyStateController() {
            data = new EconomyStateData() {
                maxFuelPrice = defaultMaxFuelPrice,
                maxGoodsPrice = defaultMaxGoodsPrice,
                maxPartsPrice = defaultMaxPartsPrice,
                minFuelPrice = defaultMinFuelPrice,
                minGoodsPrice = defaultMinGoodsPrice,
                minPartsPrice = defaultMinPartsPrice,
                fuelPurchase = defaultFuelPurchase,
                partsPurchase = defaultPartsPurchase,
                goodsPurchase = defaultGoodsPurchase,
            };
        }

        public void SetData(EconomyStateData data) {
            this.data = data;
            currentFuelPurchase = data.fuelPurchase;
            currentPartsPurchase = data.partsPurchase;
            currentPartsPurchase = data.goodsPurchase;
        }

        public void OnStartTurn() {
            float totalPurchase = currentFuelPurchase + currentPartsPurchase + currentGoodsPurchase;
            if (totalPurchase == 0) {
                currentFuelPrice = data.minFuelPrice;
                currentPartsPrice = data.minPartsPrice;
                currentGoodsPrice = data.minGoodsPrice;
            } else {
                currentFuelPrice = Mathf.RoundToInt(currentFuelPurchase / totalPurchase * (data.maxFuelPrice - data.minFuelPrice)) + data.minFuelPrice;
                currentPartsPrice = Mathf.RoundToInt(currentPartsPurchase / totalPurchase * (data.maxPartsPrice - data.minPartsPrice)) + data.minPartsPrice;
                currentGoodsPrice = Mathf.RoundToInt(currentGoodsPurchase / totalPurchase * (data.maxGoodsPrice - data.minGoodsPrice)) + data.minGoodsPrice;
            }
        }

        public void OnEndTurn() {
            data.fuelPurchase = currentFuelPurchase;
            data.partsPurchase = currentPartsPurchase;
            data.goodsPurchase = currentGoodsPurchase;
        }
    }
}