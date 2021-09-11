using UnityEngine;
using System.Collections;

namespace SST.Input
{
    /// <summary>
    ///     Generic mosue input handler
    /// </summary>
    public class CursorCameraInput : MonoBehaviour
    {
        public bool drawDebug = false;

        public Vector3 lastScreenMousePosition = Vector3.zero;
        public Vector3 lastScreenFocusPoisiton = Vector3.zero;
        public Vector3 lastWorldMouse = Vector3.zero;
        public Vector3 lastWorldFocusPoint = Vector3.zero;

        public Vector3 mouseForward => _mouseRay.direction;

        public Plane worldPlane = new Plane(Vector3.up, Vector3.zero);
        public float defaultMouseDistance = 100f;

        public Camera inputCamera;

        private Transform _cameraTransform;
        private Ray _mouseRay;
        private Ray _focusRay;
        private float _mouseWorldDistance = 0f;
        private float _focusWorldDistance = 0f;

        void Start() {
            lastScreenMousePosition = Vector3.zero;
            inputCamera = Camera.main;
            _cameraTransform = inputCamera.transform;
        }

        void Update() {
            lastScreenMousePosition = UnityEngine.Input.mousePosition;
            UpdateScreenToWorldPosition(lastScreenMousePosition, out _mouseRay, out _mouseWorldDistance, out lastWorldMouse);

            lastScreenFocusPoisiton = new Vector3(inputCamera.pixelWidth / 2f, inputCamera.pixelHeight / 2f, 0f);
            UpdateScreenToWorldPosition(lastScreenFocusPoisiton, out _focusRay, out _focusWorldDistance, out lastWorldFocusPoint);
        }

        public void UpdateScreenToWorldPosition(Vector3 screenPosition, out Ray ray, out float worldDistance, out Vector3 worldPosition) {
            if (inputCamera.orthographic) {
                ray = inputCamera.ScreenPointToRay(screenPosition);
                worldDistance = defaultMouseDistance;
            } else {
                screenPosition.z = defaultMouseDistance;
                worldPosition = inputCamera.ScreenToWorldPoint(screenPosition + new Vector3(0, 0, inputCamera.nearClipPlane));
                Vector3 mouseForward = (worldPosition - _cameraTransform.position).normalized;
                ray = new Ray(_cameraTransform.position, mouseForward);
                worldDistance = defaultMouseDistance;
            }

            float planeDistance;
            if (worldPlane.Raycast(ray, out planeDistance)) {
                worldDistance = planeDistance;
            }

            worldPosition = ray.GetPoint(worldDistance);
        }

        public bool RaycastMousePosition(out RaycastHit hit) {
            return Physics.Raycast(_mouseRay, out hit, _mouseWorldDistance, 1 << 0);
        }

        public void OnDrawGizmos() {
            if (!drawDebug) return;

            Gizmos.color = Color.red;
            Gizmos.DrawRay(_mouseRay);
            Gizmos.DrawWireSphere(_mouseRay.GetPoint(_mouseWorldDistance), 1f);


            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(_focusRay);
            Gizmos.DrawWireSphere(_focusRay.GetPoint(_focusWorldDistance), 1f);
        }
    }
}