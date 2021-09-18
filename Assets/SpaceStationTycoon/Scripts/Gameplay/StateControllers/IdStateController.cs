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
                idPool = UniqueIdUtility.CreatePool(defaultPoolChunkSize),
            };
        }

        public uint GetUniqueId() {
            return UniqueIdUtility.GetUniqueId(data.idPool, defaultPoolChunkSize);
        }

        public void RecycleId(uint id) {
            int idx = UniqueIdUtility.GetUsedUniqueIdIndex(data.idPool, id);
            if (idx != -1) {
                UniqueIdUtility.RecycleUniqueId(data.idPool, idx);
            }
        }

        public void SetData(IdStateData data) {
            this.data = data;
        }
    }
}