using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RotaryHeart.Lib.SerializableDictionary;

namespace SST.Data
{
    [System.Serializable]
    public class ModuleDef
    {
        public ModuleStorageDef stores;
        public ModuleConsumeDef consumes;
        public ModuleCreateDef creates;
        public ModuleFufillDef fufills;
    }

    [System.Serializable]
    public class ModuleStorageDef : SerializableDictionaryBase<string, int> { }

    [System.Serializable]
    public class ModuleConsumeDef : SerializableDictionaryBase<string, int> { }

    [System.Serializable]
    public class ModuleCreateDef : SerializableDictionaryBase<string, int> { }

    [System.Serializable]
    public class ModuleFufillDef : SerializableDictionaryBase<string, ModuleFufillNeedDef> { }

    [System.Serializable]
    public class ModuleFufillNeedDef : SerializableDictionaryBase<string, int> { }
}
