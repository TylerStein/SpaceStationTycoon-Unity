using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay
{
    using Building;
    using Data;
    using Controllers;
    using Modules;

    public class DebugGridInitializer : MonoBehaviour
    {
        public StateManager stateManager;

        public BuildingGrid buildingGrid;
        public SpawningController spawningController;

        public ModuleTemplateData pathTemplateData;
        public ModuleTemplateData dockTemplateData;

        // Start is called before the first frame update
        void Start() {
            for (int x = 0; x < buildingGrid.gridSize.x; x++) {
                for (int y = 0; y < buildingGrid.gridSize.y; y++) {
                    for (int z = 0; z < buildingGrid.gridSize.z; z++) {
                        int index = buildingGrid.grid.Index(x, y, z);
                        buildingGrid.grid[x, y, z] = new BuildCell() { moduleId = 0, buildType = EBuildType.CanBuild, gridIndex = index, gridLayer = z };
                    }
                }
            }

            // for testing intersection
            {
                int floorHalfX = Mathf.FloorToInt(buildingGrid.gridSize.x / 2f);
                int floorHalfY = Mathf.FloorToInt(buildingGrid.gridSize.y / 2f);
                int ceilHalfX = floorHalfX + 1;
                int ceilHalfY = floorHalfY + 1;

                SpawnAtGridPoint(dockTemplateData, new Vector3Int(0, floorHalfY, 0));
                SpawnAtGridPoint(dockTemplateData, new Vector3Int(0, ceilHalfY, 0));

                SpawnAtGridPoint(pathTemplateData, new Vector3Int(1, ceilHalfY, 0));
                SpawnAtGridPoint(pathTemplateData, new Vector3Int(1, floorHalfY, 0));

                SpawnAtGridPoint(pathTemplateData, new Vector3Int(2, ceilHalfY, 0));
                SpawnAtGridPoint(pathTemplateData, new Vector3Int(2, floorHalfY, 0));

                //grid[floorHalfX - 1, floorHalfY, 0].buildType = EBuildType.Occupied;
                //grid[floorHalfX - 1, ceilHalfY, 0].buildType = EBuildType.Occupied;
                //grid[ceilHalfX + 1, ceilHalfY, 0].buildType = EBuildType.Occupied;
                //grid[ceilHalfX + 1, floorHalfY, 0].buildType = EBuildType.Occupied;

                //grid[floorHalfX, floorHalfY - 1, 0].buildType = EBuildType.Occupied;
                //grid[floorHalfX, ceilHalfY + 1, 0].buildType = EBuildType.Occupied;
                //grid[ceilHalfX, ceilHalfY + 1, 0].buildType = EBuildType.Occupied;
                //grid[ceilHalfX, floorHalfY - 1, 0].buildType = EBuildType.Occupied;

                //grid[floorHalfX - 2, floorHalfY, 0].buildType = EBuildType.Occupied;
                //grid[floorHalfX - 2, ceilHalfY, 0].buildType = EBuildType.Occupied;
                //grid[ceilHalfX + 2, ceilHalfY, 0].buildType = EBuildType.Occupied;
                //grid[ceilHalfX + 2, floorHalfY, 0].buildType = EBuildType.Occupied;

                //grid[floorHalfX, floorHalfY - 2, 0].buildType = EBuildType.Occupied;
                //grid[floorHalfX, ceilHalfY + 2, 0].buildType = EBuildType.Occupied;
                //grid[ceilHalfX, ceilHalfY + 2, 0].buildType = EBuildType.Occupied;
                //grid[ceilHalfX, floorHalfY - 2, 0].buildType = EBuildType.Occupied;
            }

            Debug.Log("Initialized Grid");
            buildingGrid.gridChanged.Invoke();
        }

        public void SpawnAtGridPoint(ModuleTemplateData template, Vector3Int position) {
            GameObject prefab = spawningController.GetPrefab(template);
            if (prefab != null) {
                int index = buildingGrid.grid.Index(position.x, position.y, position.z);
                Vector3 worldPos = buildingGrid.GridToWorldSpace(new Vector2Int(position.x, position.y));
                GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity);
                ModuleBehaviour moduleBehaviour = instance.GetComponentInChildren<ModuleBehaviour>();
                moduleBehaviour.instanceData = new ModuleData() {
                    id = stateManager.idStateController.GetUniqueModuleId(),
                    position = position,
                    moduleName = template.moduleName
                };
                stateManager.buildStateController.AddModule(moduleBehaviour.instanceData);

                BuildCell cell = new BuildCell() {
                    gameObject = instance,
                    buildType = EBuildType.Occupied,
                    navType =  template.navType,
                    gridLayer = position.z,
                    gridIndex = index,
                    moduleId = 0,
                };
                buildingGrid.grid[position.x, position.y, position.z] = cell;
            } else {
                Debug.LogWarning("Failed to get prefab with templateName " + template.name);
            }
        }
    }
}