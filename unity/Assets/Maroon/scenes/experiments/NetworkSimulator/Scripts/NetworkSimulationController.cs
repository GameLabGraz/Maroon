using Maroon.NetworkSimulator.NetworkDevices;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class NetworkSimulationController : MonoBehaviour {
        [SerializeField]
        private CameraScript cameraScript;
        [SerializeField]
        private UIController uiController;
        [SerializeField]
        public InsideDeviceScript InsideDeviceScript;
        [SerializeField]
        private NetworkDevice[] devicePrefabs;
        [SerializeField]
        private BoxCollider networkArea;
        [SerializeField]
        private AddCableScript addCableScript;

        private readonly List<NetworkDevice> networkDevices = new List<NetworkDevice>();
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
                selectedDevice.IsInside = true;
                InsideDeviceScript.SetDevice(selectedDevice);
            }
        }
        public void ExitInsideOfDevice() {
            uiController.SetNetworkView();
            cameraScript.SetNetworkView();
            selectedDevice.IsInside = false;
            InsideDeviceScript.Clear();
        }
        public void RemoveDevice() {
            networkDevices.Remove(selectedDevice);
            selectedDevice.RemoveCables();
            Destroy(selectedDevice.gameObject);
            selectedDevice = null;
            uiController.HideDeviceOptions();
        }

        public void LoadPreset(int index) {
            if(index == 0) {
                return;
            }
            ClearNetwork();
            var preset = NetworkPresets.Presets[index - 1];
            foreach(var device in preset.Devices) {
                var instance = Instantiate(devicePrefabs[(int)device.Type], networkArea.transform);
                instance.transform.localPosition = device.Position;
                instance.PresetInitialize(this, networkArea);
                networkDevices.Add(instance);
            }
            foreach(var connection in preset.Cables) {
                addCableScript.AddCable(networkDevices[connection.Item1], networkDevices[connection.Item2]);
            }
        }

        public void ClearNetwork() {
            foreach(var device in networkDevices) {
                device.RemoveCables();
                Destroy(device.gameObject);
            }
            selectedDevice = null;
            uiController.HideDeviceOptions();
            networkDevices.Clear();
        }
    }
}
