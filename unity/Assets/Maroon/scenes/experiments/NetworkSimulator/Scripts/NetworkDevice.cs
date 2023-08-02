using Maroon.NetworkSimulator.NetworkDevices;
using System;
using System.Linq;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public abstract class NetworkDevice : MonoBehaviour {
        public enum DeviceType { Hub, Switch, Router, Computer }
        [SerializeField]
        protected NetworkSimulationController networkSimulationController;
        [SerializeField]
        private BoxCollider networkAreaCollider;
        [SerializeField]
        private float clickVsDragThreshold = 0.002f;
        [SerializeField]
        private GameObject connectableMarker;
        [SerializeField]
        private bool fromKit = false;

        private Plane plane;
        private Vector3 offset;
        private Vector3 kitPosition;
        private Vector3 dragStartPosition;
        private Vector3 clickStartPosition;
        private AddCableScript addCableScript;
        protected Computer addAddressInitiator = null;
        public bool IsInside { get; set; } = false;

        public int NumberOfPorts { get => Ports.Length; }
        public bool HasFreePort { get => Ports.Any(p => p.IsFree); }
        [SerializeField]
        protected Port[] Ports;
        public Port ConnectCableToFreePort(Cable cable) {
            var port = Ports.Where(p => p.IsFree).First();
            port.Cable = cable;
            return port;
        }

        void Start() {
            plane = new Plane(Vector3.up, transform.position);
            kitPosition = transform.position;
            addCableScript = FindObjectOfType<AddCableScript>();
        }
        protected abstract void OnAddedToNetwork();

        private void OnMouseDown() {
            clickStartPosition = Input.mousePosition;
            dragStartPosition = transform.position;
            var ray = Camera.main.ScreenPointToRay(clickStartPosition);
            plane.Raycast(ray, out var distance);
            offset = transform.position - ray.GetPoint(distance);
        }

        void OnMouseDrag() {
            if(addCableScript.IsAddingCable) {
                return;
            }
            var newMousePosition = Input.mousePosition;
            if((clickStartPosition - newMousePosition).sqrMagnitude < clickVsDragThreshold) {
                return;
            }
            var ray = Camera.main.ScreenPointToRay(newMousePosition);
            plane.Raycast(ray, out var distance);
            var newPosition = ray.GetPoint(distance) + offset;
            if(fromKit) {
                transform.position = newPosition;
            }
            else {
                var closestNetworkPoint = networkAreaCollider.ClosestPoint(newPosition);
                var isInNetworkArea = (closestNetworkPoint - newPosition).sqrMagnitude < float.Epsilon;
                if(isInNetworkArea) {
                    transform.position = newPosition;
                }
                else {
                    transform.position = closestNetworkPoint;
                }
            }
            UpdateCables();
        }

        private void OnMouseUp() {
            var isInNetworkArea = (networkAreaCollider.ClosestPoint(transform.position) - transform.position).sqrMagnitude < float.Epsilon;
            if(fromKit) {
                if(isInNetworkArea) {
                    Instantiate(this, kitPosition, Quaternion.identity, transform.parent).name = name;
                    transform.parent = networkAreaCollider.transform;
                    fromKit = false;
                    networkSimulationController.AddNetworkDevice(this);
                    OnAddedToNetwork();
                }
                else {
                    transform.position = kitPosition;
                }
            }
            else {
                var newMousePosition = Input.mousePosition;
                if((clickStartPosition - newMousePosition).sqrMagnitude < clickVsDragThreshold) {
                    ClickedDevice();
                    return;
                }
                if(!isInNetworkArea) {
                    transform.position = dragStartPosition;
                }
            }
            UpdateCables();
        }

        private void ClickedDevice() {
            if(addCableScript.IsAddingCable) {
                if(HasFreePort) {
                    addCableScript.ClickedDevice(this);
                }
            }
            else {
                networkSimulationController.SelectDevice(this);
            }
        }
        private void UpdateCables() {
            foreach(var port in Ports.Where(p => !p.IsFree)) {
                port.Cable.UpdateCurve();
            }
        }
        public void ShowConnectableMarker() {
            if(!HasFreePort) {
                return;
            }
            connectableMarker.SetActive(true);
        }
        public void HideConnectableMarker() {
            connectableMarker.SetActive(false);
        }
        public void RemoveCables() {
            foreach(var port in Ports.Where(p => !p.IsFree)) {
                port.Cable.Remove();
            }
        }
        public abstract string GetName();
        public abstract string GetButtonText();
        public abstract DeviceType GetDeviceType();
        public abstract void ClearAddressTables();
        public abstract void AddToAddressTables(IPAddress ipAddress, MACAddress macAddress, IPAddress via, Port receiver, int distance, Computer initiator);

        public abstract void ReceivePacket(Packet packet, Port receiver);

        protected void ReceivePacketInside(Packet packet, Port receiver) {
            networkSimulationController.InsideDeviceScript.ReceivePacket(packet, Array.IndexOf(Ports, receiver));
        }
        public void SendPacket(Packet packet, int portIndex) {
            Ports[portIndex].SendPacket(packet);
        }

        public bool[] GetPortConnected() {
            return Ports.Select(p => !p.IsFree).ToArray();
        }

        public void PresetInitialize(NetworkSimulationController simulationController, BoxCollider networkArea) {
            networkSimulationController = simulationController;
            networkAreaCollider = networkArea;
            OnAddedToNetwork();
        }
    }
}
