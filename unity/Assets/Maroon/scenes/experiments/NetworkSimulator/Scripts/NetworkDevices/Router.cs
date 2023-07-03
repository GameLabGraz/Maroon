using TMPro;
using UnityEngine.UI;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Router : NetworkDevice {
        public override string GetName() => "Router";
        public override string GetButtonText() => "Enter Router";

        public override void ReceivePacket(Packet packet, Port receiver) {
        }

        protected override void OnStart() {
        }
    }
}
