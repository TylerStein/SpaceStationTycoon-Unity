using UnityEngine;

namespace SST.Input.Tools
{
    using SST.Gameplay.Building;
    using SST.Gameplay.Modules;
    using SST.Gameplay.Controllers;
    using SST.Data;

    public class PlaceCursorInputTool : CursorInputTool
    {
        public BuildingGrid buildingGrid;
        public StateManager stateManager;

        public Vector3 activeSnapPoint;
        public Vector2Int activeGridPoint;
        public Vector2Int lastGridPoint;
        public BuildCell activeBuildCell;
        public Vector2Int[] pointsBuffer;

        public bool lastPlacementValid = false;
        public GameObject placeGhostObject;

        public GameObject placePrefab;
        public Vector2Int placeSize;
        public Vector2Int placePivot;
        public Portal[] portals;
        public BuildCell[] buildCellsBuffer;

        public EDirection direction = EDirection.NORTH;
        public Material validPlacementMaterial;
        public Material invalidPlacementMaterial;

        bool doDebug = false;

        public void Awake() {
            //
        }

        public override void OnActivateTool(CursorInputController cursorInputController) {
            base.OnActivateTool(cursorInputController);
        }

        public void Update() {
            if (cursorInputController.ModifierDown) {
                direction = Direction.RotateCW(direction);
                doDebug = true;
            }

            if (placeGhostObject) {
                placeGhostObject.transform.eulerAngles = Direction.ResolveEulerAngles(direction);
            }
        }

        public override void OnDeactivateTool() {
            ClearPlaceData();
            base.OnDeactivateTool();
        }

        public override void OnSelectNothing(Vector3 point) {
            UpdateSnapPoint(point);
            PlaceObject();
        }

        public override void OnSelectObject(Vector3 point, Vector3 world, GameObject gameObject) {
            UpdateSnapPoint(world);
            PlaceObject();
        }

        public override void OnStayObject(Vector3 point, Vector3 world, GameObject gameObject) {
            UpdateSnapPoint(world);
            if (activeGridPoint != lastGridPoint) {
                Debug.Log("Auto Place");
                PlaceObject();
            }
        }

        public override void OnHoverNothing(Vector3 point) {
            UpdateSnapPoint(point);
        }

        public override void OnHoverObject(Vector3 point, Vector3 world, GameObject gameObject) {
            UpdateSnapPoint(world);
        }

        public void ClearPlaceData() {
            if (placeGhostObject) Destroy(placeGhostObject);
        }

        public void SetPlaceData(PlaceData placeData) {
            ClearPlaceData();
            direction = EDirection.NORTH;
            placeSize = placeData.size;
            placePivot = placeData.pivot;
            placePrefab = placeData.prefab;
            pointsBuffer = new Vector2Int[placeData.size.x * placeData.size.y];
            buildCellsBuffer = new BuildCell[4];
            portals = placeData.portals;
            placeGhostObject = Instantiate(placeData.prefab);
            placeGhostObject.SetActive(true);
            UpdatePlacementMaterial(lastPlacementValid, true);
        }

