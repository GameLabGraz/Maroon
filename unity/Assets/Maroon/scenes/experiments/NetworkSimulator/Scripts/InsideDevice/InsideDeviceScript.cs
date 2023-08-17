using GEAR.Localization;
using Maroon.NetworkSimulator.NetworkDevices;
using Maroon.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class InsideDeviceScript : MonoBehaviour {

        public enum DeviceMode { Hub, Switch, Router }

        [SerializeField]
        private InsidePacket InsidePacketPrefab;
        [SerializeField]
        private InsidePort[] Ports;
        [SerializeField]
        private Transform queue;

        private readonly List<InsidePacket> incomingPackets = new List<InsidePacket>();
        private readonly List<InsidePacket> queuedPackets = new List<InsidePacket>();
        private readonly List<InsidePacket> outgoingPackets = new List<InsidePacket>();
        private readonly List<InsidePacket> droppedPackets = new List<InsidePacket>();
        private readonly List<InsidePacket[]> hubPackets = new List<InsidePacket[]>();
        private const int maxQueueLength = 8;
        private const float queuePacketDistance = 0.6f;
        private const float packetSpeed = 2.5f;
        private const float distanceTolerance = 0.0001f;
        private const float portSelectionDistance = 0.6f;
        private Vector3[] portWorkingPlanePositions;
        public DeviceMode Mode { get; private set; }
        private NetworkDevice device;
        private DialogueManager dialogueManager;

        public Plane WorkingPlane { get; private set; }

        public void SetDevice(NetworkDevice device) {
            this.device = device;
            if(device is Hub) {
                Mode = DeviceMode.Hub;
            }
            else if(device is Switch) {
                Mode = DeviceMode.Switch;
            }
            else if(device is Router) {
                Mode = DeviceMode.Router;
            }

            var portConnected = device.GetPortConnected();
            for(int i = 0; i < Ports.Length; i++) {
                Ports[i].SetConnected(portConnected[i]);
            }
        }

        void Start() {
            WorkingPlane = new Plane(Vector3.forward, queue.position);
            portWorkingPlanePositions = new Vector3[Ports.Length];
            for(int i = 0; i < Ports.Length; i++) {
                portWorkingPlanePositions[i] = WorkingPlane.ClosestPointOnPlane(Ports[i].Position);
            }
            dialogueManager = FindObjectOfType<DialogueManager>();
        }

        void Update() {
            if(!SimulationController.Instance.SimulationRunning || device == null) {
                return;
            }
            if(Mode == DeviceMode.Hub) {
                MoveHubPackets();
                return;
            }
            var incoming = incomingPackets.ToList();
            var queued = queuedPackets.ToList();
            var outgoing = outgoingPackets.ToList();
            var dropped = droppedPackets.ToList();
            for(int i = 0; i < incoming.Count; i++) {
                MoveToQueue(incoming[i], i);
            }
            for(int i = 0; i < queued.Count; i++) {
                if(!queued[i].IsBeingDragged) {
                    UpdateQueuePosition(queued[i], i);
                }
            }
            foreach(var packet in outgoing) {
                MoveOutgoing(packet);
            }
            foreach(var packet in dropped) {
                if(packet.Position.y > transform.position.y - 1) {
                    packet.MoveTowards(packet.Position + Vector3.down, packetSpeed * 1.5f * Time.deltaTime);
                }
                else {
                    droppedPackets.Remove(packet);
                    Destroy(packet.gameObject);
                }
            }
        }

        void MoveToQueue(InsidePacket packet, int index) {
            var queueIndex = queuedPackets.Count + index;
            if(queueIndex > maxQueueLength) {
                queueIndex = maxQueueLength;
            }
            var queueTargetPosition = queue.position - queueIndex * queuePacketDistance * queue.right;
            if(WorkingPlane.GetDistanceToPoint(packet.Position) > distanceTolerance) {
                packet.MoveTowards(packet.Position + Vector3.back, packetSpeed * Time.deltaTime);
            }
            else if(Vector3.Distance(packet.Position, queueTargetPosition) > distanceTolerance) {
                packet.MoveTowards(queueTargetPosition, packetSpeed * Time.deltaTime);
            }
            else {
                incomingPackets.Remove(packet);
                if(queueIndex >= maxQueueLength) {
                    droppedPackets.Add(packet);
                }
                else {
                    queuedPackets.Add(packet);
                }
            }
        }

        void UpdateQueuePosition(InsidePacket packet, int index) {
            packet.IsDraggable = Mode != DeviceMode.Hub && index == 0;
            var targetPosition = queue.position - index * queuePacketDistance * queue.right;
            packet.MoveTowards(targetPosition, packetSpeed * Time.deltaTime);
        }

        void MoveOutgoing(InsidePacket packet) {
            var targetOnWorkingPlane = portWorkingPlanePositions[Array.IndexOf(Ports, packet.TargetPort)];
            var distanceToTargetOnWorkingPlane = Vector3.Distance(packet.Position, targetOnWorkingPlane);
            var distanceToPort = Vector3.Distance(packet.Position, packet.TargetPort.Position);
            if(distanceToTargetOnWorkingPlane > distanceTolerance && distanceToPort > Vector3.Distance(targetOnWorkingPlane, packet.TargetPort.Position)) {
                packet.MoveTowards(targetOnWorkingPlane, packetSpeed * Time.deltaTime);
            }
            else if(distanceToPort > distanceTolerance) {
                packet.MoveTowards(packet.TargetPort.Position, packetSpeed * Time.deltaTime);
            }
            else {
                outgoingPackets.Remove(packet);
                device.SendPacket(packet.Packet, Array.IndexOf(Ports, packet.TargetPort));
                Destroy(packet.gameObject);
            }
        }

        void MoveHubPackets() {
            var incoming = incomingPackets.ToList();
            var hubP = hubPackets.ToList();
            var outgoing = outgoingPackets.ToList();

            foreach(var packet in incoming) {
                if(WorkingPlane.GetDistanceToPoint(packet.Position) > distanceTolerance) {
                    packet.MoveTowards(packet.Position + Vector3.back, packetSpeed * Time.deltaTime);
                }
                else {
                    incomingPackets.Remove(packet);
                    var portConnected = device.GetPortConnected();
                    var targetPortIndex = new List<int>();
                    for(int i = 0; i < portConnected.Length; i++) {
                        if(portConnected[i] && Ports[i] != packet.Receiver) {
                            targetPortIndex.Add(i);
                        }
                    }
                    var hubPacket = new InsidePacket[targetPortIndex.Count];
                    hubPacket[0] = packet;
                    hubPacket[0].TargetPort = Ports[targetPortIndex[0]];
                    for(int i = 1; i < hubPacket.Length; i++) {
                        hubPacket[i] = Instantiate(packet);
                        hubPacket[i].Initialize(new Packet(packet.Packet));
                        hubPacket[i].TargetPort = Ports[targetPortIndex[i]];
                    }
                    hubPackets.Add(hubPacket);
                }
            }
            foreach(var hubPacket in hubP) {
                var allAtTarget = true;
                for(int i = 0; i < hubPacket.Length; i++) {
                    var targetPosition = portWorkingPlanePositions[Array.IndexOf(Ports, hubPacket[i].TargetPort)];
                    if(Vector3.Distance(hubPacket[i].Position, targetPosition) > distanceTolerance) {
                        hubPacket[i].MoveTowards(targetPosition, packetSpeed * Time.deltaTime);
                        allAtTarget = false;
                    }
                }
                if(allAtTarget) {
                    hubPackets.Remove(hubPacket);
                    foreach(var packet in hubPacket) {
                        outgoingPackets.Add(packet);
                    }
                }
            }

            foreach(var packet in outgoing) {
                MoveOutgoing(packet);
            }
        }

        public void ReceivePacket(Packet packet, int portIndex) {
            var insidePacket = Instantiate(InsidePacketPrefab);
            insidePacket.Initialize(packet, Ports[portIndex], this);
            incomingPackets.Add(insidePacket);
        }

        public void OnPacketDrag(Vector3 position) {
            foreach(var port in Ports) {
                port.ResetStatus();
            }
            var closestPort = FindClosestPort(position);
            if(closestPort != null) {
                closestPort.ShowSelection();
            }
        }

        public void OnPacketDragEnd(InsidePacket packet) {
            var targetPort = FindClosestPort(packet.Position);
            if(targetPort != null) {
                targetPort.ResetStatus();
                packet.TargetPort = targetPort;
                packet.IsDraggable = false;
                queuedPackets.Remove(packet);
                outgoingPackets.Add(packet);

                Message dialogueMessage;
                if(Array.IndexOf(Ports, targetPort) == device.GetDestinationPortIndex(packet.Packet)) {
                    dialogueMessage = new Message(LanguageManager.Instance.GetString("PacketCorrect"), MessageIcon.MI_Ok);
                }
                else {
                    dialogueMessage = new Message(LanguageManager.Instance.GetString("PacketWrong"), MessageIcon.MI_Error);
                }
                dialogueManager.ShowMessage(dialogueMessage);
            }
        }

        private InsidePort FindClosestPort(Vector3 position) {
            var minDistance = portSelectionDistance;
            InsidePort port = null;
            for(int i = 0; i < Ports.Length; i++) {
                var distance = Vector3.Distance(position, portWorkingPlanePositions[i]);
                if(distance < portSelectionDistance && distance < minDistance) {
                    minDistance = distance;
                    port = Ports[i];
                }
            }
            return port;
        }

        public void Clear() {
            foreach(var packet in incomingPackets) {
                Destroy(packet.gameObject);
            }
            incomingPackets.Clear();
            foreach(var packet in queuedPackets) {
                Destroy(packet.gameObject);
            }
            queuedPackets.Clear();
            foreach(var hubPacket in hubPackets) {
                foreach(var packet in hubPacket) {
                    Destroy(packet.gameObject);
                }
            }
            hubPackets.Clear();
            device = null;
        }
    }
}
