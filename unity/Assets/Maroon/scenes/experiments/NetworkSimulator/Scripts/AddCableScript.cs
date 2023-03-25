using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class AddCableScript : MonoBehaviour {
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
                return;
            }
            if(secondNetworkDevice == null) {
                secondNetworkDevice = device;
                var cable = Instantiate(cablePrefab, cableParent);
                var port1 = firstNetworkDevice.ConnectCableToFreePort(cable);
                var port2 = secondNetworkDevice.ConnectCableToFreePort(cable);
                cable.Initalize(port1, port2);
                ResetState();
            }
        }

        private void ResetState() {
            IsAddingCable = false;
            firstNetworkDevice = null;
            secondNetworkDevice = null;
            meshRenderer.material = defaultMaterial;
        }
    }
}
