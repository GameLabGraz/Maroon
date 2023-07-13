using TMPro;
using UnityEngine.UI;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Switch : NetworkDevice {
        public override string GetName() => "Switch";
        public override string GetButtonText() => "Enter Switch";

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
