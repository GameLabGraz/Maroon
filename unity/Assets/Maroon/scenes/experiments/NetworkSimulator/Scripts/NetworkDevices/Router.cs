using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Router : NetworkDevice {
        public override string GetName() => "Router";
        public override string GetButtonText() => "Enter Router";
        public override void DeviceOptionsButtonClicked() {
            networkSimulationController.EnterInsideOfDevice(this);
        }
    }
}
