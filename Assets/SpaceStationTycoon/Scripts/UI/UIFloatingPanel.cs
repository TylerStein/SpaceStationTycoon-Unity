using UnityEngine.UI;
using UnityEngine;

namespace SST.UI
{
    using Controllers;

    public class UIFloatingPanel : MonoBehaviour
    {
        public Button closeButton;

        public void Awake() {
            closeButton.onClick.AddListener(ClosePanel);
        }

        public void TogglePanel() {
            if (isActiveAndEnabled) ClosePanel();
            else OpenPanel();
        }

        public void ClosePanel() {
            gameObject.SetActive(false);

        }

        public void OpenPanel() {
            gameObject.SetActive(true);
        }
    }
}