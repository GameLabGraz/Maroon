using GEAR.Localization;
using System.Linq;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Hub : NetworkDevice {
        public override string GetName() => "Hub";
        public override string GetButtonText() => LanguageManager.Instance.GetString("HubButton");
        public override DeviceType GetDeviceType() => DeviceType.Hub;

        protected override void ProcessPacket(Packet packet, Port receiver) {
            if(IsInside) {
                ReceivePacketInside(packet, receiver);
                return;
            }
            foreach(var port in Ports) {
                if(port != receiver) {
                    port.SendPacket(packet);
                }
            }
        }
        public override int GetDestinationPortIndex(Packet packet) {
            return 0;
        }

        protected override void OnAddedToNetwork() {
        }

        public override void ClearAddressTables() {
        }
        public override void AddToAddressTables(IPAddress ipAddress, MACAddress macAddress, IPAddress via, Port receiver, int distance, Computer initiator) {
            if(addAddressInitiator == initiator) {
                return;
            }
            addAddressInitiator = initiator;
            foreach(var port in Ports.Where(p => !p.IsFree)) {
                if(port != receiver) {
                    port.ConnectedDevice.AddToAddressTables(ipAddress, macAddress, via, port.Cable.OtherPort(port), distance + 1, initiator);
                }
            }
            addAddressInitiator = null;
        }
    }
}
