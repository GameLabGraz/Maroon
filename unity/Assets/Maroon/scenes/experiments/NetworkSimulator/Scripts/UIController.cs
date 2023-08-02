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

        [SerializeField]
        private Toggle macAddressTableToggle;
        [SerializeField]
        private GameObject macAddressTable;
        [SerializeField]
        private TextMeshProUGUI macAddressTableBody;
        [SerializeField]
        private Toggle arpTableToggle;
        [SerializeField]
        private GameObject arpTable;
        [SerializeField]
        private TextMeshProUGUI arpTableBody;
        [SerializeField]
        private Toggle routingTableToggle;
        [SerializeField]
        private GameObject routingTable;
        [SerializeField]
        private TextMeshProUGUI routingTableBody;

        void Start() {
            HideDeviceOptions();
        }

        void Update() {

        }

        public void ShowDeviceOptions(NetworkDevice clickedDevice) {
            deviceOptionsTitle.SetText(clickedDevice.GetName());
            deviceOptionsButtonText.SetText(clickedDevice.GetButtonText());

            ipAddressRow.SetActive(false);
            macAddressRow.SetActive(false);
            Array.ForEach(macAddressListRow, r => r.SetActive(false));

            macAddressTableToggle.gameObject.SetActive(false);
            arpTableToggle.gameObject.SetActive(false);
            routingTableToggle.gameObject.SetActive(false);
            macAddressTable.SetActive(false);
            arpTable.SetActive(false);
            routingTable.SetActive(false);

            if(clickedDevice is Computer computer) {
                ipAddressText.SetText(computer.IPAddress.ToString());
                macAddressText.SetText(computer.MACAddress.ToString());
                ipAddressRow.SetActive(true);
                macAddressRow.SetActive(true);

                arpTableBody.SetText(computer.GetARPTable());
                arpTableToggle.gameObject.SetActive(true);
                arpTable.SetActive(arpTableToggle.isOn);
            }
            else if(clickedDevice is Router router) {
                ipAddressText.SetText(router.IPAddress.ToString());
                macAddressText.SetText(string.Empty);
                for(int i = 0; i < router.MACAddress.Length; i++) {
                    macAddressListText[i].SetText(router.MACAddress[i].ToString());
                }
                ipAddressRow.SetActive(true);
                macAddressRow.SetActive(true);
                Array.ForEach(macAddressListRow, r => r.SetActive(true));

                macAddressTableBody.SetText(router.GetMACAddressTable());
                arpTableBody.SetText(router.GetARPTable());
                routingTableBody.SetText(router.GetRoutingTable());
                macAddressTableToggle.gameObject.SetActive(true);
                arpTableToggle.gameObject.SetActive(true);
                routingTableToggle.gameObject.SetActive(true);
                macAddressTable.SetActive(macAddressTableToggle.isOn);
                arpTable.SetActive(arpTableToggle.isOn);
                routingTable.SetActive(routingTableToggle.isOn);
            }
            else if(clickedDevice is Switch sw) {
                macAddressTableBody.SetText(sw.GetMACAddressTable());
                macAddressTableToggle.gameObject.SetActive(true);
                macAddressTable.SetActive(macAddressTableToggle.isOn);
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