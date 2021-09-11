using UnityEngine;

namespace SST.Gameplay.Controllers
{
    using Data;

    [System.Serializable]
    public class MetadataStateController : MonoBehaviour
    {
        [SerializeField] private MetadataStateData data;

        public MetadataStateController() {
            data = new MetadataStateData() {
                stationName = "",
                createDate = System.DateTime.Now,
            };
        }

        public void SetData(MetadataStateData data) {
            this.data = data;
        }
    }
}