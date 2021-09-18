using UnityEngine;

namespace SST.Input.Tools
{
    using SST.Gameplay.Building;
    using SST.Gameplay.Modules;
    using SST.Gameplay.Controllers;

    public class DeleteCursorInputTool : CursorInputTool
    {
        public StateManager stateManager;
        public BuildingGrid buildingGrid;
        public Vector3 activeSnapPoint;
        public Vector2Int activeGridPoint;
        public bool valid;

        public override void OnSelectNothing(Vector3 point) {
            UpdateSnapPoint(point);
            DeleteObject();
        }

        public override void OnSelectObject(Vector3 point, Vector3 world, GameObject gameObject) {
            UpdateSnapPoint(world);
            DeleteObject();
        }

        public override void OnHoverNothing(Vector3 point) {
            UpdateSnapPoint(point);
        }

        public override void OnHoverObject(Vector3 point, Vector3 world, GameObject gameObject) {
            UpdateSnapPoint(world);
        }

        private bool IsDeleteValid(Vector2Int point, int layer = 0) {
            BuildCell space = buildingGrid.GetCell(point);
            return (space.moduleId != 0);
        }

        private void UpdateSnapPoint(Vector3 worldPoint) {
            activeGridPoint = buildingGrid.WorldToGridSpace(worldPoint);
            activeSnapPoint = buildingGrid.GridToWorldSpace(activeGridPoint);
            valid = IsDeleteValid(activeGridPoint);
        }

        private void DeleteObject() {
            if (valid) {
                Debug.Log("Valid Delete");
                BuildCell cell = buildingGrid.GetCell(activeGridPoint);
                GameObject cellObject = cell.gameObject;
                Vector2Int size = Vector2Int.one;
                Vector2Int pivot = Vector2Int.zero;
                Vector2Int origin = buildingGrid.WorldToGridSpace(cellObject.transform.position);
                EDirection direction = EDirection.NORTH;
                if (cellObject) {
                    ModuleBehaviour moduleBehaviour = cellObject.GetComponentInChildren<ModuleBehaviour>();
                    direction = moduleBehaviour.instanceData.direction;
                    size = moduleBehaviour.templateData.size;
                    pivot = moduleBehaviour.templateData.pivot;
                    uint id = moduleBehaviour.instanceData.id;
                    Destroy(cellObject);

                    stateManager.moduleStateController.RemoveModule(id);
                    stateManager.idStateController.RecycleId(id);
                }

                Vector2Int[] pointsArray = new Vector2Int[size.x * size.y];
                int pointsCount = buildingGrid.GetCoveredPointsNoAlloc(pointsArray, origin, pivot, size, direction);
                for (int i = 0; i < pointsCount; i++) {
                    buildingGrid.SetCell(pointsArray[i], new BuildCell() { moduleId = 0, buildType = EBuildType.CanBuild });
                }


                buildingGrid.gridChanged.Invoke();
            } else {
                Debug.Log("Invalid Delete");
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(activeSnapPoint, Vector3.one);
        }
    }
}