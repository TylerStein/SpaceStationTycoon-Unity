using System.Collections.Generic;

namespace SST.Data
{
    [System.Serializable]
    public struct VisitorStateData
    {
        public List<ShipData> ships;
        public List<GuestData> guests;
    }
}