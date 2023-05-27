using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class NetworkSimulationController : MonoBehaviour {
        private List<NetworkDevice> networkDevices = new List<NetworkDevice>();
        public void AddNetworkDevice(NetworkDevice device) {
            networkDevices.Add(device);
        }
        public void ShowConnectableDeviceMarkers() {
            foreach(var device in networkDevices) {
                device.ShowConnectableMarker();
            }
        }
        public void HideConnectableDeviceMarkers() {
            foreach(var device in networkDevices) {
                device.HideConnectableMarker();
            }
        }
    }
}
