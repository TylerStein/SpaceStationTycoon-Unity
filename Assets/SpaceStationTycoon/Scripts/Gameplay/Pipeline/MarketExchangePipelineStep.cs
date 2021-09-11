namespace SST.Gameplay.Pipeline
{
    using Controllers;
    using Utilities;

    [System.Serializable]
    public class MarketExchangePipelineStep : GameplayPipelineStep
    {
        public override void Run(StateManager stateController) {
            // get user credits
            int credits = stateController.resourceStateController.currentCredits;

            // get user requested market intake
            int requestedFuel = stateController.economyStateController.currentFuelPurchase;
            int requestedParts = stateController.economyStateController.currentPartsPurchase;
            int requestedGoods = stateController.economyStateController.currentGoodsPurchase;

            // calculate exchange in order of resource priority
            int fuelPricePerUnit = stateController.economyStateController.CurrentFuelPrice;
            int partsPricePerUnit = stateController.economyStateController.CurrentPartsPrice;
            int goodsPricePerUnit = stateController.economyStateController.CurrentGoodsPrice;

            int totalFuelPrice = fuelPricePerUnit * requestedFuel;
            int totalPartsPrice = partsPricePerUnit * requestedParts;
            int totalGoodsPrice = goodsPricePerUnit * requestedGoods;

            credits -= totalFuelPrice;
            credits -= totalPartsPrice;
            credits -= totalGoodsPrice;

            stateController.resourceStateController.currentFuel += requestedFuel;
            stateController.resourceStateController.currentParts += requestedGoods;
            stateController.resourceStateController.currentGoods += requestedGoods;

            stateController.resourceStateController.currentCredits = credits;
        }
    }
}