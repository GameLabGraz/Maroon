using TMPro;
using UnityEngine.UI;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Router : NetworkDevice {
        public override string GetName() => "Router";
        public override string GetButtonText() => "Enter Router";
        public override void DeviceOptionsButtonClicked(Button button, TextMeshProUGUI buttonText) {
            networkSimulationController.EnterInsideOfDevice(this);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(networkSimulationController.ExitInsideOfDevice);
            buttonText.SetText("Back to network");
        }
        public override void ReceivePacket(Packet packet, Port receiver) {
        }
    }
}
