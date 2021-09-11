using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SST.Editor
{
    using SST.Gameplay.Building;

    [CustomEditor(typeof(BuildingGrid))]
    public class BuildingGridEditor : UnityEditor.Editor
    {
        private void OnSceneGUI() {
            BuildingGrid buildingGrid = (BuildingGrid)target;
            Handles.color = Color.blue;

            //Vector3Int gridSize = buildingGrid.gridSize;
            //for (int x = 0; x < gridSize.x; x++) {
            //    for (int y = 0; y < gridSize.y; y++) {
            //        for (int z = 0; z < gridSize.z; z++) {
            //            Vector3 worldPos = buildingGrid.GridToWorldSpace(new Vector3Int(x, y, z));
            //            worldPos.y = 0;
            //            Handles.Label(worldPos, $"{worldPos.x} {worldPos.y} {worldPos.z}");
            //        }
            //    }
            //}
        }
    }
}