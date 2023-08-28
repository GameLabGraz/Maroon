using Maroon.NetworkSimulator.NetworkDevices;
using Maroon.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Maroon.NetworkSimulator {
    public class UIController : MonoBehaviour {
        private static UIController instance;
        public static UIController Instance {
            get {
                if(instance == null) {
                    instance = FindObjectOfType<UIController>();
                }
                return instance;
            }
        }
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
        private AddressTableScript macAddressTable;
        [SerializeField]
        private Toggle arpTableToggle;
        [SerializeField]
        private AddressTableScript arpTable;
        [SerializeField]
        private Toggle routingTableToggle;
        [SerializeField]
        private AddressTableScript routingTable;

        [SerializeField]
        private GameObject packetInfoPanel;
        [SerializeField]
        private TextMeshProUGUI packetSourceMACAddress;
        [SerializeField]
        private TextMeshProUGUI packetDestinationMACAddress;
        [SerializeField]
        private TextMeshProUGUI packetSourceIPAddress;
        [SerializeField]
        private TextMeshProUGUI packetDestinationIPAddress;

        public DialogueManager DialogueManager { get; private set; }

        void Start() {
            HideDeviceOptions();
            HidePacketInfo();
            DialogueManager = FindObjectOfType<DialogueManager>();
        }

        public void ShowDeviceOptions(NetworkDevice clickedDevice) {
            HidePacketInfo();
            deviceOptionsTitle.SetText(clickedDevice.GetName());
            deviceOptionsButtonText.SetText(clickedDevice.GetButtonText());

            ipAddressRow.SetActive(false);
            macAddressRow.SetActive(false);
            Array.ForEach(macAddressListRow, r => r.SetActive(false));

            macAddressTableToggle.gameObject.SetActive(false);
            arpTableToggle.gameObject.SetActive(false);
            routingTableToggle.gameObject.SetActive(false);
            macAddressTable.gameObject.SetActive(false);
            arpTable.gameObject.SetActive(false);
            routingTable.gameObject.SetActive(false);

            if(clickedDevice is Computer computer) {
                ipAddressText.SetText(computer.IPAddress.ToString());
                macAddressText.SetText(computer.MACAddress.ToString());
                ipAddressRow.SetActive(true);
                macAddressRow.SetActive(true);

                macAddressTable.Clear();
                arpTable.SetRows(computer.GetARPTable());
                routingTable.SetRows(computer.GetRoutingTable());
                arpTableToggle.gameObject.SetActive(true);
                routingTableToggle.gameObject.SetActive(true);
                arpTable.gameObject.SetActive(arpTableToggle.isOn);
                routingTable.gameObject.SetActive(routingTableToggle.isOn);
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

                macAddressTable.SetRows(router.GetMACAddressTable());
                arpTable.SetRows(router.GetARPTable());
                routingTable.SetRows(router.GetRoutingTable());
                macAddressTableToggle.gameObject.SetActive(true);
                arpTableToggle.gameObject.SetActive(true);
                routingTableToggle.gameObject.SetActive(true);
                macAddressTable.gameObject.SetActive(macAddressTableToggle.isOn);
                arpTable.gameObject.SetActive(arpTableToggle.isOn);
                routingTable.gameObject.SetActive(routingTableToggle.isOn);
            }
            else if(clickedDevice is Switch sw) {
                macAddressTable.SetRows(sw.GetMACAddressTable());
                macAddressTableToggle.gameObject.SetActive(true);
                macAddressTable.gameObject.SetActive(macAddressTableToggle.isOn);
                arpTable.Clear();
                routingTable.Clear();
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

        public void ShowPacketInfo(Packet packet) {
            HideDeviceOptions();
            packetSourceMACAddress.SetText(packet.SourceMACAddress.ToString());
            packetDestinationMACAddress.SetText(packet.DestinationMACAddress.ToString());
            packetSourceIPAddress.SetText(packet.SourceIPAddress.ToString());
            packetDestinationIPAddress.SetText(packet.DestinationIPAddress.ToString());
            packetInfoPanel.SetActive(true);
        }
        public void HidePacketInfo() {
            packetInfoPanel.SetActive(false);
        }

        public void HighlightPacketAddresses(Packet packet) {
            macAddressTable.HighlightRow(packet.DestinationMACAddress.ToString());
            var row = routingTable.GetRow(packet.DestinationIPAddress.ToString());
            if(row != null) {
                row.SetFontStyleBold();
                arpTable.HighlightRow(row.Text2);
            }
            else {
                arpTable.HighlightRow(packet.DestinationIPAddress.ToString());
            }
        }
        public void HideHighlightedPacketAddresses() {
            macAddressTable.HideHighlight();
            arpTable.HideHighlight();
            routingTable.HideHighlight();
        }
    }
}