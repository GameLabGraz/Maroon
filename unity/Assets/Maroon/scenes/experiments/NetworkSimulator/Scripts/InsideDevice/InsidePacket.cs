using System;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class InsidePacket : MonoBehaviour {
        public Packet Packet { get; private set; }

        public Vector3 Position => transform.position;

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

        public void Initialize(Packet packet, InsidePort receiver, InsideDeviceScript insideDeviceScript) {
            Packet = packet;
            transform.position = receiver.Position;
            GetComponentInChildren<MeshRenderer>().material.color = packet.Color;
            this.insideDeviceScript = insideDeviceScript;
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
            if(!IsDraggable) {
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
