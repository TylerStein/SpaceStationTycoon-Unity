using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SST.Input
{
    /// <summary>
    ///     Resolves interaction for passing to an interaction consumer
    /// </summary>
    public class CursorInputController : MonoBehaviour
    {
        public CursorCameraInput cursorCameraInput;
        [SerializeField] private CursorInputTool activeTool;
        public CursorInputTool defaultTool;
        public EventSystem eventSystem;

        public UnityEvent changeTool;

        public CursorInputTool ActiveTool { get { return activeTool; } }

        private bool primaryDown = false;
        public bool PrimaryDown { get { return primaryDown; } }

        private bool primaryStay = false;
        public bool PrimaryStay { get { return primaryStay; } }

        private bool secondaryDown = false;
        public bool SecondaryDown { get { return secondaryDown; } }

        private bool modifierDown = false;
        public bool ModifierDown { get { return modifierDown; } }

        [SerializeField] private Vector3 cursorPosition = Vector3.zero;
        public Vector3 CursorPosition { get { return cursorPosition; } }

        [SerializeField] private Vector3 cursorWorldPosition = Vector3.zero;
        public Vector3 CursorWorldPosition { get { return cursorWorldPosition; } }

        private void Awake() {
            activeTool = defaultTool;
            defaultTool.OnActivateTool(this);
        }

        private void Update() {
            if (secondaryDown) {
                SetActiveTool(defaultTool);
            }

            cursorPosition = UnityEngine.Input.mousePosition;
            primaryDown = UnityEngine.Input.GetButtonDown("Fire1");
            primaryStay = UnityEngine.Input.GetButton("Fire1");
            secondaryDown = UnityEngine.Input.GetButtonDown("Fire2");
            modifierDown = UnityEngine.Input.GetButtonDown("Fire3");

            RaycastHit hit;
            bool didHit = cursorCameraInput.RaycastMousePosition(out hit);
            cursorWorldPosition = hit.point;

            if (eventSystem.IsPointerOverGameObject()) {
                // dont place if over ui
                return;
            }

            if (didHit) {
                if (primaryDown) {
                    activeTool.OnSelectObject(hit.point, cursorCameraInput.lastWorldMouse, hit.collider.gameObject);
                } else if (primaryStay) {
                    activeTool.OnStayObject(hit.point, cursorCameraInput.lastWorldMouse, hit.collider.gameObject);
                } else {
                    activeTool.OnHoverObject(hit.point, cursorCameraInput.lastWorldMouse, hit.collider.gameObject);
                }
            } else {
                if (primaryDown) {
                    activeTool.OnSelectNothing(cursorCameraInput.lastWorldMouse);
                } else if (primaryStay) {
                    activeTool.OnStayNothing(cursorCameraInput.lastWorldMouse);
                } else {
                    activeTool.OnHoverNothing(cursorCameraInput.lastWorldMouse);
                }
            }
        }

        public void SetActiveTool(CursorInputTool tool) {
            if (activeTool == tool) return;
            activeTool.OnDeactivateTool();
            activeTool = tool;
            activeTool.OnActivateTool(this);
            changeTool.Invoke();
        }
    }
}