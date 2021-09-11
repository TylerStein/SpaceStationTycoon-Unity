using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Input.Movement
{
    public class CursorMovementController : MonoBehaviour
    {
        public Transform cameraTransform;
        public Camera viewCamera;
        public CursorInputController cursorInputController;
        public CursorCameraInput cursorCameraInput;

        public Vector2 edgeSize = new Vector2(0.05f, 0.05f);
        public Vector2 maxVelocity = new Vector2(5f, 5f);
        public float edgeAcceleration = 5f;

        public float zoomSpeed = 2f;
        public float minDistance = 4f;
        public float maxDistance = 12f;

        private Vector2 velocity = Vector2.zero;

        private void Update() {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Q)) {
                cameraTransform.RotateAround(cursorCameraInput.lastWorldFocusPoint, Vector3.up, 90f);
            } else if (UnityEngine.Input.GetKeyDown(KeyCode.E)) {
                cameraTransform.RotateAround(cursorCameraInput.lastWorldFocusPoint, Vector3.up, -90f);
            }

            if (UnityEngine.Input.GetKey(KeyCode.Z)) {
                viewCamera.orthographicSize -= zoomSpeed * Time.deltaTime;
            } else if (UnityEngine.Input.GetKey(KeyCode.X)) {
                viewCamera.orthographicSize += zoomSpeed * Time.deltaTime;
            }

            Vector2 targetVelocity = Vector2.zero;
            Vector2 mouse = cursorInputController.CursorPosition;

            float sizeX = viewCamera.pixelWidth;
            float edgeX = sizeX * edgeSize.x;

            if (mouse.x < edgeX && mouse.x >= 0) targetVelocity.x = -maxVelocity.x;
            else if (mouse.x > (sizeX - edgeX) && mouse.x <= sizeX) targetVelocity.x = maxVelocity.x;

            float sizeY = viewCamera.pixelHeight;
            float edgeY = sizeY * edgeSize.y;

            if (mouse.y < edgeY && mouse.y >= 0) targetVelocity.y = -maxVelocity.y;
            else if (mouse.y > (sizeY - edgeY) && mouse.y <= sizeY) targetVelocity.y = maxVelocity.y;

            velocity = Vector2.MoveTowards(velocity, targetVelocity, edgeAcceleration * Time.deltaTime);

            Vector3 forward = cameraTransform.forward;
            forward.y = 0;
            forward.Normalize();
            forward *= velocity.y;
            cameraTransform.Translate(forward * Time.deltaTime, Space.World);

            Vector3 right = cameraTransform.right;
            right.y = 0;
            right.Normalize();
            right *= velocity.x;
            cameraTransform.Translate(right * Time.deltaTime, Space.World);
        }
    }
}