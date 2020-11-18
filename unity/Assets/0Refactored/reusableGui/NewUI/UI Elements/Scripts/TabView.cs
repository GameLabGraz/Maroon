using System.Collections.Generic;
using UnityEngine;

namespace Maroon.UI
{
    public class TabView : MonoBehaviour
    {
        [SerializeField] private Color activeColor = new Color(1f, 1f, 1f, 0.4f);
        [SerializeField] private Color inactiveColor = new Color(0.4f, 0.4f, 0.4f, 0.4f);

        private readonly List<Tab> _tabs = new List<Tab>();

        private int _selectedTabIndex;
        private Tab selectedTab => _tabs[_selectedTabIndex];


        private void Start()
        {
            _tabs.AddRange(GetComponentsInChildren<Tab>());
            if(_tabs.Count > 0)
                SelectTab(0);
        }

        public int GetTabIndex(Tab tab)
        {
            return _tabs.FindIndex(t => t == tab);
        }

        public void SelectTab(int index)
        {
            if (index < 0 || index >= _tabs.Count)
                return;

            selectedTab.SetInactive();
            _selectedTabIndex = index;
            selectedTab.SetActive();
        }

        public void DeselectTab(int index)
        {
            if (index < 0 || index >= _tabs.Count)
                return;

            selectedTab.SetInactive();
            _selectedTabIndex = 0;
            selectedTab.SetActive();
        }
    }
}
