using System.Collections.Generic;

namespace SST.Utilities
{
    using Data;

    [System.Serializable]
    public class UniqueIdUtility
    {
        public static IdPool CreatePool(int chunkSize) {
            List<uint> unusedPool = new List<uint>();
            for (int i = chunkSize - 1; i >= 0; i--) {
                unusedPool.Add((uint)(i + 1));
            }

            return new IdPool() {
                unused = unusedPool,
                used = new List<uint>(),
                maxSize = chunkSize,
            };
        }

        public static void IncreasePoolSize(ref List<uint> pool, ref int maxSize, int chunkSize) {
            for (int i = chunkSize - 1; i >= 0; i--) {
                pool.Add((uint)(maxSize + i + 1));
            }
            maxSize = maxSize + chunkSize;
        }

        public static uint GetUniqueId(IdPool pool, int chunkSize) {
            return GetUniqueId(ref pool.unused, ref pool.used, ref pool.maxSize, chunkSize);
        }

        public static uint GetUniqueId(ref List<uint> unusedPool, ref List<uint> usedPool, ref int maxSize, int chunkSize) {
            if (unusedPool.Count == 0) {
                IncreasePoolSize(ref unusedPool, ref maxSize, chunkSize);
            }

            int index = unusedPool.Count - 1;
            uint value = unusedPool[index];
            unusedPool.RemoveAt(index);
            usedPool.Add(value);
            return value;
        }

        public static void RecycleUniqueId(IdPool pool, int recycleIndex) {
            RecycleUniqueId(ref pool.unused, ref pool.used, recycleIndex);
        }

        public static void RecycleUniqueId(ref List<uint> unusedPool, ref List<uint> usedPool, int recycleIndex) {
            uint value = usedPool[recycleIndex];
            usedPool.RemoveAt(recycleIndex);
            unusedPool.Add(value);
        }

        public static int GetUsedUniqueIdIndex(IdPool pool, uint id) {
            return pool.used.IndexOf(id);
        }
    }
}
