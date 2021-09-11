using UnityEngine;
using System;

namespace SST
{
    [System.Serializable]
    public enum EDirection
    {
        NORTH = 1,
        EAST = 2,
        SOUTH = 4,
        WEST = 8
    }

    [System.Serializable]
    [System.Flags]
    public enum DirectionFlags
    {
        NORTH = 1,
        EAST = 2,
        SOUTH = 4,
        WEST = 8
    }

    public static class Direction
    {
        public static Vector3 EulerAngleNorth = Vector3.zero;
        public static Vector3 EulerAngleEast = new Vector3(0f, 90f, 0f);
        public static Vector3 EulerAngleSouth = new Vector3(0f, 180f, 0f);
        public static Vector3 EulerAngleWest = new Vector3(0f, 270f, 0f);
        public static DirectionFlags AllDirections = DirectionFlags.NORTH | DirectionFlags.EAST | DirectionFlags.SOUTH | DirectionFlags.WEST;

        public static DirectionFlags RotateFlags(DirectionFlags value, EDirection direction) {
            if (direction == EDirection.NORTH) {
                return value;
            } else if (direction == EDirection.EAST) {
                DirectionFlags rotated = 0;
                if (value.HasFlag(DirectionFlags.NORTH)) rotated |= DirectionFlags.EAST;
                if (value.HasFlag(DirectionFlags.EAST)) rotated |= DirectionFlags.SOUTH;
                if (value.HasFlag(DirectionFlags.SOUTH)) rotated |= DirectionFlags.WEST;
                if (value.HasFlag(DirectionFlags.WEST)) rotated |= DirectionFlags.NORTH;
                return rotated;
            } else if (direction == EDirection.SOUTH) {
                DirectionFlags rotated = 0;
                if (value.HasFlag(DirectionFlags.NORTH)) rotated |= DirectionFlags.SOUTH;
                if (value.HasFlag(DirectionFlags.WEST)) rotated |= DirectionFlags.EAST;
                if (value.HasFlag(DirectionFlags.SOUTH)) rotated |= DirectionFlags.NORTH;
                if (value.HasFlag(DirectionFlags.EAST)) rotated |= DirectionFlags.WEST;
                return rotated;
            } else {
                DirectionFlags rotated = 0;
                if (value.HasFlag(DirectionFlags.NORTH)) rotated |= DirectionFlags.WEST;
                if (value.HasFlag(DirectionFlags.WEST)) rotated |= DirectionFlags.SOUTH;
                if (value.HasFlag(DirectionFlags.SOUTH)) rotated |= DirectionFlags.EAST;
                if (value.HasFlag(DirectionFlags.EAST)) rotated |= DirectionFlags.NORTH;
                return rotated;
            }
        }

        public static DirectionFlags ToFlags(EDirection value) {
            return (DirectionFlags)value;
        }

        public static EDirection RotateCW(EDirection value) {
            if (value == EDirection.WEST) return EDirection.NORTH;
            else return (EDirection)((int)value * 2);
        }

        public static EDirection RotateCCW(EDirection value) {
            if (value == EDirection.NORTH) return EDirection.WEST;
            else return (EDirection)((int)value / 2);
        }

        public static Vector3 ResolveEulerAngles(EDirection value) {
            switch(value) {
                case EDirection.EAST: return EulerAngleEast;
                case EDirection.SOUTH: return EulerAngleSouth;
                case EDirection.WEST: return EulerAngleWest;
                default: return EulerAngleNorth;
            }
        }

        public static Vector2Int ResolvePointRotation(EDirection value, Vector2Int point) {
            switch (value) {
                case EDirection.EAST: return new Vector2Int(-point.y, point.x);
                case EDirection.SOUTH: return new Vector2Int(-point.x, -point.y);
                case EDirection.WEST: return new Vector2Int(point.y, -point.x);
                default: return point;
            }
        }
    }
}
