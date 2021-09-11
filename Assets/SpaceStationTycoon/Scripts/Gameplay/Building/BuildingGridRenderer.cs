using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay.Building
{
    public class BuildingGridRenderer : MonoBehaviour
    {
        public BuildingGrid buildingGrid;
        public Renderer renderTarget;
        public Texture2D activeTexture;
        public string renderTargetTextureName = "_MainTex";
        private byte[] textureData;

        public Color32 colorNone = Color.black;
        public Color32 colorCanExpand= Color.white;
        public Color32 colorCanBuild = Color.yellow;
        public Color32 colorOccupied = Color.red;

        public void Awake() {
            buildingGrid.gridChanged.AddListener(() => {
                UpdateTexture();
            });
        }

        public void Start() {
            UpdateTexture();
        }

        public void ApplyTexture() {
            Debug.Log("Applied Grid Texture");
        }

        public Color32 ResolveCellColor(EBuildType buildType) {
            if (buildType == EBuildType.Occupied) {
                return colorOccupied;
            } else if (buildType == EBuildType.CanBuild) {
                return colorCanBuild;
            } else if (buildType == EBuildType.CanExpand) {
                return colorCanExpand;
            } else {
                return colorNone;
            }
        }

        public void UpdateTexture() {
            int zDepth = 0;

            activeTexture = new Texture2D(buildingGrid.gridSize.x, buildingGrid.gridSize.y, TextureFormat.RGB24, false, false);
            activeTexture.filterMode = FilterMode.Point;
            activeTexture.name = "GridRendererTex";
            activeTexture.wrapMode = TextureWrapMode.Clamp;

            // Byte array is 3x size for RGB24 format (one pixel is 3 bytes)
            int size = buildingGrid.gridSize.x * buildingGrid.gridSize.y * 3;
            textureData = new byte[size];
            for (int x = 0; x < buildingGrid.gridSize.x; x++) {
                for (int y = 0; y < buildingGrid.gridSize.y; y++) {
                    int index = (buildingGrid.gridSize.x * x + y) * 3;
                    Color32 color = ResolveCellColor(buildingGrid.grid[x, y, zDepth].buildType);
                    textureData[index] = color.r;
                    textureData[index + 1] = color.g;
                    textureData[index + 2] = color.b;
                }
            }

            activeTexture.SetPixelData(textureData, 0, 0);
            activeTexture.Apply();

            renderTarget.material.SetTexture(renderTargetTextureName, activeTexture);
        }
    }
}