using Maroon.NetworkSimulator.NetworkDevices;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class NetworkSimulationController : MonoBehaviour {
        [SerializeField]
        private CameraScript cameraScript;
        [SerializeField]
        private UIController uiController;
        private List<NetworkDevice> networkDevices = new List<NetworkDevice>();
        private NetworkDevice selectedDevice = null;
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
        public void SelectDevice(NetworkDevice device) {
            selectedDevice = device;
            uiController.ShowDeviceOptions(selectedDevice);
        }
        public void EnterInsideOfDevice() {
            uiController.SetInsideDeviceView();
            if(selectedDevice is Computer) {
                cameraScript.SetComputerView(selectedDevice.transform.position);
            }
            else {
                cameraScript.SetInsideDeviceView();
            }
        }
        public void ExitInsideOfDevice() {
            uiController.SetNetworkView();
            cameraScript.SetNetworkView();
        }
        public void RemoveDevice() {
            networkDevices.Remove(selectedDevice);
            selectedDevice.RemoveCables();
            Destroy(selectedDevice.gameObject);
            selectedDevice = null;
            uiController.HideDeviceOptions();
        }
    }
}
