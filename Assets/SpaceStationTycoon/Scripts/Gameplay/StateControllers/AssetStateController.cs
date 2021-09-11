using UnityEngine;

namespace SST.Gameplay.Controllers
{
    using SST.Data;

    [System.Serializable]
    public class AssetStateController : MonoBehaviour
    {
        [SerializeField] private AssetStateData data;

        public AssetStateController() {
            data = new AssetStateData() {
                shipNames = new string[0]
            };
        }

        public void SetData(AssetStateData data) {
            this.data = data;
        }
    }
}