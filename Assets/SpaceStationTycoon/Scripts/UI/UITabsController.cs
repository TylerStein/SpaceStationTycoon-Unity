using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SST.UI.Controllers
{
    public class UITabsController : MonoBehaviour
    {

        [Header("Selected")]
        public ColorBlock selectedTabBackgroundColor;
        public Color selectedTabTextColor;

        [Header("Unselected")]
        public ColorBlock unselectedTabBackgroundColor;
        public Color unselectedTabTextColor;

        [Header("Tabs")]
        public List<UITab> tabs = new List<UITab>();
        [SerializeField] private int activeTab;

        public void Awake() {
            UpdateDisplay();
            for (int i = 0; i < tabs.Count; i++) {
                Button tabButton = tabs[i].tabButton;
                tabs[i].tabButton.onClick.AddListener(() => OnClickTab(tabButton));
            }
        }

        public void OnClickTab(Button button) {
            var tabIndex = tabs.FindIndex((x) => x.tabButton == button);
            if (tabIndex != -1) {
                SetActiveTab(tabIndex);
            }
        }

        public void SetActiveTab(int index) {
            if (index > tabs.Count || index < 0 || activeTab == index) return;
            activeTab = index;
            UpdateDisplay();
        }

        public void UpdateDisplay() {
            for (int i = 0; i < tabs.Count; i++) {
                if (i == activeTab) {
                    tabs[i].tabButton.colors = selectedTabBackgroundColor;
                    tabs[i].tabForeground.color = selectedTabTextColor;
                    tabs[i].tabContent.SetActive(true);
                } else {
                    tabs[i].tabButton.colors = unselectedTabBackgroundColor;
                    tabs[i].tabForeground.color = unselectedTabTextColor;
                    tabs[i].tabContent.SetActive(false);
                }
            }
        }
    }

    [System.Serializable]
    public struct UITab
    {
        public Button tabButton;
        public Graphic tabForeground;
        public GameObject tabContent;
    }
}