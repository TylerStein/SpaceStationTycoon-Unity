using UnityEngine;

namespace SST.Gameplay.Controllers {
    using Data;

    public class StateManager : MonoBehaviour
    {
        public AssetStateController assetStateController;
        public ModuleStateController moduleStateController;
        public VisitorStateController visitorStateController;
        public EconomyStateController economyStateController;
        public IdBindingStateController idBindingStateData;
        public IdStateController idStateController;
        public LogStateController logStateController;
        public MetadataStateController metadataStateController;
        public ResourceStateController resourceStateController;
        public TimeStateController timeStateController;

        public StateManager() {
            //
        }

        public void LoadFromStates(StateData data) {
            assetStateController.SetData(data.assetState);
            moduleStateController.SetData(data.moduleState);
            visitorStateController.SetData(data.visitorState);
            economyStateController.SetData(data.economyState);
            idBindingStateData.SetData(data.idBindingState);
            idStateController.SetData(data.idState);
            logStateController.SetData(data.logState);
            metadataStateController.SetData(data.metadataState);
            resourceStateController.SetData(data.resourceState);
            timeStateController.SetData(data.timeState);
        }
    }
}