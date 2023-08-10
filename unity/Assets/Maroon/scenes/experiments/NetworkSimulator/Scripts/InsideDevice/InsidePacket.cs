using TMPro;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class InsidePacket : MonoBehaviour {
        [SerializeField]
        private TextMeshPro macAddress;
        [SerializeField]
        private TextMeshPro ipAddress;
        public Packet Packet { get; private set; }

        public Vector3 Position => transform.position;

        public InsidePort Receiver { get; private set; }
        public InsidePort TargetPort { get; set; }

        public bool IsDraggable { get; set; }
        public bool IsBeingDragged { get; private set; }
        private InsideDeviceScript insideDeviceScript;
        private Vector3 offset;
        private Vector3 clickStartPosition;

        private void Start() {
            IsDraggable = false;
            IsBeingDragged = false;
        }

        public void Initialize(Packet packet) {
            Packet = packet;
            GetComponentInChildren<MeshRenderer>().material.color = packet.Color;
            macAddress.SetText(packet.DestinationMACAddress.ToString());
            ipAddress.SetText(packet.DestinationIPAddress.ToString());
            macAddress.color = new Color(0, 0, 0, 0.25f);
            ipAddress.color = new Color(0, 0, 0, 0.25f);
        }
        public void Initialize(Packet packet, InsidePort receiver, InsideDeviceScript insideDeviceScript) {
            Packet = packet;
            Receiver = receiver;
            transform.position = receiver.Position;
            GetComponentInChildren<MeshRenderer>().material.color = packet.Color;
            this.insideDeviceScript = insideDeviceScript;
            macAddress.SetText(packet.DestinationMACAddress.ToString());
            ipAddress.SetText(packet.DestinationIPAddress.ToString());
            if(insideDeviceScript.Mode == InsideDeviceScript.DeviceMode.Hub) {
                macAddress.color = new Color(0, 0, 0, 0.25f);
                ipAddress.color = new Color(0, 0, 0, 0.25f);
            }
            else if(insideDeviceScript.Mode == InsideDeviceScript.DeviceMode.Switch) {
                macAddress.color = new Color(0, 0, 0);
                ipAddress.color = new Color(0, 0, 0, 0.25f);
            }
            else if(insideDeviceScript.Mode == InsideDeviceScript.DeviceMode.Router) {
                macAddress.color = new Color(0, 0, 0, 0.25f);
                ipAddress.color = new Color(0, 0, 0);
            }
        }

        public void MoveTowards(Vector3 target, float maxDistanceDelta) {
            transform.position = Vector3.MoveTowards(transform.position, target, maxDistanceDelta);
        }

        private void OnMouseDown() {
            if(!IsDraggable) {
                return;
            }
            clickStartPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(clickStartPosition);
            insideDeviceScript.WorkingPlane.Raycast(ray, out var distance);
            offset = transform.position - ray.GetPoint(distance);
        }

        private void OnMouseDrag() {
            if(!IsDraggable || !SimulationController.Instance.SimulationRunning) {
                return;
            }
            IsBeingDragged = true;
            var newMousePosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(newMousePosition);
            insideDeviceScript.WorkingPlane.Raycast(ray, out var distance);
            var newPosition = ray.GetPoint(distance) + offset;
            transform.position = newPosition;
            insideDeviceScript.OnPacketDrag(newPosition);
        }

        private void OnMouseUp() {
            if(!IsDraggable) {
                return;
            }
            IsBeingDragged = false;
            insideDeviceScript.OnPacketDragEnd(this);
        }

    }
}
