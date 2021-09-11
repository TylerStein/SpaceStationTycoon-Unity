using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SST.UI.Graphics
{
    public class UILineRenderer : Graphic
    {
        public Vector2Int gridSize = new Vector2Int(1, 1);
        public List<Vector2> points;
        public float thickness = 1f;

        Vector2 min;
        Vector2 size;
        Vector2 unitSize;
        float halfThickness;

        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();

            min = rectTransform.rect.min;
            size = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            unitSize = size / gridSize;
            halfThickness = thickness / 2f;

            if (points.Count < 2) return;

            float angle = 0f;
            for (int i = 0; i < points.Count; i++) {
                if (i < points.Count - 1) {
                    angle = GetAngle(points[i], points[i + 1]) + 45f;
                }
                
                DrawVerticesForPoint(points[i], vh, angle);
            }

            for (int i = 0; i < points.Count - 1; i++) {
                int index = i * 2;
                vh.AddTriangle(index + 0, index + 1, index + 3);
                vh.AddTriangle(index + 3, index + 2, index + 0);
            }
        }

        void DrawVerticesForPoint(Vector2 point, VertexHelper vh, float angle) {
            UIVertex vertex = UIVertex.simpleVert;
            vertex.color = color;

            Vector2 rectSpacePoint = (point * unitSize) + min;
            Vector2 thicknessVector = Quaternion.Euler(0, 0, angle) * (Vector3.right * halfThickness);

            vertex.position = -thicknessVector + rectSpacePoint;
            vh.AddVert(vertex);

            vertex.position = thicknessVector + rectSpacePoint;
            vh.AddVert(vertex);
        }

        float GetAngle(Vector2 a, Vector2 b) {
            return Mathf.Atan2(b.y - a.y, a.x - b.x) * (180f * Mathf.PI);
        }
    }
}