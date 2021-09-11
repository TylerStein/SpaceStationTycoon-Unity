using UnityEngine;

namespace SST.Input.Tools
{
    public class SelectCursorInputTool : CursorInputTool
    {
        public GameObject hoverObject;
        public GameObject selectedObject;

        public override void OnDeactivateTool() {
            hoverObject = null;
            selectedObject = null;
            base.OnDeactivateTool();
        }

        public override void OnSelectNothing(Vector3 point) {
            selectedObject = null;
        }

        public override void OnSelectObject(Vector3 point, Vector3 world, GameObject gameObject) {
            selectedObject = gameObject;
        }

        public override void OnHoverNothing(Vector3 point) {
            hoverObject = null;
        }

        public override void OnHoverObject(Vector3 point, Vector3 world, GameObject gameObject) {
            hoverObject = gameObject;
        }
    }
}