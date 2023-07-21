using UnityEngine;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Computer : NetworkDevice {
        private int TrafficInterval = 5;
        private int TrafficIntervalRange = 2;
        private int NextTrafficTimeout => TrafficInterval + Random.Range(-TrafficIntervalRange, TrafficIntervalRange + 1);
        public override string GetName() => "Computer";
        public override string GetButtonText() => "Computer action";
        public override DeviceType GetDeviceType() => DeviceType.Computer;

        public override void ReceivePacket(Packet packet, Port receiver) {
        }

        protected override void OnStart() {
            Invoke(nameof(GenerateTraffic), NextTrafficTimeout);
        }

        private void GenerateTraffic() {
            Ports[0].SendPacket(new Packet());
            Invoke(nameof(GenerateTraffic), NextTrafficTimeout);
        }
    }
}
