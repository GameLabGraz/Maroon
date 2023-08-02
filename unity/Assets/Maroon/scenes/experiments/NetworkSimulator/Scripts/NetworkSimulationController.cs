using Maroon.NetworkSimulator.NetworkDevices;
using System.Collections.Generic;
using System.Linq;
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
        private IEnumerable<Computer> computers => networkDevices.Where(d => d is Computer).Cast<Computer>();
        private NetworkDevice selectedDevice = null;
        private int ipAddressCounter = 1;
        private int macAddressCounter = 0;

        private readonly float TrafficInterval = 4;
        private readonly float TrafficIntervalRange = 2;
        private float NextTrafficTimeout => TrafficInterval + Random.Range(-TrafficIntervalRange, TrafficIntervalRange);

        private void Start() {
            Invoke(nameof(GenerateTraffic), NextTrafficTimeout);
        }
        private void GenerateTraffic() {
            var nextTrafficTimeout = NextTrafficTimeout;
            if(computers.Count() > 1) {
                var source = computers.ElementAt(Random.Range(0, computers.Count()));
                var destinations = computers.Where(c => c != source);
                var destinationIPAddress = destinations.ElementAt(Random.Range(0, destinations.Count())).IPAddress;
                try {
                    source.SendPacket(destinationIPAddress);
                }
                catch (KeyNotFoundException) {
                    nextTrafficTimeout = 0.2f;
                }
            }
            Invoke(nameof(GenerateTraffic), nextTrafficTimeout);
        }

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
            UpdateAddressTables();
        }

        public void ClearNetwork() {
            foreach(var device in networkDevices) {
                device.RemoveCables();
                Destroy(device.gameObject);
            }
            selectedDevice = null;
            uiController.HideDeviceOptions();
            networkDevices.Clear();
            ipAddressCounter = 1;
            macAddressCounter = 0;
        }

        public void UpdateAddressTables() {
            foreach(var device in networkDevices) {
                device.ClearAddressTables();
            }
            foreach(var computer in computers) {
                computer.StartAddingAddressToTables();
            }
        }

        public IPAddress GetIPAddress() {
            var address = new System.Net.IPAddress(new byte[] { 10, 0, (byte)(ipAddressCounter >> 8), (byte)(ipAddressCounter % 255) });
            ipAddressCounter++;
            return new IPAddress(address.ToString());
        }

        public MACAddress GetMACAddress(NetworkDevice.DeviceType deviceType) {
            var bytes = new byte[6];
            bytes[0] = (byte)Random.Range(0, 255);
            bytes[1] = (byte)Random.Range(0, 255);
            bytes[2] = (byte)(macAddressCounter >> 8);
            bytes[3] = (byte)(macAddressCounter % 255);
            bytes[4] = (byte)Random.Range(0, 255);
            bytes[5] = (byte)Random.Range(0, 255);
            macAddressCounter++;
            string address = "";
            if(deviceType == NetworkDevice.DeviceType.Computer) {
                address = string.Join(":", bytes.Select(b => b.ToString("X2")));
            }
            else if(deviceType == NetworkDevice.DeviceType.Router) {
                address = string.Join(":", bytes.Take(5).Select(b => b.ToString("X2")));
            }
            return new MACAddress(address);
        }
    }
}
