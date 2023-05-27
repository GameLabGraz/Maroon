using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class UIController : MonoBehaviour {
        [SerializeField] 
        private GameObject deviceOptionsPanel;

        void Start() {
            HideDeviceOptions();
        }

        void Update() {

        }

        public void ShowDeviceOptions() {
            deviceOptionsPanel.SetActive(true);
        }
        public void HideDeviceOptions() {
            deviceOptionsPanel.SetActive(false);
        }
    }
}