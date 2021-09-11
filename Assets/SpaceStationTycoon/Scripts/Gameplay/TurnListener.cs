using UnityEngine;

namespace SST
{
    public interface ITurnListener
    {
        public void OnStartTurn();
        public void OnEndTurn();
    }
}