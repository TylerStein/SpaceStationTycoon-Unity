using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Data
{
    [System.Serializable]
    public struct IdBindingStateData
    {
        public Dictionary<uint, List<uint>> moduleVisitorIdBinding;
    }
}