namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Router : NetworkDevice {
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
    }
}
