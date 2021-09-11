using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SST.UI.Controllers
{
    public class UIFloatingPanelsController : MonoBehaviour
    {
        public List<UIFloatingPanel> floatingPanels;

        public void CloseAllPanels() {
            foreach (UIFloatingPanel panel in floatingPanels) {
                panel.ClosePanel();
            }
        }
    }
}