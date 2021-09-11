using UnityEngine;

namespace SST.Gameplay.Pipeline
{
    using SST.Gameplay.Controllers;

    [System.Serializable]
    public abstract class GameplayPipelineStep : MonoBehaviour
    {
        public abstract void Run(StateManager stateController);
    }
}