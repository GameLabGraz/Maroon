using TMPro;
using UnityEngine.UI;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Computer : NetworkDevice {
        public override string GetName() => "Computer";
        public override string GetButtonText() => "Computer action";
        public override void DeviceOptionsButtonClicked(Button button, TextMeshProUGUI buttonText) {
            networkSimulationController.ShowComputerScreen(this);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(networkSimulationController.CloseComputerScreen);
            buttonText.SetText("Back to network");
        }
        public override void ReceivePacket(Packet packet, Port receiver) {
        }
    }
}
