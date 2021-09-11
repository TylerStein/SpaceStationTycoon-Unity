using UnityEngine;

namespace SST.Data
{
    [System.Serializable]
    public class ModuleData
    {
        public uint id = 0;
        public Vector3Int position = Vector3Int.zero;
        public EDirection direction = EDirection.NORTH;
        public string moduleName = "";
    }
}
