using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class Cable : MonoBehaviour {
        private Port device1;
        private Port device2;
        private LineRenderer lineRenderer;

        const int NumberOfCurveSteps = 25;

        [SerializeField]
        private TravellingPacket TravellingPacketPrefab;

        private List<TravellingPacket> travellingPackets = new List<TravellingPacket>();
        private float travellingSpeed = 1;

        private void Start() {
        }

        private void Awake() {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void Update() {
            foreach(var packet in travellingPackets.ToList()) {
                if(packet.progress > 1 - float.Epsilon) {
                    travellingPackets.Remove(packet);
                    packet.receiver.ReceivePacket(packet.packet);
                    Destroy(packet.gameObject);
                }
                else {
                    packet.progress += Time.deltaTime * travellingSpeed;
                    var linePositionIndexA = Mathf.FloorToInt(packet.progress / (1f / NumberOfCurveSteps));
                    var linePositionIndexB = linePositionIndexA + 1;
                    if(packet.sender == device2) {
                        linePositionIndexA = NumberOfCurveSteps - linePositionIndexA;
                        linePositionIndexB = linePositionIndexA - 1;
                    }
                    Vector3 a, b;
                    if(linePositionIndexA < 0) {
                        a = device1.Position;
                    }
                    else if(linePositionIndexA >= NumberOfCurveSteps) {
                        a = device2.Position;
                    }
                    else {
                        a = lineRenderer.GetPosition(linePositionIndexA);
                    }

                    if(linePositionIndexB < 0) {
                        b = device1.Position;
                    }
                    else if(linePositionIndexB >= NumberOfCurveSteps) {
                        b = device2.Position;
                    }
                    else {
                        b = lineRenderer.GetPosition(linePositionIndexB);
                    }
                    packet.transform.position = Vector3.Lerp(a, b, packet.progress % 1);
                }
            }
        }

        public void Initalize(Port port1, Port port2) {
            device1 = port1;
            device2 = port2;
            UpdateCurve();
        }

        public void UpdateCurve() {
            lineRenderer.positionCount = NumberOfCurveSteps;
            lineRenderer.SetPositions(GetBezierCurve(NumberOfCurveSteps));
        }

        private Vector3[] GetBezierCurve(int steps) {
            var curve = new Vector3[steps];
            var stepSize = 1f / (steps - 1);
            for (int i = 0; i < steps; i++) {
                curve[i] = Bezier(i * stepSize, device1.Position, device1.BezierPoint, device2.BezierPoint, device2.Position);
            }
            return curve;
        }

        private Vector3 Bezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
            var add1 = Mathf.Pow(1 - t, 3) * p0;
            var add2 = 3 * Mathf.Pow(1 - t, 2) * t * p1;
            var add3 = 3 * (1 - t) * Mathf.Pow(t, 2) * p2;
            var add4 = Mathf.Pow(t, 3) * p3;
            return add1 + add2 + add3 + add4;
        }

        public void SendPacket(Packet packet, Port sender) {
            if(sender != device1 && sender != device2) {
                throw new System.ArgumentException("Cable not connected to port", nameof(sender));
            }
            var travellingPacket = Instantiate(TravellingPacketPrefab);
            travellingPacket.Initialize(packet, sender, sender == device1 ? device2 : device1);
            travellingPackets.Add(travellingPacket);
        }
    }
}
