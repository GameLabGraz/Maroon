using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.NetworkSimulator {
    public class UIController : MonoBehaviour {
        [SerializeField] 
        private GameObject deviceOptionsPanel;
        [SerializeField]
        private TextMeshProUGUI deviceOptionsTitle;
        [SerializeField]
        private Button enterDeviceButton;
        [SerializeField]
        private Button backToNetworkButton;
        [SerializeField]
        private Button removeDeviceButton;
        [SerializeField]
        private TextMeshProUGUI deviceOptionsButtonText;

        void Start() {
            HideDeviceOptions();
        }

        void Update() {

        }

        public void ShowDeviceOptions(NetworkDevice clickedDevice) {
            deviceOptionsTitle.SetText(clickedDevice.GetName());
            deviceOptionsButtonText.SetText(clickedDevice.GetButtonText());
            deviceOptionsPanel.SetActive(true);
        }
        public void HideDeviceOptions() {
            deviceOptionsPanel.SetActive(false);
        }

        public void SetNetworkView() {
            enterDeviceButton.gameObject.SetActive(true);
            backToNetworkButton.gameObject.SetActive(false);
            removeDeviceButton.gameObject.SetActive(true);
        }
        public void SetInsideDeviceView() {
            enterDeviceButton.gameObject.SetActive(false);
            backToNetworkButton.gameObject.SetActive(true);
            removeDeviceButton.gameObject.SetActive(false);
        }
    }
}