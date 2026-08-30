using GEAR.Localization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Computer : NetworkDevice {
        [SerializeField]
        private ComputerUI ui;
        [SerializeField]
        private GameObject screen;
        [SerializeField]
        private GameObject chassis;

        public IPAddress IPAddress;
        public MACAddress MACAddress;
        private readonly Dictionary<IPAddress, AddressTableEntry<MACAddress>> arpTable = new Dictionary<IPAddress, AddressTableEntry<MACAddress>>();
        private readonly Dictionary<IPAddress, AddressTableEntry<IPAddress>> routingTable = new Dictionary<IPAddress, AddressTableEntry<IPAddress>>();
        private readonly Queue<Packet> sendQueue = new Queue<Packet>();
        private bool isProcessingSendQueue = false;
        public override string GetName() => "Computer";
        public override string GetButtonText() => LanguageManager.Instance.GetString("ComputerButton");
        public override DeviceType GetDeviceType() => DeviceType.Computer;

        void Update() {
            if(!SimulationController.Instance.SimulationRunning) {
                return;
            }
            if(!isProcessingSendQueue && sendQueue.Any()) {
                isProcessingSendQueue = true;
                StartCoroutine(ProcessSendQueue());
            }
        }

        public void Hide() {
            screen.SetActive(false);
            chassis.SetActive(false);
        }
        public void Show() {
            screen.SetActive(true);
            chassis.SetActive(true);
        }

        public override void ReceivePacket(Packet packet, Port receiver) {
            if(packet.DestinationIPAddress == IPAddress && packet.HasMessage) {
                ui.ReceiveMessage(packet.SourceIPAddress, packet.Message);
            }
        }
        protected override void ProcessPacket(Packet packet, Port receiver) {
        }
        public override int GetDestinationPortIndex(Packet packet) {
            return 0;
        }

        public void SendPacket(IPAddress destinationIPAddress, string message = null) {
            MACAddress destination;
            if(routingTable.ContainsKey(destinationIPAddress)) {
                destination = arpTable[routingTable[destinationIPAddress].Value].Value;
            }
            else {
                destination = arpTable[destinationIPAddress].Value;
            }
            sendQueue.Enqueue(new Packet(IPAddress, destinationIPAddress, MACAddress, destination, message));
        }

        private IEnumerator ProcessSendQueue() {
            while(sendQueue.Any()) {
                var packet = sendQueue.Dequeue();
                Ports[0].SendPacket(packet);
                yield return new WaitForSeconds(0.25f);
            }
            isProcessingSendQueue = false;
        }

        protected override void OnAddedToNetwork() {
            IPAddress = NetworkSimulationController.Instance.GetIPAddress();
            MACAddress = NetworkSimulationController.Instance.GetMACAddress(GetDeviceType());
            ui.gameObject.SetActive(true);
            ui.Initialize(this);
        }

        public override void ClearAddressTables() {
            arpTable.Clear();
            routingTable.Clear();
        }

        public override void AddToAddressTables(IPAddress ipAddress, MACAddress macAddress, IPAddress via, Port receiver, int distance, Computer initiator) {
            if(ipAddress == via) {
                if(!arpTable.ContainsKey(ipAddress) || arpTable[ipAddress].Distance > distance) {
                    arpTable[ipAddress] = new AddressTableEntry<MACAddress>(macAddress, distance);
                }
            }
            else {
                if(!arpTable.ContainsKey(via) || arpTable[via].Distance > distance) {
                    arpTable[via] = new AddressTableEntry<MACAddress>(macAddress, distance);
                }
                if(!routingTable.ContainsKey(ipAddress) || routingTable[ipAddress].Distance > distance) {
                    routingTable[ipAddress] = new AddressTableEntry<IPAddress>(via, distance);
                }
            }
        }
        public void StartAddingAddressToTables() {
            if(Ports[0].IsFree) {
                return;
            }
            Ports[0].ConnectedDevice.AddToAddressTables(IPAddress, MACAddress, IPAddress, Ports[0].Cable.OtherPort(Ports[0]), 0, this);
        }
        public IEnumerable<(string, string)> GetARPTable() {
            return arpTable.Select(x => (x.Key.ToString(), x.Value.Value.ToString()));
        }
        public IEnumerable<(string, string)> GetRoutingTable() {
            return routingTable.Select(x => (x.Key.ToString(), x.Value.Value.ToString()));
        }

        public void ActivateUI() {
            ui.Activate();
        }
        public void DeactivateUI() {
            ui.Deactivate();
        }
        public IEnumerable<string> GetRecipients() {
            var ipAddresses = arpTable.Keys.Concat(routingTable.Keys);
            var routerAddresses = routingTable.Values.Select(v => v.Value);
            var recipients = ipAddresses.Except(routerAddresses);
            return recipients.Select(r => r.ToString());
        }
    }
}
