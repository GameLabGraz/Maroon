using Maroon.NetworkSimulator.NetworkDevices;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class InsideDeviceScript : MonoBehaviour {

        enum Mode { Hub, Switch, Router }

        [SerializeField]
        private InsidePacket InsidePacketPrefab;
        [SerializeField]
        private InsidePort[] Ports;
        [SerializeField]
        private Transform queue;

        private readonly List<InsidePacket> incomingPackets = new List<InsidePacket>();
        private readonly List<InsidePacket> queuedPackets = new List<InsidePacket>();
        private readonly List<InsidePacket> outgoingPackets = new List<InsidePacket>();
        private const int maxQueueLength = 8;
        private const float queuePacketDistance = 0.6f;
        private const float packetSpeed = 2.5f;
        private Plane workingPlane;
        private Mode mode;
        private NetworkDevice device;

        public void SetDevice(NetworkDevice device) {
            this.device = device;
            if(device is Hub) {
                mode = Mode.Hub;
            }
            else if(device is Switch) {
                mode = Mode.Switch;
            }
            else if(device is Router) {
                mode = Mode.Router;
            }
        }

        void Start() {
            workingPlane = new Plane(Vector3.forward, queue.position);
        }

        void Update() {
            var list = incomingPackets.ToList();
            for(int i = 0; i < list.Count; i++) {
                MoveToQueue(list[i], i);
            }
            foreach(var packet in queuedPackets.ToList()) {

            }
            foreach(var packet in outgoingPackets.ToList()) {

            }
        }

        void MoveToQueue(InsidePacket packet, int index) {
            if(workingPlane.GetDistanceToPoint(packet.Position) > 0.0001f) {
                packet.MoveTowards(packet.Position + Vector3.back, packetSpeed * Time.deltaTime);
            }
            else if(Vector3.Distance(packet.Position, queue.position) > 0.0001f) {
                var queueIndex = queuedPackets.Count + index;
                if(queueIndex > maxQueueLength) {
                    queueIndex = maxQueueLength;
                }
                var targetPosition = queue.position - queueIndex * queuePacketDistance * queue.right;
                packet.MoveTowards(targetPosition, packetSpeed * Time.deltaTime);
            }
            else {
                incomingPackets.Remove(packet);
                queuedPackets.Add(packet);
            }
        }

        public void ReceivePacket(Packet packet, int portIndex) {
            var insidePacket = Instantiate(InsidePacketPrefab);
            insidePacket.Initialize(packet, Ports[portIndex]);
            incomingPackets.Add(insidePacket);
        }

        private void SendPacket(InsidePacket packet, InsidePort port) {
            device.SendPacket(packet.Packet, Array.IndexOf(Ports, port));
        }
    }
}
