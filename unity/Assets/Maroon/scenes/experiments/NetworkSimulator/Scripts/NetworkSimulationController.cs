using Maroon.NetworkSimulator.NetworkDevices;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class NetworkSimulationController : MonoBehaviour {
        [SerializeField]
        private CameraScript cameraScript;
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
        public void EnterInsideOfDevice(NetworkDevice networkDevice) {
            cameraScript.SetInsideDeviceView();
        }
        public void ShowComputerScreen(Computer computer) {
            cameraScript.SetComputerView(computer.transform.position);
        }
    }
}
