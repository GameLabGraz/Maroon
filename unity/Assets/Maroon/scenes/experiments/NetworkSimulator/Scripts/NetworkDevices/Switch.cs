using TMPro;
using UnityEngine.UI;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Switch : NetworkDevice {
        public override string GetName() => "Switch";
        public override string GetButtonText() => "Enter Switch";
        public override void DeviceOptionsButtonClicked(Button button, TextMeshProUGUI buttonText) {
            networkSimulationController.EnterInsideOfDevice(this);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(networkSimulationController.ExitInsideOfDevice);
            buttonText.SetText("Back to network");
        }

        public override void ReceivePacket(Packet packet, Port receiver) {
        }

        protected override void OnStart() {
        }
    }
}
