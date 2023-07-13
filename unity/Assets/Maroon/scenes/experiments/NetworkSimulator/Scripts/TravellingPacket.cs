using UnityEngine;

namespace Maroon.NetworkSimulator {
    public class TravellingPacket : MonoBehaviour {

        public Packet Packet { get; private set; }
        public Port Sender { get; private set; }
        public Port Receiver { get; private set; }
        public float Progress { get; set; }

        private void Start() {
            Progress = 0f;
        }
        public void Initialize(Packet packet, Port sender, Port receiver) {
            Packet = packet;
            Sender = sender;
            Receiver = receiver;
            transform.position = sender.Position;
            GetComponentInChildren<MeshRenderer>().material.color = packet.Color;
        }

    }
}
