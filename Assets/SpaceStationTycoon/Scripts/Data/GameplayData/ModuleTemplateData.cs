using UnityEngine;

namespace SST.Data
{
    using SST.Gameplay.Modules;
    using SST.Gameplay.Building;

    [CreateAssetMenu(fileName = "Module Template", menuName = "SpaceStationTycoon/Module Template", order = 0)]
    public class ModuleTemplateData : ScriptableObject
    {
        public string moduleName;
        public int cost;
        public ENavType navType;
        public Vector2Int size;
        public Vector2Int pivot;
        public GameObject prefab;
        public Portal portal;
        public ModuleDef def;

        public PlaceData GetPlaceData() => new PlaceData() {
            pivot = pivot,
            size = size,
            prefab = prefab,
            portals = new Portal[1] { portal }
        };
    }
}
