using Maroon.NetworkSimulator.NetworkDevices;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.NetworkSimulator {
    public class UIController : MonoBehaviour {
        [SerializeField]
        private GameObject generalOptionsPanel;
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
        [SerializeField]
        private GameObject ipAddressRow;
        [SerializeField]
        private TextMeshProUGUI ipAddressText;
        [SerializeField]
        private GameObject macAddressRow;
        [SerializeField]
        private TextMeshProUGUI macAddressText;
        [SerializeField]
        private GameObject[] macAddressListRow;
        [SerializeField]
        private TextMeshProUGUI[] macAddressListText;

        void Start() {
            HideDeviceOptions();
        }

        void Update() {

        }

        public void ShowDeviceOptions(NetworkDevice clickedDevice) {
            deviceOptionsTitle.SetText(clickedDevice.GetName());
            deviceOptionsButtonText.SetText(clickedDevice.GetButtonText());
            if(clickedDevice is Computer computer) {
                ipAddressText.SetText(computer.IPAddress);
                macAddressText.SetText(computer.MACAddress);
                ipAddressRow.SetActive(true);
                macAddressRow.SetActive(true);
                Array.ForEach(macAddressListRow, r => r.SetActive(false));
            }
            else if(clickedDevice is Router router) {
                ipAddressText.SetText(router.IPAddress);
                macAddressText.SetText("");
                for(int i = 0; i < router.MACAddress.Length; i++) {
                    macAddressListText[i].SetText(router.MACAddress[i]);
                }
                ipAddressRow.SetActive(true);
                macAddressRow.SetActive(true);
                Array.ForEach(macAddressListRow, r => r.SetActive(true));
            }
            else {
                ipAddressRow.SetActive(false);
                macAddressRow.SetActive(false);
                Array.ForEach(macAddressListRow, r => r.SetActive(false));
            }
            deviceOptionsPanel.SetActive(true);
        }
        public void HideDeviceOptions() {
            deviceOptionsPanel.SetActive(false);
        }

        public void SetNetworkView() {
            generalOptionsPanel.SetActive(true);
            enterDeviceButton.gameObject.SetActive(true);
            backToNetworkButton.gameObject.SetActive(false);
            removeDeviceButton.gameObject.SetActive(true);
        }
        public void SetInsideDeviceView() {
            generalOptionsPanel.SetActive(false);
            enterDeviceButton.gameObject.SetActive(false);
            backToNetworkButton.gameObject.SetActive(true);
            removeDeviceButton.gameObject.SetActive(false);
        }
    }
}