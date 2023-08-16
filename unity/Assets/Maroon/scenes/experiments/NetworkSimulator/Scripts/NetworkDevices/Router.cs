using System;
using System.Collections.Generic;
using System.Linq;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Router : NetworkDevice {
        public IPAddress IPAddress;
        public MACAddress[] MACAddress;
        private readonly Dictionary<MACAddress, AddressTableEntry<int>> macAddressTable = new Dictionary<MACAddress, AddressTableEntry<int>>();
        private readonly Dictionary<IPAddress, AddressTableEntry<MACAddress>> arpTable = new Dictionary<IPAddress, AddressTableEntry<MACAddress>>();
        private readonly Dictionary<IPAddress, AddressTableEntry<IPAddress>> routingTable = new Dictionary<IPAddress, AddressTableEntry<IPAddress>>();
        public override string GetName() => "Router";
        public override string GetButtonText() => "Enter Router";
        public override DeviceType GetDeviceType() => DeviceType.Router;

        protected override void ProcessPacket(Packet packet, Port receiver) {
            var portIndex = GetDestinationPortIndex(packet);
            var port = Ports[portIndex];
            packet.DestinationMACAddress = GetDestinationMACAddress(packet);
            packet.SourceMACAddress = MACAddress[portIndex];
            if(port != receiver) {
                if(IsInside) {
                    ReceivePacketInside(packet, receiver);
                }
                else {
                    port.SendPacket(packet);
                }
            }
        }
        private MACAddress GetDestinationMACAddress(Packet packet) {
            if(routingTable.ContainsKey(packet.DestinationIPAddress)) {
                return arpTable[routingTable[packet.DestinationIPAddress].Value].Value;
            }
            else {
                return arpTable[packet.DestinationIPAddress].Value;
            }
        }
        public override int GetDestinationPortIndex(Packet packet) {
            return macAddressTable[GetDestinationMACAddress(packet)].Value;
        }

        protected override void OnAddedToNetwork() {
            IPAddress = NetworkSimulationController.Instance.GetIPAddress();
            var macAddress = NetworkSimulationController.Instance.GetMACAddress(GetDeviceType());
            MACAddress = new MACAddress[] {
                new MACAddress(macAddress + ":01"),
                new MACAddress(macAddress + ":02"),
                new MACAddress(macAddress + ":03"),
                new MACAddress(macAddress + ":04")
            };
        }

        public override void ClearAddressTables() {
            arpTable.Clear();
            routingTable.Clear();
        }
        public override void AddToAddressTables(IPAddress ipAddress, MACAddress macAddress, IPAddress via, Port receiver, int distance, Computer initiator) {
            if(addAddressInitiator == initiator) {
                return;
            }
            addAddressInitiator = initiator;

            if(!macAddressTable.ContainsKey(macAddress) || macAddressTable[macAddress].Distance > distance) {
                macAddressTable[macAddress] = new AddressTableEntry<int>(Array.IndexOf(Ports, receiver), distance);
            }
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

            foreach(var port in Ports.Where(p => !p.IsFree)) {
                if(port != receiver) {
                    port.ConnectedDevice.AddToAddressTables(ipAddress, MACAddress[Array.IndexOf(Ports, port)], IPAddress, port.Cable.OtherPort(port), distance + 1, initiator);
                }
            }
            addAddressInitiator = null;
        }
        public IEnumerable<(string, string)> GetMACAddressTable() {
            return macAddressTable.Select(x => (x.Key.ToString(), $"Port{x.Value.Value}"));
        }
        public IEnumerable<(string, string)> GetARPTable() {
            return arpTable.Select(x => (x.Key.ToString(), x.Value.Value.ToString()));
        }
        public IEnumerable<(string, string)> GetRoutingTable() {
            return routingTable.Select(x => (x.Key.ToString(), x.Value.Value.ToString()));
        }
    }
}
