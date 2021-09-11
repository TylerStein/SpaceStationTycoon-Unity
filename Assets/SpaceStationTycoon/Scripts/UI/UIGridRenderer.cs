using UnityEngine;
using UnityEngine.UI;

namespace SST.UI.Graphics
{
    public class UIGridRenderer : Graphic
    {
        public float thickness = 0.1f;
        public Vector2Int gridSize = new Vector2Int(1, 1);

        Vector2 min;
        Vector2 max;
        float cellWidth;
        float cellHeight;

        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();

            min = rectTransform.rect.min;
            max = rectTransform.rect.max;

            cellWidth = rectTransform.rect.width / (float)gridSize.x;
            cellHeight = rectTransform.rect.height / (float)gridSize.y;

            int count = 0;
            for (int y = 0; y < gridSize.y; y++) {
                for (int x = 0; x < gridSize.x; x++) {
                    DrawCell(x, y, count, vh);
                    count++;
                }
            }


            // vh.AddTriangle(0, 1, 2);
            // vh.AddTriangle(2, 3, 0);
        }

        private void DrawRect(UIVertex vertex, VertexHelper vh, Vector2 min, Vector2 max) {
            vertex.position = min;
            vh.AddVert(vertex);

            vertex.position = new Vector3(min.x, max.y);
            vh.AddVert(vertex);

            vertex.position = max;
            vh.AddVert(vertex);

            vertex.position = new Vector3(max.x, min.y);
            vh.AddVert(vertex);
        }

        private void DrawCell(int x, int y, int index, VertexHelper vh) {
            float xPos = min.x + (cellWidth * x);
            float yPos = min.y + (cellHeight * y);

            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            Vector2 pos = new Vector2(xPos, yPos);
            Vector2 size = new Vector2(cellWidth, cellHeight);

            DrawRect(vertex, vh, pos, pos + size);

            float widthSqr = thickness * thickness;
            float distanceSqr = widthSqr / 2f;
            float distance = Mathf.Sqrt(distanceSqr);

            Vector2 d2 = new Vector2(distance, distance);
            DrawRect(vertex, vh, pos + d2, pos + size - d2);

            int offset = index * 8;

            // Left Edge
            vh.AddTriangle(offset + 0, offset + 1, offset + 5);
            vh.AddTriangle(offset + 5, offset + 4, offset + 0);

            // Top Edge
            vh.AddTriangle(offset + 1, offset + 2, offset + 6);
            vh.AddTriangle(offset + 6, offset + 5, offset + 1);

            // Right Edge
            vh.AddTriangle(offset + 2, offset + 3, offset + 7);
            vh.AddTriangle(offset + 7, offset + 6, offset + 2);

            // Bottom Edge
            vh.AddTriangle(offset + 3, offset + 0, offset + 4);
            vh.AddTriangle(offset + 4, offset + 7, offset + 3);
        }
    }
}