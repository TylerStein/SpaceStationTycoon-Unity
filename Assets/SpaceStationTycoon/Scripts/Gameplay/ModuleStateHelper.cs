using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay
{
    using Data;
    using Modules;
    using Controllers;
    using Building;

    public class ModuleStateHelper : MonoBehaviour
    {
        public StateManager stateManager;
        public BuildingGrid buildingGrid;

        public Dictionary<string, List<uint>> moduleIdsByStores = new Dictionary<string, List<uint>>();
        public Dictionary<string, List<uint>> moduleIdsByConsumes = new Dictionary<string, List<uint>>();
        public Dictionary<string, List<uint>> moduleIdsByCreates = new Dictionary<string, List<uint>>();
        public Dictionary<string, List<uint>> moduleIdsByFufills = new Dictionary<string, List<uint>>();

        public void Start() {
            Rebuild();
            buildingGrid.gridChanged.AddListener(Rebuild);
        }

        public void Rebuild() {
            moduleIdsByStores.Clear();
            moduleIdsByConsumes.Clear();
            moduleIdsByCreates.Clear();
            moduleIdsByFufills.Clear();

            foreach (ModuleBehaviour module in stateManager.moduleStateController.AllModuleBehaviours) {
                AddModule(module.instanceData.id, module.templateData);
            }
        }

        public void AddModule(uint id, ModuleTemplateData templateData) {
            foreach (var stores in templateData.def.stores) {
                AddStores(stores.Key, id);
            }

            foreach (var consumes in templateData.def.consumes) {
                AddConsumes(consumes.Key, id);
            }

            foreach (var creates in templateData.def.creates) {
                AddCreates(creates.Key, id);
            }

            foreach (var fufills in templateData.def.fufills) {
                AddFufills(fufills.Key, id);
            }
        }

        public void AddStores(string key, uint moduleId) {
            if (moduleIdsByStores.ContainsKey(key)) {
                moduleIdsByStores[key].Add(moduleId);
            } else {
                moduleIdsByStores[key] = new List<uint>() { moduleId };
            }
        }

        public void AddConsumes(string key, uint moduleId) {
            if (moduleIdsByConsumes.ContainsKey(key)) {
                moduleIdsByConsumes[key].Add(moduleId);
            } else {
                moduleIdsByConsumes[key] = new List<uint>() { moduleId };
            }
        }

        public void AddCreates(string key, uint moduleId) {
            if (moduleIdsByCreates.ContainsKey(key)) {
                moduleIdsByCreates[key].Add(moduleId);
            } else {
                moduleIdsByCreates[key] = new List<uint>() { moduleId };
            }
        }

        public void AddFufills(string key, uint moduleId) {
            if (moduleIdsByFufills.ContainsKey(key)) {
                moduleIdsByFufills[key].Add(moduleId);
            } else {
                moduleIdsByFufills[key] = new List<uint>() { moduleId };
            }
        }
    }
}
