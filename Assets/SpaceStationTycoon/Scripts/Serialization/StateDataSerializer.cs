using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Serialization {
    using Data;

    public class StateDataSerializer : MonoBehaviour
    {
        public string SerializeGameState(StateData data) {
            string serialized = JsonUtility.ToJson(data);
            return serialized;
        }

        public StateData DeserializeGameState(string data) {
            StateData deserialized = JsonUtility.FromJson<StateData>(data);
            return deserialized;
        }
    }
}