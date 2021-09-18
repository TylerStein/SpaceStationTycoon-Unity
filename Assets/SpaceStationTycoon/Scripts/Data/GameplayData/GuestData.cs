using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Data
{
    [System.Serializable]
    public class GuestData
    {
        public uint id = 0;
        public uint shipId = 0;

        public int goods;
        public int credits;
        public int energy;
        public int social;
        public int hunger;
        public int fun;
        public int safety;
        public int comfort;
    }
}