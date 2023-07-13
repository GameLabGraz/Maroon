using TMPro;
using UnityEngine.UI;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Hub : NetworkDevice {
        public override string GetName() => "Hub";
        public override string GetButtonText() => "Enter Hub";

        public override void ReceivePacket(Packet packet, Port receiver) {
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

        protected override void OnStart() {
        }
    }
}
