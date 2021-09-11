using UnityEngine;

namespace SST.Input
{
    public class CursorInputTool : MonoBehaviour
    {
        protected bool isActiveTool;
        public CursorInputController cursorInputController;

        public virtual void OnActivateTool(CursorInputController cursorInputController) {
            this.cursorInputController = cursorInputController;
            isActiveTool = true;
        }

        public virtual void OnDeactivateTool() {
            isActiveTool = false;
        }

        public virtual void OnSelectNothing(Vector3 point) {
            //
        }

        public virtual void OnSelectObject(Vector3 point, Vector3 world, GameObject gameObject) {
            //
        }

        public virtual void OnStayNothing(Vector3 point) {
            //
        }

        public virtual void OnStayObject(Vector3 point, Vector3 world, GameObject gameObject) {
            //
        }

        public virtual void OnHoverNothing(Vector3 point) {
            //
        }

        public virtual void OnHoverObject(Vector3 point, Vector3 world, GameObject gameObject) {
            //
        }
    }
}