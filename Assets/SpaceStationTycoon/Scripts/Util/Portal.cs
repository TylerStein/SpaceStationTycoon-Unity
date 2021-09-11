using UnityEngine;

namespace SST
{
    [System.Serializable]
    public class Portal
    {
        public DirectionFlags direction = Direction.AllDirections;
        public Vector2Int point = Vector2Int.zero;
    }
}
