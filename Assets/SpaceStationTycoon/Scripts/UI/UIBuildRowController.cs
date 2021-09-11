using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SST.UI.Controllers
{
    using SST.Data;
    using SST.Input.Tools;

    public class UIBuildRowController : MonoBehaviour
    {
        public PlaceCursorInputTool placeCursorInputTool;
        public List<ModuleTemplateData> modules;
        public GameObject buildModuleButtonPrefab;
        public List<Button> buildModuleButtons;
        public RectTransform container;

        private void Awake() {
            for (int i = 0; i < buildModuleButtons.Count; i++) {
                Destroy(buildModuleButtons[i]);
            }

            buildModuleButtons.Clear();

            for (int i = 0; i < modules.Count; i++) {
                GameObject instance = Instantiate(buildModuleButtonPrefab, container);
                Button button = instance.GetComponent<Button>();
                Text text = instance.GetComponentInChildren<Text>();
                ModuleTemplateData module = modules[i];
                text.text = module.moduleName;
                button.onClick.AddListener(() => SelectModule(module));
            }
        }

        private void SelectModule(ModuleTemplateData module) {
            placeCursorInputTool.SetPlaceData(module.GetPlaceData());
        }
    }
}