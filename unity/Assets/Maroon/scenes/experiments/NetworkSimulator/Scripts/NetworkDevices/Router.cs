using System;
using System.Collections.Generic;
using System.Linq;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Router : NetworkDevice {
        public IPAddress IPAddress;
        public MACAddress[] MACAddress;
        private readonly Dictionary<MACAddress, AddressTableEntry<Port>> macAddressTable = new Dictionary<MACAddress, AddressTableEntry<Port>>();
        private readonly Dictionary<IPAddress, AddressTableEntry<MACAddress>> arpTable = new Dictionary<IPAddress, AddressTableEntry<MACAddress>>();
        private readonly Dictionary<IPAddress, AddressTableEntry<IPAddress>> routingTable = new Dictionary<IPAddress, AddressTableEntry<IPAddress>>();
        public override string GetName() => "Router";
        public override string GetButtonText() => "Enter Router";
        public override DeviceType GetDeviceType() => DeviceType.Router;

        public override void ReceivePacket(Packet packet, Port receiver) {
            if(IsInside) {
                ReceivePacketInside(packet, receiver);
                return;
            }
            MACAddress destination;
            if(routingTable.ContainsKey(packet.DestinationIPAddress)) {
                destination = arpTable[routingTable[packet.DestinationIPAddress].Value].Value;
            }
            else {
                destination = arpTable[packet.DestinationIPAddress].Value;
            }
            packet.DestinationMACAddress = destination;
            var port = macAddressTable[packet.DestinationMACAddress].Value;
            packet.SourceMACAddress = MACAddress[Array.IndexOf(Ports, port)];
            port.SendPacket(packet);
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
                macAddressTable[macAddress] = new AddressTableEntry<Port>(receiver, distance);
            }
            if(!arpTable.ContainsKey(ipAddress) || arpTable[ipAddress].Distance > distance) {
                arpTable[ipAddress] = new AddressTableEntry<MACAddress>(macAddress, distance);
            }
            if(!arpTable.ContainsKey(via) || arpTable[via].Distance > distance) {
                arpTable[via] = new AddressTableEntry<MACAddress>(macAddress, distance);
            }
            if(ipAddress != via && (!routingTable.ContainsKey(ipAddress) || routingTable[ipAddress].Distance > distance)) {
                routingTable[ipAddress] = new AddressTableEntry<IPAddress>(via, distance);
            }
            foreach(var port in Ports.Where(p => !p.IsFree)) {
                if(port != receiver) {
                    port.ConnectedDevice.AddToAddressTables(ipAddress, MACAddress[Array.IndexOf(Ports, port)], IPAddress, port.Cable.OtherPort(port), distance + 1, initiator);
                }
            }
            addAddressInitiator = null;
        }
        public string GetMACAddressTable() {
            return string.Join(Environment.NewLine, macAddressTable.Select(x => $"{x.Key,-17}  Port{Array.IndexOf(Ports, x.Value.Value)}"));
        }
        public string GetARPTable() {
            return string.Join(Environment.NewLine, arpTable.Select(x => $"{x.Key,-15}  {x.Value.Value}"));
        }
        public string GetRoutingTable() {
            return string.Join(Environment.NewLine, routingTable.Select(x => $"{x.Key,-15}  {x.Value.Value}"));
        }
    }
}