        private bool IsPlacementValid(int layer = 0) {
            BuildCell bufferCell;
            int pointsCount = buildingGrid.GetCoveredPointsNoAlloc(pointsBuffer, activeGridPoint, placePivot, placeSize, direction);
            for (int i = 0; i < pointsCount; i++) {
                bufferCell = buildingGrid.GetCell(pointsBuffer[i], layer);
                if (!CanBuildOnCell(bufferCell)) {
                    return false;
                }
            }

            if (doDebug) {
                Debug.Log("da bug");
                doDebug = false;
            }

            for (int i = 0; i < portals.Length; i++) {
                Vector2Int rotatedPoint = buildingGrid.GetRotatedPoint(placePivot, portals[i].point, direction) + activeGridPoint;
                DirectionFlags rotatedDirection = Direction.RotateFlags(portals[i].direction, direction);
                if (rotatedDirection.HasFlag(DirectionFlags.NORTH)) {
                    bufferCell = buildingGrid.GetCell(rotatedPoint + Vector2Int.up, layer);
                    Debug.DrawLine(activeSnapPoint, buildingGrid.GridToWorldSpace(rotatedPoint + Vector2Int.up));
                    if (bufferCell.navType == ENavType.Walkable) {
                        return true;
                    }
                }

                if (rotatedDirection.HasFlag(DirectionFlags.EAST)) {
                    bufferCell = buildingGrid.GetCell(rotatedPoint + Vector2Int.left, layer);
                    Debug.DrawLine(activeSnapPoint, buildingGrid.GridToWorldSpace(rotatedPoint + Vector2Int.right));
                    if (bufferCell.navType == ENavType.Walkable) {
                        return true;
                    }
                }

                if (rotatedDirection.HasFlag(DirectionFlags.SOUTH)) {
                    bufferCell = buildingGrid.GetCell(rotatedPoint + Vector2Int.down, layer);
                    Debug.DrawLine(activeSnapPoint, buildingGrid.GridToWorldSpace(rotatedPoint + Vector2Int.down));
                    if (bufferCell.navType == ENavType.Walkable) {
                        return true;
                    }
                }

                if (rotatedDirection.HasFlag(DirectionFlags.WEST)) {
                    bufferCell = buildingGrid.GetCell(rotatedPoint + Vector2Int.right, layer);
                    Debug.DrawLine(activeSnapPoint, buildingGrid.GridToWorldSpace(rotatedPoint + Vector2Int.left));
                    if (bufferCell.navType == ENavType.Walkable) {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CanBuildOnCell(BuildCell cell) {
            if (cell.buildType != EBuildType.CanBuild) {
                return false;
            }
            return true;
        }

        private void UpdateSnapPoint(Vector3 worldPoint) {
            var gridPoint = buildingGrid.WorldToGridSpace(worldPoint);
            lastGridPoint = activeGridPoint;
            activeGridPoint = gridPoint;
            activeSnapPoint = buildingGrid.GridToWorldSpace(activeGridPoint);
            activeBuildCell = buildingGrid.GetCell(activeGridPoint);
            bool valid = IsPlacementValid();
            UpdatePlacementMaterial(valid, false);
            lastPlacementValid = valid;

            if (placeGhostObject) {
                placeGhostObject.transform.position = activeSnapPoint + new Vector3(0, 0.05f, 0f);
                placeGhostObject.transform.localScale = Vector3.one * 1.05f;
            }
        }

        private void UpdatePlacementMaterial(bool valid, bool force) {
            if (!placeGhostObject) return;
            if (valid != lastPlacementValid || force) {
                if (valid) placeGhostObject.GetComponentInChildren<Renderer>().material = validPlacementMaterial;
                else placeGhostObject.GetComponentInChildren<Renderer>().material = invalidPlacementMaterial;
            }
        }

        private void PlaceObject() {
            if (lastPlacementValid && placePrefab) {
                Debug.Log("Valid Placement");

                uint moduleId = stateManager.idStateController.GetUniqueModuleId();

                GameObject instance = Instantiate(placePrefab);
                instance.transform.position = activeSnapPoint;
                instance.transform.eulerAngles = Direction.ResolveEulerAngles(direction);

                ModuleBehaviour moduleBehaviour = instance.GetComponentInChildren<ModuleBehaviour>();
                moduleBehaviour.instanceData = new ModuleData() {
                    id = stateManager.idStateController.GetUniqueModuleId(),
                    direction = direction,
                    position = new Vector3Int(activeGridPoint.x, activeGridPoint.y, 0),
                    moduleName = moduleBehaviour.templateData.moduleName
                };

                stateManager.buildStateController.AddModule(moduleBehaviour.instanceData);

                int pointsCount = buildingGrid.GetCoveredPointsNoAlloc(pointsBuffer, activeGridPoint, placePivot, placeSize, direction);
                for (int i = 0; i < pointsCount; i++) {
                    buildingGrid.SetCell(pointsBuffer[i], new BuildCell() {
                        moduleId = 1,
                        buildType = EBuildType.Occupied,
                        navType = moduleBehaviour.templateData.navType,
                        gameObject = instance,
                    });
                }

                buildingGrid.gridChanged.Invoke();
            } else {
                BuildCell cell = buildingGrid.GetCell(activeGridPoint);
                Debug.Log($"Invalid Placement @ ActiveGridPoint ({activeGridPoint.x}, {activeGridPoint.y}) / BuildCell ({cell.buildType}, {cell.moduleId})");
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(activeSnapPoint, Vector3.one);
        }
    }
}