namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Router : NetworkDevice {
        public string IPAddress;
        public string[] MACAddress;
        public override string GetName() => "Router";
        public override string GetButtonText() => "Enter Router";
        public override DeviceType GetDeviceType() => DeviceType.Router;

        public override void ReceivePacket(Packet packet, Port receiver) {
            if(IsInside) {
                ReceivePacketInside(packet, receiver);
                return;
            }
        }

        protected override void OnStart() {
        }
        protected override void OnAddedToNetwork() {
            IPAddress = networkSimulationController.GetIPAddress();
            var macAddress = networkSimulationController.GetMACAddress(GetDeviceType());
            MACAddress = new string[] {
                macAddress + ":01",
                macAddress + ":02",
                macAddress + ":03",
                macAddress + ":04"
            };
        }
    }
}
