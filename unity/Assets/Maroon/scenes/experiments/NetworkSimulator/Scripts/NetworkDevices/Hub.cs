using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Maroon.NetworkSimulator.NetworkDevices {
    public class Hub : NetworkDevice {
        public override string GetName() => "Hub";
        public override string GetButtonText() => "Enter Hub";
        public override void DeviceOptionsButtonClicked() {
            networkSimulationController.EnterInsideOfDevice(this);
        }
    }
}
