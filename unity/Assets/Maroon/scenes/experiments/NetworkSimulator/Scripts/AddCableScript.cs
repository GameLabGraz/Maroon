using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class AddCableScript : MonoBehaviour {
        [SerializeField]
        private Cable cablePrefab;
        [SerializeField]
        private Transform cableParent;
        [SerializeField]
        private Color activeColor;
        private Color defaultColor;
        private MeshRenderer meshRenderer;

        public bool IsAddingCable { get; private set; }

        private NetworkDevice firstNetworkDevice;

        void Start() {
            meshRenderer = GetComponent<MeshRenderer>();
            defaultColor = meshRenderer.material.color;
        }

        private void OnMouseUpAsButton() {
            IsAddingCable = !IsAddingCable;
            if(IsAddingCable) {
                meshRenderer.material.color = activeColor;
                NetworkSimulationController.Instance.ShowAllConnectableDeviceMarkers();
            }
            else {
                ResetState();
            }
        }

        public void ClickedDevice(NetworkDevice device) {
            if(!IsAddingCable) {
                return;
            }
            if(firstNetworkDevice == null) {
                firstNetworkDevice = device;
                NetworkSimulationController.Instance.HideConnectableDeviceMarkers();
                if(firstNetworkDevice.HasFreePort) {
                    NetworkSimulationController.Instance.ShowFreeConnectableDeviceMarkers();
                }
                firstNetworkDevice.HideConnectableMarker();
                firstNetworkDevice.ShowRemoveableMarkers();
                return;
            }
            if(device != firstNetworkDevice) {
                if(firstNetworkDevice.IsConnectedTo(device)) {
                    firstNetworkDevice.RemoveCable(device);
                    ResetState();
                }
                else if(firstNetworkDevice.HasFreePort && device.HasFreePort) {
                    AddCable(firstNetworkDevice, device);
                    ResetState();
                }
            }
        }

        public void AddCable(NetworkDevice device1, NetworkDevice device2) {
            var cable = Instantiate(cablePrefab, cableParent);
            var port1 = device1.ConnectCableToFreePort(cable);
            var port2 = device2.ConnectCableToFreePort(cable);
            cable.Initalize(port1, port2);
            if(IsAddingCable) {
                NetworkSimulationController.Instance.UpdateAddressTables();
            }
        }

        private void ResetState() {
            IsAddingCable = false;
            firstNetworkDevice = null;
            meshRenderer.material.color = defaultColor;
            NetworkSimulationController.Instance.HideConnectableDeviceMarkers();
        }
    }
}
