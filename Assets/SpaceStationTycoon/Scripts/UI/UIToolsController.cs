using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SST.UI.Controllers
{
    using SST.Input;
    using SST.Input.Tools;

    public class UIToolsController : MonoBehaviour
    {
        public CursorInputController cursorInputController;
        public UIBuildRowController buildRowController;

        public PlaceCursorInputTool placeCursorInputTool;
        public SelectCursorInputTool selectCursorInputTool;
        public DeleteCursorInputTool deleteCursorInputTool;

        public Button buildButton;
        public Button selectButton;
        public Button deleteButton;

        private void Awake() {
            cursorInputController.changeTool.AddListener(OnChangeTool);

            buildButton.onClick.AddListener(() => {
                cursorInputController.SetActiveTool(placeCursorInputTool);
            });

            selectButton.onClick.AddListener(() => {
                cursorInputController.SetActiveTool(selectCursorInputTool);
            });

            deleteButton.onClick.AddListener(() => {
                cursorInputController.SetActiveTool(deleteCursorInputTool);
            });
        }

        private void OnChangeTool() {
            if (cursorInputController.ActiveTool == placeCursorInputTool) {
                buildRowController.gameObject.SetActive(true);
            } else {
                buildRowController.gameObject.SetActive(false);
            }
        }
    }
}