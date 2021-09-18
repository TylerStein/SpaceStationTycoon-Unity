using UnityEngine;

namespace SST.Utilities
{
    public class Interpolation
    {
        public float t;

        public Interpolation() {
            t = 0;
        }

        public void Reset() {
            t = 0;
        }

        public void Update(float d) {
            t = Mathf.Clamp01(t + d);
        }

        public Vector3 Cosine(Vector3 from, Vector3 to) {
            return Interpolation.Cosine(from, to, t);
        }

        public static Vector3 Cosine(Vector3 from, Vector3 to, float t) {
            float t2 = (1f - Mathf.Cos(t * Mathf.PI)) / 2f;
            return (from * (1f - t2) + to * t2);
        }
    }
}
