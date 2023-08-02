using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class AddCableScript : MonoBehaviour {
        [SerializeField]
        private NetworkSimulationController networkSimulationController;
        [SerializeField]
        private Cable cablePrefab;
        [SerializeField]
        private Transform cableParent;
        [SerializeField]
        private Material activeMaterial;
        private Material defaultMaterial;
        private MeshRenderer meshRenderer;

        public bool IsAddingCable { get; private set; }

        private NetworkDevice firstNetworkDevice;
        private NetworkDevice secondNetworkDevice;

        void Start() {
            meshRenderer = GetComponent<MeshRenderer>();
            defaultMaterial = meshRenderer.material;
        }

        void Update() {
        }

        private void OnMouseUpAsButton() {
            IsAddingCable = !IsAddingCable;
            if(IsAddingCable) {
                meshRenderer.material = activeMaterial;
                networkSimulationController.ShowConnectableDeviceMarkers();
            }
            else {
                ResetState();
            }
        }

        public void ClickedDevice(NetworkDevice device) {
            if(!IsAddingCable || !device.HasFreePort) {
                return;
            }
            if(firstNetworkDevice == null) {
                firstNetworkDevice = device;
                firstNetworkDevice.HideConnectableMarker();
                return;
            }
            if(secondNetworkDevice == null && device != firstNetworkDevice) {
                secondNetworkDevice = device;
                AddCable(firstNetworkDevice, secondNetworkDevice);
                ResetState();
            }
        }

        public void AddCable(NetworkDevice device1, NetworkDevice device2) {
            var cable = Instantiate(cablePrefab, cableParent);
            var port1 = device1.ConnectCableToFreePort(cable);
            var port2 = device2.ConnectCableToFreePort(cable);
            cable.Initalize(port1, port2);
            if(IsAddingCable) {
                networkSimulationController.UpdateAddressTables();
            }
        }

        private void ResetState() {
            IsAddingCable = false;
            firstNetworkDevice = null;
            secondNetworkDevice = null;
            meshRenderer.material = defaultMaterial;
            networkSimulationController.HideConnectableDeviceMarkers();
        }
    }
}
