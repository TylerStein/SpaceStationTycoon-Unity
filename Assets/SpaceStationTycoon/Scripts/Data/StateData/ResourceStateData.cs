namespace SST.Data
{
    using System.Collections.Generic;

    [System.Serializable]
    public struct ResourceStateData
    {
        public List<int> creditsHistory;
        public List<int> reputationHistory;
        public List<int> fuelHistory;
        public List<int> goodsHistory;
        public List<int> partsHistory;
    }
}