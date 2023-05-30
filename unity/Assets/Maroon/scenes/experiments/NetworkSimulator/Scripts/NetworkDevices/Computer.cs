using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Computer : NetworkDevice {
        public override string GetName() => "Computer";
        public override string GetButtonText() => "Computer action";
        public override void DeviceOptionsButtonClicked() {
            networkSimulationController.ShowComputerScreen(this);
        }
    }
}
