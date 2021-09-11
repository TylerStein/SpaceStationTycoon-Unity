using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SST.Gameplay.Building
{
    public class BuildingGrid : MonoBehaviour {
        public bool drawDebug = false;

        public Grid3D<BuildCell> grid;
        public Vector3 worldSpaceOffset = new Vector3(0f, 0f, 0f);
        public Vector3Int gridSize = new Vector3Int(100, 100, 100);
        public UnityEvent gridChanged = new UnityEvent();
        public bool dirty = true;

        public void Awake() {
            if (gridSize.x % 2 == 0 || gridSize.y % 2 == 0) {
                Debug.LogWarning("Grid should be an odd XY size for alignment");
            }
            InitializeGrid(gridSize);
            dirty = true;
        }

        public void LateUpdate() {
            dirty = false;
        }

        public void RandomizeCells(int layer = 0) {
            float scale = 4f;

            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    float xCoord = (float)x / (float)gridSize.x * scale;
                    float yCoord = (float)y / (float)gridSize.y * scale;
                    float noise = Mathf.PerlinNoise(xCoord, yCoord);
                    int index = grid.Index(x, y, layer);

                    if (noise < 0.33f) {
                        grid[x, y, layer] = new BuildCell() { moduleId = 0, buildType = EBuildType.None, gridIndex = index, gridLayer = layer };
                    } else if (noise < 0.66f) {
                        grid[x, y, layer] = new BuildCell() { moduleId = 0, buildType = EBuildType.CanBuild, gridIndex = index, gridLayer = layer };
                    } else {
                        grid[x, y, layer] = new BuildCell() { moduleId = 0, buildType = EBuildType.Occupied, gridIndex = index, gridLayer = layer };
                    }
                }
            }

            Debug.Log("Randomization Complete");
            gridChanged.Invoke();
            dirty = true;
        }

        public void InitializeGrid(Vector3Int gridSize) {
            this.gridSize = gridSize;
            grid = new Grid3D<BuildCell>(gridSize.x, gridSize.y, gridSize.z, new BuildCell());
            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    for (int z = 0; z < gridSize.z; z++) {
                        int index = grid.Index(x, y, z);
                        grid[x, y, z] = new BuildCell() { moduleId = 0, buildType = EBuildType.CanBuild, gridIndex = index, gridLayer = z };
                    }
                }
            }

            Debug.Log("Initialized Grid");
            gridChanged.Invoke();
        }

        public int GetCellsWithNavTypeInDirections(Vector2Int point, DirectionFlags directions, ENavType navType, BuildCell[] cells, int layer = 0) {
            int count = 0;
            if (directions.HasFlag(DirectionFlags.NORTH)) {
                BuildCell cell = GetCell(point + Vector2Int.up, layer);
                if (cell.navType == navType) {
                    cells[count] = cell;
                    count++;
                }
            }

            if (directions.HasFlag(DirectionFlags.EAST)) {
                BuildCell cell = GetCell(point + Vector2Int.left, layer);
                if (cell.navType == navType) {
                    cells[count] = cell;
                    count++;
                }
            }

            if (directions.HasFlag(DirectionFlags.SOUTH)) {
                BuildCell cell= GetCell(point + Vector2Int.down, layer);
                if (cell.navType == navType) {
                    cells[count] = cell;
                    count++;
                }
            }

            if (directions.HasFlag(DirectionFlags.WEST)) {
                BuildCell cell = GetCell(point + Vector2Int.right, layer);
                if (cell.navType == navType) {
                    cells[count] = cell;
                    count++;
                }
            }

            return count;
        }

        public bool CheckCellNavTypeInDirections(Vector2Int point, DirectionFlags directions, ENavType navType, out BuildCell cell, int layer = 0) {
            if (directions.HasFlag(DirectionFlags.NORTH)) {
                cell = GetCell(point + Vector2Int.up, layer);
                if (cell.navType == navType) return true;
            }

            if (directions.HasFlag(DirectionFlags.EAST)) {
                cell = GetCell(point + Vector2Int.left, layer);
                if (cell.navType == navType) return true;
            }

            if (directions.HasFlag(DirectionFlags.SOUTH)) {
                cell = GetCell(point + Vector2Int.down, layer);
                if (cell.navType == navType) return true;
            }

            if (directions.HasFlag(DirectionFlags.WEST)) {
                cell = GetCell(point + Vector2Int.right, layer);
                if (cell.navType == navType) return true;
            }

            cell = null;
            return false;
        }

        public BuildCell GetCell(Vector2Int point, int layer = 0) {
            if (IsPointInGrid(point, layer) == false) {
                return new BuildCell() { moduleId = 0, buildType = EBuildType.None };
            }
            return grid[point.x, point.y, layer];
        }

        public List<BuildCell> FindCells(EBuildType flags, int layer) {
            List<BuildCell> result = new List<BuildCell>();
            for (int x = 0; x < gridSize.x; x++) {
                for (int y = 0; y < gridSize.y; y++) {
                    BuildCell cell = grid[x, y, layer];
                    if (cell.buildType.HasFlag(flags)) {
                        //if ((x + 1) > max.x) max.x = x + 1;
                        //else if (x < min.x) min.x = x + 1;

                        //if ((y + 1) > max.y) max.y = y + 1;
                        //else if (y < min.y) min.y = y + 1;

                        result.Add(cell);
                    }
                }
            }
            return result;
        }

        public void GetNeighborsNoAlloc(Vector2Int gridPoint, BuildCell[] cells, int layer = 0) {
            cells[0] = GetCell(new Vector2Int(gridPoint.x, gridPoint.y + 1), layer);
            cells[1] = GetCell(new Vector2Int(gridPoint.x + 1, gridPoint.y), layer);
            cells[2] = GetCell(new Vector2Int(gridPoint.x, gridPoint.y - 1), layer);
            cells[3] = GetCell(new Vector2Int(gridPoint.x - 1, gridPoint.y), layer);
        }

        public void GetDirectionNeighborsNoAlloc(Vector2Int gridPoint, DirectionFlags direction, BuildCell[] cells, int layer = 0) {
            cells[0] = direction.HasFlag(DirectionFlags.NORTH) ? GetCell(new Vector2Int(gridPoint.x, gridPoint.y + 1), layer) : default;
            cells[1] = direction.HasFlag(DirectionFlags.EAST) ? GetCell(new Vector2Int(gridPoint.x + 1, gridPoint.y), layer) : default;
            cells[2] = direction.HasFlag(DirectionFlags.SOUTH) ? GetCell(new Vector2Int(gridPoint.x, gridPoint.y - 1), layer) : default;
            cells[3] = direction.HasFlag(DirectionFlags.WEST) ? GetCell(new Vector2Int(gridPoint.x - 1, gridPoint.y), layer) : default;
        }

        public void SetCell(Vector2Int point, BuildCell cell, int layer = 0) {
            if (IsPointInGrid(point, layer) == false) return;
            cell.gridIndex = grid.Index(point.x, point.y, layer);
            cell.gridLayer = layer;
            grid[point.x, point.y, layer] = cell;
            dirty = true;
        }

        public Vector3 GridToWorldSpace(Vector2Int gridPoint) {
            return new Vector3(
                gridPoint.y - worldSpaceOffset.x,
                0f,
                gridPoint.x - worldSpaceOffset.z
            );
        }
        
        public Vector2Int WorldToGridSpace(Vector3 worldPosition) {
            return new Vector2Int(
                Mathf.RoundToInt(worldPosition.z + worldSpaceOffset.x),
                Mathf.RoundToInt(worldPosition.x + worldSpaceOffset.z)
            );
        }

        public Vector3Int IndexToGridSpace(int index) {
            int x, y, z;
            grid.Position(index, out x, out y, out z);
            return new Vector3Int(x, y, z);
        }

        public bool IsPointInGrid(Vector2Int point, int layer = 0) {
            return !(point.x >= gridSize.x || point.y >= gridSize.y || layer >= gridSize.z || point.x < 0 || point.y < 0 || layer < 0);
        }


        public int GetCoveredPointsNoAlloc(Vector2Int[] points, Vector2Int point, Vector2Int pivot, Vector2Int size, EDirection direction) {
            for (int x = 0; x < size.x; x++) {
                for (int y = 0; y < size.y; y++) {
                    Vector2Int subPoint = new Vector2Int(x, y);
                    subPoint = GetRotatedPoint(pivot, subPoint, direction);
                    points[size.y * x + y] = point + new Vector2Int(subPoint.x, subPoint.y);
                }
            }
            return size.x * size.y;
        }

        public Vector2Int[] GetCoveredPoints(Vector2Int point, Vector2Int pivot, Vector2Int size, EDirection direction) {
            Vector2Int[] points = new Vector2Int[size.x * size.y];
            GetCoveredPointsNoAlloc(points, point, pivot, size, direction);
            return points;
        }

        public Vector2Int GetRotatedPoint(Vector2Int pivot, Vector2Int point, EDirection direction = EDirection.NORTH) {
            Vector2Int rotated = point - pivot;
            rotated = Direction.ResolvePointRotation(direction, rotated);
            rotated += pivot;
            return rotated;
        }

        public Vector2Int GetRotatedPointCW(Vector2Int pivot, Vector2Int point) {
            Vector2Int rotated = point - pivot;
            rotated = new Vector2Int(-rotated.y, rotated.x);
            rotated += pivot;
            return rotated;
        }

        public Vector2Int GetRotatedPointCCW(Vector2Int pivot, Vector2Int point) {
            Vector2Int rotated = point - pivot;
            rotated = new Vector2Int(rotated.y, -rotated.x);
            rotated += pivot;
            return rotated;
        }

        public void OnDrawGizmosSelected() {
            if (!drawDebug) return;

            Vector3 worldMin = GridToWorldSpace(new Vector2Int(0, 0));
            Vector3 worldMax = GridToWorldSpace(new Vector2Int(gridSize.x - 1, gridSize.y - 1));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(worldMin, worldMax);

            if (grid != null) {
                for (int x = 0; x < gridSize.x; x++) {
                    for (int y = 0; y < gridSize.y; y++) {
                        Vector3 worldPos = GridToWorldSpace(new Vector2Int(x, y));
                        EBuildType flags = grid[x, y, 0].buildType;
                        if (flags.HasFlag(EBuildType.None)) Gizmos.color = Color.black;
                        else if (flags.HasFlag(EBuildType.None)) Gizmos.color = Color.grey;
                        else if (flags.HasFlag(EBuildType.CanExpand)) Gizmos.color = Color.cyan;
                        else if (flags.HasFlag(EBuildType.CanBuild)) Gizmos.color = Color.green;
                        else if (flags.HasFlag(EBuildType.Occupied)) Gizmos.color = Color.magenta;

                        Gizmos.DrawWireCube(worldPos, Vector3.one * 0.25f);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public enum ESpace : int
    {
        WorldSpace = 0,
        GridSpace = 1,
    }

    [System.Serializable]
    public enum EBuildType : sbyte
    {
        None = 0,
        CanExpand = 2,
        CanBuild = 3,
        Occupied = 4,
    }

    [System.Serializable]
    public enum ENavType : sbyte
    {
        None = 0,
        Walkable = 1,
        Blocked = 2, 
    }

    [System.Serializable]
    public class BuildCell
    {
        // 32 to 64 bits (reference)
        public GameObject gameObject;

        // 8 bits (sbyte)
        public EBuildType buildType;

        // 8 bits (sbyte)
        public ENavType navType;

        // 32 bits (int)
        public int gridLayer;

        // 32 bits (int)
        public int gridIndex;

        // 32 bits (uint)
        public uint moduleId;
    }
}