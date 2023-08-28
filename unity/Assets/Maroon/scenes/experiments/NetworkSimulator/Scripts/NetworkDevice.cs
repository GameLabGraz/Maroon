using GEAR.Localization;
using Maroon.NetworkSimulator.NetworkDevices;
using Maroon.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public abstract class NetworkDevice : MonoBehaviour {
        public enum DeviceType { Hub, Switch, Router, Computer }
        [SerializeField]
        private BoxCollider networkAreaCollider;
        [SerializeField]
        private float clickVsDragThreshold = 0.002f;
        [SerializeField]
        private GameObject connectableMarker;
        [SerializeField]
        private MeshRenderer selectionObject;
        [SerializeField]
        private Color selectionColor;
        [SerializeField]
        private Color hoverColor;
        [SerializeField]
        private bool fromKit = false;

        private Plane plane;
        private Vector3 offset;
        private Vector3 kitPosition;
        private Vector3 dragStartPosition;
        private Vector3 clickStartPosition;
        private Color selectionObjectColor;
        private AddCableScript addCableScript;
        protected Computer addAddressInitiator = null;
        private readonly Queue<(Packet, Port)> packetQueue = new Queue<(Packet, Port)>();
        private bool isProcessingQueue = false;
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
            selectionObjectColor = selectionObject.material.color;
        }
        private void Update() {
            if(!SimulationController.Instance.SimulationRunning) {
                return;
            }
            if(!isProcessingQueue && packetQueue.Any()) {
                isProcessingQueue = true;
                StartCoroutine(ProcessQueue());
            }
        }
        protected abstract void OnAddedToNetwork();

        private void OnMouseDown() {
            if(!NetworkSimulationController.Instance.NetworkInteractionEnabled) {
                return;
            }
            clickStartPosition = Input.mousePosition;
            dragStartPosition = transform.position;
            var ray = Camera.main.ScreenPointToRay(clickStartPosition);
            plane.Raycast(ray, out var distance);
            offset = transform.position - ray.GetPoint(distance);
        }

        void OnMouseDrag() {
            if(addCableScript.IsAddingCable || !NetworkSimulationController.Instance.NetworkInteractionEnabled) {
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
            if(!NetworkSimulationController.Instance.NetworkInteractionEnabled) {
                return;
            }
            var isInNetworkArea = (networkAreaCollider.ClosestPoint(transform.position) - transform.position).sqrMagnitude < float.Epsilon;
            if(fromKit) {
                if(isInNetworkArea) {
                    if(NetworkSimulationController.Instance.NetworkDeviceCount < NetworkSimulationController.MaxNetworkDeviceCount) {
                        Instantiate(this, kitPosition, Quaternion.identity, transform.parent).name = name;
                        transform.parent = networkAreaCollider.transform;
                        fromKit = false;
                        NetworkSimulationController.Instance.AddNetworkDevice(this);
                        OnAddedToNetwork();
                    }
                    else {
                        UIController.Instance.DialogueManager.ShowMessage(new Message(LanguageManager.Instance.GetString("ReachedMaxDeviceCount"), MessageIcon.MI_Warning));
                        transform.position = kitPosition;
                    }
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

        private void OnMouseEnter() {
            if(!NetworkSimulationController.Instance.NetworkInteractionEnabled) {
                return;
            }
            if(!fromKit && selectionObject.material.color == selectionObjectColor) {
                selectionObject.material.color = hoverColor;
            }
        }

        private void OnMouseExit() {
            if(selectionObject.material.color == hoverColor) {
                ResetSelectionColor();
            }
        }

        private void ClickedDevice() {
            if(addCableScript.IsAddingCable) {
                if(HasFreePort) {
                    addCableScript.ClickedDevice(this);
                }
            }
            else {
                NetworkSimulationController.Instance.SelectDevice(this);
                selectionObject.material.color = selectionColor;
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
        public void ResetSelectionColor() {
            selectionObject.material.color = selectionObjectColor;
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

        public virtual void ReceivePacket(Packet packet, Port receiver) {
            packetQueue.Enqueue((packet, receiver));
        }
        private IEnumerator ProcessQueue() {
            while(packetQueue.Any()) {
                var packet = packetQueue.Dequeue();
                try {
                    ProcessPacket(packet.Item1, packet.Item2);
                }
                catch(KeyNotFoundException) {
                }
                yield return new WaitForSeconds(0.25f);
            }
            isProcessingQueue = false;
        }
        protected abstract void ProcessPacket(Packet packet, Port receiver);
        public abstract int GetDestinationPortIndex(Packet packet);

        protected void ReceivePacketInside(Packet packet, Port receiver) {
            NetworkSimulationController.Instance.InsideDeviceScript.ReceivePacket(packet, Array.IndexOf(Ports, receiver));
        }
        public void SendPacket(Packet packet, int portIndex) {
            Ports[portIndex].SendPacket(packet);
        }

        public bool[] GetPortConnected() {
            return Ports.Select(p => !p.IsFree).ToArray();
        }

        public void PresetInitialize(BoxCollider networkArea) {
            networkAreaCollider = networkArea;
            OnAddedToNetwork();
        }
    }
}
