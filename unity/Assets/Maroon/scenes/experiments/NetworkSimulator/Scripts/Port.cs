using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class Port : MonoBehaviour {
        [SerializeField]
        private NetworkDevice Device;
        public Cable Cable { get; set; }
        public NetworkDevice ConnectedDevice => Cable != null ? Cable.OtherPort(this).Device : null;
        public Vector3 Position => transform.position;
        public Vector3 BezierPoint => transform.position + transform.forward * 0.2f;

        public bool IsFree { get => Cable == null; }

        public void SendPacket(Packet packet) {
            packet = new Packet(packet);
            if(!IsFree) {
                packet.HopCount--;
                Cable.SendPacket(packet, this);
            }
        }

        public void ReceivePacket(Packet packet) {
            if(packet.HopCount == 0) {
                return;
            }
            Device.ReceivePacket(packet, this);
        }
    }
}
