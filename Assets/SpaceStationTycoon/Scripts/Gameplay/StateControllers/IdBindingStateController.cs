using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay.Controllers {
    using Data;

    [System.Serializable]
    public class IdBindingStateController : MonoBehaviour
    {
        [SerializeField] private IdBindingStateData data;

        public IdBindingStateController() {
            data = new IdBindingStateData() {
                moduleVisitorIdBinding = new Dictionary<uint, List<uint>>(),
            };
        }

        public void SetData(IdBindingStateData data) {
            this.data = data;
        }
    }
}