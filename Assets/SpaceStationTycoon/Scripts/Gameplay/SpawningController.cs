using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay
{
    using SST.Data;
    using SST.Gameplay.Modules;

    public class SpawningController : MonoBehaviour
    {
        public GameObject visitorPrefab;
        public GameObject shipPrefab;
        public GameObject crewPrefab;

        public List<GameObject> modulePrefabs;
        [SerializeField] private Dictionary<ModuleTemplateData, GameObject> modulePrefabDictionary;

        private void Awake() {
            modulePrefabDictionary = new Dictionary<ModuleTemplateData, GameObject>();
            foreach (GameObject obj in modulePrefabs) {
                ModuleBehaviour module = obj.GetComponent<ModuleBehaviour>();
                if (!module) throw new UnityException("SpawningController module prefab must have a ModuleBehavior component");
                modulePrefabDictionary[module.templateData] = obj;
            }
        }

        public GameObject GetPrefab(ModuleTemplateData template) {
            return modulePrefabDictionary[template];
        }

        public bool TryGetPrefab(ModuleTemplateData template, out GameObject prefab) {
            return modulePrefabDictionary.TryGetValue(template, out prefab);
        }
    }
}