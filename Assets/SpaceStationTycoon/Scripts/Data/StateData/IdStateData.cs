namespace SST.Data
{
    using System.Collections.Generic;

    [System.Serializable]
    public struct IdStateData
    {
        public IdPool shipIdPool;
        public IdPool idPool;
    }

    public struct IdPool
    {
        public List<uint> used;
        public List<uint> unused;
        public int maxSize;
    }
}