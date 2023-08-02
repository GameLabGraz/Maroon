using System;
using System.Collections.Generic;
using System.Linq;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Computer : NetworkDevice {
        public IPAddress IPAddress;
        public MACAddress MACAddress;
        private readonly Dictionary<IPAddress, AddressTableEntry<MACAddress>> arpTable = new Dictionary<IPAddress, AddressTableEntry<MACAddress>>();
        public override string GetName() => "Computer";
        public override string GetButtonText() => "Computer action";
        public override DeviceType GetDeviceType() => DeviceType.Computer;

        public override void ReceivePacket(Packet packet, Port receiver) {
        }

        public void SendPacket(IPAddress destinationIPAddress) {
            Ports[0].SendPacket(new Packet(IPAddress, destinationIPAddress, MACAddress, arpTable[destinationIPAddress].Value));
        }

        protected override void OnAddedToNetwork() {
            IPAddress = NetworkSimulationController.Instance.GetIPAddress();
            MACAddress = NetworkSimulationController.Instance.GetMACAddress(GetDeviceType());
        }

        public override void ClearAddressTables() {
            arpTable.Clear();
        }

        public override void AddToAddressTables(IPAddress ipAddress, MACAddress macAddress, IPAddress via, Port receiver, int distance, Computer initiator) {
            if(!arpTable.ContainsKey(ipAddress) || arpTable[ipAddress].Distance > distance) {
                arpTable[ipAddress] = new AddressTableEntry<MACAddress>(macAddress, distance);
            }
        }
        public void StartAddingAddressToTables() {
            if(Ports[0].IsFree) {
                return;
            }
            Ports[0].ConnectedDevice.AddToAddressTables(IPAddress, MACAddress, IPAddress, Ports[0].Cable.OtherPort(Ports[0]), 0, this);
        }
        public string GetARPTable() {
            return string.Join(Environment.NewLine, arpTable.Select(x => $"{x.Key,-15}  {x.Value.Value}"));
        }
    }
}
