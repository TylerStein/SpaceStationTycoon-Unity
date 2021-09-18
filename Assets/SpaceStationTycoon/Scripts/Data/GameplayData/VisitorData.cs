using UnityEngine;

namespace SST.Data
{
    [System.Serializable]
    public class ShipData
    {
        public uint id = 0;

        public int currentFuel;
        public int maxFuel;
        public int fuelType;

        public int currentRepair;
        public int maxRepair;
        public int repairType;
    }
}