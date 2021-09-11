using UnityEngine;

namespace SST.Gameplay.Controllers {
    using Data;
    using Utilities;

    public class IdStateController : MonoBehaviour
    {
        private static int defaultPoolChunkSize = 50;
        [SerializeField] private IdStateData data;

        public IdStateController() {
            data = new IdStateData() {
                shipIdPool = UniqueIdUtility.CreatePool(defaultPoolChunkSize),
                moduleIdPool = UniqueIdUtility.CreatePool(defaultPoolChunkSize),
            };
        }

        public uint GetUniqueModuleId() {
            return UniqueIdUtility.GetUniqueId(data.moduleIdPool, defaultPoolChunkSize);
        }

        public void RecycleModuleId(uint id) {
            int idx = UniqueIdUtility.GetUsedUniqueIdIndex(data.moduleIdPool, id);
            if (idx != -1) {
                UniqueIdUtility.RecycleUniqueId(data.moduleIdPool, idx);
            }
        }

        public void SetData(IdStateData data) {
            this.data = data;
        }
    }
}