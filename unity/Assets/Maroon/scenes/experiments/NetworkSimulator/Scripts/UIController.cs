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
        private Button deviceOptionsButton;
        [SerializeField]
        private TextMeshProUGUI deviceOptionsButtonText;

        void Start() {
            HideDeviceOptions();
        }

        void Update() {

        }

        public void ShowDeviceOptions(NetworkDevice clickedDevice) {
            deviceOptionsTitle.SetText(clickedDevice.GetName());
            deviceOptionsButton.onClick.RemoveAllListeners();
            deviceOptionsButton.onClick.AddListener(() => clickedDevice.DeviceOptionsButtonClicked(deviceOptionsButton, deviceOptionsButtonText));
            deviceOptionsButtonText.SetText(clickedDevice.GetButtonText());
            deviceOptionsPanel.SetActive(true);
        }
        public void HideDeviceOptions() {
            deviceOptionsPanel.SetActive(false);
        }
    }
}