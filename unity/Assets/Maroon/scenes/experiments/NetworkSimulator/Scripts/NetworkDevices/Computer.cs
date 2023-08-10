using System;
using System.Collections.Generic;
using System.Linq;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Computer : NetworkDevice {
        public IPAddress IPAddress;
        public MACAddress MACAddress;
        private readonly Dictionary<IPAddress, AddressTableEntry<MACAddress>> arpTable = new Dictionary<IPAddress, AddressTableEntry<MACAddress>>();
        private readonly Dictionary<IPAddress, AddressTableEntry<IPAddress>> routingTable = new Dictionary<IPAddress, AddressTableEntry<IPAddress>>();
        public override string GetName() => "Computer";
        public override string GetButtonText() => "Computer action";
        public override DeviceType GetDeviceType() => DeviceType.Computer;

        public override void ReceivePacket(Packet packet, Port receiver) {
        }

        public void SendPacket(IPAddress destinationIPAddress) {
            MACAddress destination;
            if(routingTable.ContainsKey(destinationIPAddress)) {
                destination = arpTable[routingTable[destinationIPAddress].Value].Value;
            }
            else {
                destination = arpTable[destinationIPAddress].Value;
            }
            Ports[0].SendPacket(new Packet(IPAddress, destinationIPAddress, MACAddress, destination));
        }

        protected override void OnAddedToNetwork() {
            IPAddress = NetworkSimulationController.Instance.GetIPAddress();
            MACAddress = NetworkSimulationController.Instance.GetMACAddress(GetDeviceType());
        }

        public override void ClearAddressTables() {
            arpTable.Clear();
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
    }
}
