using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.Gameplay.Controllers
{
    using Pipeline;

    public class GameplayPipelineController : MonoBehaviour
    {
        public StateManager stateController;
        public List<ITurnListener> turnListeners = new List<ITurnListener>();
        public List<GameplayPipelineStep> steps;

        public void RegisterListener<T>(T listener) where T : Object, ITurnListener {
            if (turnListeners.Contains(listener)) {
                Debug.LogWarning("Cannot register the same listener more than once", listener);
                return;
            }

            turnListeners.Add(listener);
        }

        public void UnregisterListener<T>(T listener) where T : Object, ITurnListener {
            if (turnListeners == null) return;
            bool removed = turnListeners.Remove(listener);
            if (!removed) {
                Debug.LogWarning("Cannot unregister listener, element was not in the list", listener);
            }
        }

        public void OnTurn() {
            foreach (ITurnListener listener in turnListeners) {
                listener.OnStartTurn();
            }

            foreach (GameplayPipelineStep step in steps) {
                step.Run(stateController);
            }

            foreach (ITurnListener listener in turnListeners) {
                listener.OnEndTurn();
            }
        }
    }
}