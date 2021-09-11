using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay.Building
{
    [System.Serializable]
    public struct PlaceData
    {
        public Portal[] portals;
        public Vector2Int size;
        public Vector2Int pivot;
        public GameObject prefab;
    }
}