using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Switch : NetworkDevice {
        public override string GetName() => "Switch";
        public override string GetButtonText() => "Enter Switch";
        public override void DeviceOptionsButtonClicked() {
            networkSimulationController.EnterInsideOfDevice(this);
        }
    }
}
