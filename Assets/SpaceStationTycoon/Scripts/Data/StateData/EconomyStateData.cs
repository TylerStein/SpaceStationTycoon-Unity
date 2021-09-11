using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Data
{
    [System.Serializable]
    public struct EconomyStateData
    {
        public int maxFuelPrice;
        public int maxPartsPrice;
        public int maxGoodsPrice;

        public int minFuelPrice;
        public int minPartsPrice;
        public int minGoodsPrice;

        public int fuelPurchase;
        public int partsPurchase;
        public int goodsPurchase;
    }
}