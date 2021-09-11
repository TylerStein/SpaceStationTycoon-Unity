using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace SST.Gameplay.Controllers
{
    using SST.Data;
    using SST.DomainTree;
    using SST.Gameplay.Modules;

    [System.Serializable]
    public class BuildStateController : MonoBehaviour
    {
        [SerializeField] private BuildStateData data;
        private Dictionary<uint, ModuleBehaviour> moduleBehaviours;
        private DomainTree<List<uint>> moduleDomainTree;

        public BuildStateController() {
            moduleBehaviours = new Dictionary<uint, ModuleBehaviour>();
            moduleDomainTree = new DomainTree<List<uint>>();
            data = new BuildStateData() {
                builtModules = new List<ModuleData>(),
            };
        }

        public void AddModule(ModuleBehaviour module) {
            int index = data.builtModules.FindIndex((x) => x.id == module.instanceData.id);
            if (index == -1) {
                moduleBehaviours.Add(module.instanceData.id, module);
                var existing = moduleDomainTree.Search(module.instanceData.moduleName);
                if (existing != null && existing.value.Contains(module.instanceData.id) == false) {
                    existing.value.Add(module.instanceData.id);
                } else {
                    List<uint> ids = new List<uint>();
                    ids.Add(module.instanceData.id);
                    moduleDomainTree.Add(module.instanceData.moduleName, ids);
                }

                data.builtModules.Add(module.instanceData);
            }
        }

        public void RemoveModule(uint moduleId) {
            int index = data.builtModules.FindIndex((x) => x.id == moduleId);
            if (index != -1) {
                moduleBehaviours.Remove(moduleId);
                var existing = moduleDomainTree.Search(data.builtModules[index].moduleName);
                if (existing != null) {
                    existing.value.Remove(moduleId);
                    if (existing.value.Count == 0) {
                        moduleDomainTree.Remove(data.builtModules[index].moduleName);
                    }
                }

                data.builtModules.RemoveAt(index);
            }
        }

        public List<ModuleData> FindModules(string templateDomain) {
            // TODO: Find a better way to do this!
            var domains = moduleDomainTree.SearchAllChildren(templateDomain);
            var moduleData = domains
                .Where((x) => x.hasValue == true)
                .SelectMany((x) => x.value)
                .Select((x) => data.builtModules.Find((y) => x == y.id))
                .Where((x) => x != null)
                .ToList();
            return moduleData;
        }

        public ModuleBehaviour GetModuleBehaviour(uint moduleId) {
            return moduleBehaviours[moduleId];
        }

        public void SetData(BuildStateData data) {
            this.data = data;
        }
    }
}