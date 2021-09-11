using UnityEngine;
using System.Collections.Generic;

namespace SST.Gameplay.Controllers
{
    using SST.Data;

    [System.Serializable]
    public class BuildStateController : MonoBehaviour
    {
        [SerializeField] private BuildStateData data;

        public BuildStateController() {
            data = new BuildStateData() {
                builtModules = new List<ModuleData>(),
            };
        }

        public void AddModule(ModuleData moduleData) {
            int index = data.builtModules.FindIndex((x) => x.id == moduleData.id);
            if (index == -1) {
                data.builtModules.Add(moduleData);
            }
        }

        public void RemoveModule(uint moduleId) {
            int index = data.builtModules.FindIndex((x) => x.id == moduleId);
            if (index != -1) {
                data.builtModules.RemoveAt(index);
            }
        }

        public void SetData(BuildStateData data) {
            this.data = data;
        }
    }
}