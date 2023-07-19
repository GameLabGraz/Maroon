using System;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class InsidePacket : MonoBehaviour {
        public Packet Packet { get; private set; }

        public Vector3 Position => transform.position;

        public InsidePort TargetPort { get; set; }

        public bool IsDraggable { get; set; }
        public bool IsBeingDragged { get; private set; }
        private Plane plane;
        private Vector3 offset;
        private Vector3 clickStartPosition;
        private Action<InsidePacket> onDragEnd;

        private void Start() {
            IsDraggable = false;
            IsBeingDragged = false;
        }

        public void Initialize(Packet packet, InsidePort receiver, Plane workingPlane, Action<InsidePacket> onDragEnd) {
            Packet = packet;
            transform.position = receiver.Position;
            GetComponentInChildren<MeshRenderer>().material.color = packet.Color;
            plane = workingPlane;
            this.onDragEnd = onDragEnd;
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
            plane.Raycast(ray, out var distance);
            offset = transform.position - ray.GetPoint(distance);
        }

        private void OnMouseDrag() {
            if(!IsDraggable) {
                return;
            }
            IsBeingDragged = true;
            var newMousePosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(newMousePosition);
            plane.Raycast(ray, out var distance);
            var newPosition = ray.GetPoint(distance) + offset;
            transform.position = newPosition;
        }

        private void OnMouseUp() {
            if(!IsDraggable) {
                return;
            }
            IsBeingDragged = false;
            onDragEnd(this);
        }

    }
}
