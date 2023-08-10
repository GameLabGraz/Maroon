using System;
using System.Collections.Generic;
using System.Linq;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Switch : NetworkDevice {
        private readonly Dictionary<MACAddress, AddressTableEntry<Port>> macAddressTable = new Dictionary<MACAddress, AddressTableEntry<Port>>();
        public override string GetName() => "Switch";
        public override string GetButtonText() => "Enter Switch";
        public override DeviceType GetDeviceType() => DeviceType.Switch;

        public override void ReceivePacket(Packet packet, Port receiver) {
            var port = macAddressTable[packet.DestinationMACAddress].Value;
            if(port != receiver) {
                if(IsInside) {
                    ReceivePacketInside(packet, receiver);
                }
                else {
                    port.SendPacket(packet);
                }
            }
        }

        protected override void OnAddedToNetwork() {
        }

        public override void ClearAddressTables() {
            macAddressTable.Clear();
        }
        public override void AddToAddressTables(IPAddress ipAddress, MACAddress macAddress, IPAddress via, Port receiver, int distance, Computer initiator) {
            if(addAddressInitiator == initiator) {
                return;
            }
            addAddressInitiator = initiator;

            if(!macAddressTable.ContainsKey(macAddress) || macAddressTable[macAddress].Distance > distance) {
                macAddressTable[macAddress] = new AddressTableEntry<Port>(receiver, distance);
            }

            foreach(var port in Ports.Where(p => !p.IsFree)) {
                if(port != receiver) {
                    port.ConnectedDevice.AddToAddressTables(ipAddress, macAddress, via, port.Cable.OtherPort(port), distance + 1, initiator);
                }
            }
            addAddressInitiator = null;
        }
        public IEnumerable<(string, string)> GetMACAddressTable() {
            return macAddressTable.Select(x => (x.Key.ToString(), $"Port{Array.IndexOf(Ports, x.Value.Value)}"));
        }
    }
}
